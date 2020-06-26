using Excel.File.Service.Service.Contracts;
using Excel.File.Service.Service.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel.File.Service.Service
{
    public class ExportService : IExportService
    {
        public async Task<string> ExportToBase64TxtAsync<T>(List<T> registers, string delimiter = ";") where T : class
        {
            if (registers == null)
                throw new ArgumentNullException(nameof(registers), $"{nameof(registers)} is invalid");

            byte[] file = await GenerateFile(registers, delimiter);

            return Convert.ToBase64String(file);
        }

        public async Task<MemoryStream> ExportToMemoryStreamTxtAsync<T>(List<T> registers, string delimiter = ";") where T : class
        {
            if (registers == null)
                throw new ArgumentNullException(nameof(registers), $"{nameof(registers)} is invalid");

            return await GenerateMemoryStream(registers, delimiter);
        }

        async Task<byte[]> GenerateFile<T>(List<T> registers, string delimiter) where T : class
        {
            string path = $"{Path.GetTempPath()}ExportServiceTemporaryFile.txt";

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            FileStream fs = System.IO.File.OpenWrite(path);

            foreach (var register in registers)
                await WriteLine(register, fs, delimiter);

            fs.Close();

            fs = System.IO.File.OpenRead(path);

            var file = fs.ToByteArray();

            fs.Close();

            System.IO.File.Delete(path);

            return (file);
        }

        async Task<MemoryStream> GenerateMemoryStream<T>(List<T> registers, string delimiter) where T : class
        {
            string path = $"{Path.GetTempPath()}ExportServiceTemporaryFile.txt";

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            FileStream fs = System.IO.File.OpenWrite(path);

            foreach (var register in registers)
                await WriteLine(register, fs, delimiter);

            fs.Close();

            fs = System.IO.File.OpenRead(path);

            var memoryStream = new MemoryStream();

            fs.CopyTo(memoryStream);

            fs.Close();

            return memoryStream;
        }

        async Task WriteLine<T>(T register, FileStream fs, string delimiter) where T : class
            => await Task.Run(() =>
            {
                for (int i = 0; i < register.GetType().GetProperties().Count(); i++)
                {
                    if (!i.Equals(0))
                        fs.WriteAsync(new UTF8Encoding(true).GetBytes(delimiter));

                    var property = register.GetType().GetProperties()[i].Name;

                    fs.WriteAsync(new UTF8Encoding(true).GetBytes(register.GetType().GetProperty(property).GetValueOrDefault(register, string.Empty).ToString()));
                }

                fs.WriteAsync(new UTF8Encoding(true).GetBytes("\r\n"));
            }); 
    }
}
