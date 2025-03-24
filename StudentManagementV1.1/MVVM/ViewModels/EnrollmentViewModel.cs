using Microsoft.EntityFrameworkCore;
using StudentManagementV1._1.MVVM.Commands;
using StudentManagementV1._1.MVVM.Data;
using StudentManagementV1._1.MVVM.Models;
using StudentManagementV1._1.MVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._1.MVVM.ViewModels
{
    public class EnrollmentViewModel : ViewModelBase
    {
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Course> _courseRepository;
        
        private ObservableCollection<Enrollment> _enrollments;
        private ObservableCollection<Student> _students;
        private ObservableCollection<Course> _courses;
        private Enrollment? _selectedEnrollment;
        private Student? _selectedStudent;
        private Course? _selectedCourse;
        private string _status = "Enrolled";
        private string? _notes;
        private bool _isWaitlisted;
        private int? _waitlistPosition;
        
        // Commands
        public ICommand AddEnrollmentCommand { get; }
        public ICommand UpdateEnrollmentCommand { get; }
        public ICommand DeleteEnrollmentCommand { get; }
        public ICommand ClearFormCommand { get; }
        
        public ObservableCollection<Enrollment> Enrollments
        {
            get => _enrollments;
            set => SetProperty(ref _enrollments, value);
        }
        
        public ObservableCollection<Student> Students
        {
            get => _students;
            set => SetProperty(ref _students, value);
        }
        
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }
        
        public Enrollment? SelectedEnrollment
        {
            get => _selectedEnrollment;
            set
            {
                if (SetProperty(ref _selectedEnrollment, value) && value != null)
                {
                    // Populate the form fields with selected enrollment
                    SelectedStudent = Students.FirstOrDefault(s => s.Id == value.StudentId);
                    SelectedCourse = Courses.FirstOrDefault(c => c.Id == value.CourseId);
                    Status = value.Status;
                    Notes = value.Notes;
                    IsWaitlisted = value.IsWaitlisted;
                    WaitlistPosition = value.WaitlistPosition;
                }
            }
        }
        
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set => SetProperty(ref _selectedStudent, value);
        }
        
        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set => SetProperty(ref _selectedCourse, value);
        }
        
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        public string? Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
        
        public bool IsWaitlisted
        {
            get => _isWaitlisted;
            set => SetProperty(ref _isWaitlisted, value);
        }
        
        public int? WaitlistPosition
        {
            get => _waitlistPosition;
            set => SetProperty(ref _waitlistPosition, value);
        }
        
        public EnrollmentViewModel()
        {
            // Initialize repositories with context
            var context = new StudentManagementContext();
            _enrollmentRepository = new Repository<Enrollment>(context);
            _studentRepository = new Repository<Student>(context);
            _courseRepository = new Repository<Course>(context);
            
            // Initialize collections
            _enrollments = new ObservableCollection<Enrollment>();
            _students = new ObservableCollection<Student>();
            _courses = new ObservableCollection<Course>();
            
            // Initialize commands
            AddEnrollmentCommand = new RelayCommand(async param => await AddEnrollment(), CanAddUpdateEnrollment);
            UpdateEnrollmentCommand = new RelayCommand(async param => await UpdateEnrollment(), CanAddUpdateEnrollment);
            DeleteEnrollmentCommand = new RelayCommand(async param => await DeleteEnrollment(), param => SelectedEnrollment != null);
            ClearFormCommand = new RelayCommand(param => ClearForm());
            
            // Load data
            LoadAllData();
        }
        
        private async void LoadAllData()
        {
            try
            {
                // Load students
                var students = await _studentRepository.GetAllAsync();
                Students.Clear();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
                
                // Load courses
                var courses = await _courseRepository.GetAllAsync();
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
                
                // Load enrollments with navigation properties
                using (var context = new StudentManagementContext())
                {
                    var enrollments = await context.Enrollments
                        .Include(e => e.Student)
                        .Include(e => e.Course)
                        .ToListAsync();
                        
                    Enrollments.Clear();
                    foreach (var enrollment in enrollments)
                    {
                        Enrollments.Add(enrollment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task AddEnrollment()
        {
            try
            {
                if (SelectedStudent == null || SelectedCourse == null)
                {
                    MessageBox.Show("Please select a student and a course.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Check if student is already enrolled in this course
                var existingEnrollment = Enrollments.FirstOrDefault(e => 
                    e.StudentId == SelectedStudent.Id && 
                    e.CourseId == SelectedCourse.Id &&
                    e.Status != "Dropped");
                    
                if (existingEnrollment != null)
                {
                    MessageBox.Show("This student is already enrolled in this course.", "Duplicate Enrollment", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var enrollment = new Enrollment
                {
                    StudentId = SelectedStudent.Id,
                    CourseId = SelectedCourse.Id,
                    EnrollmentDate = DateTime.Now,
                    Status = Status,
                    Notes = Notes,
                    IsWaitlisted = IsWaitlisted,
                    WaitlistPosition = IsWaitlisted ? WaitlistPosition : null
                };
                
                await _enrollmentRepository.AddAsync(enrollment);
                await _enrollmentRepository.SaveChangesAsync();
                
                // Reload to get the navigation properties
                using (var context = new StudentManagementContext())
                {
                    enrollment = await context.Enrollments
                        .Include(e => e.Student)
                        .Include(e => e.Course)
                        .FirstOrDefaultAsync(e => e.Id == enrollment.Id);
                        
                    if (enrollment != null)
                    {
                        Enrollments.Add(enrollment);
                    }
                }
                
                ClearForm();
                MessageBox.Show("Enrollment added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding enrollment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateEnrollment()
        {
            try
            {
                if (SelectedEnrollment == null || SelectedStudent == null || SelectedCourse == null)
                    return;
                    
                SelectedEnrollment.StudentId = SelectedStudent.Id;
                SelectedEnrollment.CourseId = SelectedCourse.Id;
                SelectedEnrollment.Status = Status;
                SelectedEnrollment.Notes = Notes;
                SelectedEnrollment.IsWaitlisted = IsWaitlisted;
                SelectedEnrollment.WaitlistPosition = IsWaitlisted ? WaitlistPosition : null;
                
                await _enrollmentRepository.UpdateAsync(SelectedEnrollment);
                await _enrollmentRepository.SaveChangesAsync();
                
                // Reload to refresh navigation properties
                LoadAllData();
                
                MessageBox.Show("Enrollment updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating enrollment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task DeleteEnrollment()
        {
            try
            {
                if (SelectedEnrollment == null)
                    return;
                    
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete this enrollment?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    await _enrollmentRepository.DeleteAsync(SelectedEnrollment);
                    await _enrollmentRepository.SaveChangesAsync();
                    Enrollments.Remove(SelectedEnrollment);
                    ClearForm();
                    MessageBox.Show("Enrollment deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting enrollment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            SelectedEnrollment = null;
            SelectedStudent = null;
            SelectedCourse = null;
            Status = "Enrolled";
            Notes = null;
            IsWaitlisted = false;
            WaitlistPosition = null;
        }
        
        private bool CanAddUpdateEnrollment(object? param)
        {
            return SelectedStudent != null && SelectedCourse != null && !string.IsNullOrWhiteSpace(Status);
        }
    }
}
