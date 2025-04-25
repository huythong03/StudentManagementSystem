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

		public AdminDashboard()
		{
			InitializeComponent();
			try
			{
				vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			}
			catch (TimeZoneNotFoundException)
			{
				vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone("Vietnam Time", TimeSpan.FromHours(7), "Vietnam Time", "Vietnam Time");
			}
			SetupTimer();
		}

		private void SetupTimer()
		{
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
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

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
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

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			this.Close();
		}
	}
}