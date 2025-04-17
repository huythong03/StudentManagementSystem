using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class UpdateAccountForm : Window
	{
		private readonly DataAccess dataAccess;
		private readonly string username;

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
				if (!string.IsNullOrWhiteSpace(txtNewUsername.Text))
				{
					// Note: Updating username requires updating related records or ensuring no conflicts
					// For simplicity, we assume username updates are allowed and handled in DataAccess
					var user = dataAccess.GetUsers().Find(u => u.Username == username);
					if (user != null)
					{
						user.Username = txtNewUsername.Text;
						user.ModifiedAt = System.DateTime.Now;
						dataAccess.UpdateUser(user);
					}
				}

				if (!string.IsNullOrWhiteSpace(txtNewPassword.Password))
				{
					dataAccess.UpdateUserPassword(username, txtNewPassword.Password);
				}

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}