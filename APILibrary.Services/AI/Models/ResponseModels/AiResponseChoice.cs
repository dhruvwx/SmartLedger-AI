using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APILibrary.Services.AI.Models.ResponseModels
{
    public class AiResponseChoice
    {
        [JsonPropertyName("message")]
        public AiResponseMessage Message { get; set; }
    }
}
