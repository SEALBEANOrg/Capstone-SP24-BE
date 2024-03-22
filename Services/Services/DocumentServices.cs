using AutoMapper;
using Google.Apis.Logging;
using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Repositories;
using Services.Interfaces;
using Services.Utilities;
using Services.ViewModels;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.OMath;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace Services.Services
{
    public class DocumentServices : IDocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IS3Services _s3Services;

        public DocumentServices(IUnitOfWork unitOfWork, IMapper mapper, IS3Services s3Services)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _s3Services = s3Services;
        }

        public async Task<bool> AddDocument(DocumentCreate documentCreate, Guid currentUser)
        {
            try {
                var document = _mapper.Map<Repositories.Models.Document>(documentCreate);
                
                var file = documentCreate.FileImport;
                var stream = file.OpenReadStream();
                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var keyS3 = "";
                if (documentCreate.Type == 0)
                {
                    keyS3 = "templates/answer-sheet/" + $"{Utils.FormatFileName(documentCreate.Name).Trim()}-{DateTime.Now.Ticks}.docx";
                }
                else if (documentCreate.Type == 1)
                {
                    keyS3 = "templates/paper/" + $"{Utils.FormatFileName(documentCreate.Name).Trim()}-{DateTime.Now.Ticks}.docx";
                }
                var statusCode = await _s3Services.UploadFileIntoS3Async(memoryStream, keyS3);
                if (statusCode == HttpStatusCode.OK)
                {
                    document.KeyS3 = keyS3;
                }
                else
                {
                    return false;
                }
                document.CreatedBy = currentUser;           
                document.CreatedOn = DateTime.Now;
            
                _unitOfWork.DocumentRepo.AddAsync(document);
                var result = await _unitOfWork.SaveChangesAsync();
            
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - AddDocument: " + e.Message);
            }
        }

        public async Task<bool> DeleteDocument(Guid documentId)
        {
            try
            {
                var document = await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == documentId);
                
                if (document == null)
                {
                    return false;
                }

                _unitOfWork.DocumentRepo.Remove(document);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0) 
                {
                    var statusCode = await _s3Services.DeleteObjectAsync(document.KeyS3);
                    return statusCode == HttpStatusCode.NoContent;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - DeleteDocument: " + e.Message);
            }
        }

        public async Task<IEnumerable<DocumentViewModels>> GetAllDocument(int? type)
        {
            try
            {
                var documents = await _unitOfWork.DocumentRepo.GetAllAsync();
                documents = type != null ? documents.Where(document => document.Type == type).ToList() : documents;

                if (documents == null)
                {
                    return null;
                }

                var documentViewModels = new List<DocumentViewModels>();

                foreach (var document in documents)
                {
                    var documentViewModel = _mapper.Map<DocumentViewModels>(document);
                    documentViewModel.UrlS3 = await _s3Services.GetObjectUrlAsync(document.KeyS3);
                    documentViewModels.Add(documentViewModel);
                }

                return documentViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - GetAllDocument: " + e.Message);
            }
        }

        public async Task<DocumentViewModel> GetDocumentById(Guid documentId)
        {
            try
            {
                var document = await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == documentId);

                if (document == null)
                {
                    return null;
                }

                var documentViewModel = _mapper.Map<DocumentViewModel>(document);
                documentViewModel.UrlS3 = await _s3Services.GetObjectUrlAsync(document.KeyS3);

                return documentViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - GetDocumentById: " + e.Message);
            }
        }

        //public async Task<bool> UploadDocument(MemoryStream stream, DocumentCreate documentCreate)
        //{
        //    try
        //    {
        //        var document = new Repositories.Models.Document
        //        {
        //            Name = documentCreate.Name,
        //            Type = documentCreate.Type,
        //            CreatedOn = DateTime.Now,
        //            CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"),
        //            Description = documentCreate.Description,
        //            Data = stream.ToArray(),
        //        };

        //        _unitOfWork.DocumentRepo.AddAsync(document);
        //        var result = await _unitOfWork.SaveChangesAsync();

        //        return result > 0;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Lỗi ở DocumentServices - UploadDocument: " + e.Message);
        //    }
        //}

        public async Task<Guid> CreateTestPaper(Guid currentUserId, Guid paperSetId, DetailOfPaper detailOfPaper, Guid templatePaperId, bool shuffleAnswers, ExcelWorksheet worksheet)
        {
            try
            {
                //header
                // trường của creator
                // name cuộc thi : description của exam
                // môn học - khối :  lúc chọn exam có lựa môn - khối thì lấy từ class
                // mã đề thi, save paper xong lấy mã đề thi update vào

                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                List<string> startAnswer = new List<string> { "A", "B", "C", "D" };
                string correctAnswer = "";

                //read file
                var keyS3OfTemplate = (await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == templatePaperId)).KeyS3;
                var binaryTemplate = await _s3Services.GetByteOfFileAsync(keyS3OfTemplate);

                using (var stream = new MemoryStream(binaryTemplate))
                {
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    Document document = new Document();
                    document.LoadFromStream(memoryStream, FileFormat.Auto);
                    Document newDoc = document.Clone();

                    newDoc.Replace("{Name}", $"{detailOfPaper.NameOfTest}", false, true);
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
                        //paragraph.AppendBreak(BreakType.LineBreak);

                        numOfQuestion++;
                    }

                    correctAnswer = correctAnswer.TrimEnd('|');

                    //newDoc.Sections[0].Paragraphs.Add(endPara);

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
                        CreatedOn = DateTime.Now,
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

        private void ProcessContent(string prefix, string content, Document newDoc)
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

//private Paragraph ProcessContent(Run prefix, string content, RunProperties runProperties, WordprocessingDocument doc)
//{
//    Paragraph paragraph = new Paragraph(); 

//    if (prefix != null)
//    {
//        paragraph.Append(new Run(prefix.OuterXml));
//        paragraph.Append(new Run(new Text(" ")));
//    }

//    Run run = new Run();
//    run.RunProperties = runProperties;
//    MatchCollection matches = Regex.Matches(content, "<img>");
//    MatchCollection matches1 = Regex.Matches(content, "</img>");
//    MatchCollection matches2 = Regex.Matches(content, "<latex>");
//    MatchCollection matches3 = Regex.Matches(content, "</latex>");

//    List<int> startImg = new List<int>();
//    List<int> endImg = new List<int>();
//    List<int> startLatex = new List<int>();
//    List<int> endLatex = new List<int>();

//    foreach (Match match in matches)
//    {
//        startImg.Add(match.Index);
//    }

//    foreach (Match match in matches2)
//    {
//        startLatex.Add(match.Index);
//    }

//    if (startImg.Count > 0 || startLatex.Count > 0)
//    {
//        foreach (Match match in matches1)
//        {
//            endImg.Add(match.Index);
//        }

//        foreach (Match match in matches3)
//        {
//            endLatex.Add(match.Index);
//        }

//        int length = content.Length;

//        int indexImg = 0;
//        int indexLatex = 0;
//        int maxImg = startImg.Count;
//        int maxLatex = startLatex.Count;
//        int currentIndex = 0;

//        while (currentIndex < length)
//        {
//            if (!startImg.Contains(currentIndex) && !startLatex.Contains(currentIndex))
//            {
//                if (indexImg < maxImg || indexLatex < maxLatex)
//                {
//                    int s = 0;
//                    if (maxImg == 0 && maxLatex != 0)
//                    {
//                        s = startLatex[indexLatex];
//                    }
//                    else if (maxLatex == 0 && maxImg != 0)
//                    {
//                        s = startImg[indexImg];
//                    }
//                    else 
//                    {
//                        s = Math.Min(startImg[indexImg], startLatex[indexLatex]);
//                    }
//                    run = new Run(new Text(content.Substring(currentIndex, s - currentIndex)));
//                    paragraph.Append(run.CloneNode(true));
//                    currentIndex = s;

//                }
//                else
//                {
//                    run = new Run(new Text(content.Substring(currentIndex)));
//                    paragraph.Append(run.CloneNode(true));
//                    break;
//                }

//            }
//            else if (startImg.Contains(currentIndex))
//            {
//                // Add drawing to paragraph
//                AddImageToParagraph(content.Substring(startImg[indexImg] + 5, endImg[indexImg] - startImg[indexImg] - 5), paragraph, doc);
//                currentIndex = endImg[indexImg] + 6;
//                indexImg++;
//            }
//            else
//            {
//                string mathML = content.Substring(startLatex[indexLatex] + 7, endLatex[indexLatex] - startLatex[indexLatex] - 7);

//                Run run1 = new Run();
//                run1.InnerXml = "<m:oMathPara><m:oMath><m:r><m:t>" + mathML + "</m:t></m:r></m:oMath></m:oMathPara>";

//                paragraph.Append(run1.CloneNode(true));

//                currentIndex = endLatex[indexLatex] + 8;
//                indexLatex++;
//            }
//        }
//    }
//    else
//    {
//        run.Append(new Text(content));
//        paragraph.Append(run.CloneNode(true));
//    }

//    return paragraph;
//}

//private void AddImageToParagraph(string base64, Paragraph paragraph, WordprocessingDocument doc)
//{
//    byte[] imageBytes = Convert.FromBase64String(base64.Trim());
//    long imageWidth = 0;
//    long imageHeight = 0;
//    // Create image part
//    MainDocumentPart mainPart = doc.MainDocumentPart;
//    using (MemoryStream ms = new MemoryStream(imageBytes))
//    {   
//        using (Image image = Image.FromStream(ms))
//        {
//            imageWidth = image.Width;
//            imageHeight = image.Height;
//        }
//        var imagePart = mainPart.AddImagePart(ImagePartType.Png);
//        imagePart.FeedData(ms);
//        string relationshipId = mainPart.GetIdOfPart(imagePart);

//        // Add drawing to paragraph
//        Drawing drawing = CreateImageDrawing(relationshipId, imageWidth, imageHeight);
//        paragraph.AppendChild(new Run(drawing));
//    }

//}
//private Drawing CreateImageDrawing(string relationshipId, long imgWidth, long imgHeight)
//{
//    // Create Drawing object
//    Drawing drawing = new Drawing();

//    // Create a BlipFill element with a reference to the ImagePart
//    Pic.BlipFill blipFill = new Pic.BlipFill(
//        new A.Blip() { Embed = relationshipId },
//        new A.Stretch(new A.FillRectangle())
//    );

//    imgWidth = (long)((double)imgWidth * 9525 / 2.29);
//    imgHeight = (long)((double)imgHeight * 9525 / 2.29);

//    // Create ShapeProperties for the image
//    Pic.ShapeProperties shapeProperties = new Pic.ShapeProperties(
//        new A.Transform2D(
//            new A.Offset() { X = 0L, Y = 0L },
//            new A.Extents() { Cx = imgWidth, Cy = imgHeight }
//        ),
//        new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
//    );

//    // Create GraphicData with the BlipFill and ShapeProperties
//    A.GraphicData graphicData = new A.GraphicData(
//        new Pic.Picture(
//            new Pic.NonVisualPictureProperties(),
//            blipFill,
//            shapeProperties
//        )
//    );

//    // Create Graphic object with GraphicData
//    A.Graphic graphic = new A.Graphic(graphicData);

//    // Create Inline object to specify the image position and size
//    DW.Inline inline = new DW.Inline(
//        new DW.Extent() { Cx = imgWidth, Cy = imgHeight }, // Adjust the size as needed
//        new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
//        new DW.DocProperties() { Id = (UInt32Value)1U, Name = "Image 1" },
//        new DW.NonVisualGraphicFrameDrawingProperties(
//            new A.GraphicFrameLocks() { NoChangeAspect = true }
//        ),
//        graphic
//    );

//    // Append the Inline object to the Drawing object
//    drawing.Append(inline);

//    return drawing;
//}

//public async Task<byte[]> CreateTestPaper(DetailOfPaper detailOfPaper)
//{
//    try
//    {
//        //header
//        // trường của creator
//        // name cuộc thi : description của exam
//        // môn học - khối :  lúc chọn exam có lựa môn - khối thì lấy từ class
//        // mã đề thi, save paper xong lấy mã đề thi update vào

//        string filePath = @"D:\FinalCapstone\Other\TemplateDe1.docx";
//        using (var stream = new MemoryStream())
//        {
//            FileStream oldFile = File.OpenRead(filePath);
//            oldFile.CopyTo(stream);
//            using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
//            {
//                var shuffledQuestionIds = detailOfPaper.QuestionIds.OrderBy(q => Guid.NewGuid()).ToList();

//                var shuffledQuestions = new List<Paragraph>();

//                //foreach (var question in shuffledQuestions)
//                //{
//                //    //get format of question
//                //    var questionFormat = doc.MainDocumentPart.Document.Body.Elements<Paragraph>().FirstOrDefault(p => p.InnerText.Contains(question.ToString()));

//                //}

//                var header = doc.MainDocumentPart.Document.Body.Elements<Table>().ToList();

//                var paragraphs = doc.MainDocumentPart.Document.Body.Elements<Paragraph>().ToList();

//                foreach (var h in header)
//                {
//                    var columns = h.Elements<TableRow>().First().Elements<TableCell>().Count();

//                    foreach (var row in h.Elements<TableRow>())
//                    {
//                        foreach (var cell in row.Elements<TableCell>())
//                        {
//                            foreach (var rc in cell.Elements<Paragraph>())
//                            {
//                                foreach (var run in rc.Elements<Run>())
//                                {
//                                    var text = run.GetFirstChild<Text>();

//                                    if (text.InnerText == "{School Name}")
//                                    {
//                                        text.Text = "name";
//                                    }
//                                    else if (text.InnerText == "{Name}")
//                                    {
//                                        text.Text = "name";
//                                    }
//                                    else if (text.InnerText == "{Subject} - Lớp {Grade}")
//                                    {
//                                        text.Text = "name";
//                                    }
//                                    else if (text.InnerText == "Thời gian làm bài: {Time}p")
//                                    {
//                                        text.Text = $"Thời gian làm bài: {detailOfPaper.TimeOfTest}p";
//                                    }
//                                    else if (text.InnerText == "Mã đề thi: {Code}")
//                                    {
//                                        text.Text = $"Mã đề thi: {"000"}";
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                Paragraph end = paragraphs.Last();
//                RunProperties begin = new RunProperties();
//                RunProperties content = new RunProperties();

//                foreach (var p in paragraphs[1])
//                {
//                    if (p.InnerText == paragraphs[1].InnerText.Substring(0,1))
//                    {
//                        begin = p.GetFirstChild<RunProperties>();
//                        begin.Append(new Bold());
//                    }
//                    else if (p.InnerText == paragraphs[1].InnerText.Substring(6))
//                    {
//                        content = p.GetFirstChild<RunProperties>(); 
//                    }
//                }

//                // Xóa từng đoạn văn
//                foreach (var paragraph in paragraphs)
//                {
//                    paragraph.Remove();
//                }

//                // Lưu tài liệu sau khi xóa
//                doc.MainDocumentPart.Document.Save();

//                int numOfQuestion = 1;
//                foreach (var questionId in shuffledQuestionIds)
//                {
//                    //chèn câu
//                    Run run = new Run(new Text($"Câu {numOfQuestion}. "));

//                    if (begin != null)
//                    {
//                        run.RunProperties = begin.CloneNode(true) as RunProperties;
//                    }

//                    var question = await _unitOfWork.QuestionRepo.FindByField(q => q.QuestionId == questionId);

//                    List<ParagraphProcessing> paragraphProcessings = new List<ParagraphProcessing>();

//                    Paragraph contentQuestion = ProcessContent(run, question.QuestionPart, content.CloneNode(true) as RunProperties, doc);

//                    // Thêm đáp án
//                    Run run2 = new Run(new Text("\tA. "));
//                    run2.RunProperties = begin.CloneNode(true) as RunProperties;
//                    Paragraph answer1 = ProcessContent(run2, question.Answer1, content.CloneNode(true) as RunProperties, doc);

//                    run2 = new Run(new Text("\tB. "));
//                    run2.RunProperties = begin.CloneNode(true) as RunProperties;
//                    Paragraph answer2 = ProcessContent(run2, question.Answer2, content.CloneNode(true) as RunProperties, doc);

//                    run2 = new Run(new Text("\tC. "));
//                    run2.RunProperties = begin.CloneNode(true) as RunProperties;
//                    Paragraph answer3 = ProcessContent(run2, question.Answer3, content.CloneNode(true) as RunProperties, doc);

//                    run2 = new Run(new Text("\tD. "));
//                    run2.RunProperties = begin.CloneNode(true) as RunProperties;
//                    Paragraph answer4 = ProcessContent(run2, question.Answer4, content.CloneNode(true) as RunProperties, doc);

//                    doc.MainDocumentPart.Document.Body.Append(contentQuestion);
//                    doc.MainDocumentPart.Document.Body.Append(answer1);
//                    doc.MainDocumentPart.Document.Body.Append(answer2);
//                    doc.MainDocumentPart.Document.Body.Append(answer3);
//                    doc.MainDocumentPart.Document.Body.Append(answer4);
//                    doc.MainDocumentPart.Document.Body.Append(new Paragraph(new Break()));

//                    numOfQuestion++;
//                }
//                doc.MainDocumentPart.Document.Body.Append(end);
//                doc.Save();
//            }

//            return stream.ToArray();
//        }
//        return null;

//    }
//    catch (Exception e)
//    {
//        throw new Exception("Lỗi ở DocumentServices - UpdateDocument: " + e.Message);
//    }
//}