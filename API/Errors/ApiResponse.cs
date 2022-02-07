using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null )
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch 
            {
                400 => "(Bad) Hatalı bir istek...",
                401 => "Yetkisiz işlem",
                404 => "Kaynağa erişilemedi",
                500 => "Sunucu hatası oluştu...",
                _ => null
            };
        }

    }
}