using StudentManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class UserEnrollRequestsView : UserControl
	{
		private readonly DataAccess dataAccess;
		private readonly string username;

		public UserEnrollRequestsView(string username)
		{
			InitializeComponent();
			this.username = username;
			dataAccess = new DataAccess();
			LoadRequests();
		}

		private void LoadRequests()
		{
			try
			{
				RequestsGrid.ItemsSource = dataAccess.GetUserEnrollRequests(username);
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

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (RequestsGrid.SelectedItem is EnrollRequest selectedRequest)
			{
				if (selectedRequest.Status != "Pending")
				{
					MessageBox.Show("Only pending requests can be canceled.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var result = MessageBox.Show("Are you sure you want to cancel this request?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						bool success = dataAccess.CancelEnrollRequest(selectedRequest.RequestId, username);
						if (success)
						{
							MessageBox.Show("Request canceled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
							LoadRequests();
						}
						else
						{
							MessageBox.Show("Failed to cancel request. It may have been processed already.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a request to cancel.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}