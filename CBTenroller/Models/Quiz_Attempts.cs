using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTenroller.Models
{
    [Table("mdl_quiz_attempts")]
    public class Quiz_Attempts
    {
        [Key]
        public Int64 id { get; set; }        
        public Int64 quiz { get; set; }       
        public Int64 userid { get; set; }
        public Int32 attempt { get; set; }
        public Int64 uniqueid { get; set; }
        public string layout { get; set; }
        public Int64 currentpage { get; set; }
        public Int16 preview { get; set; }
        public string state { get; set; }
        public Int64 timestart { get; set; }
        public Int64 timefinish { get; set; }
        public Int64 timemodified { get; set; }
        public Int64 timecheckstate { get; set; }
        public decimal sumgrades { get; set; }
    }


}
