﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Excel.File.Service.Service.Contracts
{
    public interface IImportService
    {
        Task<List<T>> ReadAsync<T>(string base64File) where T : class, new();

        Task<List<T>> ReadAsync<T>(IFormFile file) where T : class, new();

        Task<List<T>> ReadAsync<T>(Stream file) where T : class, new();
    }
}
