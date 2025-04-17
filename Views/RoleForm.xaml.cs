using StudentManagementSystem.Models;
using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class RoleForm : Window
	{
		public Role Role { get; private set; }
		public bool IsNew { get; private set; }

		public RoleForm(Role role)
		{
			InitializeComponent();
			IsNew = role == null;
			Role = role ?? new Role { Status = true };
			InitializeForm();
		}

		private void InitializeForm()
		{
			if (!IsNew)
			{
				txtId.Text = Role.Id;
				txtName.Text = Role.Name;
				chkStatus.IsChecked = Role.Status;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Role.Id = txtId.Text;
			Role.Name = txtName.Text;
			Role.Status = chkStatus.IsChecked ?? false;

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