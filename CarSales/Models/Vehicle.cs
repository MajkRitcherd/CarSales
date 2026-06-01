using System.Xml.Serialization;

namespace CarSales.Models
{
    /// <summary>
    /// Represents one single vehicle with basic properties like model name, net price, VAT, ... <br />
    /// Data Transfer Object used exclusively for XML serialization/deserialization.
    /// </summary>
    public class Vehicle
    {
        private const string _NOT_SOLD = "NOT SOLD";

        /// <summary>
        /// Gets or sets a date of sale (Can be NULL indicating that the vehicle was not yet sold).
        /// </summary>
        [XmlElement("SoldOn")]
        public DateTime? SoldOn { get; set; }

        /// <summary>
        /// Gets Gross price (Price including VAT).
        /// </summary>
        public double GrossPrice => NetPrice * (1 + (VatPercent / 100));

        /// <summary>
        /// Gets or sets ID of a vehicle.
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of vehicle model.
        /// </summary>
        [XmlElement("Model")]
        public required string ModelName { get; set; }

        /// <summary>
        /// Gets or sets Net price (Price excluding VAT).
        /// </summary>
        [XmlElement("NetPrice")]
        public required double NetPrice { get; set; }

        /// <summary>
        /// Gets or sets VAT.
        /// </summary>
        [XmlElement("VatPercent")]
        public required double VatPercent { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Vehicle model '{ModelName}', " +
                $"Net price '{NetPrice}', " +
                $"VAT '{VatPercent}', " +
                $"Sold on '{(SoldOn.HasValue ? SoldOn.Value : _NOT_SOLD)}'";
        }
    }
}