using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class SearchGradesView : UserControl
	{
		private readonly DataAccess dataAccess;

		public SearchGradesView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
		}

		private void Search_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtStudentId.Text))
			{
				MessageBox.Show("Please enter a student ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				GradesGrid.ItemsSource = dataAccess.SearchStudentGrades(txtStudentId.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}