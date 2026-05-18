using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBox_CS.WebAPI.Models
{
    public class Response
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public ResponseData Data { get; set; }
    }

    public class ResponseData
    {
        [JsonPropertyName("error_data")]
        public List<string> ErrorData { get; set; } = new();

    }
}
    
