using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeDesk.Models
{
   public class EmployeeResponse
    {
        
            public int code { get; set; }
            public Meta meta { get; set; }
            [JsonPropertyName("data")]
            public List<Employee> data { get; set; }
        }
        
        public class EmployeeData
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string gender { get; set; }
            public string status { get; set; }
        }

        public class Meta
        {
            public Pagination pagination { get; set; }
        }

        public class Pagination
        {
            public int total { get; set; }
            public int pages { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
        }
        public class CreateEmployeeResponse
        {
            public int code { get; set; }
            public object meta { get; set; }
            public EmployeeData data { get; set; }
        }

    }

