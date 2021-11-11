using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Dto
{
    public class GetStudentDto
    {
        [Required]
        public string MatNo { get; set; }
        [Required]
        public string CourseCode { get; set; }
        [Required]       
        public string hall { get; set; }
    }
}
