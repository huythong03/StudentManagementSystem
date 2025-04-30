using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class ClassManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Class selectedClass;

		public ClassManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadClasses();
		}

		private void LoadClasses()
		{
			try
			{
				ClassesGrid.ItemsSource = dataAccess.GetClasses();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddClass_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedClass = null;
			PanelTitle.Text = "Add Class";
			txtId.Text = "";
			txtName.Text = "";
			txtSubjectId.Text = "";
			dpStartDate.SelectedDate = null;
			dpEndDate.SelectedDate = null;
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditClass_Click(object sender, RoutedEventArgs e)
		{
			if (ClassesGrid.SelectedItem is Class classObj)
			{
				isEditMode = true;
				selectedClass = classObj;
				PanelTitle.Text = "Edit Class";
				txtId.Text = classObj.Id;
				txtId.IsReadOnly = true; // Prevent editing ID in edit mode
				txtName.Text = classObj.Name;
				txtSubjectId.Text = classObj.SubjectId;
				dpStartDate.SelectedDate = classObj.StartDate;
				dpEndDate.SelectedDate = classObj.EndDate;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a class to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteClass_Click(object sender, RoutedEventArgs e)
		{
			if (ClassesGrid.SelectedItem is Class classObj)
			{
				var result = MessageBox.Show($"Are you sure you want to delete class {classObj.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteClass(classObj.Id);
						LoadClasses();
						MessageBox.Show("Class deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a class to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveClass_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) ||
				string.IsNullOrWhiteSpace(txtName.Text) ||
				string.IsNullOrWhiteSpace(txtSubjectId.Text) ||
				!dpStartDate.SelectedDate.HasValue ||
				!dpEndDate.SelectedDate.HasValue)
			{
				MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (dpEndDate.SelectedDate < dpStartDate.SelectedDate)
			{
				MessageBox.Show("End Date must be after Start Date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var classObj = new Class
				{
					Id = txtId.Text,
					Name = txtName.Text,
					SubjectId = txtSubjectId.Text,
					StartDate = dpStartDate.SelectedDate.Value,
					EndDate = dpEndDate.SelectedDate.Value
				};

				if (isEditMode)
				{
					dataAccess.UpdateClass(classObj);
				}
				else
				{
					dataAccess.AddClass(classObj);
				}

				LoadClasses();
				InputPanel.Visibility = Visibility.Collapsed;
				txtId.IsReadOnly = false; // Reset for next add
				MessageBox.Show("Class saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			txtId.IsReadOnly = false; // Reset for next add
		}

		private void ClassesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
			txtId.IsReadOnly = false; // Reset for next add
		}
	}
}