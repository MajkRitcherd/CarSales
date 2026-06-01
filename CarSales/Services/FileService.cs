using System.IO;
using System.Xml.Serialization;
using CarSales.Models;

namespace CarSales.Services
{
    /// <summary>
    /// Service to handle work with Files.
    /// </summary>
    internal class FileService
    {
        private readonly string[] _allowedExtensions = [".xml", ".csv"];
        private readonly XmlSerializer _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        public FileService()
        {
            _xmlSerializer = new XmlSerializer(typeof(SalesData));
        }

        /// <summary>
        /// Loads sales data.
        /// </summary>
        /// <param name="xmlOrCsvFilePath">File path to sales data (.xml or .csv file).</param>
        /// <returns>Sales data.</returns>
        public SalesData LoadSalesData(string xmlOrCsvFilePath)
        {
            ValidateSalesDataFile(xmlOrCsvFilePath);

            var extension = GetFileExtension(xmlOrCsvFilePath);
            if (extension == _allowedExtensions[0])
            {
                return LoadSalesDataFromXml(xmlOrCsvFilePath);
            }
            else
            {
                return LoadSalesDataFromCsv(xmlOrCsvFilePath);
            }
        }

        /// <summary>
        /// Gets file extensions.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File extension.</returns>
        private static string GetFileExtension(string filePath) => Path.GetExtension(filePath).ToLowerInvariant();

        /// <summary>
        /// Loads sales data from CSV file.
        /// </summary>
        /// <param name="csvFilePath">Filepath to CSV file with sales data.</param>
        /// <returns>Sales data.</returns>
        private SalesData LoadSalesDataFromCsv(string csvFilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads sales data from XML file.
        /// </summary>
        /// <param name="xmlFilePath">Filepath to XML file with sales data.</param>
        /// <returns>Sales data.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private SalesData LoadSalesDataFromXml(string xmlFilePath)
        {
            try
            {
                using var reader = new StreamReader(xmlFilePath);
                var salesData = (SalesData?)_xmlSerializer.Deserialize(reader)
                    ?? throw new InvalidOperationException($"The file '{xmlFilePath}' was sucessfully read, but its content is empty or invalid.");

                return salesData;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize file '{xmlFilePath}'. Validate the structure of XML file.", ex);
            }
        }

        /// <summary>
        /// Validate sales data file whether it exists, have required file extension, ...
        /// </summary>
        /// <param name="xmlOrCsvFilePath">XML or CSV path to file.</param>
        /// <exception cref="ArgumentException">Thrown when file extension is not one of <see cref="_allowedExtensions"/>.</exception>
        /// <exception cref="FileNotFoundException">Thrown when file does not exists.</exception>
        private void ValidateSalesDataFile(string xmlOrCsvFilePath)
        {
            // Check allowed file extensions
            var extension = GetFileExtension(xmlOrCsvFilePath);
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException($"File extensions '{extension}' is not suppoerted. Supported file extensions: '{string.Join(", ", _allowedExtensions)}'");

            // Check file existance
            if (!File.Exists(xmlOrCsvFilePath))
                throw new FileNotFoundException($"File was not found. Path to file: '{xmlOrCsvFilePath}'");
        }
    }
}