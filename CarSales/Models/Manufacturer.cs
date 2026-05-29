using System.Xml.Serialization;

namespace CarSales.Models
{
    /// <summary>
    /// Represents Car manufacturer with a list of sold vehicles. <br />
    /// Data Transfer Object used exclusively for XML serialization/deserialization.
    /// </summary>
    internal class Manufacturer
    {
        /// <summary>
        /// Gets or sets manufacturer's name.
        /// </summary>
        [XmlAttribute("Manufacturer")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets list of sold vehicles.
        /// </summary>
        public VehicleList Vehicles { get; private set; } = [];

        /// <inheritdoc/>
        /// <remarks>
        /// Shows only vechile count to not overwhelm the string with each vehicle data.
        /// </remarks>
        public override string ToString()
        {
            return $"Manufacturer name '{Name}', " +
                $"Vehicle count '{Vehicles.Count}'";
        }
    }
}