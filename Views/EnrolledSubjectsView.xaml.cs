using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class EnrolledSubjectsView : UserControl
	{
		private readonly DataAccess dataAccess;
		private readonly string username;

		public EnrolledSubjectsView(string username)
		{
			InitializeComponent();
			this.username = username;
			dataAccess = new DataAccess();
			LoadEnrolledSubjects();
		}

		private void LoadEnrolledSubjects()
		{
			try
			{
				EnrolledSubjectsGrid.ItemsSource = dataAccess.GetEnrolledSubjects(username);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}