using System;
using System.Windows;
using System.Windows.Controls;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Views
{
	public partial class StudentManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Student selectedStudent;

		public StudentManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadStudents();
			LoadProvinces();
			SetupGenderComboBox();
		}

		private void LoadStudents()
		{
			StudentsGrid.ItemsSource = dataAccess.GetStudents();
		}

		private void LoadProvinces()
		{
			cmbProvince.ItemsSource = dataAccess.GetProvinces();
		}

		private void SetupGenderComboBox()
		{
			cmbGender.SelectedIndex = 0; // Default to Male
		}

		private void AddStudent_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedStudent = null;
			PanelTitle.Text = "Add Student";
			txtId.Text = "";
			txtName.Text = "";
			dpBOF.SelectedDate = null;
			cmbGender.SelectedIndex = 0;
			cmbProvince.SelectedIndex = -1;
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditStudent_Click(object sender, RoutedEventArgs e)
		{
			if (StudentsGrid.SelectedItem is Student student)
			{
				isEditMode = true;
				selectedStudent = student;
				PanelTitle.Text = "Edit Student";
				txtId.Text = student.Id;
				txtName.Text = student.Name;
				dpBOF.SelectedDate = student.BOF;
				cmbGender.SelectedValue = student.Gender ? "True" : "False";
				cmbProvince.SelectedValue = student.IdProvince;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a student to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteStudent_Click(object sender, RoutedEventArgs e)
		{
			if (StudentsGrid.SelectedItem is Student student)
			{
				var result = MessageBox.Show($"Are you sure you want to delete student {student.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteStudent(student.Id);
						LoadStudents();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a student to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveStudent_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text) || dpBOF.SelectedDate == null || cmbProvince.SelectedValue == null)
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var student = new Student
				{
					Id = txtId.Text,
					Name = txtName.Text,
					BOF = dpBOF.SelectedDate.Value,
					Gender = ((ComboBoxItem)cmbGender.SelectedItem).Tag.ToString() == "True",
					IdProvince = (int)cmbProvince.SelectedValue
				};

				if (isEditMode)
				{
					dataAccess.UpdateStudent(student);
				}
				else
				{
					dataAccess.AddStudent(student);
				}

				LoadStudents();
				InputPanel.Visibility = Visibility.Collapsed;
				MessageBox.Show("Student saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}