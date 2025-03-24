using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int CreditHours { get; set; }
        
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Department { get; set; } = string.Empty;
        
        public int? MaxEnrollment { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<CoursePrerequisite>? Prerequisites { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; }
        public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; }
    }
    
    public class CoursePrerequisite
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        [ForeignKey("PrerequisiteCourse")]
        public int PrerequisiteCourseId { get; set; }
        
        // Navigation Properties
        public virtual Course? Course { get; set; }
        public virtual Course? PrerequisiteCourse { get; set; }
    }
}
