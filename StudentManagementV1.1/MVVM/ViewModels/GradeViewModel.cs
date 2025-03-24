using Microsoft.EntityFrameworkCore;
using StudentManagementV1._1.MVVM.Commands;
using StudentManagementV1._1.MVVM.Data;
using StudentManagementV1._1.MVVM.Models;
using StudentManagementV1._1.MVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace StudentManagementV1._1.MVVM.ViewModels
{
    public class GradeViewModel : ViewModelBase
    {
        private readonly IRepository<Grade> _gradeRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        
        private ObservableCollection<Grade> _grades;
        private ObservableCollection<Student> _students;
        private ObservableCollection<Course> _courses;
        private ObservableCollection<Assignment> _assignments;
        
        private Grade? _selectedGrade;
        private Student? _selectedStudent;
        private Course? _selectedCourse;
        private Assignment? _selectedAssignment;
        
        private decimal _score;
        private string _letterGrade = string.Empty;
        private string _gradeType = "Assignment";
        private decimal _weight = 1.0m;
        private string? _comments;
        
        private string _searchText = string.Empty;
        private ICollectionView? _gradesView;
        
        // Commands
        public ICommand AddGradeCommand { get; }
        public ICommand UpdateGradeCommand { get; }
        public ICommand DeleteGradeCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand CalculateLetterGradeCommand { get; }
        public ICommand GenerateReportCommand { get; }
        
        public ObservableCollection<Grade> Grades
        {
            get => _grades;
            set => SetProperty(ref _grades, value);
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
        
        public ObservableCollection<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value);
        }
        
        public Grade? SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                if (SetProperty(ref _selectedGrade, value) && value != null)
                {
                    // Populate the form with selected grade
                    SelectedStudent = Students.FirstOrDefault(s => s.Id == value.StudentId);
                    SelectedCourse = Courses.FirstOrDefault(c => c.Id == value.CourseId);
                    SelectedAssignment = Assignments.FirstOrDefault(a => a?.Id == value.AssignmentId);
                    Score = value.Score;
                    LetterGrade = value.LetterGrade;
                    GradeType = value.GradeType;
                    Weight = value.Weight;
                    Comments = value.Comments;
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
            set
            {
                if (SetProperty(ref _selectedCourse, value))
                {
                    // When course changes, update the available assignments
                    LoadAssignmentsForCourse();
                }
            }
        }
        
        public Assignment? SelectedAssignment
        {
            get => _selectedAssignment;
            set => SetProperty(ref _selectedAssignment, value);
        }
        
        public decimal Score
        {
            get => _score;
            set
            {
                if (SetProperty(ref _score, value))
                {
                    // Auto-calculate letter grade when score changes
                    CalculateLetterGrade();
                }
            }
        }
        
        public string LetterGrade
        {
            get => _letterGrade;
            set => SetProperty(ref _letterGrade, value);
        }
        
        public string GradeType
        {
            get => _gradeType;
            set => SetProperty(ref _gradeType, value);
        }
        
        public decimal Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }
        
        public string? Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _gradesView?.Refresh();
                }
            }
        }
        
        public GradeViewModel()
        {
            // Initialize repositories
            var context = new StudentManagementContext();
            _gradeRepository = new Repository<Grade>(context);
            _studentRepository = new Repository<Student>(context);
            _courseRepository = new Repository<Course>(context);
            _assignmentRepository = new Repository<Assignment>(context);
            
            // Initialize collections
            _grades = new ObservableCollection<Grade>();
            _students = new ObservableCollection<Student>();
            _courses = new ObservableCollection<Course>();
            _assignments = new ObservableCollection<Assignment>();
            
            // Initialize commands
            AddGradeCommand = new RelayCommand(async param => await AddGrade(), CanAddUpdateGrade);
            UpdateGradeCommand = new RelayCommand(async param => await UpdateGrade(), CanAddUpdateGrade);
            DeleteGradeCommand = new RelayCommand(async param => await DeleteGrade(), param => SelectedGrade != null);
            ClearFormCommand = new RelayCommand(param => ClearForm());
            SearchCommand = new RelayCommand(param => _gradesView?.Refresh());
            CalculateLetterGradeCommand = new RelayCommand(param => CalculateLetterGrade());
            GenerateReportCommand = new RelayCommand(param => GenerateGradeReport());
            
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
                
                // Load all assignments initially
                var assignments = await _assignmentRepository.GetAllAsync();
                Assignments.Clear();
                foreach (var assignment in assignments)
                {
                    Assignments.Add(assignment);
                }
                
                // Load grades with navigation properties
                using (var context = new StudentManagementContext())
                {
                    var grades = await context.Grades
                        .Include(g => g.Student)
                        .Include(g => g.Course)
                        .Include(g => g.Assignment)
                        .OrderByDescending(g => g.DateRecorded)
                        .ToListAsync();
                        
                    Grades.Clear();
                    foreach (var grade in grades)
                    {
                        Grades.Add(grade);
                    }
                }
                
                // Create collection view for filtering
                _gradesView = CollectionViewSource.GetDefaultView(Grades);
                _gradesView.Filter = FilterGrades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void LoadAssignmentsForCourse()
        {
            if (SelectedCourse == null)
                return;
                
            try
            {
                using (var context = new StudentManagementContext())
                {
                    var courseAssignments = await context.Assignments
                        .Where(a => a.CourseId == SelectedCourse.Id)
                        .ToListAsync();
                        
                    Assignments.Clear();
                    foreach (var assignment in courseAssignments)
                    {
                        Assignments.Add(assignment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading assignments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool FilterGrades(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;
                
            if (obj is Grade grade)
            {
                return (grade.Student?.FirstName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                       (grade.Student?.LastName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                       (grade.Course?.CourseCode?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                       (grade.Assignment?.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                       (grade.LetterGrade?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                       (grade.GradeType?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }
            
            return false;
        }
        
        private void CalculateLetterGrade()
        {
            // Simple letter grade calculation
            if (Score >= 90)
                LetterGrade = "A";
            else if (Score >= 80)
                LetterGrade = "B";
            else if (Score >= 70)
                LetterGrade = "C";
            else if (Score >= 60)
                LetterGrade = "D";
            else
                LetterGrade = "F";
        }
        
        private async Task AddGrade()
        {
            try
            {
                if (SelectedStudent == null || SelectedCourse == null)
                {
                    MessageBox.Show("Please select a student and a course.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var grade = new Grade
                {
                    StudentId = SelectedStudent.Id,
                    CourseId = SelectedCourse.Id,
                    AssignmentId = SelectedAssignment?.Id,
                    Score = Score,
                    LetterGrade = LetterGrade,
                    GradeType = GradeType,
                    Weight = Weight,
                    DateRecorded = DateTime.Now,
                    Comments = Comments
                };
                
                await _gradeRepository.AddAsync(grade);
                await _gradeRepository.SaveChangesAsync();
                
                // Reload to get navigation properties
                using (var context = new StudentManagementContext())
                {
                    grade = await context.Grades
                        .Include(g => g.Student)
                        .Include(g => g.Course)
                        .Include(g => g.Assignment)
                        .FirstOrDefaultAsync(g => g.Id == grade.Id);
                        
                    if (grade != null)
                    {
                        Grades.Add(grade);
                    }
                }
                
                ClearForm();
                MessageBox.Show("Grade added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateGrade()
        {
            try
            {
                if (SelectedGrade == null || SelectedStudent == null || SelectedCourse == null)
                    return;
                    
                SelectedGrade.StudentId = SelectedStudent.Id;
                SelectedGrade.CourseId = SelectedCourse.Id;
                SelectedGrade.AssignmentId = SelectedAssignment?.Id;
                SelectedGrade.Score = Score;
                SelectedGrade.LetterGrade = LetterGrade;
                SelectedGrade.GradeType = GradeType;
                SelectedGrade.Weight = Weight;
                SelectedGrade.Comments = Comments;
                
                await _gradeRepository.UpdateAsync(SelectedGrade);
                await _gradeRepository.SaveChangesAsync();
                
                // Reload to refresh the list with updated data
                LoadAllData();
                
                MessageBox.Show("Grade updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task DeleteGrade()
        {
            try
            {
                if (SelectedGrade == null)
                    return;
                    
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete this grade record?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    await _gradeRepository.DeleteAsync(SelectedGrade);
                    await _gradeRepository.SaveChangesAsync();
                    Grades.Remove(SelectedGrade);
                    ClearForm();
                    MessageBox.Show("Grade deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            SelectedGrade = null;
            SelectedStudent = null;
            SelectedCourse = null;
            SelectedAssignment = null;
            Score = 0;
            LetterGrade = string.Empty;
            GradeType = "Assignment";
            Weight = 1.0m;
            Comments = null;
        }
        
        private bool CanAddUpdateGrade(object? param)
        {
            return SelectedStudent != null && 
                   SelectedCourse != null && 
                   !string.IsNullOrWhiteSpace(LetterGrade) &&
                   !string.IsNullOrWhiteSpace(GradeType);
        }
        
        private void GenerateGradeReport()
        {
            // This would generate a grade report
            MessageBox.Show("This feature will generate a grade report for the selected student/course.", 
                           "Report Generation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
