using CarSales.Models;

namespace CarSales.ViewModels
{
    /// <summary>
    /// View model of vehicle row.
    /// </summary>
    /// <param name="manufacturerName">Manufacturer's name.</param>
    /// <param name="vehicle">Vehicle.</param>
    public class VehicleRowViewModel(string manufacturerName, Vehicle vehicle)
    {
        /// <summary>
        /// Gets manufacturer's name.
        /// </summary>
        public string ManufacturerName { get; init; } = manufacturerName;

        /// <summary>
        /// Gets a vehicle.
        /// </summary>
        public Vehicle Vehicle { get; init; } = vehicle;
    }
}