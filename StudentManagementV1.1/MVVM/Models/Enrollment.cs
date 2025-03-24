using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        [MaxLength(20)]
        public string Status { get; set; } = "Enrolled"; // Enrolled, Waitlisted, Dropped, Completed
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public bool IsWaitlisted { get; set; } = false;
        
        public int? WaitlistPosition { get; set; }
        
        // Navigation Properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }
    }
}
