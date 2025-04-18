using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class UpdateAccountForm : Window
	{
		private readonly DataAccess dataAccess;
		private string username;

		public UpdateAccountForm(string username)
		{
			InitializeComponent();
			this.username = username;
			dataAccess = new DataAccess();
			txtCurrentUsername.Text = username;
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtNewUsername.Text) && string.IsNullOrWhiteSpace(txtNewPassword.Password))
			{
				MessageBox.Show("Please provide a new username or password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var user = dataAccess.GetUsers().Find(u => u.Username == username);
				if (user == null)
				{
					MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				bool usernameChanged = !string.IsNullOrWhiteSpace(txtNewUsername.Text) && txtNewUsername.Text != username;
				bool passwordChanged = !string.IsNullOrWhiteSpace(txtNewPassword.Password);

				if (usernameChanged)
				{
					user.Username = txtNewUsername.Text;
					user.ModifiedAt = DateTime.Now;
					dataAccess.UpdateUser(user);
					username = txtNewUsername.Text;
					txtCurrentUsername.Text = username;
				}

				if (passwordChanged)
				{
					dataAccess.UpdateUserPassword(username, txtNewPassword.Password);
				}

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to update account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}