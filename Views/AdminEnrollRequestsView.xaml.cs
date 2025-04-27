using StudentManagementSystem.Models;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class AdminEnrollRequestsView : UserControl
	{
		private readonly DataAccess dataAccess;

		public AdminEnrollRequestsView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadRequests();
		}

		private void LoadRequests()
		{
			try
			{
				RequestsGrid.ItemsSource = dataAccess.GetPendingEnrollRequests();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			LoadRequests();
		}

		private void ApproveButton_Click(object sender, RoutedEventArgs e)
		{
			if (RequestsGrid.SelectedItem is EnrollRequest selectedRequest)
			{
				try
				{
					bool success = dataAccess.ApproveEnrollRequest(selectedRequest.RequestId);
					if (success)
					{
						MessageBox.Show("Request approved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
						LoadRequests();
					}
					else
					{
						MessageBox.Show("Failed to approve request. It may have been processed already.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("Please select a request to approve.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void RejectButton_Click(object sender, RoutedEventArgs e)
		{
			if (RequestsGrid.SelectedItem is EnrollRequest selectedRequest)
			{
				try
				{
					bool success = dataAccess.RejectEnrollRequest(selectedRequest.RequestId);
					if (success)
					{
						MessageBox.Show("Request rejected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
						LoadRequests();
					}
					else
					{
						MessageBox.Show("Failed to reject request. It may have been processed already.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("Please select a request to reject.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}