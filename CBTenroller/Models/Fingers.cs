using System;
using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Models
{
    public class Fingers
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string UserId { get; set; }
        public string FIR { get; set; }
       
        public DateTime FIR_Date { get; set; }

    }


}
