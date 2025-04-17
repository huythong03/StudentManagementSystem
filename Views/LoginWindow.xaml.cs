using StudentManagementSystem.Views;
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
				if (role == "admin")
				{
					AdminDashboard adminDashboard = new AdminDashboard();
					adminDashboard.Show();
				}
				else
				{
					UserDashboard userDashboard = new UserDashboard(username);
					userDashboard.Show();
				}
				Close();
			}
			else
			{
				MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}