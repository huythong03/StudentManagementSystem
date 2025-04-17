using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<User> users;

		public UserManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadUsers();
		}

		private void LoadUsers()
		{
			users = dataAccess.GetUsers();
			UsersGrid.ItemsSource = users;
		}

		private void AddUser_Click(object sender, RoutedEventArgs e)
		{
			UserForm userForm = new UserForm(null);
			if (userForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddUser(userForm.User);
					LoadUsers();
					MessageBox.Show("User added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Error adding user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditUser_Click(object sender, RoutedEventArgs e)
		{
			if (UsersGrid.SelectedItem is User selectedUser)
			{
				UserForm userForm = new UserForm(selectedUser);
				if (userForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateUser(userForm.User);
						LoadUsers();
						MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error updating user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a user to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteUser_Click(object sender, RoutedEventArgs e)
		{
			if (UsersGrid.SelectedItem is User selectedUser)
			{
				if (MessageBox.Show($"Are you sure you want to delete {selectedUser.Username}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteUser(selectedUser.IdStudent);
						LoadUsers();
						MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error deleting user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}