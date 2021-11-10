using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CBTenroller.Models
{
    public class MoodleUser
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string Number { get; set; }
        [MaxLength(60)]
        public string FullName { get; set; } //for moodle & Auth consumption
        [MaxLength(60)]
        public string Surname { get; set; } //for moodle & Auth consumption
        [MaxLength(60)]
        public string FirstName { get; set; } //for moodle & Auth consumption
        [MaxLength(25)]
        public string Email { get; set; }
        [MaxLength(10)]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public MoodleUser()
        {
            CourseEnrollments = new Collection<CourseEnrollment>();
        }
    }

    
}
