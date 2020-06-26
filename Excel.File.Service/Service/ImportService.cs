using Excel.File.Service.Service.Contracts;
using Excel.File.Service.Service.Extensions;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Excel.File.Service.Service
{
    internal class ImportService : IImportService
    {
        static bool _useHeaderRow;

        public ImportService() { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); }

        public async Task<List<T>> ReadAsync<T>(string base64File, bool? useHeaderRow = null) where T : class, new()
            => await ReadAsync<T>(base64File, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(string base64File, int sheetIndex, bool? useHeaderRow = null) where T : class, new()
        {
            byte[] buffer = Convert.FromBase64String(base64File);

            var memoryStream = new MemoryStream(buffer, 0, buffer.Length);

            return await ReadAsync<T>(memoryStream, sheetIndex, useHeaderRow);
        }

        public async Task<List<T>> ReadAsync<T>(IFormFile file, bool? useHeaderRow = null) where T : class, new()
            => await ReadAsync<T>(file, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(IFormFile file, int sheetIndex, bool? useHeaderRow = null) where T : class, new()
        {
            var stream = file.OpenReadStream();

            return await ReadAsync<T>(stream, sheetIndex, useHeaderRow);
        }

        public async Task<List<T>> ReadAsync<T>(Stream file, bool? useHeaderRow = null) where T : class, new()
            => await ReadAsync<T>(file, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(Stream file, int sheetIndex, bool? useHeaderRow = null) where T : class, new()
        {
            _useHeaderRow = useHeaderRow ?? true;

            return await GenerateImportAsync<T>(file, sheetIndex);
        }

        async Task<List<T>> GenerateImportAsync<T>(Stream fileStream, int sheetIndex) where T : class, new()
        {
            var dataTable = GenerateDataTable(fileStream, sheetIndex);

            return await GenerateRegistersAsync<T>(dataTable);
        }

        DataTable GenerateDataTable(Stream stream, int sheetIndex)
        {
            IExcelDataReader data = ExcelReaderFactory.CreateReader(stream);

            ExcelDataSetConfiguration conf = GetConfiguration();

            return data.AsDataSet(conf).Tables[sheetIndex];
        }

        ExcelDataSetConfiguration GetConfiguration()
            => new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = _useHeaderRow
                }
            };

        async Task<List<T>> GenerateRegistersAsync<T>(DataTable dataTable) where T : class, new()
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable), $"{nameof(dataTable)} is null");

            List<T> registers = await FillRegistersAsync<T>(dataTable);

            if (registers == null)
                throw new InvalidDataException(nameof(registers), new Exception("Invalid registers"));

            return registers;
        }

        async Task<List<T>> FillRegistersAsync<T>(DataTable dataTable) where T : class, new()
        {
            var registers = new List<T>();

            await Task.Run(() =>
            {
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    var register = new T();

                    for (int column = 0; column < dataTable.Columns.Count; column++)
                    {
                        string property = register.GetType().GetProperties()[column].Name;

                        var dataValue = dataTable.Rows[row][column].ToString();

                        var propertyType = register.GetType().GetProperties()[column].PropertyType;

                        if (!TypeDescriptor.GetConverter(propertyType).IsValid(dataValue))
                            throw new InvalidOperationException($"The conversion of the cell {column.ToLetterRepresentation()}{(_useHeaderRow ? row + 2 : row++)} to the type {propertyType.Name} is impossible or not supported.");

                        var parsedValue = dataValue.Parse(propertyType);

                        register.GetType().GetProperty(property).SetValue(register, parsedValue);
                    }

                    if (string.IsNullOrEmpty(register.GetType().GetProperty(register.GetType().GetProperties()[0].Name).GetValue(register)?.ToString()))
                        continue;

                    registers.Add(register);
                }
            });

            return registers;
        }
    }
}
