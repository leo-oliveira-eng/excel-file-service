using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Excel.File.Service.Service.Contracts
{
    public interface IImportService
    {
        Task<List<T>> ReadAsync<T>(string base64File, bool? useHeaderRow) where T : class, new();

        Task<List<T>> ReadAsync<T>(string base64File, int sheetIndex, bool? useHeaderRow) where T : class, new();

        Task<List<T>> ReadAsync<T>(IFormFile file, bool? useHeaderRow) where T : class, new();

        Task<List<T>> ReadAsync<T>(IFormFile file, int sheetIndex, bool? useHeaderRow) where T : class, new();

        Task<List<T>> ReadAsync<T>(Stream file, bool? useHeaderRow) where T : class, new();

        Task<List<T>> ReadAsync<T>(Stream file, int sheetIndex, bool? useHeaderRow) where T : class, new();
    }
}
