using CBTenroller.Models;

namespace CBTenroller.Dto
{
    public class MoodleEnrollDto
    {
        public MoodleUser MoodleUser { get; set; }
        public string CourseCode { get; set; }
        public int MoodleCourseId { get; set; }
    }
}
