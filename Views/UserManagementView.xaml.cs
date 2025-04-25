using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private User selectedUser;

		public UserManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadUsers();
		}

		private void LoadUsers()
		{
			UsersGrid.ItemsSource = dataAccess.GetUsers();
		}

		private void AddUser_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedUser = null;
			PanelTitle.Text = "Add User";
			txtStudentId.Text = "";
			txtUsername.Text = "";
			txtPassword.Password = "";
			txtNote.Text = "";
			chkStatus.IsChecked = true;
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditUser_Click(object sender, RoutedEventArgs e)
		{
			if (UsersGrid.SelectedItem is User user)
			{
				isEditMode = true;
				selectedUser = user;
				PanelTitle.Text = "Edit User";
				txtStudentId.Text = user.IdStudent;
				txtUsername.Text = user.Username;
				txtPassword.Password = user.Password;
				txtNote.Text = user.Note;
				chkStatus.IsChecked = user.Status;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a user to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteUser_Click(object sender, RoutedEventArgs e)
		{
			if (UsersGrid.SelectedItem is User user)
			{
				var result = MessageBox.Show($"Are you sure you want to delete user {user.Username}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteUser(user.IdStudent); 
						LoadUsers();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a user to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveUser_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtStudentId.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var user = new User
				{
					IdStudent = txtStudentId.Text,
					Username = txtUsername.Text,
					Password = txtPassword.Password,
					Note = txtNote.Text,
					Status = chkStatus.IsChecked ?? false,
					CreatedAt = isEditMode ? selectedUser.CreatedAt : DateTime.Now,
					ModifiedAt = DateTime.Now
				};

				if (isEditMode)
				{
					//user.Id = selectedUser.Id;
					dataAccess.UpdateUser(user);
				}
				else
				{
					dataAccess.AddUser(user);
				}

				LoadUsers();
				InputPanel.Visibility = Visibility.Collapsed;
				MessageBox.Show("User saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}