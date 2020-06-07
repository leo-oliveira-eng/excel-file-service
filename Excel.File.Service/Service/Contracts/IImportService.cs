using System.Collections.Generic;
using System.Threading.Tasks;

namespace Excel.File.Service.Service.Contracts
{
    public interface IImportService
    {
        Task<List<T>> ReadAsync<T>(string base64File) where T : class, new();
    }
}
