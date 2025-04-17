using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserForm : Window
	{
		public User User { get; private set; }
		public bool IsNew { get; private set; }
		private readonly DataAccess dataAccess;

		public UserForm(User user)
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			IsNew = user == null;
			User = user ?? new User { Status = true, CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now };
			LoadStudents();
			InitializeForm();
		}

		private void LoadStudents()
		{
			cbStudentId.ItemsSource = dataAccess.GetStudents();
		}

		private void InitializeForm()
		{
			if (!IsNew)
			{
				cbStudentId.SelectedValue = User.IdStudent;
				txtUsername.Text = User.Username;
				txtPassword.Text = User.Password;
				txtNote.Text = User.Note;
				chkStatus.IsChecked = User.Status;
				dpCreatedAt.SelectedDate = User.CreatedAt;
			}
			else
			{
				dpCreatedAt.SelectedDate = DateTime.Now;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (cbStudentId.SelectedValue == null || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			User.IdStudent = cbStudentId.SelectedValue.ToString();
			User.Username = txtUsername.Text;
			User.Password = txtPassword.Text;
			User.Note = txtNote.Text;
			User.Status = chkStatus.IsChecked ?? false;
			User.CreatedAt = dpCreatedAt.SelectedDate ?? DateTime.Now;
			User.ModifiedAt = DateTime.Now;

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