using StudentManagementSystem.Views;
using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class AdminDashboard : Window
	{
		public AdminDashboard()
		{
			InitializeComponent();
		}

		private void ManageStudents_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new StudentManagementView();
		}

		private void ManageUsers_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new UserManagementView();
		}

		private void ManageRoles_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new RoleManagementView();
		}

		private void ManageUserRoles_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new UserRoleManagementView();
		}

		private void ManageSubjects_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new SubjectManagementView();
		}

		private void ManageProvinces_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new ProvinceManagementView();
		}

		private void ViewEnrollments_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EnrollmentView();
		}

		private void SearchGrades_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new SearchGradesView();
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			Close();
		}
	}
}