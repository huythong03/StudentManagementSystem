using StudentManagementSystem.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class ProvinceManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private bool isEditMode;
		private Province selectedProvince;

		public ProvinceManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadProvinces();
		}

		private void LoadProvinces()
		{
			ProvincesGrid.ItemsSource = dataAccess.GetProvinces();
		}

		private void AddProvince_Click(object sender, RoutedEventArgs e)
		{
			isEditMode = false;
			selectedProvince = null;
			PanelTitle.Text = "Add Province";
			txtName.Text = "";
			InputPanel.Visibility = Visibility.Visible;
		}

		private void EditProvince_Click(object sender, RoutedEventArgs e)
		{
			if (ProvincesGrid.SelectedItem is Province province)
			{
				isEditMode = true;
				selectedProvince = province;
				PanelTitle.Text = "Edit Province";
				txtName.Text = province.Name;
				InputPanel.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Please select a province to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteProvince_Click(object sender, RoutedEventArgs e)
		{
			if (ProvincesGrid.SelectedItem is Province province)
			{
				var result = MessageBox.Show($"Are you sure you want to delete province {province.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteProvince(province.Id);
						LoadProvinces();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to delete province: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a province to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveProvince_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtName.Text))
			{
				MessageBox.Show("Please enter a province name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var province = new Province
				{
					Name = txtName.Text
				};

				if (isEditMode)
				{
					province.Id = selectedProvince.Id;
					dataAccess.UpdateProvince(province);
				}
				else
				{
					// Get the highest existing ID and increment by 1
					var provinces = dataAccess.GetProvinces();
					int maxId = provinces.Any() ? provinces.Max(p => p.Id) : 0;
					province.Id = maxId + 1;
					dataAccess.AddProvince(province);
				}

				LoadProvinces();
				InputPanel.Visibility = Visibility.Collapsed;
				MessageBox.Show("Province saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save province: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}

		private void ProvincesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InputPanel.Visibility = Visibility.Collapsed;
		}
	}
}