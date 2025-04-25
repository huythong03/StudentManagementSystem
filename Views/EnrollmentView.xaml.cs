using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Collections.Generic;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Views
{
	public partial class EnrollmentView : UserControl
	{
		private readonly DataAccess dataAccess;

		public EnrollmentView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadEnrollments();
		}

		private void LoadEnrollments()
		{
			try
			{
				var enrollments = dataAccess.GetAllEnrollments();
				EnrollmentsGrid.ItemsSource = enrollments;
				Debug.WriteLine($"LoadEnrollments: Loaded {enrollments.Count} enrollments.");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"LoadEnrollments: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Search_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string searchValue = txtSearchStudentName.Text.Trim();
				if (string.IsNullOrWhiteSpace(searchValue))
				{
					MessageBox.Show("Please enter a student name or ID to search.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var grades = dataAccess.SearchStudentGrades(searchValue);
				if (grades.Count == 0)
				{
					MessageBox.Show($"No enrollments found for search value '{searchValue}'.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					EnrollmentsGrid.ItemsSource = new List<Enrol>();
					return;
				}

				// Get all students to map StudentName to IdStudent
				var students = dataAccess.GetStudents();
				var enrollments = new List<Enrol>();

				foreach (var grade in grades)
				{
					if (!string.IsNullOrWhiteSpace(grade.SubjectName))
					{
						// Find IdStudent from StudentName
						var student = students.Find(s => s.Name == grade.StudentName);
						if (student == null)
						{
							Debug.WriteLine($"Search_Click: StudentName '{grade.StudentName}' not found in Student table.");
							continue; // Skip if student not found
						}

						// Find IdSubject from SubjectName
						var subjects = dataAccess.GetSubjects();
						var subject = subjects.Find(s => s.Name == grade.SubjectName);

						var enrol = new Enrol
						{
							IdStudent = student.Id,
							IdSubject = subject?.Id,
							Name = grade.SubjectName,
							StudentName = grade.StudentName,
							Mark = grade.Mark
						};
						enrollments.Add(enrol);
						Debug.WriteLine($"Search_Click: Added enrol - IdStudent={enrol.IdStudent}, StudentName={enrol.StudentName}, SubjectName={enrol.Name}, Mark={enrol.Mark}");
					}
				}

				if (enrollments.Count == 0)
				{
					MessageBox.Show($"No matching enrollments found for search value '{searchValue}'.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					EnrollmentsGrid.ItemsSource = new List<Enrol>();
					return;
				}

				EnrollmentsGrid.ItemsSource = enrollments;
				EnrollmentsGrid.Items.Refresh(); // Force refresh
				Debug.WriteLine($"Search_Click: Found {enrollments.Count} enrollments for search value '{searchValue}'.");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Search_Click: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			txtSearchStudentName.Text = string.Empty;
			LoadEnrollments();
			Debug.WriteLine("Refresh_Click: Enrollments refreshed.");
		}
	}
}