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
    public class AttendanceViewModel : ViewModelBase
    {
        private readonly IRepository<AttendanceRecord> _attendanceRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<ClassSchedule> _scheduleRepository;
        
        private ObservableCollection<AttendanceRecord> _attendanceRecords;
        private ObservableCollection<Student> _students;
        private ObservableCollection<Course> _courses;
        private ObservableCollection<ClassSchedule> _schedules;
        
        private AttendanceRecord? _selectedAttendanceRecord;
        private Student? _selectedStudent;
        private Course? _selectedCourse;
        private ClassSchedule? _selectedSchedule;
        private DateTime _date = DateTime.Today;
        private string _status = "Present";
        private string? _remarks;
        private TimeSpan? _timeIn;
        private TimeSpan? _timeOut;
        
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Today;
        private string _searchText = string.Empty;
        private ICollectionView? _attendanceView;
        
        // Commands
        public ICommand AddAttendanceCommand { get; }
        public ICommand UpdateAttendanceCommand { get; }
        public ICommand DeleteAttendanceCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand BulkAddCommand { get; }
        
        public ObservableCollection<AttendanceRecord> AttendanceRecords
        {
            get => _attendanceRecords;
            set => SetProperty(ref _attendanceRecords, value);
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
        
        public ObservableCollection<ClassSchedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }
        
        public AttendanceRecord? SelectedAttendanceRecord
        {
            get => _selectedAttendanceRecord;
            set
            {
                if (SetProperty(ref _selectedAttendanceRecord, value) && value != null)
                {
                    // Populate the form with selected record
                    SelectedStudent = Students.FirstOrDefault(s => s.Id == value.StudentId);
                    SelectedCourse = Courses.FirstOrDefault(c => c.Id == value.CourseId);
                    SelectedSchedule = Schedules.FirstOrDefault(s => s.Id == value.ClassScheduleId);
                    Date = value.Date;
                    Status = value.Status;
                    Remarks = value.Remarks;
                    TimeIn = value.TimeIn;
                    TimeOut = value.TimeOut;
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
                    // When course changes, update the available schedules
                    LoadSchedulesForCourse();
                }
            }
        }
        
        public ClassSchedule? SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }
        
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }
        
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        public string? Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }
        
        public TimeSpan? TimeIn
        {
            get => _timeIn;
            set => SetProperty(ref _timeIn, value);
        }
        
        public TimeSpan? TimeOut
        {
            get => _timeOut;
            set => SetProperty(ref _timeOut, value);
        }
        
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    _attendanceView?.Refresh();
                }
            }
        }
        
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    _attendanceView?.Refresh();
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
                    _attendanceView?.Refresh();
                }
            }
        }
        
        public AttendanceViewModel()
        {
            // Initialize repositories
            var context = new StudentManagementContext();
            _attendanceRepository = new Repository<AttendanceRecord>(context);
            _studentRepository = new Repository<Student>(context);
            _courseRepository = new Repository<Course>(context);
            _scheduleRepository = new Repository<ClassSchedule>(context);
            
            // Initialize collections
            _attendanceRecords = new ObservableCollection<AttendanceRecord>();
            _students = new ObservableCollection<Student>();
            _courses = new ObservableCollection<Course>();
            _schedules = new ObservableCollection<ClassSchedule>();
            
            // Initialize commands
            AddAttendanceCommand = new RelayCommand(async param => await AddAttendanceRecord(), CanAddUpdateAttendance);
            UpdateAttendanceCommand = new RelayCommand(async param => await UpdateAttendanceRecord(), CanAddUpdateAttendance);
            DeleteAttendanceCommand = new RelayCommand(async param => await DeleteAttendanceRecord(), param => SelectedAttendanceRecord != null);
            ClearFormCommand = new RelayCommand(param => ClearForm());
            SearchCommand = new RelayCommand(param => _attendanceView?.Refresh());
            GenerateReportCommand = new RelayCommand(param => GenerateAttendanceReport());
            BulkAddCommand = new RelayCommand(param => BulkAddAttendance());
            
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
                
                // Load all schedules initially - course-specific schedules will be loaded when a course is selected
                var schedules = await _scheduleRepository.GetAllAsync();
                Schedules.Clear();
                foreach (var schedule in schedules)
                {
                    Schedules.Add(schedule);
                }
                
                // Load attendance records with navigation properties
                using (var context = new StudentManagementContext())
                {
                    var records = await context.AttendanceRecords
                        .Include(a => a.Student)
                        .Include(a => a.Course)
                        .Include(a => a.ClassSchedule)
                        .Where(a => a.Date >= StartDate && a.Date <= EndDate)
                        .OrderByDescending(a => a.Date)
                        .ToListAsync();
                        
                    AttendanceRecords.Clear();
                    foreach (var record in records)
                    {
                        AttendanceRecords.Add(record);
                    }
                }
                
                // Create collection view for filtering
                _attendanceView = CollectionViewSource.GetDefaultView(AttendanceRecords);
                _attendanceView.Filter = FilterAttendanceRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void LoadSchedulesForCourse()
        {
            if (SelectedCourse == null)
                return;
                
            try
            {
                using (var context = new StudentManagementContext())
                {
                    var courseSchedules = await context.ClassSchedules
                        .Where(s => s.CourseId == SelectedCourse.Id)
                        .Include(s => s.Teacher)
                        .ToListAsync();
                        
                    Schedules.Clear();
                    foreach (var schedule in courseSchedules)
                    {
                        Schedules.Add(schedule);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool FilterAttendanceRecords(object obj)
        {
            if (obj is not AttendanceRecord record)
                return false;
                
            // Filter by date range
            bool dateFilter = record.Date >= StartDate && record.Date <= EndDate;
            
            // Filter by search text if provided
            bool textFilter = true;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                textFilter = (record.Student?.FirstName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                              (record.Student?.LastName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                              (record.Course?.CourseCode?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                              (record.Status?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }
            
            return dateFilter && textFilter;
        }
        
        private async Task AddAttendanceRecord()
        {
            try
            {
                if (SelectedStudent == null)
                {
                    MessageBox.Show("Please select a student.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Check if a record already exists for this student, date, and course/schedule
                bool duplicateExists = false;
                foreach (var record in AttendanceRecords)
                {
                    if (record.StudentId == SelectedStudent.Id && 
                        record.Date.Date == Date.Date && 
                        ((SelectedCourse != null && record.CourseId == SelectedCourse.Id) ||
                         (SelectedSchedule != null && record.ClassScheduleId == SelectedSchedule.Id)))
                    {
                        duplicateExists = true;
                        break;
                    }
                }
                
                if (duplicateExists)
                {
                    MessageBox.Show("An attendance record already exists for this student on this date and course/schedule.",
                                    "Duplicate Record", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var attendanceRecord = new AttendanceRecord
                {
                    StudentId = SelectedStudent.Id,
                    CourseId = SelectedCourse?.Id,
                    ClassScheduleId = SelectedSchedule?.Id,
                    Date = Date,
                    Status = Status,
                    Remarks = Remarks,
                    TimeIn = TimeIn,
                    TimeOut = TimeOut
                };
                
                await _attendanceRepository.AddAsync(attendanceRecord);
                await _attendanceRepository.SaveChangesAsync();
                
                // Reload to get navigation properties
                using (var context = new StudentManagementContext())
                {
                    attendanceRecord = await context.AttendanceRecords
                        .Include(a => a.Student)
                        .Include(a => a.Course)
                        .Include(a => a.ClassSchedule)
                        .FirstOrDefaultAsync(a => a.Id == attendanceRecord.Id);
                        
                    if (attendanceRecord != null)
                    {
                        AttendanceRecords.Add(attendanceRecord);
                    }
                }
                
                ClearForm();
                MessageBox.Show("Attendance record added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding attendance record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateAttendanceRecord()
        {
            try
            {
                if (SelectedAttendanceRecord == null || SelectedStudent == null)
                    return;
                    
                SelectedAttendanceRecord.StudentId = SelectedStudent.Id;
                SelectedAttendanceRecord.CourseId = SelectedCourse?.Id;
                SelectedAttendanceRecord.ClassScheduleId = SelectedSchedule?.Id;
                SelectedAttendanceRecord.Date = Date;
                SelectedAttendanceRecord.Status = Status;
                SelectedAttendanceRecord.Remarks = Remarks;
                SelectedAttendanceRecord.TimeIn = TimeIn;
                SelectedAttendanceRecord.TimeOut = TimeOut;
                
                await _attendanceRepository.UpdateAsync(SelectedAttendanceRecord);
                await _attendanceRepository.SaveChangesAsync();
                
                // Reload to refresh the list with updated data
                LoadAllData();
                
                MessageBox.Show("Attendance record updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating attendance record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task DeleteAttendanceRecord()
        {
            try
            {
                if (SelectedAttendanceRecord == null)
                    return;
                    
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete this attendance record?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    await _attendanceRepository.DeleteAsync(SelectedAttendanceRecord);
                    await _attendanceRepository.SaveChangesAsync();
                    AttendanceRecords.Remove(SelectedAttendanceRecord);
                    ClearForm();
                    MessageBox.Show("Attendance record deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting attendance record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            SelectedAttendanceRecord = null;
            SelectedStudent = null;
            SelectedCourse = null;
            SelectedSchedule = null;
            Date = DateTime.Today;
            Status = "Present";
            Remarks = null;
            TimeIn = null;
            TimeOut = null;
        }
        
        private bool CanAddUpdateAttendance(object? param)
        {
            return SelectedStudent != null && !string.IsNullOrWhiteSpace(Status);
        }
        
        private void GenerateAttendanceReport()
        {
            // This would generate an attendance report
            MessageBox.Show("This feature will generate an attendance report for the selected date range.", 
                           "Report Generation", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Implementation would involve:
            // 1. Collecting attendance data for the specified period
            // 2. Generating statistics (attendance rates, most absences, etc.)
            // 3. Creating a document or display for viewing/printing
        }
        
        private void BulkAddAttendance()
        {
            // This would allow adding attendance for multiple students at once
            MessageBox.Show("This feature will allow adding attendance records for multiple students at once.", 
                           "Bulk Add", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Implementation would involve:
            // 1. Showing a list of students for a course
            // 2. Allowing the user to check/mark attendance for all of them
            // 3. Saving multiple records at once
        }
    }
}
