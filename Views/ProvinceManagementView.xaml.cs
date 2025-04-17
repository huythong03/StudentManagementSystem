using StudentManagementSystem.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementSystem.Views
{
	public partial class ProvinceManagementView : UserControl
	{
		private readonly DataAccess dataAccess;
		private List<Province> provinces;

		public ProvinceManagementView()
		{
			InitializeComponent();
			dataAccess = new DataAccess();
			LoadProvinces();
		}

		private void LoadProvinces()
		{
			try
			{
				provinces = dataAccess.GetProvinces();
				ProvincesGrid.ItemsSource = provinces;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddProvince_Click(object sender, RoutedEventArgs e)
		{
			ProvinceForm provinceForm = new ProvinceForm(null);
			if (provinceForm.ShowDialog() == true)
			{
				try
				{
					dataAccess.AddProvince(provinceForm.Province);
					LoadProvinces();
					MessageBox.Show("Province added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void EditProvince_Click(object sender, RoutedEventArgs e)
		{
			if (ProvincesGrid.SelectedItem is Province selectedProvince)
			{
				ProvinceForm provinceForm = new ProvinceForm(selectedProvince);
				if (provinceForm.ShowDialog() == true)
				{
					try
					{
						dataAccess.UpdateProvince(provinceForm.Province);
						LoadProvinces();
						MessageBox.Show("Province updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a province to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DeleteProvince_Click(object sender, RoutedEventArgs e)
		{
			if (ProvincesGrid.SelectedItem is Province selectedProvince)
			{
				if (MessageBox.Show($"Are you sure you want to delete {selectedProvince.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						dataAccess.DeleteProvince(selectedProvince.Id);
						LoadProvinces();
						MessageBox.Show("Province deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a province to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}