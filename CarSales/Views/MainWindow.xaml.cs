using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CarSales.Models;
using CarSales.Services;
using CarSales.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;

namespace CarSales.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        private readonly FileService _fileService;

        [ObservableProperty]
        private double _averageNetPrice;

        private SalesData? _salesData;

        [ObservableProperty]
        private double _totalRevenue;

        [ObservableProperty]
        private int _totalVehiclesCount;

        public MainWindow()
        {
            InitializeComponent();
            _fileService = new FileService();
            this.DataContext = this;
        }

        /// <summary>
        /// Gets collection of vehicle row view models.
        /// </summary>
        public ObservableCollection<VehicleRowViewModel> VehicleRowViewModels { get; } = [];

        /// <summary>
        /// Gets collection of vehicle row view models that were sold during weekend.
        /// </summary>
        public ObservableCollection<WeekendVehicleRowSummaryViewModel> WeekendVehicleRowSummaryViewModels { get; } = [];

        private void BtnClick_LoadSalesData(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Podporované soubory (*.xml;*.csv)|*.xml;*.csv|XML soubory (*.xml)|*.xml|CSV soubory (*.csv)|*.csv",
                Title = "Vyberte soubor s prodeji aut"
            };

            // Return when failed to open dialog
            if (openFileDialog.ShowDialog() == false)
                return;

            _salesData = _fileService.LoadSalesData(openFileDialog.FileName);

            UpdateGridData();
            RecalculateStatistics();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignore other selection than TabControl
            if (e.Source is TabControl)
                RecalculateStatistics();
        }

        /// <summary>
        /// Recalculates statistics.
        /// </summary>
        private void RecalculateStatistics()
        {
            if (VehicleRowViewModels == null || !VehicleRowViewModels.Any())
                return;

            int vehicleCount;
            double totalRevenue, averageNetPrice;
            if (MainTabControl.SelectedIndex == 0) // All vehicles
            {
                vehicleCount = VehicleRowViewModels.Count;
                totalRevenue = VehicleRowViewModels.Sum(x => x.Vehicle.GrossPrice);
                averageNetPrice = vehicleCount > 0
                    ? VehicleRowViewModels.Average(x => x.Vehicle.NetPrice)
                    : 0;
            }
            else // Weekend vehicles
            {
                vehicleCount = WeekendVehicleRowSummaryViewModels.Count;
                totalRevenue = WeekendVehicleRowSummaryViewModels.Sum(x => x.TotalGrossPrice);
                averageNetPrice = vehicleCount > 0
                    ? WeekendVehicleRowSummaryViewModels.Sum(x => x.TotalNetPrice) / vehicleCount
                    : 0;
            }

            TotalVehiclesCount = vehicleCount;
            TotalRevenue = totalRevenue;
            AverageNetPrice = averageNetPrice;
        }

        /// <summary>
        /// Updates grid data.
        /// </summary>
        private void UpdateGridData()
        {
            if (_salesData == null)
                return;

            VehicleRowViewModels.Clear();
            foreach (var manufacturer in _salesData.Manufacturers)
            {
                foreach (var vehicle in manufacturer.Vehicles)
                {
                    var row = new VehicleRowViewModel(manufacturer.Name, vehicle);

                    VehicleRowViewModels.Add(row);
                }
            }

            var weekendSummaryModels = VehicleRowViewModels
                        .Where(x => x.Vehicle.SoldOn.HasValue
                                    && (x.Vehicle.SoldOn.Value.DayOfWeek == DayOfWeek.Saturday || x.Vehicle.SoldOn.Value.DayOfWeek == DayOfWeek.Sunday))
                        .GroupBy(x => new { x.ManufacturerName, x.Vehicle.ModelName })
                        .Select(group => new WeekendVehicleRowSummaryViewModel()
                        {
                            VehicleManufacturerName = group.Key.ManufacturerName,
                            VehicleModelName = group.Key.ModelName,
                            TotalVehiclesSold = group.Count(),
                            TotalNetPrice = group.Sum(x => x.Vehicle.NetPrice),
                            TotalGrossPrice = group.Sum(x => x.Vehicle.GrossPrice),
                        });

            WeekendVehicleRowSummaryViewModels.Clear();
            foreach (var weekendSummary in weekendSummaryModels)
            {
                WeekendVehicleRowSummaryViewModels.Add(weekendSummary);
            }
        }
    }
}