using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementV1._1.MVVM.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? MiddleName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;
        
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        // Emergency Contact
        [MaxLength(100)]
        public string EmergencyContactName { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string EmergencyContactPhone { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        
        // Admission Details
        public DateTime EnrollmentDate { get; set; }
        
        [MaxLength(50)]
        public string EnrollmentStatus { get; set; } = "Active";
        
        [MaxLength(50)]
        public string? StudentId { get; set; }
        
        // Navigation Properties
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual HealthRecord? HealthRecord { get; set; }
        public virtual ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
        public virtual ICollection<Grade>? Grades { get; set; }
        public virtual ICollection<BehaviorRecord>? BehaviorRecords { get; set; }
    }
}
