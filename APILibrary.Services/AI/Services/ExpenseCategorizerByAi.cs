using APILibrary.Services.AI.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary.Services.AI.Models;
using System.Text.Json;

namespace APILibrary.Services.AI.Services
{
    public class ExpenseCategorizerByAi : IExpenseCategorizerByAi
    {

        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        public ExpenseCategorizerByAi(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.config = config;
        }


        public async Task<string> CategorizeExpenseAsync(string description)
        {
            string apiKey = config["AISettings:ApiKey"];
            string model = config["AISettings:AIModel"];

            string llmApiUrl = "https://api.groq.com/openai/v1/chat/completions";

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var aiExpenseCategorizationRequest = new AiExpenseCategorizationRequest
            {
                AiModel = model,
                MsgsSentToAi = new List<MsgToAiModel>
                {
                    new MsgToAiModel
                    {
                        MsgSenderRole = "system",
                        MsgContentSentToAI = "You Categorize Expenses , Return only one category name , choose only from these exact categories [ Food/Dining , Travel , Utilities , Entertainment , Business , Shopping , HealthCare ] do not explain anything , no extra words , return just one of these categories"
                    },

                    new MsgToAiModel
                    {
                        MsgSenderRole = "user",
                        MsgContentSentToAI = description
                    }
                }

            };

            string jsonMsg = JsonSerializer.Serialize(aiExpenseCategorizationRequest);

            Console.WriteLine(jsonMsg);

            var httpBodyObjectWithJson = new StringContent(jsonMsg, Encoding.UTF8, "application/json");

            var responseSend = await httpClient.PostAsync(llmApiUrl, httpBodyObjectWithJson);
            responseSend.EnsureSuccessStatusCode();

            var resonseRecievedFromAi = await responseSend.Content.ReadAsStringAsync();

            Console.WriteLine(resonseRecievedFromAi);

            return resonseRecievedFromAi;
        }
    }
}
