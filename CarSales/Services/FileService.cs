using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using CarSales.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace CarSales.Services
{
    /// <summary>
    /// Service to handle work with Files.
    /// </summary>
    public class FileService
    {
        private readonly string _supportedCsvDelimiter = ",";
        private readonly string[] _supportedFileExtensions = [".xml", ".csv"];
        private readonly XmlSerializer _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        public FileService()
        {
            _xmlSerializer = new XmlSerializer(typeof(SalesData));
        }

        /// <summary>
        /// Gets file extensions.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File extension.</returns>
        public static string GetFileExtension(string filePath) => Path.GetExtension(filePath).ToLowerInvariant();

        /// <summary>
        /// Loads sales data.
        /// </summary>
        /// <param name="xmlOrCsvFilePath">File path to sales data (.xml or .csv file).</param>
        /// <returns>Sales data.</returns>
        public SalesData LoadSalesData(string xmlOrCsvFilePath)
        {
            ValidateSalesDataFile(xmlOrCsvFilePath);

            var extension = GetFileExtension(xmlOrCsvFilePath);
            if (extension == _supportedFileExtensions[0])
                return LoadSalesDataFromXml(xmlOrCsvFilePath);
            else if (extension == _supportedFileExtensions[1])
                return LoadSalesDataFromCsv(xmlOrCsvFilePath);
            else
                throw new NotImplementedException($"Loading from file with extension '{extension}' is not supported.");
        }

        /// <summary>
        /// Saves sales data.
        /// </summary>
        /// <param name="xmlOrCsvFilePath">File path to sales data (.xml or .csv file).</param>
        /// <param name="salesData">Sales data.</param>
        public void SaveSalesData(string xmlOrCsvFilePath, SalesData salesData)
        {
            ValidateFileExtensions(xmlOrCsvFilePath, _supportedFileExtensions);

            var extension = GetFileExtension(xmlOrCsvFilePath);
            if (extension == _supportedFileExtensions[0])
                SaveSalesDataToXml(xmlOrCsvFilePath, salesData);
            else if (extension == _supportedFileExtensions[1])
                SaveSalesDataToCsv(xmlOrCsvFilePath, salesData);
            else
                throw new NotImplementedException($"Saving with file extension '{extension}' is not supported.");
        }

        /// <summary>
        /// Converts CSV DTOs to Sales data.
        /// </summary>
        /// <param name="csvDtos">DTO rows from CSV file.</param>
        /// <returns>Sales data.</returns>
        private SalesData ConvertCsvDTOsToSalesData(List<CsvSalesDataRowDto> csvDtos)
        {
            var salesData = new SalesData();
            foreach (var dto in csvDtos)
            {
                if (!salesData.Manufacturers.Select(m => m.Name).Contains(dto.Manufacturer))
                {
                    salesData.Manufacturers.Add(new Manufacturer()
                    {
                        Name = dto.Manufacturer,
                    });
                }

                var manufacturer = salesData.Manufacturers.First(m => m.Name == dto.Manufacturer);
                manufacturer.Vehicles.Add(new Vehicle()
                {
                    ModelName = dto.Model,
                    NetPrice = dto.NetPrice,
                    VatPercent = dto.VatPercent,
                    SoldOn = dto.SoldOn,
                });
            }

            return salesData;
        }

        /// <summary>
        /// Loads sales data from CSV file.
        /// </summary>
        /// <param name="csvFilePath">Filepath to CSV file with sales data.</param>
        /// <returns>Sales data.</returns>
        private SalesData LoadSalesDataFromCsv(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = _supportedCsvDelimiter,
                PrepareHeaderForMatch = args => args.Header.ToLower().Trim(),
            };

            List<CsvSalesDataRowDto> list;
            try
            {
                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, config);
                list = [.. csv.GetRecords<CsvSalesDataRowDto>()];
            }
            catch (Exception ex)
            {
                throw new FileServiceException($"Failed to load sales data from CSV '{csvFilePath}'.", ex);
            }

            return ConvertCsvDTOsToSalesData(list);
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
            catch (Exception ex)
            {
                throw new FileServiceException($"Failed to load sales data from XML '{xmlFilePath}'.", ex);
            }
        }

        /// <summary>
        /// Saves sales data to CSV file.
        /// </summary>
        /// <param name="csvFilePath">FilePath to a CSV file.</param>
        /// <param name="salesData">Sales data.</param>
        private void SaveSalesDataToCsv(string csvFilePath, SalesData salesData)
        {
            var csvRows = salesData.Manufacturers
                .SelectMany(m => m.Vehicles.Select(v => new CsvSalesDataRowDto()
                {
                    Manufacturer = m.Name,
                    Model = v.ModelName,
                    SoldOn = v.SoldOn,
                    NetPrice = v.NetPrice,
                    VatPercent = v.VatPercent,
                }))
                .ToList();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = _supportedCsvDelimiter,
            };

            try
            {
                using var writer = new StreamWriter(csvFilePath, false, System.Text.Encoding.UTF8);
                using var csv = new CsvWriter(writer, config);

                csv.WriteRecords(csvRows);
            }
            catch (Exception ex)
            {
                throw new FileServiceException($"Failed to save sales data to CSV file '{csvFilePath}'.", ex);
            }
        }

        /// <summary>
        /// Saves sales data to XML file.
        /// </summary>
        /// <param name="xmlFilePath">FilePath to a XML file.</param>
        /// <param name="salesData">Sales data.</param>
        private void SaveSalesDataToXml(string xmlFilePath, SalesData salesData)
        {
            try
            {
                using var writer = new StreamWriter(xmlFilePath);
                _xmlSerializer.Serialize(writer, salesData);
            }
            catch (Exception ex)
            {
                throw new FileServiceException($"Failed to save sales data to XML file '{xmlFilePath}'.", ex);
            }
        }

        /// <summary>
        /// Validates whether or not a filepath has allowed extension.
        /// </summary>
        /// <param name="filePath">FilePath.</param>
        /// <param name="allowedFileExtensions">Array of allowed file extensions.</param>
        /// <exception cref="ArgumentException">Thrown when filePath has unsupported file extension.</exception>
        private void ValidateFileExtensions(string filePath, string[] allowedFileExtensions)
        {
            var extension = GetFileExtension(filePath);
            if (!allowedFileExtensions.Contains(extension))
                throw new ArgumentException($"File extensions '{extension}' is not suppoerted. Supported file extensions: '{string.Join(", ", _supportedFileExtensions)}'");
        }

        /// <summary>
        /// Validate sales data file whether it exists, have required file extension, ...
        /// </summary>
        /// <param name="xmlOrCsvFilePath">XML or CSV path to file.</param>
        /// <exception cref="FileNotFoundException">Thrown when file does not exists.</exception>
        private void ValidateSalesDataFile(string xmlOrCsvFilePath)
        {
            ValidateFileExtensions(xmlOrCsvFilePath, _supportedFileExtensions);

            // Check file existance
            if (!File.Exists(xmlOrCsvFilePath))
                throw new FileNotFoundException($"File was not found. Path to file: '{xmlOrCsvFilePath}'");
        }

        public class FileServiceException(string? message, Exception? ex) : Exception(message, ex)
        {
        }

        /// <summary>
        /// Represents one row of sales data in the CSV file. <br />
        /// Data Transfer Object used exclusively for CSV serialization/deserialization.
        /// </summary>
        private sealed class CsvSalesDataRowDto
        {
            /// <summary>
            /// Gets manufacturer's name.
            /// </summary>
            public string Manufacturer { get; init; } = string.Empty;

            /// <summary>
            /// Gets model's name.
            /// </summary>
            public string Model { get; init; } = string.Empty;

            /// <summary>
            /// Gets net price.
            /// </summary>
            public double NetPrice { get; init; }

            /// <summary>
            /// Gets a date of sale (Can be NULL indicating that the vehicle was not yet sold).
            /// </summary>
            public DateTime? SoldOn { get; init; }

            /// <summary>
            /// Gets var percent.
            /// </summary>
            public double VatPercent { get; init; }
        }
    }
}