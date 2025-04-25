using StudentManagementSystem.Models;
using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class LoginWindow : Window
	{
		private readonly DataAccess dataAccess;

		public LoginWindow()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
		}

		private void Login_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Password;

			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (dataAccess.ValidateLogin(username, password))
			{
				string role = dataAccess.GetUserRole(username);
				User user = dataAccess.GetUserByUsername(username);
				if (user == null)
				{
					MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				if (role == "admin")
				{
					AdminDashboard adminDashboard = new AdminDashboard();
					adminDashboard.Show();
				}
				else
				{
					UserDashboard userDashboard = new UserDashboard(user);
					userDashboard.Show();
				}
				Close();
			}
			else
			{
				MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}