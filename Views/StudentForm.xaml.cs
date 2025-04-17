using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class StudentForm : Window
	{
		private readonly DataAccess dataAccess;
		public Student Student { get; private set; }
		public bool IsNew { get; private set; }

		public StudentForm(Student student)
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			IsNew = student == null;
			Student = student ?? new Student();
			InitializeForm();
		}

		private void InitializeForm()
		{
			try
			{
				// Load provinces
				cmbProvince.ItemsSource = dataAccess.GetProvinces();

				if (!IsNew)
				{
					txtId.Text = Student.Id;
					txtName.Text = Student.Name;
					dpBOF.SelectedDate = Student.BOF;
					cmbProvince.SelectedItem = dataAccess.GetProvinces().Find(p => p.Id == Student.IdProvince);
					cmbGender.SelectedIndex = Student.Gender ? 0 : 1; // Male (True) = 0, Female (False) = 1
				}
				else
				{
					cmbGender.SelectedIndex = -1; // No default selection
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error initializing form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Validate inputs
				if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
				{
					MessageBox.Show("ID and Name are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (!dpBOF.SelectedDate.HasValue)
				{
					MessageBox.Show("Please select a Date of Birth.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (cmbProvince.SelectedItem == null)
				{
					MessageBox.Show("Please select a Province.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (cmbGender.SelectedItem == null)
				{
					MessageBox.Show("Please select a Gender.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Populate Student object
				Student.Id = txtId.Text;
				Student.Name = txtName.Text;
				Student.BOF = dpBOF.SelectedDate.Value;
				Student.IdProvince = ((Province)cmbProvince.SelectedItem).Id;

				// Convert Gender ComboBox selection to bool
				var selectedGender = (ComboBoxItem)cmbGender.SelectedItem;
				Student.Gender = bool.Parse(selectedGender.Tag.ToString());

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error saving student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}