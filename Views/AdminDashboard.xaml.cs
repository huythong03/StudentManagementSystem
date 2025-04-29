using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using StudentManagementSystem.Views;

namespace StudentManagementSystem.Views
{
	public partial class AdminDashboard : Window
	{
		private DispatcherTimer timer;
		private TimeZoneInfo vietnamTimeZone;
		private readonly DataAccess dataAccess;
		private readonly UIElement dashboardContent;

		public AdminDashboard()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			UpdateDashboard();

			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			DateTextBlock.Text = DateTime.Now.ToString("dd/MM/yyyy");
			TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
		}

		private void UpdateDashboard()
		{
			try
			{
				TotalStudentsTextBlock.Text = dataAccess.GetTotalStudents().ToString("N0");
				TotalEnrollmentsTextBlock.Text = dataAccess.GetTotalEnrollments().ToString("N0");
				AverageGradeTextBlock.Text = dataAccess.GetAverageGrade().ToString("F2");
				ActiveCoursesTextBlock.Text = dataAccess.GetActiveCourses().ToString("N0");
				NewStudentsTextBlock.Text = dataAccess.GetNewStudentsThisYear().ToString("N0");
				PendingRequestsTextBlock.Text = dataAccess.GetPendingEnrollmentRequestsCount().ToString("N0");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error updating dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				TotalStudentsTextBlock.Text = "0";
				TotalEnrollmentsTextBlock.Text = "0";
				AverageGradeTextBlock.Text = "0.00";
				ActiveCoursesTextBlock.Text = "0";
				NewStudentsTextBlock.Text = "0";
				PendingRequestsTextBlock.Text = "0";
			}
		}

		private void Home_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = dashboardContent;
			UpdateDashboard();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			timer?.Stop();
			Application.Current.Shutdown();
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

		private void ManageEnrollRequests_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new AdminEnrollRequestsView();
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			timer?.Stop();
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			Close();
		}
	}
}