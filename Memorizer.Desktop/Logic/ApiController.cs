using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using Memorizer.Model;
using Newtonsoft.Json;

namespace Memorizer.Logic
{
    public enum MemoAnswerType
    {
        Bad = 1,
        Tomorrow,
        Later,
        Cool
    }

    public class ApiController
    {
        private string BaseUrl { get; }

        public ApiController()
        {
#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            BaseUrl = "https://localhost:3000/api";
#else
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            BaseUrl = ConfigurationManager.AppSettings["ApiBaseDomain"];
#endif
        }

        public async Task<IEnumerable<Memo>> GetMemos(bool isRepeat)
        {
            var result = await GetAsync($"{BaseUrl}/GetMemos");
            var rawMemos = JsonConvert.DeserializeObject<IEnumerable<Memo>>(result);

            var memos = rawMemos
                .Where(memo => memo.RepeatDate < DateTime.Now)
                .Where(memo => isRepeat ? memo.PostponeLevel != 0 : memo.PostponeLevel == 0)
                .OrderBy(memo => memo.RepeatDate)
                .ThenByDescending(memo => memo.PostponeLevel);

            return memos;
        }

        //TODO find by key
        public async Task<IEnumerable<Memo>> Find(string searchWord = "")
        {
            var result = await GetAsync($"{BaseUrl}/FindMemo/ /{searchWord}");
            var memos = JsonConvert.DeserializeObject<IEnumerable<Memo>>(result);

            return memos;
        }

        public async Task SubmitAnswer(int id, MemoAnswerType answerType)
        {
            var data = new { id, answerType };
            await PostAsync($"{BaseUrl}/SubmitAnswer", data);
        }

        public async Task Add(Memo memo)
        {
            await PostAsync($"{BaseUrl}/AddMemo", memo);
        }

        public async Task Update(Memo memo)
        {
            await PostAsync($"{BaseUrl}/UpdateMemo", memo);
        }

        public async Task Delete(int id)
        {
            await PostAsync($"{BaseUrl}/DeleteMemo", id);
        }

        private static async Task<string> GetAsync(string url)
        {
            using (var apiClient = new HttpClient())
            {
                AddAuthHeader(apiClient);

                var response = await apiClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(result);
                }

                return result;
            }
        }

        private static async Task PostAsync(string url, object data)
        {
            using (var apiClient = new HttpClient())
            {
                AddAuthHeader(apiClient);

                var response = await apiClient.PostAsJsonAsync(url, data);
                var message = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(message);
                }
            }
        }

        private static void AddAuthHeader(HttpClient apiClient)
        {
            var username = ConfigurationManager.AppSettings["ApiUsername"];
            var password = ConfigurationManager.AppSettings["ApiPassword"];
            var encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"));

            apiClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
        }
    }
}
