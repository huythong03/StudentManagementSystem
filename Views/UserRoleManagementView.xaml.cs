using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserRoleManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private UserRole selectedUserRole;

		public UserRoleManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadUserRoles();
			LoadComboBoxData();
		}

		private void LoadUserRoles()
		{
			try
			{
				UserRolesGrid.ItemsSource = dataAccess.GetUserRoles();
				Debug.WriteLine("LoadUserRoles: User roles loaded successfully.");
				SetStatusMessage("User roles loaded successfully.", false);
			}
			catch (Exception ex)
			{
				SetStatusMessage($"Failed to load user roles: {ex.Message}", true);
				Debug.WriteLine($"LoadUserRoles: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show($"Failed to load user roles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadComboBoxData()
		{
			try
			{
				// Load IdStudent from User table
				var userStudentIds = dataAccess.GetUserStudentIds();
				if (userStudentIds == null || !userStudentIds.Any())
				{
					SetStatusMessage("No user student IDs available.", true);
					Debug.WriteLine("LoadComboBoxData: No user student IDs available.");
					MessageBox.Show("No user student IDs available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				cbStudentId.ItemsSource = userStudentIds;
				cbStudentId.DisplayMemberPath = "IdStudent";
				cbStudentId.SelectedValuePath = "IdStudent";
				Debug.WriteLine($"LoadComboBoxData: Loaded {userStudentIds.Count} user student IDs:");
				foreach (var user in userStudentIds)
				{
					Debug.WriteLine($"  - IdStudent: {user.IdStudent}");
				}

				// Load Role IDs
				var roles = dataAccess.GetRoles();
				if (roles == null || !roles.Any())
				{
					SetStatusMessage("No roles available.", true);
					Debug.WriteLine("LoadComboBoxData: No roles available.");
					MessageBox.Show("No roles available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				cbRoleId.ItemsSource = roles;
				cbRoleId.DisplayMemberPath = "Id";
				cbRoleId.SelectedValuePath = "Id";
				Debug.WriteLine($"LoadComboBoxData: Loaded {roles.Count} role IDs:");
				foreach (var role in roles)
				{
					Debug.WriteLine($"  - Role ID: {role.Id}");
				}

				SetStatusMessage("ComboBox data loaded successfully.", false);
			}
			catch (Exception ex)
			{
				SetStatusMessage($"Failed to load combo box data: {ex.Message}", true);
				Debug.WriteLine($"LoadComboBoxData: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show($"Failed to load combo box data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddUserRole_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedUserRole = null;
			PanelTitle.Text = "Add User Role";
			cbStudentId.SelectedIndex = -1;
			cbRoleId.SelectedIndex = -1;
			InputPanel.Visibility = Visibility.Visible;
			SetStatusMessage("Ready to add new user role.", false);
			Debug.WriteLine("AddUserRole_Click: Add panel opened.");
		}

		private void EditUserRole_Click(object sender, RoutedEventArgs e)
		{
			if (UserRolesGrid.SelectedItem is UserRole userRole)
			{
				isEditMode = true;
				selectedUserRole = userRole;
				PanelTitle.Text = "Edit User Role";
				cbStudentId.SelectedValue = userRole.IdStudent;
				cbRoleId.SelectedValue = userRole.IdRole;
				InputPanel.Visibility = Visibility.Visible;
				SetStatusMessage($"Editing user role {userRole.Id}.", false);
				Debug.WriteLine($"EditUserRole_Click: Editing UserRole (Id={userRole.Id}, IdStudent={userRole.IdStudent}, IdRole={userRole.IdRole})");
			}
			else
			{
				SetStatusMessage("Please select a user role to edit.", true);
				Debug.WriteLine("EditUserRole_Click: No user role selected.");
				MessageBox.Show("Please select a user role to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteUserRole_Click(object sender, RoutedEventArgs e)
		{
			if (UserRolesGrid.SelectedItem is UserRole userRole)
			{
				var result = MessageBox.Show($"Are you sure you want to delete user role {userRole.Id}?",
					"Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteUserRole(userRole.Id);
						LoadUserRoles();
						SetStatusMessage($"User role {userRole.Id} deleted successfully.", false);
						Debug.WriteLine($"DeleteUserRole_Click: Deleted UserRole (Id={userRole.Id})");
						MessageBox.Show("User role deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						SetStatusMessage($"Failed to delete user role: {ex.Message}", true);
						Debug.WriteLine($"DeleteUserRole_Click: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
						MessageBox.Show($"Failed to delete user role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				SetStatusMessage("Please select a user role to delete.", true);
				Debug.WriteLine("DeleteUserRole_Click: No user role selected.");
				MessageBox.Show("Please select a user role to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveUserRole_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra đầu vào
			if (cbStudentId.SelectedValue == null || cbRoleId.SelectedValue == null)
			{
				SetStatusMessage("Please select both Student ID and Role ID.", true);
				Debug.WriteLine("SaveUserRole_Click: SelectedValue is null (Student ID or Role ID).");
				MessageBox.Show("Please select both Student ID and Role ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			string studentId = cbStudentId.SelectedValue.ToString();
			string roleId = cbRoleId.SelectedValue.ToString();

			if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(roleId))
			{
				SetStatusMessage("Selected Student ID or Role ID is invalid.", true);
				Debug.WriteLine($"SaveUserRole_Click: Invalid values (StudentId='{studentId}', RoleId='{roleId}')");
				MessageBox.Show("Selected Student ID or Role ID is invalid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Debug.WriteLine($"SaveUserRole_Click: Attempting to save UserRole (StudentId={studentId}, RoleId={roleId}, IsEditMode={isEditMode})");

			try
			{
				var userRole = new UserRole
				{
					IdStudent = studentId,
					IdRole = roleId
				};

				// Kiểm tra IdStudent tồn tại trong [User]
				if (!UserStudentIdExists(studentId))
				{
					SetStatusMessage($"Student ID {studentId} does not exist in User table.", true);
					Debug.WriteLine($"SaveUserRole_Click: IdStudent {studentId} not found in [User].");
					MessageBox.Show($"Student ID {studentId} does not exist in User table.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Kiểm tra IdRole tồn tại
				if (!RoleIdExists(roleId))
				{
					SetStatusMessage($"Role ID {roleId} does not exist.", true);
					Debug.WriteLine($"SaveUserRole_Click: Role ID {roleId} not found in [Role].");
					MessageBox.Show($"Role ID {roleId} does not exist.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (isEditMode)
				{
					if (selectedUserRole == null)
					{
						SetStatusMessage("No user role selected for editing.", true);
						Debug.WriteLine("SaveUserRole_Click: selectedUserRole is null in Edit mode.");
						MessageBox.Show("No user role selected for editing.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					userRole.Id = selectedUserRole.Id;
					Debug.WriteLine($"SaveUserRole_Click: Updating UserRole (Id={userRole.Id})");
					dataAccess.UpdateUserRole(userRole);
					SetStatusMessage($"User role {userRole.Id} updated successfully.", false);
					MessageBox.Show("User role updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					Debug.WriteLine("SaveUserRole_Click: Checking if UserRole exists.");
					if (dataAccess.UserRoleExists(userRole.IdStudent, userRole.IdRole))
					{
						SetStatusMessage("This student already has this role.", true);
						Debug.WriteLine("SaveUserRole_Click: UserRole already exists.");
						MessageBox.Show("This student already has this role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					userRole.Id = dataAccess.GetNextUserRoleId();
					Debug.WriteLine($"SaveUserRole_Click: Adding UserRole (Id={userRole.Id})");
					dataAccess.AddUserRole(userRole);
					SetStatusMessage($"User role {userRole.Id} added successfully.", false);
					MessageBox.Show($"User role added successfully with ID {userRole.Id}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				LoadUserRoles();
				InputPanel.Visibility = Visibility.Collapsed;
				Debug.WriteLine("SaveUserRole_Click: Operation completed successfully.");
			}
			catch (Exception ex)
			{
				SetStatusMessage($"Failed to save user role: {ex.Message}", true);
				Debug.WriteLine($"SaveUserRole_Click: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show($"Failed to save user role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			SetStatusMessage("Input cancelled.", false);
			Debug.WriteLine("CancelInput_Click: Input panel closed.");
		}

		private void UserRolesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			SetStatusMessage("", false);
			Debug.WriteLine("UserRolesGrid_SelectionChanged: Input panel hidden.");
		}

		private void SetStatusMessage(string message, bool isError)
		{
			DebugText.Text = message;
			DebugText.Foreground = isError ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black;
			DebugText.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
		}

		private bool UserStudentIdExists(string studentId)
		{
			try
			{
				var userStudentIds = dataAccess.GetUserStudentIds();
				return userStudentIds.Any(u => u.IdStudent == studentId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"UserStudentIdExists: Error checking IdStudent {studentId} - {ex.Message}");
				return false;
			}
		}

		private bool RoleIdExists(string roleId)
		{
			try
			{
				var roles = dataAccess.GetRoles();
				return roles.Any(r => r.Id == roleId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RoleIdExists: Error checking Role ID {roleId} - {ex.Message}");
				return false;
			}
		}
	}
}