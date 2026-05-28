using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APILibrary.Services.AI.Models.ResponseModels
{
    public class AiExpenseCategorizationResponse
    {
        [JsonPropertyName("choices")]
        public List<AiResponseChoice> Choices { get; set; }
    }
}
