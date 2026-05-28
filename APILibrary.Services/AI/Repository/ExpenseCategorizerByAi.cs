using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary.Services.AI.Models;
using System.Text.Json;
using APILibrary.Services.AI.Models.RequestModels;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.AI.Models.ResponseModels;

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
                AIModel = model,
                MsgsSentToAi = new List<MsgToAiModel>
                {
                    new MsgToAiModel
                    {
                        MsgSenderRole = "system",
                        MsgContentSentToAI = "you are an expense categorizer system ,your task is tp Categorize Expenses ,you must Return only one category name , choose only from these exact categories [ Food/Dining , Travel , Utilities , Entertainment , Business , Shopping , HealthCare ] RULES : RETURN EXCATLY ONE CATEGORY NAME OUT OF THE 7 I GAVE YOU , do not explain anything , no extra words ,no sentences, no punctuation, no reasoning , no category outside list, no explanations , return just one of the categories out of Food/Dining , Travel , Utilities , Entertainment , Business , Shopping , HealthCare , make sure u give no other letter or word except the 7 categories i gave u nothing else not a single letter"
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

            var responseRecievedFromAi = await responseSend.Content.ReadAsStringAsync();

            var deSerializedResponse = JsonSerializer.Deserialize<AiExpenseCategorizationResponse>(responseRecievedFromAi);

            var categoryGivenByAi = deSerializedResponse.Choices[0].Message.Content;
            Console.WriteLine(categoryGivenByAi);
            return categoryGivenByAi;
        }
    }
}
  