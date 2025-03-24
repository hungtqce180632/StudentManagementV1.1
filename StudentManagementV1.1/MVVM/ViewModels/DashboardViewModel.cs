using StudentManagementV1._1.MVVM.Data;

namespace StudentManagementV1._1.MVVM.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private int _totalStudents;
        private int _totalTeachers;
        private int _activeCourses;
        
        public int TotalStudents
        {
            get => _totalStudents;
            set => SetProperty(ref _totalStudents, value);
        }
        
        public int TotalTeachers
        {
            get => _totalTeachers;
            set => SetProperty(ref _totalTeachers, value);
        }
        
        public int ActiveCourses
        {
            get => _activeCourses;
            set => SetProperty(ref _activeCourses, value);
        }
        
        public DashboardViewModel()
        {
            LoadDashboardData();
        }
        
        private async void LoadDashboardData()
        {
            try
            {
                using (var context = new StudentManagementContext())
                {
                    // These will be implemented when the corresponding tables are created
                    TotalStudents = await context.Students.CountAsync();
                    ActiveCourses = await context.Courses.Where(c => c.IsActive).CountAsync();
                    
                    // Teacher count will be added when Teacher model is implemented
                    TotalTeachers = 0;
                }
            }
            catch (Exception)
            {
                // If database is not yet created or other error
                TotalStudents = 0;
                TotalTeachers = 0;
                ActiveCourses = 0;
            }
        }
    }
}
