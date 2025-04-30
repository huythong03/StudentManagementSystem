using StudentManagementSystem.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class TeacherClassAssignmentView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private TeacherClassAssignment selectedAssignment;
		private string oldTeacherId;
		private string oldClassId;

		public TeacherClassAssignmentView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadAssignments();
			LoadTeachers();
			LoadClasses();
		}

		private void LoadAssignments()
		{
			try
			{
				var assignments = dataAccess.GetTeacherClassAssignments();
				AssignmentsGrid.ItemsSource = assignments;
				Debug.WriteLine($"Loaded {assignments.Count} assignments.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load assignments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadTeachers()
		{
			try
			{
				var teachers = dataAccess.GetTeachers();
				cbTeacher.ItemsSource = teachers;
				Debug.WriteLine($"Loaded {teachers.Count} teachers.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadClasses()
		{
			try
			{
				var classes = dataAccess.GetClasses();
				cbClass.ItemsSource = classes;
				Debug.WriteLine($"Loaded {classes.Count} classes.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddAssignment_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedAssignment = null;
			PanelTitle.Text = "Add Assignment";
			cbTeacher.SelectedIndex = -1;
			cbClass.SelectedIndex = -1;
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditAssignment_Click(object sender, RoutedEventArgs e)
		{
			if (AssignmentsGrid.SelectedItem is TeacherClassAssignment assignment)
			{
				isEditMode = true;
				selectedAssignment = assignment;
				oldTeacherId = assignment.TeacherId;
				oldClassId = assignment.ClassId;
				PanelTitle.Text = "Edit Assignment";
				cbTeacher.SelectedValue = assignment.TeacherId;
				cbClass.SelectedValue = assignment.ClassId;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select an assignment to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteAssignment_Click(object sender, RoutedEventArgs e)
		{
			if (AssignmentsGrid.SelectedItem is TeacherClassAssignment assignment)
			{
				var result = MessageBox.Show($"Are you sure you want to delete this assignment?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteTeacherClassAssignment(assignment.TeacherId, assignment.ClassId);
						LoadAssignments();
						MessageBox.Show("Assignment deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select an assignment to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveAssignment_Click(object sender, RoutedEventArgs e)
		{
			if (cbTeacher.SelectedValue == null || cbClass.SelectedValue == null)
			{
				MessageBox.Show("Please select both a teacher and a class.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var assignment = new TeacherClassAssignment
				{
					TeacherId = cbTeacher.SelectedValue.ToString(),
					TeacherName = (cbTeacher.SelectedItem as Teacher)?.Name,
					ClassId = cbClass.SelectedValue.ToString(),
					Name = (cbClass.SelectedItem as Class)?.Name
				};

				Debug.WriteLine($"Saving Assignment: TeacherId={assignment.TeacherId}, TeacherName={assignment.TeacherName}, ClassId={assignment.ClassId}, ClassName={assignment.Name}");

				if (isEditMode)
				{
					dataAccess.UpdateTeacherClassAssignment(assignment, oldTeacherId, oldClassId);
				}
				else
				{
					dataAccess.AddTeacherClassAssignment(assignment);
				}

				LoadAssignments();
				InputPanel.Visibility = Visibility.Collapsed;
				Debug.WriteLine("Assignment saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				string errorDetails = ex.InnerException != null ? $"\nDetails: {ex.InnerException.Message}" : "";
				Debug.WriteLine($"Failed to save assignment: {ex.Message}{errorDetails}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void AssignmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}