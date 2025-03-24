using StudentManagementV1._1.MVVM.Commands;
using System.Windows.Input;

namespace StudentManagementV1._1.MVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        
        // Navigation commands
        public ICommand NavigateToStudentCommand { get; }
        public ICommand NavigateToCoursesCommand { get; }
        public ICommand NavigateToEnrollmentCommand { get; }
        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToAttendanceCommand { get; }public ICommand NavigateToAttendanceCommand { get; }
        public ICommand NavigateToGradesCommand { get; }
        
        // Constructorublic MainViewModel()
        public MainViewModel()
        {
            // Initialize with Dashboard_currentViewModel = new DashboardViewModel();
            _currentViewModel = new DashboardViewModel();
            
            // Setup navigation commands
            NavigateToStudentCommand = new RelayCommand(param => NavigateToStudent());
            NavigateToCoursesCommand = new RelayCommand(param => NavigateToCourses()););
            NavigateToEnrollmentCommand = new RelayCommand(param => NavigateToEnrollment());   NavigateToDashboardCommand = new RelayCommand(param => NavigateToDashboard());
            NavigateToDashboardCommand = new RelayCommand(param => NavigateToDashboard());    NavigateToAttendanceCommand = new RelayCommand(param => NavigateToAttendance());
            NavigateToAttendanceCommand = new RelayCommand(param => NavigateToAttendance());
            NavigateToGradesCommand = new RelayCommand(param => NavigateToGrades());
        }
        
        private void NavigateToStudent()    CurrentViewModel = new StudentViewModel();
        {
            CurrentViewModel = new StudentViewModel();
        }
        
        private void NavigateToCourses()    CurrentViewModel = new CourseViewModel();
        {
            CurrentViewModel = new CourseViewModel();
        }
        
        private void NavigateToEnrollment()    CurrentViewModel = new EnrollmentViewModel();
        {
            CurrentViewModel = new EnrollmentViewModel();
        }
        
        private void NavigateToDashboard()       CurrentViewModel = new DashboardViewModel();
        {       }
            CurrentViewModel = new DashboardViewModel();        














}    }        }            CurrentViewModel = new GradeViewModel();        {        private void NavigateToGrades()                }            CurrentViewModel = new AttendanceViewModel();        {        private void NavigateToAttendance()                }        private void NavigateToAttendance()
        {
            CurrentViewModel = new AttendanceViewModel();
        }
    }
}
