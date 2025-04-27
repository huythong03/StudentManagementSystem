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
			// Lưu nội dung Dashboard Overview
			dashboardContent = MainContent.Content as UIElement;
			try
			{
				vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			}
			catch (TimeZoneNotFoundException)
			{
				vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone("Vietnam Time", TimeSpan.FromHours(7), "Vietnam Time", "Vietnam Time");
			}
			SetupTimer();
			UpdateDashboard();
		}

		private void SetupTimer()
		{
			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			timer.Tick += Timer_Tick;
			timer.Start();
			UpdateDateTime();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateDateTime();
		}

		private void UpdateDateTime()
		{
			DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
			DateTextBlock.Text = vietnamTime.ToString("dd/MM/yyyy");
			TimeTextBlock.Text = vietnamTime.ToString("HH:mm:ss");
		}

		private void UpdateDashboard()
		{
			try
			{
				if (dataAccess == null)
				{
					System.Diagnostics.Debug.WriteLine("UpdateDashboard: dataAccess is null");
					throw new Exception("DataAccess is not initialized.");
				}
				int totalStudents = dataAccess.GetTotalStudents();
				TotalStudentsTextBlock.Text = totalStudents.ToString("N0");
				System.Diagnostics.Debug.WriteLine($"UpdateDashboard: Set TotalStudentsTextBlock to {totalStudents}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"UpdateDashboard: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show($"Error loading total students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				TotalStudentsTextBlock.Text = "0";
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