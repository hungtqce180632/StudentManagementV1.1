using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        
        public DateTime HireDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, On Leave, Terminated
        
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Qualifications { get; set; }
        
        [MaxLength(500)]
        public string? Biography { get; set; }
        
        // Navigation Properties
        public virtual ICollection<CourseAssignment>? CourseAssignments { get; set; }
    }
}
