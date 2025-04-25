using System;
using System.Windows;
using System.Windows.Controls;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Views
{
	public partial class SubjectManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Subject selectedSubject;

		public SubjectManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadSubjects();
		}

		private void LoadSubjects()
		{
			SubjectsGrid.ItemsSource = dataAccess.GetSubjects();
		}

		private void AddSubject_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedSubject = null;
			PanelTitle.Text = "Add Subject";
			txtId.Text = "";
			txtName.Text = "";
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditSubject_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject subject)
			{
				isEditMode = true;
				selectedSubject = subject;
				PanelTitle.Text = "Edit Subject";
				txtId.Text = subject.Id;
				txtName.Text = subject.Name;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a subject to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteSubject_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject subject)
			{
				var result = MessageBox.Show($"Are you sure you want to delete subject {subject.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteSubject(subject.Id);
						LoadSubjects();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a subject to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveSubject_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var subject = new Subject
				{
					Id = txtId.Text,
					Name = txtName.Text
				};

				if (isEditMode)
				{
					dataAccess.UpdateSubject(subject);
				}
				else
				{
					dataAccess.AddSubject(subject);
				}

				LoadSubjects();
				InputPanel.Visibility = Visibility.Collapsed;
				MessageBox.Show("Subject saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void SubjectsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}