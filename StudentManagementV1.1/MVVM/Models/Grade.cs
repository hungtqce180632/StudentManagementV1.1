using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        [ForeignKey("Assignment")]
        public int? AssignmentId { get; set; }
        
        public decimal Score { get; set; }
        
        [MaxLength(5)]
        public string LetterGrade { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string GradeType { get; set; } = "Assignment"; // Assignment, Exam, Final, etc.
        
        public decimal Weight { get; set; } = 1.0m;
        
        public DateTime DateRecorded { get; set; } = DateTime.Now;
        
        [MaxLength(500)]
        public string? Comments { get; set; }
        
        // Navigation Properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Assignment? Assignment { get; set; }
    }
}
