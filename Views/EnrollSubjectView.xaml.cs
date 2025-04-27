using StudentManagementSystem.Models;
using System.Windows;
using System.Windows.Controls;
namespace StudentManagementSystem.Views
{
	public partial class EnrollSubjectView : UserControl
	{
		private readonly DataAccess dataAccess;
		private readonly string username;

		public EnrollSubjectView(string username)
		{
			InitializeComponent();
			this.username = username;
			dataAccess = new DataAccess();
			LoadSubjects();
		}

		private void LoadSubjects()
		{
			try
			{
				SubjectsGrid.ItemsSource = dataAccess.GetAvailableSubjects(username);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RequestEnrollment_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject selectedSubject)
			{
				try
				{
					int requestId = dataAccess.CreateEnrollRequest(username, selectedSubject.Id);
					MessageBox.Show("Enrollment request submitted successfully. Waiting for admin approval.",
								  "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					LoadSubjects();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("Please select a subject to enroll.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}