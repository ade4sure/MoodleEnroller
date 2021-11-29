using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string Matric { get; set; }
        public string FullName { get; set; } //for moodle & Auth consumption
      
        public int Level { get; set; }
       
        public decimal AmountPaid { get; set; }
        [MaxLength(10)]
        public string Semester { get; set; }
        [MaxLength(10)]
        public string Session { get; set; }
        [MaxLength(5)]
        public string Payment_Type { get; set; }
      

    }


}
