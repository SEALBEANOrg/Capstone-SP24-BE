using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc;
using System.Drawing;
using System.IO;
using System.Text;
using Spire.Doc.Fields.OMath;
using System.Xml.Xsl;
using System.Xml;
using System.Linq;
using ExagenSharedProject;

namespace DucumentProcessing
{
    public class ImportQuestionSet
    {
        public static string ReadDocx(string filePath)
        {
            // Load the document
            Document document = new Document();
            document.LoadFromFile(filePath);

            StringBuilder sb = new StringBuilder();

            // Iterate through sections
            foreach (Section section in document.Sections)
            {
                // Iterate through paragraphs
                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    // Iterate through document objects in paragraph
                    foreach (DocumentObject docObject in paragraph.ChildObjects)
                    {
                        switch (docObject.DocumentObjectType)
                        {
                            case DocumentObjectType.TextRange:
                                TextRange? textRange = docObject as TextRange;
                                if (textRange != null)
                                {
                                    sb.Append(textRange.Text);
                                }
                                break;
                            case DocumentObjectType.Picture:
                                DocPicture? pic = docObject as DocPicture;
                                if (pic != null && pic.Image != null)
                                {
                                    ImageConverter converter = new ImageConverter();
                                    byte[] imageBytes = (byte[])converter.ConvertTo(pic.Image, typeof(byte[]));

                                    // Convert byte array to Base64 String
                                    string base64String = "<img>" + Convert.ToBase64String(imageBytes) + "</img>";
                                    sb.Append(base64String);
                                }
                                break;
                            case DocumentObjectType.OfficeMath:
                                OfficeMath? math = docObject as OfficeMath;
                                if (math != null)
                                {
                                    string mathML = math.ToMathMLCode();
                                    mathML = "<latex>" + ConvertMathMLToLatex(mathML).Replace("$ ", "").Replace("$", "") + "</latex>";
                                    sb.Append(mathML);
                                }
                                break;
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string ConvertMathMLToLatex(string mathML)
        {
            // Load the MathML
            using (var reader = XmlReader.Create(new StringReader(mathML)))
            {
                // Load the XSLT
                XslCompiledTransform xslt = new XslCompiledTransform();
                XsltSettings sets = new XsltSettings(true, true);
                var resolver = new XmlUrlResolver();
                xslt.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mmltex/mmltex.xsl"), sets, resolver);

                // Transform the MathML to LaTeX
                StringWriter latex = new StringWriter();
                xslt.Transform(reader, null, latex);
                return latex.ToString();
            }
        }

        public static Question SplitQuestionAndAnswer(string strDocxContent)
        {
            Question question = new Question();
            //QuestionPart = text before the first "A. " or "A/ "
            int questionType = 0; // 0: A. 1: A/
            int endOfQuestion = strDocxContent.IndexOf("A. ");
            if (endOfQuestion == -1)
            {
                questionType = 1;
                endOfQuestion = strDocxContent.IndexOf("A/ ");
            }
            question.QuestionPart = strDocxContent.Substring(0, endOfQuestion);
            if (questionType == 0)
            {
                // question.Answer1 = text between "A. " and "B. "
                int endOf1stAnswer = strDocxContent.IndexOf("B. ");
                question.Answer1 = strDocxContent.Substring(endOfQuestion + 3, endOf1stAnswer - endOfQuestion - 3).Trim();

                // question.Answer2 = text between "B. " and "C. "
                int endOf2ndAnswer = strDocxContent.IndexOf("C. ");
                question.Answer2 = strDocxContent.Substring(endOf1stAnswer + 3, endOf2ndAnswer - endOf1stAnswer - 3).Trim();

                // question.Answer3 = text between "C. " and "D. "
                int endOf3rdAnswer = strDocxContent.IndexOf("D. ");
                question.Answer3 = strDocxContent.Substring(endOf2ndAnswer + 3, endOf3rdAnswer - endOf2ndAnswer - 3).Trim();

                // question.Answer4 = text between "D. " and ("Đáp án" or "Độ khó")
                int endOf4thAnswer = strDocxContent.IndexOf("Đáp án");
                if (endOf4thAnswer > strDocxContent.IndexOf("Độ khó"))
                {
                    endOf4thAnswer = strDocxContent.IndexOf("Độ khó");
                }
                if (endOf4thAnswer == -1)
                {
                    endOf4thAnswer = strDocxContent.Length;
                }
                question.Answer4 = strDocxContent.Substring(endOf3rdAnswer + 3, endOf4thAnswer - endOf3rdAnswer - 3).Trim();

                question.CorrectAnswer = GetAnswer(strDocxContent.Substring(endOf4thAnswer));

                question.Difficulty = GetDifficulty(strDocxContent.Substring(endOf4thAnswer));
            }
            else
            {
                // question.Answer1 = text between "A/ " and "B/ "
                int endOf1stAnswer = strDocxContent.IndexOf("B/ ");
                question.Answer1 = strDocxContent.Substring(endOfQuestion + 3, endOf1stAnswer - endOfQuestion - 3).Trim();

                // question.Answer2 = text between "B/ " and "C/ "
                int endOf2ndAnswer = strDocxContent.IndexOf("C/ ");
                question.Answer2 = strDocxContent.Substring(endOf1stAnswer + 3, endOf2ndAnswer - endOf1stAnswer - 3).Trim();

                // question.Answer3 = text between "C/ " and "D/ "
                int endOf3rdAnswer = strDocxContent.IndexOf("D/ ");
                question.Answer3 = strDocxContent.Substring(endOf2ndAnswer + 3, endOf3rdAnswer - endOf2ndAnswer - 3).Trim();

                // question.Answer4 = text between "D/ " and "Đáp án"
                int endOf4thAnswer = strDocxContent.IndexOf("Đáp án");
                if (endOf4thAnswer == -1)
                {
                    endOf4thAnswer = strDocxContent.Length;
                }
                question.Answer4 = strDocxContent.Substring(endOf3rdAnswer + 3, endOf4thAnswer - endOf3rdAnswer - 3).Trim();

                question.CorrectAnswer = GetAnswer(strDocxContent.Substring(endOf4thAnswer));

                question.Difficulty = GetDifficulty(strDocxContent.Substring(endOf4thAnswer));
            }

            return question;
        }

        private static int? GetDifficulty(string strQuestionContext)
        {
            strQuestionContext= strQuestionContext.ToLower();
            //find in strDocxContent if there is a "NB" return 0, "TH" return 1, "VD" return 2, "VDC" return 3
            if (strQuestionContext.Contains("nb") || strQuestionContext.Contains("nhận biết"))
            {
                return 0;
            }
            else if (strQuestionContext.Contains("vdc") || strQuestionContext.Contains("vận dụng cao"))
            {
                return 3;
            }
            else if (strQuestionContext.Contains("vd") || strQuestionContext.Contains("vdt") || strQuestionContext.Contains("vận dụng"))
            {
                return 2;
            }
            else if (strQuestionContext.Contains("th") || strQuestionContext.Contains("thông hiểu"))
            {
                return 1;
            }
            return null;
        }

        private static string GetAnswer(string str)
        {
            try
            {
                //loop from the back to find the first letter is the Answer
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    if (char.IsLetter(str[i]))
                    {
                        //Check if the letter is A, B, C, D
                        if ((str[i] == 'A' || str[i] == 'B' || str[i] == 'C' || str[i] == 'D') && !char.IsLetter(str[i - 1]))
                            return str[i].ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static List<Question> ImportQuestions(string filePath)
        {
            List<Question> questions = new List<Question>();
            string strDocxContent = ReadDocx(filePath);

            //Delete before "Câu"
            int index = strDocxContent.IndexOf("Câu");
            strDocxContent = strDocxContent.Substring(index);

            //Split by "Câu"
            string[] arrDocxContent = strDocxContent.Split("Câu");

            //First element is empty
            for (int i = 1; i < arrDocxContent.Length; i++)
            {
                //Delete until meet the first letter 
                int index1 = 0;
                for (int j = 0; j < arrDocxContent[i].Length; j++)
                {
                    if (char.IsLetter(arrDocxContent[i][j]))
                    {
                        index1 = j;
                        break;
                    }
                }
                arrDocxContent[i] = arrDocxContent[i].Substring(index1);
                Console.WriteLine(SplitQuestionAndAnswer(arrDocxContent[i]).ToString() + '\n');
                questions.Add(SplitQuestionAndAnswer(arrDocxContent[i]));
            }

            return questions;
        }
    }

    public class Question
    {
        public string QuestionPart { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public int? Difficulty { get; set;} 

        public override string? ToString()
        {
            //Print the question and its answers
            return "Question: " + QuestionPart + '\n' + "A: " + Answer1 + '\n' + "B: " + Answer2 + '\n' + "C: " + Answer3 + '\n' + "D: " + Answer4 + '\n' + "Đáp án: " + CorrectAnswer + '\n' + "Độ khó: " +
                //Display the value of Difficulty in SharedProject
                EnumStatus.Difficulty.FirstOrDefault(x => x.Key == Difficulty).Value;

        }
    }
}
