using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class ClassSchedule
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        
        [MaxLength(100)]
        public string Room { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string DayOfWeek { get; set; } = string.Empty; // Monday, Tuesday, etc.
        
        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsRecurring { get; set; } = true;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        // Navigation Properties
        public virtual Course? Course { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
    }
}
