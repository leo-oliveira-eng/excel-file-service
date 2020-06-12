using Excel.File.Service.Service.Contracts;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Excel.File.Service.Service
{
    internal class ImportService : IImportService
    {
        public async Task<List<T>> ReadAsync<T>(string base64File, bool? useHeaderRow) where T : class, new()
            => await ReadAsync<T>(base64File, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(string base64File, int sheetIndex, bool? useHeaderRow) where T : class, new()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            byte[] buffer = Convert.FromBase64String(base64File);

            var memoryStream = new MemoryStream(buffer, 0, buffer.Length);

            return await ReadAsync<T>(memoryStream, sheetIndex, useHeaderRow);
        }

        public async Task<List<T>> ReadAsync<T>(IFormFile file, bool? useHeaderRow) where T : class, new()
            => await ReadAsync<T>(file, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(IFormFile file, int sheetIndex, bool? useHeaderRow) where T : class, new()
        {
            var stream = file.OpenReadStream();

            return await ReadAsync<T>(stream, sheetIndex, useHeaderRow);
        }

        public async Task<List<T>> ReadAsync<T>(Stream file, bool? useHeaderRow) where T : class, new()
            => await ReadAsync<T>(file, default, useHeaderRow);

        public async Task<List<T>> ReadAsync<T>(Stream file, int sheetIndex, bool? useHeaderRow) where T : class, new()
            => await GenerateImportAsync<T>(file, sheetIndex, useHeaderRow);

        async Task<List<T>> GenerateImportAsync<T>(Stream fileStream, int sheetIndex, bool? useHeaderRow) where T : class, new()
        {
            var dataTable = GenerateDataTable(fileStream, sheetIndex, useHeaderRow);

            return await GenerateRegistersAsync<T>(dataTable);
        }

        DataTable GenerateDataTable(Stream stream, int sheetIndex, bool? useHeaderRow = true)
        {
            IExcelDataReader data = ExcelReaderFactory.CreateReader(stream);

            ExcelDataSetConfiguration conf = GetConfiguration(useHeaderRow.Value);

            return data.AsDataSet(conf).Tables[sheetIndex];
        }

        ExcelDataSetConfiguration GetConfiguration(bool useHeaderRow)
            => new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = useHeaderRow
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
                        var property = register.GetType().GetProperties()[column].Name;
                        register.GetType().GetProperty(property).SetValue(register, dataTable.Rows[row][column].ToString());
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
