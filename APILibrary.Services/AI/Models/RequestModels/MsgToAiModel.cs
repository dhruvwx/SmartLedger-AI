using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APILibrary.Services.AI.Models.RequestModels
{
    public class MsgToAiModel
    {
        [JsonPropertyName("role")]
        public string MsgSenderRole { get; set; } //system / user

        [JsonPropertyName("content")]
        public string MsgContentSentToAI { get; set; }
    }
}
