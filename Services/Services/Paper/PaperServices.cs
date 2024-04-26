using AutoMapper;
using OfficeOpenXml;
using Repositories;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.OMath;
using Services.Interfaces.Paper;
using Services.ViewModels;
using System.Net;
using System.Text.RegularExpressions;
using Services.Interfaces.Storage;

namespace Services.Services.Paper
{
    public class PaperServices : IPaperServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IS3Services _s3Services;

        public PaperServices(IUnitOfWork unitOfWork, IMapper mapper, IS3Services s3Services)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _s3Services = s3Services;
        }

        public async Task<Guid> CreateTestPaper(Guid currentUserId, Guid paperSetId, DetailOfPaper detailOfPaper, Guid templatePaperId, bool shuffleAnswers, ExcelWorksheet worksheet)
        {
            try
            {
                //header
                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                List<string> startAnswer = new List<string> { "A", "B", "C", "D" };
                string correctAnswer = "";

                //read template
                var keyS3OfTemplate = (await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == templatePaperId)).KeyS3;
                var binaryTemplate = await _s3Services.GetByteOfFileAsync(keyS3OfTemplate);

                using (var stream = new MemoryStream(binaryTemplate))
                {
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    Spire.Doc.Document document = new Spire.Doc.Document();
                    document.LoadFromStream(memoryStream, FileFormat.Auto);
                    Spire.Doc.Document newDoc = document.Clone();

                    if (currentUser.SchoolId != null)
                    {
                        var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == currentUser.SchoolId);
                        newDoc.Replace("EXAGEN Generator", school.Name, false, true);
                    }
                    newDoc.Replace("{Name}", $"{detailOfPaper.NameInTest}", false, true);
                    newDoc.Replace("{Subject} - Lớp {Grade}", $"{detailOfPaper.SubjectName} - Lớp {detailOfPaper.Grade}", false, true);
                    newDoc.Replace("Thời gian làm bài: {Time}p", $"Thời gian làm bài: {detailOfPaper.TimeOfTest}p", false, true);
                    newDoc.Replace("Mã đề thi: {Code}", $"Mã đề thi: {detailOfPaper.PaperCode}", false, true);

                    Paragraph formatContent;
                    Paragraph formatAnswer;
                    Paragraph endPara;

                    foreach (Section section in document.Sections)
                    {
                        endPara = section.Paragraphs[section.Paragraphs.Count - 1];
                        formatContent = section.Paragraphs[1];
                        formatAnswer = section.Paragraphs[2];
                    }

                    int numOfQuestion = 1;

                    //CLEAR ALL PARAgraph of new doc
                    newDoc.Sections[0].Paragraphs.Clear();

                    var pCode = detailOfPaper.PaperCode.ToString("D3");
                    worksheet.Cells[2, detailOfPaper.PaperCode + 1].Value = pCode;
                    worksheet.Cells[2, detailOfPaper.PaperCode + 1].Style.Font.Bold = true;

                    foreach (var questionId in detailOfPaper.QuestionIds)
                    {
                        var question = await _unitOfWork.QuestionRepo.FindByField(q => q.QuestionId == questionId);
                        var shuffleStartAnswer = shuffleAnswers ? startAnswer.OrderBy(a => Guid.NewGuid()).ToList() : startAnswer;

                        ProcessContent($"Câu {numOfQuestion}. ", question.QuestionPart, newDoc);
                        int no = 0;
                        foreach (var a in shuffleStartAnswer)
                        {
                            string prefix = "";
                            switch (no)
                            {
                                case 0:
                                    prefix = "     A. ";
                                    break;
                                case 1:
                                    prefix = "     B. ";
                                    break;
                                case 2:
                                    prefix = "     C. ";
                                    break;
                                case 3:
                                    prefix = "     D. ";
                                    break;
                            }

                            switch (a)
                            {
                                case "A":
                                    ProcessContent(prefix, question.Answer1, newDoc);
                                    break;
                                case "B":
                                    ProcessContent(prefix, question.Answer2, newDoc);
                                    break;
                                case "C":
                                    ProcessContent(prefix, question.Answer3, newDoc);
                                    break;
                                case "D":
                                    ProcessContent(prefix, question.Answer4, newDoc);
                                    break;
                            }

                            if (question.CorrectAnswer == a)
                            {
                                correctAnswer += $"{questionId}~{numOfQuestion}:{prefix.TrimStart().First()}|";

                                worksheet.Cells[numOfQuestion + 2, detailOfPaper.PaperCode + 1].Value = prefix.TrimStart().First();
                            }

                            no++;
                        }

                        Paragraph paragraph = newDoc.Sections[0].AddParagraph();
                        numOfQuestion++;
                    }

                    correctAnswer = correctAnswer.TrimEnd('|');

                    // Save new doc to stream
                    newDoc.SaveToStream(memoryStream, FileFormat.Docx);

                    var statusCode = await _s3Services.UploadFileIntoS3Async(memoryStream, $"papers/{currentUserId}/{detailOfPaper.NameOfTest}/{detailOfPaper.PaperCode}.docx");

                    if (statusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Lỗi khi upload file lên S3");
                    }

                    var paper = new Repositories.Models.Paper
                    {
                        CreatedBy = currentUserId,
                        CreatedOn = DateTime.Now.AddHours(7),
                        KeyS3 = $"papers/{currentUserId}/{detailOfPaper.NameOfTest}/{detailOfPaper.PaperCode}.docx",
                        PaperAnswer = correctAnswer,
                        PaperCode = detailOfPaper.PaperCode,
                        PaperSetId = paperSetId
                    };

                    _unitOfWork.PaperRepo.AddAsync(paper);
                    var result = await _unitOfWork.SaveChangesAsync();

                    if (result <= 0)
                    {
                        return Guid.Empty;
                    }

                    foreach (var q in detailOfPaper.QuestionIds)
                    {
                        var questionInPaper = new Repositories.Models.QuestionInPaper
                        {
                            PaperId = paper.PaperId,
                            QuestionId = q,
                        };

                        _unitOfWork.QuestionInPaperRepo.AddAsync(questionInPaper);
                        result = await _unitOfWork.SaveChangesAsync();
                    }

                    return result > 0 ? paper.PaperId : Guid.Empty;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - CreateTestPaper: " + e.Message);
            }
        }

        public async Task<string> GetPaperById(Guid paperId)
        {
            try
            {
                var paper = await _unitOfWork.PaperRepo.FindByField(paper => paper.PaperId == paperId);
                if (paper == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }
                var urlS3 = await _s3Services.GetObjectUrlAsync(paper.KeyS3);

                return urlS3;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetPaperById: " + e.Message);
            }
        }

        private void ProcessContent(string prefix, string content, Spire.Doc.Document newDoc)
        {
            Section section = newDoc.Sections[0];
            Paragraph paragraph = section.AddParagraph();


            if (prefix != null)
            {
                TextRange bold = paragraph.AppendText(prefix);
                bold.CharacterFormat.Bold = true;
            }

            MatchCollection matches = Regex.Matches(content, "<img>");
            MatchCollection matches1 = Regex.Matches(content, "</img>");
            MatchCollection matches2 = Regex.Matches(content, "<latex>");
            MatchCollection matches3 = Regex.Matches(content, "</latex>");

            List<int> startImg = new List<int>();
            List<int> endImg = new List<int>();
            List<int> startLatex = new List<int>();
            List<int> endLatex = new List<int>();

            foreach (Match match in matches)
            {
                startImg.Add(match.Index);
            }

            foreach (Match match in matches2)
            {
                startLatex.Add(match.Index);
            }

            if (startImg.Count > 0 || startLatex.Count > 0)
            {
                foreach (Match match in matches1)
                {
                    endImg.Add(match.Index);
                }

                foreach (Match match in matches3)
                {
                    endLatex.Add(match.Index);
                }

                int length = content.Length;

                int indexImg = 0;
                int indexLatex = 0;
                int maxImg = startImg.Count;
                int maxLatex = startLatex.Count;
                int currentIndex = 0;

                while (currentIndex < length)
                {
                    if (!startImg.Contains(currentIndex) && !startLatex.Contains(currentIndex))
                    {
                        if (indexImg < maxImg || indexLatex < maxLatex)
                        {
                            int s = 0;
                            if (maxImg == 0 && maxLatex != 0)
                            {
                                s = startLatex[indexLatex];
                            }
                            else if (maxLatex == 0 && maxImg != 0)
                            {
                                s = startImg[indexImg];
                            }
                            else
                            {
                                s = Math.Min(startImg[indexImg], startLatex[indexLatex]);
                            }
                            paragraph.AppendText(content.Substring(currentIndex, s - currentIndex));
                            currentIndex = s;
                        }
                        else
                        {
                            paragraph.AppendText(content.Substring(currentIndex));
                            break;
                        }
                    }
                    else if (startImg.Contains(currentIndex))
                    {
                        // Add drawing to paragraph
                        byte[] imageBytes = Convert.FromBase64String(content.Substring(startImg[indexImg] + 5, endImg[indexImg] - startImg[indexImg] - 5));
                        paragraph.AppendPicture(imageBytes);
                        currentIndex = endImg[indexImg] + 6;
                        indexImg++;
                    }
                    else
                    {
                        string mathLatex = content.Substring(startLatex[indexLatex] + 7, endLatex[indexLatex] - startLatex[indexLatex] - 7);
                        OfficeMath officeMath_1 = new OfficeMath(newDoc);
                        officeMath_1.FromLatexMathCode(mathLatex);
                        paragraph.Items.Add(officeMath_1);
                        currentIndex = endLatex[indexLatex] + 8;
                        indexLatex++;
                    }
                }
            }
            else
            {
                paragraph.AppendText(content);
            }
        }

    }
}

