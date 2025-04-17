using StudentManagementSystem.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class SubjectManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<Subject> subjects;

		public SubjectManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadSubjects();
		}

		private void LoadSubjects()
		{
			subjects = dataAccess.GetSubjects();
			SubjectsGrid.ItemsSource = subjects;
		}

		private void AddSubject_Click(object sender, RoutedEventArgs e)
		{
			SubjectForm subjectForm = new SubjectForm(null);
			if (subjectForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddSubject(subjectForm.Subject);
					LoadSubjects();
					MessageBox.Show("Subject added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Error adding subject: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditSubject_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject selectedSubject)
			{
				SubjectForm subjectForm = new SubjectForm(selectedSubject);
				if (subjectForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateSubject(subjectForm.Subject);
						LoadSubjects();
						MessageBox.Show("Subject updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error updating subject: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a subject to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteSubject_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject selectedSubject)
			{
				if (MessageBox.Show($"Are you sure you want to delete {selectedSubject.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteSubject(selectedSubject.Id);
						LoadSubjects();
						MessageBox.Show("Subject deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqlException ex)
					{
						MessageBox.Show($"Error deleting subject: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a subject to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}