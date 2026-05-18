using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DBox_CS.WebAPI.Models
{
    public class EmployeeRequest
    {
        [JsonPropertyName("employee_id")]
        public string EmployeeId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("date_of_joining")]
        public string DateOfJoining { get; set; }

        [JsonPropertyName("document_type")]
        public string DocumentType { get; set; }

        [JsonPropertyName("document_content")]
        public string DocumentContent { get; set; }
    }
}
