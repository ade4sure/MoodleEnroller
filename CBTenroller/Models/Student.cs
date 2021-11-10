using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CBTenroller.Models
{
    public class Student
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string UserNumber { get; set; }
        public string FullName { get; set; } //for moodle & Auth consumption
        [MaxLength(25)]
        public string Email { get; set; }
        [MaxLength(6)]
        public string DeptCode { get; set; }
        [MaxLength(50)]
        public string ImageName { get; set; }
        public string ImageURL
        {
            get
            {
                if (ImageName != null)
                {
                    return "/images/" + ImageName;
                }
                
                return "/images/Avatar.png";
            }
        }
        public string FIR { get; set; }
        public DateTime FIR_Date { get; set; }

    }

    public class Fingers
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string UserId { get; set; }
        public string FIR { get; set; }
       
        public DateTime FIR_Date { get; set; }

    }


}
