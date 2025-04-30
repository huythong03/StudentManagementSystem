using StudentManagementSystem.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StudentManagementSystem.Views
{
	public partial class TeacherManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Teacher selectedTeacher;

		public TeacherManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadTeachers();
		}

		private void LoadTeachers()
		{
			try
			{
				TeachersGrid.ItemsSource = dataAccess.GetTeachers();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string GetNextTeacherId()
		{
			try
			{
				var teachers = dataAccess.GetTeachers();
				if (!teachers.Any())
					return "TCH0001";

				int maxId = teachers
					.Select(t => int.Parse(t.Id.Substring(3)))
					.Max();
				return $"TCH{(maxId + 1):D4}";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to generate next teacher ID: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return "TCH0001";
			}
		}

		private void AddTeacher_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedTeacher = null;
			PanelTitle.Text = "Add Teacher";
			txtId.Text = GetNextTeacherId();
			txtId.IsReadOnly = true;
			txtName.Text = "";
			txtEmail.Text = "";
			txtMajor.Text = "";
			txtProfessionalQualification.Text = "";
			chkGender.IsChecked = null;
			txtEthnicity.Text = "";
			chkPartyMember.IsChecked = false;
			txtForeignLanguageLevel.Text = "";
			txtITLevel.Text = "";
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditTeacher_Click(object sender, RoutedEventArgs e)
		{
			if (TeachersGrid.SelectedItem is Teacher teacher)
			{
				isEditMode = true;
				selectedTeacher = teacher;
				PanelTitle.Text = "Edit Teacher";
				txtId.Text = teacher.Id;
				txtId.IsReadOnly = true;
				txtName.Text = teacher.Name;
				txtEmail.Text = teacher.Email;
				txtMajor.Text = teacher.Major;
				txtProfessionalQualification.Text = teacher.ProfessionalQualification;
				chkGender.IsChecked = teacher.Gender;
				txtEthnicity.Text = teacher.Ethnicity;
				chkPartyMember.IsChecked = teacher.PartyMember;
				txtForeignLanguageLevel.Text = teacher.ForeignLanguageLevel;
				txtITLevel.Text = teacher.ITLevel;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a teacher to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
		{
			if (TeachersGrid.SelectedItem is Teacher teacher)
			{
				var result = MessageBox.Show($"Are you sure you want to delete teacher {teacher.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteTeacher(teacher.Id);
						LoadTeachers();
						MessageBox.Show("Teacher deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete teacher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a teacher to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveTeacher_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) ||
				string.IsNullOrWhiteSpace(txtName.Text) ||
				string.IsNullOrWhiteSpace(txtEmail.Text))
			{
				MessageBox.Show("Please fill in all required fields (ID, Name, Email).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			// Validate Email format
			if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var teacher = new Teacher
				{
					Id = txtId.Text,
					Name = txtName.Text,
					Email = txtEmail.Text,
					Major = string.IsNullOrWhiteSpace(txtMajor.Text) ? null : txtMajor.Text,
					ProfessionalQualification = string.IsNullOrWhiteSpace(txtProfessionalQualification.Text) ? null : txtProfessionalQualification.Text,
					Gender = chkGender.IsChecked,
					Ethnicity = string.IsNullOrWhiteSpace(txtEthnicity.Text) ? null : txtEthnicity.Text,
					PartyMember = chkPartyMember.IsChecked,
					ForeignLanguageLevel = string.IsNullOrWhiteSpace(txtForeignLanguageLevel.Text) ? null : txtForeignLanguageLevel.Text,
					ITLevel = string.IsNullOrWhiteSpace(txtITLevel.Text) ? null : txtITLevel.Text
				};

				if (isEditMode)
				{
					dataAccess.UpdateTeacher(teacher);
				}
				else
				{
					dataAccess.AddTeacher(teacher);
				}

				LoadTeachers();
				InputPanel.Visibility = Visibility.Collapsed;
				txtId.IsReadOnly = false;
				MessageBox.Show("Teacher saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

				txtId.Text = GetNextTeacherId();
				txtId.IsReadOnly = true;
			}
			catch (Exception ex)
			{
				string errorDetails = ex.InnerException != null ? $"\nDetails: {ex.InnerException.Message}" : "";
				MessageBox.Show($"Failed to save teacher: {ex.Message}{errorDetails}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			txtId.IsReadOnly = false;
			txtId.Text = GetNextTeacherId();
			txtId.IsReadOnly = true;
		}

		private void TeachersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			txtId.IsReadOnly = false;
			txtId.Text = GetNextTeacherId();
			txtId.IsReadOnly = true;
		}
	}

	public class GenderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool gender)
			{
				return gender == true ? "Male" : "Female";
			}
			return "Unknown";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}