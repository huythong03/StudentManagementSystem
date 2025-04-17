using StudentManagementSystem.Models;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserRoleForm : Window
	{
		public UserRole UserRole { get; private set; }
		public bool IsNew { get; private set; }
		private readonly DataAccess dataAccess;

		public UserRoleForm(UserRole userRole)
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			IsNew = userRole == null;
			UserRole = userRole ?? new UserRole();
			LoadData();
			InitializeForm();
		}

		private void LoadData()
		{
			cbStudentId.ItemsSource = dataAccess.GetStudents();
			cbRoleId.ItemsSource = dataAccess.GetRoles();
		}

		private void InitializeForm()
		{
			if (!IsNew)
			{
				txtId.Text = UserRole.Id;
				cbStudentId.SelectedValue = UserRole.IdStudent;
				cbRoleId.SelectedValue = UserRole.IdRole;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || cbStudentId.SelectedValue == null || cbRoleId.SelectedValue == null)
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			UserRole.Id = txtId.Text;
			UserRole.IdStudent = cbStudentId.SelectedValue.ToString();
			UserRole.IdRole = cbRoleId.SelectedValue.ToString();

			DialogResult = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}