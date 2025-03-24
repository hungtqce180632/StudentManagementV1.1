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
    public class StudentViewModel : ViewModelBase
    {
        private readonly IRepository<Student> _studentRepository;
        
        private ObservableCollection<Student> _students;
        private Student? _selectedStudent;
        private string _searchText = string.Empty;
        private ICollectionView? _studentsView;
        
        // Student properties for editing/adding
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string? _middleName;
        private DateTime _dateOfBirth = DateTime.Now.AddYears(-18);
        private string _gender = string.Empty;
        private string _address = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private string _emergencyContactName = string.Empty;
        private string _emergencyContactPhone = string.Empty;
        private string _emergencyContactRelationship = string.Empty;
        private string _studentId = string.Empty;
        
        // Commands
        public ICommand AddStudentCommand { get; }
        public ICommand UpdateStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand SearchCommand { get; }
        
        public ObservableCollection<Student> Students
        {
            get => _students;
            set => SetProperty(ref _students, value);
        }
        
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (SetProperty(ref _selectedStudent, value) && value != null)
                {
                    // Populate the form fields with selected student
                    FirstName = value.FirstName;
                    LastName = value.LastName;
                    MiddleName = value.MiddleName;
                    DateOfBirth = value.DateOfBirth;
                    Gender = value.Gender;
                    Address = value.Address;
                    Phone = value.Phone;
                    Email = value.Email;
                    EmergencyContactName = value.EmergencyContactName;
                    EmergencyContactPhone = value.EmergencyContactPhone;
                    EmergencyContactRelationship = value.EmergencyContactRelationship;
                    StudentId = value.StudentId ?? string.Empty;
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
                    _studentsView?.Refresh();
                }
            }
        }
        
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        
        public string? MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }
        
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }
        
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }
        
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }
        
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        
        public string EmergencyContactName
        {
            get => _emergencyContactName;
            set => SetProperty(ref _emergencyContactName, value);
        }
        
        public string EmergencyContactPhone
        {
            get => _emergencyContactPhone;
            set => SetProperty(ref _emergencyContactPhone, value);
        }
        
        public string EmergencyContactRelationship
        {
            get => _emergencyContactRelationship;
            set => SetProperty(ref _emergencyContactRelationship, value);
        }
        
        public string StudentId
        {
            get => _studentId;
            set => SetProperty(ref _studentId, value);
        }
        
        public StudentViewModel()
        {
            // Initialize repository with context
            var context = new StudentManagementContext();
            _studentRepository = new Repository<Student>(context);
            
            // Initialize collection
            _students = new ObservableCollection<Student>();
            
            // Initialize commands
            AddStudentCommand = new RelayCommand(async param => await AddStudent(), CanAddUpdateStudent);
            UpdateStudentCommand = new RelayCommand(async param => await UpdateStudent(), CanAddUpdateStudent);
            DeleteStudentCommand = new RelayCommand(async param => await DeleteStudent(), param => SelectedStudent != null);
            ClearFormCommand = new RelayCommand(param => ClearForm());
            SearchCommand = new RelayCommand(param => _studentsView?.Refresh());
            
            // Load data
            LoadStudents();
        }
        
        private async void LoadStudents()
        {
            try
            {
                var students = await _studentRepository.GetAllAsync();
                Students.Clear();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
                
                // Create a collection view for filtering
                _studentsView = CollectionViewSource.GetDefaultView(Students);
                _studentsView.Filter = FilterStudents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool FilterStudents(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;
                
            if (obj is Student student)
            {
                return student.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || student.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || student.StudentId?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    || student.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            
            return false;
        }
        
        private async Task AddStudent()
        {
            try
            {
                var student = new Student
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    DateOfBirth = DateOfBirth,
                    Gender = Gender,
                    Address = Address,
                    Phone = Phone,
                    Email = Email,
                    EmergencyContactName = EmergencyContactName,
                    EmergencyContactPhone = EmergencyContactPhone,
                    EmergencyContactRelationship = EmergencyContactRelationship,
                    StudentId = StudentId,
                    EnrollmentDate = DateTime.Now
                };
                
                await _studentRepository.AddAsync(student);
                await _studentRepository.SaveChangesAsync();
                Students.Add(student);
                ClearForm();
                MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateStudent()
        {
            try
            {
                if (SelectedStudent == null)
                    return;
                    
                SelectedStudent.FirstName = FirstName;
                SelectedStudent.LastName = LastName;
                SelectedStudent.MiddleName = MiddleName;
                SelectedStudent.DateOfBirth = DateOfBirth;
                SelectedStudent.Gender = Gender;
                SelectedStudent.Address = Address;
                SelectedStudent.Phone = Phone;
                SelectedStudent.Email = Email;
                SelectedStudent.EmergencyContactName = EmergencyContactName;
                SelectedStudent.EmergencyContactPhone = EmergencyContactPhone;
                SelectedStudent.EmergencyContactRelationship = EmergencyContactRelationship;
                SelectedStudent.StudentId = StudentId;
                
                await _studentRepository.UpdateAsync(SelectedStudent);
                await _studentRepository.SaveChangesAsync();
                
                // Refresh view
                int index = Students.IndexOf(SelectedStudent);
                if (index >= 0)
                {
                    Students[index] = SelectedStudent;
                }
                
                MessageBox.Show("Student updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task DeleteStudent()
        {
            try
            {
                if (SelectedStudent == null)
                    return;
                    
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete {SelectedStudent.FirstName} {SelectedStudent.LastName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    await _studentRepository.DeleteAsync(SelectedStudent);
                    await _studentRepository.SaveChangesAsync();
                    Students.Remove(SelectedStudent);
                    ClearForm();
                    MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            SelectedStudent = null;
            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleName = null;
            DateOfBirth = DateTime.Now.AddYears(-18);
            Gender = string.Empty;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            EmergencyContactName = string.Empty;
            EmergencyContactPhone = string.Empty;
            EmergencyContactRelationship = string.Empty;
            StudentId = string.Empty;
        }
        
        private bool CanAddUpdateStudent(object? param)
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Gender) &&
                   !string.IsNullOrWhiteSpace(Email);
        }
    }
}
