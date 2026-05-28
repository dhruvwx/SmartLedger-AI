using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace APILibrary.Services.AI.Models.RequestModels
{
    public class AiExpenseCategorizationRequest
    {
        [JsonPropertyName("model")]
        public string AIModel { get; set; }

        [JsonPropertyName("messages")]
        public List<MsgToAiModel> MsgsSentToAi { get; set; }
    }
}
