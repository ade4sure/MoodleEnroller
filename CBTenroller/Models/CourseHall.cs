using System;

namespace CBTenroller.Models
{
    public class CourseHall
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }
        public bool Active
        {
            get
            {
                if (StartTime < DateTime.Now && StopTime > DateTime.Now) return true;
                return false;
            }
        }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }       
        public bool EnforceBatch { get; set; }
        public int StartNumber { get; set; }
        public int LastNumber { get; set; }
    }

    
}
