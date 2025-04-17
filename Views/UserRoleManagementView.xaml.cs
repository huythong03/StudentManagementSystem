using StudentManagementSystem.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserRoleManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<UserRole> userRoles;

		public UserRoleManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadUserRoles();
		}

		private void LoadUserRoles()
		{
			userRoles = dataAccess.GetUserRoles();
			UserRolesGrid.ItemsSource = userRoles;
		}

		private void AddUserRole_Click(object sender, RoutedEventArgs e)
		{
			UserRoleForm userRoleForm = new UserRoleForm(null);
			if (userRoleForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddUserRole(userRoleForm.UserRole);
					LoadUserRoles();
					MessageBox.Show("User role added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Error adding user role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditUserRole_Click(object sender, RoutedEventArgs e)
		{
			if (UserRolesGrid.SelectedItem is UserRole selectedUserRole)
			{
				UserRoleForm userRoleForm = new UserRoleForm(selectedUserRole);
				if (userRoleForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateUserRole(userRoleForm.UserRole);
						LoadUserRoles();
						MessageBox.Show("User role updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error updating user role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a user role to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteUserRole_Click(object sender, RoutedEventArgs e)
		{
			if (UserRolesGrid.SelectedItem is UserRole selectedUserRole)
			{
				if (MessageBox.Show($"Are you sure you want to delete user role {selectedUserRole.Id}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteUserRole(selectedUserRole.Id);
						LoadUserRoles();
						MessageBox.Show("User role deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error deleting user role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a user role to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}