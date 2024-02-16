using DucumentProcessing;
using System.Text;

Console.OutputEncoding = System.Text.Encoding.Unicode;

string result = ImportQuestionSet.ReadDocx("D:\\Capstone SP24\\backend\\DucumentProcessing\\LichSu12.docx");
File.WriteAllText("output.txt", result, Encoding.Unicode);
Console.WriteLine(result);
