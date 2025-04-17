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

		private void Enroll_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectsGrid.SelectedItem is Subject selectedSubject)
			{
				try
				{
					dataAccess.EnrollSubject(username, selectedSubject.Id);
					LoadSubjects();
					MessageBox.Show("Subject enrolled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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