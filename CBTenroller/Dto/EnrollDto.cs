using System;
using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Dto
{
    public class EnrollDto
    {
        [Required]
        public string MatNo { get; set; }
        [Required]
        public string CourseCode { get; set; }
        [Required]
        public string Pwd { get; set; }
        public string Token { get; set; }
        public DateTime EnrollDate { get; set; }
    }
}
