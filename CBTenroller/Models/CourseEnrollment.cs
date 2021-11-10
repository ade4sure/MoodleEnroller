using System;
using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Models
{
    public class CourseEnrollment //For moodle Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int MoodleUserId { get; set; }
        public MoodleUser MoodleUser { get; set; }
        [MaxLength(10)]
        public string MoodleCourseCode { get; set; } //For Moodle consumption
        [MaxLength(15)]
        public string StudentNumber { get; set; } //For Moodle consumption
        [MaxLength(10)]
        public DateTime EnrolledAt { get; set; }

    }

    
}
