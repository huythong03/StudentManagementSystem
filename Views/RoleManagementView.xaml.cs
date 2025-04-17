using StudentManagementSystem.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class RoleManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<Role> roles;

		public RoleManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadRoles();
		}

		private void LoadRoles()
		{
			roles = dataAccess.GetRoles();
			RolesGrid.ItemsSource = roles;
		}

		private void AddRole_Click(object sender, RoutedEventArgs e)
		{
			RoleForm roleForm = new RoleForm(null);
			if (roleForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddRole(roleForm.Role);
					LoadRoles();
					MessageBox.Show("Role added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Error adding role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditRole_Click(object sender, RoutedEventArgs e)
		{
			if (RolesGrid.SelectedItem is Role selectedRole)
			{
				RoleForm roleForm = new RoleForm(selectedRole);
				if (roleForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateRole(roleForm.Role);
						LoadRoles();
						MessageBox.Show("Role updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error updating role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a role to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteRole_Click(object sender, RoutedEventArgs e)
		{
			if (RolesGrid.SelectedItem is Role selectedRole)
			{
				if (MessageBox.Show($"Are you sure you want to delete {selectedRole.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteRole(selectedRole.Id);
						LoadRoles();
						MessageBox.Show("Role deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error deleting role: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a role to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}