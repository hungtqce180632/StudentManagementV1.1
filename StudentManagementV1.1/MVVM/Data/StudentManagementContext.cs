using Microsoft.EntityFrameworkCore;
using StudentManagementV1._1.MVVM.Models;

namespace StudentManagementV1._1.MVVM.Data
{
    public class StudentManagementContext : DbContext
    {
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<HealthRecord> HealthRecords { get; set; } = null!;
        public DbSet<Immunization> Immunizations { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<CourseAssignment> CourseAssignments { get; set; } = null!;
        public DbSet<Grade> Grades { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; } = null!;
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
        public DbSet<ClassSchedule> ClassSchedules { get; set; } = null!;
        public DbSet<BehaviorRecord> BehaviorRecords { get; set; } = null!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=StudentManagementDB;Trusted_Connection=True;");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships and constraints
            
            // One-to-One: Student - HealthRecord
            modelBuilder.Entity<Student>()
                .HasOne(s => s.HealthRecord)
                .WithOne(h => h.Student)
                .HasForeignKey<HealthRecord>(h => h.StudentId);
                
            // One-to-Many: Course - CoursePrerequisite
            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.PrerequisiteCourse)
                .WithMany()
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // One-to-Many: Course - Assignment
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId);
                
            // One-to-Many: Assignment - AssignmentSubmission
            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(sub => sub.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(sub => sub.AssignmentId);
                
            // One-to-Many: Course - ClassSchedule
            modelBuilder.Entity<ClassSchedule>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.ClassSchedules)
                .HasForeignKey(cs => cs.CourseId);
                
            // Many-to-Many: Teacher - Course (through CourseAssignment)
            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Teacher)
                .WithMany(t => t.CourseAssignments)
                .HasForeignKey(ca => ca.TeacherId);
                
            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Course)
                .WithMany()
                .HasForeignKey(ca => ca.CourseId);
        }
    }
}
