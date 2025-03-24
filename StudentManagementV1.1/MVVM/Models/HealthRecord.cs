using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class HealthRecord
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        [MaxLength(500)]
        public string? Allergies { get; set; }
        
        [MaxLength(500)]
        public string? Medications { get; set; }
        
        [MaxLength(500)]
        public string? MedicalConditions { get; set; }
        
        public bool EmergencyCarePermission { get; set; }
        
        [MaxLength(100)]
        public string? BloodType { get; set; }
        
        public DateTime? LastPhysicalExam { get; set; }
        
        [MaxLength(200)]
        public string? PhysicianName { get; set; }
        
        [MaxLength(15)]
        public string? PhysicianPhone { get; set; }
        
        // Navigation Property
        public virtual Student? Student { get; set; }
        public virtual ICollection<Immunization>? Immunizations { get; set; }
    }
    
    public class Immunization
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("HealthRecord")]
        public int HealthRecordId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public DateTime DateAdministered { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        [MaxLength(200)]
        public string? Notes { get; set; }
        
        // Navigation Property
        public virtual HealthRecord? HealthRecord { get; set; }
    }
}
