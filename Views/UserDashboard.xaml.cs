using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class UserDashboard : Window
	{
		private readonly DataAccess dataAccess;
		private readonly string username;

		public UserDashboard(string username)
		{
			InitializeComponent();
			this.username = username;
			dataAccess = new DataAccess();
		}

		private void EnrollSubject_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EnrollSubjectView(username);
		}

		private void ViewEnrolledSubjects_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EnrolledSubjectsView(username);
		}

		private void UpdateAccount_Click(object sender, RoutedEventArgs e)
		{
			UpdateAccountForm updateAccountForm = new UpdateAccountForm(username);
			if (updateAccountForm.ShowDialog() == true)
			{
				MessageBox.Show("Account updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			Close();
		}
	}
}