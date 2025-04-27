using System;
using System.Windows;
using System.Windows.Controls;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Views
{
	public partial class UpdateAccountForm : UserControl
	{
		private readonly DataAccess dataAccess;
		private readonly User currentUser;

		public UpdateAccountForm(User user)
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			currentUser = user;
			LoadUserData();
		}

		private void LoadUserData()
		{
			if (currentUser != null)
			{
				txtUsername.Text = currentUser.Username;
				txtPassword.Password = currentUser.Password;
				txtNote.Text = currentUser.Note;
			}
		}

		private void SaveAccount_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var updatedUser = new User
				{
					IdStudent = currentUser.IdStudent,
					Username = txtUsername.Text,
					Password = txtPassword.Password,
					Note = txtNote.Text,
					CreatedAt = currentUser.CreatedAt,
					ModifiedAt = DateTime.Now
				};

				dataAccess.UpdateUser(updatedUser);
				MessageBox.Show("Account updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

				if (this.Parent is ContentControl contentControl)
				{
					contentControl.Content = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to update account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			if (this.Parent is ContentControl contentControl)
			{
				contentControl.Content = null;
			}
		}
	}
}