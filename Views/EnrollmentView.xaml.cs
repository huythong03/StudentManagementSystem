using System.Windows;
using System.Windows.Controls;

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
				EnrollmentsGrid.ItemsSource = dataAccess.GetAllEnrollments();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}