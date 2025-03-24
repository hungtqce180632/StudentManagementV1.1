using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class BehaviorRecord
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        public DateTime IncidentDate { get; set; }
        
        [MaxLength(100)]
        public string IncidentType { get; set; } = string.Empty; // Positive, Negative, Neutral
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Location { get; set; }
        
        [MaxLength(100)]
        public string? ReportedBy { get; set; }
        
        [MaxLength(100)]
        public string? ActionTaken { get; set; }
        
        public bool ParentNotified { get; set; }
        
        public DateTime? ParentNotificationDate { get; set; }
        
        [MaxLength(500)]
        public string? FollowUpNotes { get; set; }
        
        // Navigation Property
        public virtual Student? Student { get; set; }
    }
}
