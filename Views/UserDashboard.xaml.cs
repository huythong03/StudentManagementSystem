using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Views
{
	public partial class UserDashboard : Window
	{
		private readonly User currentUser;
		private DispatcherTimer timer;
		private TimeZoneInfo vietnamTimeZone;

		public UserDashboard(User user)
		{
			InitializeComponent();
			currentUser = user;
			string studentName = GetStudentNameById(currentUser.Username);
			GreetingTextBlock.Text = $"Hello {studentName ?? currentUser.Username}";
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

		private string GetStudentNameById(string studentId)
		{
			string studentName = null;
			string connectionString = ConfigurationManager.ConnectionStrings["QuanLySinhVienConnection"].ConnectionString;
			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string query = "SELECT Name FROM Student WHERE Id = @id";
					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@id", studentId);
						var result = command.ExecuteScalar();
						if (result != null)
						{
							studentName = result.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi lấy tên sinh viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return studentName;
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

		private void StudentManagement_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new StudentManagementView();
		}

		private void SubjectManagement_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new SubjectManagementView();
		}

		private void UserManagement_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new UserManagementView();
		}

		private void EnrollSubject_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EnrollSubjectView(currentUser.Username);
		}

		private void ViewEnrolledSubjects_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EnrolledSubjectsView(currentUser.Username);
		}

		private void UpdateAccount_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new UpdateAccountForm(currentUser);
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
			var loginWindow = new LoginWindow();
			loginWindow.Show();
			Close();
		}
	}
}