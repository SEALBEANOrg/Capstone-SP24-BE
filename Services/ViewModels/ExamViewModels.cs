using Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class ExamViewModels
    {
        public Guid ExamId { get; set; }
        public string? Description { get; set; } //name of exam
        public int TestCode { get; set; }
        public string ClassName { get; set; }
        public string HasMark { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ResultForScan
    {
        [JsonPropertyName("Image")]
        public string image { get; set; }
        [JsonPropertyName("Result")]
        public string result { get; set; }
        [JsonPropertyName("PaperCode")]
        public int paperCode { get; set; }
        [JsonPropertyName("StudentNo")]
        public int studentNo { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("Image")]
        public string image { get; set; }
        [JsonPropertyName("Result")]
        public string result { get; set; }
        //[JsonPropertyName("PaperCode")]
        //public int paperCode { get; set; }
        //[JsonPropertyName("StudentNo")]
        //public int studentNo { get; set; }
    }

    public class ResultForScanViewModel
    {
        [JsonPropertyName("Image")]
        public string image { get; set; }
    }


    public class ResultToSave
    {
        public int PaperCode { get; set; }
        public string AnswersSelected { get; set; }
        [Required]
        public Guid ExamMarkId { get; set; }
    }

    public class  ClassModel
    {
        public Guid ClassId { get; set; }
        public string Name { get; set; }
    }

    public class ExamInfo
    {
        public Guid ExamId { get; set; }
        public string? Description { get; set; } //name of exam
        public int TestCode { get; set; }
        public string ClassName { get; set; }
        public string HasMark { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<ResultOfStudent> Students { get; set; }
    }

    public class ExamCreate
    {
        public string? NameOfExam { get; set; }
        public Guid ClassId { get; set; }
        public int Grade { get; set; }
        public int Duration { get; set; }
        public int Subject { get; set; }
        public List<SectionUse> Sections { get; set; }
        public int NumOfDiffPaper { get; set; }  // 2 đề
        public int NumOfPaperCode { get; set; } // mỗi đề có 5 mã thì có 10 đề
    }

    public class SectionUse
    {
        public Guid SectionId { get; set; }
        public int Difficulty { get; set; }
        public int CHCN { get; set; }
        public int NHD { get; set; }
    }
}

