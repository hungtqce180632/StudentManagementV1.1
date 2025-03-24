using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementV1._1.MVVM.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        
        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        
        [ForeignKey("ClassSchedule")]
        public int? ClassScheduleId { get; set; }
        
        public DateTime Date { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Present"; // Present, Absent, Excused, Tardy
        
        [MaxLength(500)]
        public string? Remarks { get; set; }
        
        public TimeSpan? TimeIn { get; set; }
        
        public TimeSpan? TimeOut { get; set; }
        
        // Navigation Properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }
        public virtual ClassSchedule? ClassSchedule { get; set; }
    }
}
