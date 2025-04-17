using StudentManagementSystem.Models;
using System.Windows;

namespace StudentManagementSystem.Views
{
	public partial class ProvinceForm : Window
	{
		public Province Province { get; private set; }
		public bool IsNew { get; private set; }

		public ProvinceForm(Province province)
		{
			InitializeComponent();
			IsNew = province == null;
			Province = province ?? new Province();
			InitializeForm();
		}

		private void InitializeForm()
		{
			if (!IsNew)
			{
				txtId.Text = Province.Id.ToString();
				txtName.Text = Province.Name;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (!int.TryParse(txtId.Text, out int id))
			{
				MessageBox.Show("ID must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Province.Id = id;
			Province.Name = txtName.Text;

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