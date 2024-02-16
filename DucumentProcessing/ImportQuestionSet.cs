using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc;
using System.Drawing;
using System.IO;
using System.Text;
using Spire.Doc.Fields.OMath;
using System.Xml.Xsl;
using System.Xml;

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
                xslt.Load("D:\\Capstone SP24\\backend\\DucumentProcessing\\mmltex\\mmltex.xsl", sets, resolver);

                // Transform the MathML to LaTeX
                StringWriter latex = new StringWriter();
                xslt.Transform(reader, null, latex);
                return latex.ToString();
            }
        }
    }
}
