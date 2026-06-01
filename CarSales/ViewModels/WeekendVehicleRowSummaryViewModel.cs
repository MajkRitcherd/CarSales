namespace CarSales.ViewModels
{
    /// <summary>
    /// Weekend vehicle row summary view model to hold summary data of one vehicle model.
    /// </summary>
    public class WeekendVehicleRowSummaryViewModel
    {
        /// <summary>
        /// Gets total gross price.
        /// </summary>
        public double TotalGrossPrice { get; init; }

        /// <summary>
        /// Gets total net price.
        /// </summary>
        public double TotalNetPrice { get; init; }

        /// <summary>
        /// Gets total amount of sold vehicles.
        /// </summary>
        public int TotalVehiclesSold { get; init; }

        /// <summary>
        /// Gets vehicle's manufacturer name.
        /// </summary>
        public string VehicleManufacturerName { get; init; } = string.Empty;

        /// <summary>
        /// Gets vehicle's model name.
        /// </summary>
        public string VehicleModelName { get; init; } = string.Empty;
    }
}