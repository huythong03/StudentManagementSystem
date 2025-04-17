using StudentManagementSystem.Models;
using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class SubjectForm : Window
	{
		public Subject Subject { get; private set; }
		public bool IsNew { get; private set; }

		public SubjectForm(Subject subject)
		{
			InitializeComponent();
			IsNew = subject == null;
			Subject = subject ?? new Subject();
			InitializeForm();
		}

		private void InitializeForm()
		{
			if (!IsNew)
			{
				txtId.Text = Subject.Id;
				txtName.Text = Subject.Name;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Subject.Id = txtId.Text;
			Subject.Name = txtName.Text;

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