using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Services.Common
{
    public static class FormFileFactory
    {
        public static IFormFile CreateFormFile(string name = "test.txt", string content = "hello")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "file", name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
        }
    }
}
