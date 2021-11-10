using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Dto
{
    public class GetStudentDto
    {
        [Required]
        public int MatNo { get; set; }
        [Required]
        public string CourseCode { get; set; }
        [Required]       
        public string hall { get; set; }
    }
}
