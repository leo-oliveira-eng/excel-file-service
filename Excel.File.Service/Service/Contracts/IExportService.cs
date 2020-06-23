using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Excel.File.Service.Service.Contracts
{
    public interface IExportService
    {
        Task<string> ExportToBase64TxtAsync<T>(List<T> registers, string delimiter = ";") where T : class;

        Task<MemoryStream> ExportToMemoryStreamTxtAsync<T>(List<T> registers, string delimiter = ";") where T : class;
    }
}
