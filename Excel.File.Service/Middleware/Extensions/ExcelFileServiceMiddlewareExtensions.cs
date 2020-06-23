using Excel.File.Service.Service;
using Excel.File.Service.Service.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Excel.File.Service.Middleware.Extensions
{
    public static class ExcelFileServiceMiddlewareExtensions
    {
        public static IServiceCollection AddExcelReader(this IServiceCollection services)
        {
            services.AddTransient<IExportService, ExportService>();
            return services.AddTransient<IImportService, ImportService>();
        }
    }
}
