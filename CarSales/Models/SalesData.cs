using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace CarSales.Models
{
    /// <summary>
    /// Represents sales data. <br />
    /// Data Transfer Object used exclusively for XML serialization/deserialization.
    /// </summary>
    [XmlRoot("SalesData")]
    public class SalesData
    {
        /// <summary>
        /// Gets collection of manufacturers.
        /// </summary>
        [XmlElement("Manufacturer")]
        public ObservableCollection<Manufacturer> Manufacturers { get; private set; } = [];
    }
}
