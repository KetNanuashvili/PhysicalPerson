using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interface
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile);
        void ModifyFile(string fileNameWithExtension);
        void DeleteFile(string fileNameWithExtension);
        public Task<byte[]> GetFileAsync(string imageName);
    }
}
