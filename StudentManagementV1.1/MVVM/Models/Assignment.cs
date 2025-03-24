using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public decimal TotalPoints { get; set; } = 100;
        
        [MaxLength(50)]
        public string AssignmentType { get; set; } = "Homework"; // Homework, Project, Quiz, Exam
        
        public decimal Weight { get; set; } = 1.0m;
        
        public bool IsPublished { get; set; } = false;
        
        // Navigation Properties
        public virtual Course? Course { get; set; }
        public virtual ICollection<AssignmentSubmission>? Submissions { get; set; }
        public virtual ICollection<Grade>? Grades { get; set; }
    }
    
    public class AssignmentSubmission
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public string? FilePath { get; set; }
        
        public bool IsLate { get; set; }
        
        // Navigation Properties
        public virtual Assignment? Assignment { get; set; }
        public virtual Student? Student { get; set; }
    }
}
