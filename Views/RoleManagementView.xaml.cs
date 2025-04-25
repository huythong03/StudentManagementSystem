using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class RoleManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Role selectedRole;

		public RoleManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadRoles();
		}

		private void LoadRoles()
		{
			try
			{
				RolesGrid.ItemsSource = dataAccess.GetRoles();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load roles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddRole_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedRole = null;
			PanelTitle.Text = "Add Role";
			txtName.Text = "";
			chkStatus.IsChecked = true;
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditRole_Click(object sender, RoutedEventArgs e)
		{
			if (RolesGrid.SelectedItem is Role role)
			{
				isEditMode = true;
				selectedRole = role;
				PanelTitle.Text = "Edit Role";
				txtName.Text = role.Name;
				chkStatus.IsChecked = role.Status;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a role to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteRole_Click(object sender, RoutedEventArgs e)
		{
			if (RolesGrid.SelectedItem is Role role)
			{
				var result = MessageBox.Show($"Are you sure you want to delete role {role.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteRole(role.Id);
						LoadRoles();
						MessageBox.Show("Role deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a role to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveRole_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please enter a role name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var role = new Role
				{
					Name = txtName.Text,
					Status = chkStatus.IsChecked ?? false
				};

				if (isEditMode)
				{
					role.Id = selectedRole.Id;
					dataAccess.UpdateRole(role);
					MessageBox.Show("Role updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					role.Id = dataAccess.GetNextRoleId();
					dataAccess.AddRole(role);
					MessageBox.Show("Role added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				LoadRoles();
				InputPanel.Visibility = Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void RolesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}