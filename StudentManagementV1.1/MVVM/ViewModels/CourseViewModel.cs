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
    public class CourseViewModel : ViewModelBase
    {
        private readonly IRepository<Course> _courseRepository;
        
        private ObservableCollection<Course> _courses;
        private Course? _selectedCourse;
        private string _searchText = string.Empty;
        private ICollectionView? _coursesView;
        
        // Course properties for editing/adding
        private string _title = string.Empty;
        private string? _description;
        private int _creditHours;
        private string _courseCode = string.Empty;
        private string _department = string.Empty;
        private int? _maxEnrollment;
        private bool _isActive = true;
        
        // Commands
        public ICommand AddCourseCommand { get; }
        public ICommand UpdateCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand SearchCommand { get; }
        
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }
        
        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                if (SetProperty(ref _selectedCourse, value) && value != null)
                {
                    // Populate the form fields with selected course
                    Title = value.Title;
                    Description = value.Description;
                    CreditHours = value.CreditHours;
                    CourseCode = value.CourseCode;
                    Department = value.Department;
                    MaxEnrollment = value.MaxEnrollment;
                    IsActive = value.IsActive;
                }
            }
        }
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _coursesView?.Refresh();
                }
            }
        }
        
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        
        public int CreditHours
        {
            get => _creditHours;
            set => SetProperty(ref _creditHours, value);
        }
        
        public string CourseCode
        {
            get => _courseCode;
            set => SetProperty(ref _courseCode, value);
        }
        
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }
        
        public int? MaxEnrollment
        {
            get => _maxEnrollment;
            set => SetProperty(ref _maxEnrollment, value);
        }
        
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        
        public CourseViewModel()
        {
            // Initialize repository with context
            var context = new StudentManagementContext();
            _courseRepository = new Repository<Course>(context);
            
            // Initialize collection
            _courses = new ObservableCollection<Course>();
            
            // Initialize commands
            AddCourseCommand = new RelayCommand(async param => await AddCourse(), CanAddUpdateCourse);
            UpdateCourseCommand = new RelayCommand(async param => await UpdateCourse(), CanAddUpdateCourse);
            DeleteCourseCommand = new RelayCommand(async param => await DeleteCourse(), param => SelectedCourse != null);
            ClearFormCommand = new RelayCommand(param => ClearForm());
            SearchCommand = new RelayCommand(param => _coursesView?.Refresh());
            
            // Load data
            LoadCourses();
        }
        
        private async void LoadCourses()
        {
            try
            {
                var courses = await _courseRepository.GetAllAsync();
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
                
                // Create a collection view for filtering
                _coursesView = CollectionViewSource.GetDefaultView(Courses);
                _coursesView.Filter = FilterCourses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool FilterCourses(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;
                
            if (obj is Course course)
            {
                return course.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || course.CourseCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || course.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || course.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
            }
            
            return false;
        }
        
        private async Task AddCourse()
        {
            try
            {
                var course = new Course
                {
                    Title = Title,
                    Description = Description,
                    CreditHours = CreditHours,
                    CourseCode = CourseCode,
                    Department = Department,
                    MaxEnrollment = MaxEnrollment,
                    IsActive = IsActive
                };
                
                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();
                Courses.Add(course);
                ClearForm();
                MessageBox.Show("Course added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateCourse()
        {
            try
            {
                if (SelectedCourse == null)
                    return;
                    
                SelectedCourse.Title = Title;
                SelectedCourse.Description = Description;
                SelectedCourse.CreditHours = CreditHours;
                SelectedCourse.CourseCode = CourseCode;
                SelectedCourse.Department = Department;
                SelectedCourse.MaxEnrollment = MaxEnrollment;
                SelectedCourse.IsActive = IsActive;
                
                await _courseRepository.UpdateAsync(SelectedCourse);
                await _courseRepository.SaveChangesAsync();
                
                // Refresh view
                int index = Courses.IndexOf(SelectedCourse);
                if (index >= 0)
                {
                    Courses[index] = SelectedCourse;
                }
                
                MessageBox.Show("Course updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task DeleteCourse()
        {
            try
            {
                if (SelectedCourse == null)
                    return;
                    
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the course '{SelectedCourse.Title}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    await _courseRepository.DeleteAsync(SelectedCourse);
                    await _courseRepository.SaveChangesAsync();
                    Courses.Remove(SelectedCourse);
                    ClearForm();
                    MessageBox.Show("Course deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            SelectedCourse = null;
            Title = string.Empty;
            Description = null;
            CreditHours = 0;
            CourseCode = string.Empty;
            Department = string.Empty;
            MaxEnrollment = null;
            IsActive = true;
        }
        
        private bool CanAddUpdateCourse(object? param)
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(CourseCode) &&
                   !string.IsNullOrWhiteSpace(Department) &&
                   CreditHours > 0;
        }
    }
}
