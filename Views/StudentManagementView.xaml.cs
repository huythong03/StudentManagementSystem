using StudentManagementSystem.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class StudentManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<Student> students;

		public StudentManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadStudents();
		}

		private void LoadStudents()
		{
			students = dataAccess.GetStudents();
			StudentsGrid.ItemsSource = students;
		}

		private void AddStudent_Click(object sender, RoutedEventArgs e)
		{
			StudentForm studentForm = new StudentForm(null);
			if (studentForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddStudent(studentForm.Student);
					LoadStudents();
					MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Error adding student: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditStudent_Click(object sender, RoutedEventArgs e)
		{
			if (StudentsGrid.SelectedItem is Student selectedStudent)
			{
				StudentForm studentForm = new StudentForm(selectedStudent);
				if (studentForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateStudent(studentForm.Student);
						LoadStudents();
						MessageBox.Show("Student updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error updating student: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a student to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteStudent_Click(object sender, RoutedEventArgs e)
		{
			if (StudentsGrid.SelectedItem is Student selectedStudent)
			{
				if (MessageBox.Show($"Are you sure you want to delete {selectedStudent.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteStudent(selectedStudent.Id);
						LoadStudents();
						MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error deleting student: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a student to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}