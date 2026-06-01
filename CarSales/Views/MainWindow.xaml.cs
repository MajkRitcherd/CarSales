using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        [ObservableProperty]
        private Visibility _isCsvConverterVisible = Visibility.Hidden;

        [ObservableProperty]
        private string _openedFilePath = "Žádný soubor není načten";

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

        [GeneratedRegex("[^0-9.,]+")]
        private static partial Regex DoubleNumericRegex();

        /// <summary>
        /// Adds vehicle record to table.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        private void BtnClick_AddVehicleRecord(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TxtNewNetPrice.Text, out var netPrice))
                throw new ArgumentException($"Failed to parse Net price '{TxtNewNetPrice.Text}' to double.");

            if (!double.TryParse(TxtNewVatPercent.Text, out var vatPercent))
                throw new ArgumentException($"Failed to parse Vat percent '{TxtNewVatPercent.Text}' to double.");

            var vehicle = new Vehicle()
            {
                ModelName = TxtNewModel.Text,
                NetPrice = netPrice,
                VatPercent = vatPercent,
                SoldOn = DpNewSoldOn.SelectedDate,
            };

            var manufacturer = _salesData?.Manufacturers
                .FirstOrDefault(m => m.Name.Equals(TxtNewManufacturer.Text, StringComparison.OrdinalIgnoreCase));

            if (manufacturer == null)
            {
                manufacturer = new Manufacturer() { Name = TxtNewManufacturer.Text };
                _salesData?.Manufacturers.Add(manufacturer);
            }

            manufacturer.Vehicles.Add(vehicle);

            UpdateView();
        }

        /// <summary>
        /// Loads sales data from file.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
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

            var selectedFilePath = openFileDialog.FileName;
            _salesData = _fileService.LoadSalesData(selectedFilePath);
            OpenedFilePath = selectedFilePath;
            IsCsvConverterVisible = string.Equals(FileService.GetFileExtension(selectedFilePath), ".csv")
                ? Visibility.Visible
                : Visibility.Hidden;

            UpdateView();
        }

        /// <summary>
        /// Recalculates statistics on tab selection change.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
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
        /// Validates textbox for numeric values on CTRL+V.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        private void Txtnumeric_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (DoubleNumericRegex().IsMatch(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Validates textbox for numeric values.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        private void TxtNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = DoubleNumericRegex().IsMatch(e.Text);
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

        /// <summary>
        /// Updates a view.
        /// </summary>
        private void UpdateView()
        {
            UpdateGridData();
            RecalculateStatistics();
        }
    }
}