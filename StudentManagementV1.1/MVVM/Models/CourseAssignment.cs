using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class CourseAssignment
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        [MaxLength(50)]
        public string Role { get; set; } = "Instructor"; // Instructor, Assistant, etc.
        
        public DateTime AssignmentDate { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual Teacher? Teacher { get; set; }
        public virtual Course? Course { get; set; }
    }
}
