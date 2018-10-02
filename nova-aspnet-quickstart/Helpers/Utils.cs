using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nova_aspnet_quickstart.Helpers {
    public static class Utils {
        public static IConfiguration Configuration { get; set; }
        public static Dictionary<String, String> receivedReportData;

        static int BORROW_LOAN_DECISION_THRESHOLD = 650;

        static void ParseNovaPassport(String userArgs, String publicToken, JObject creditPassport) {
            // See https://docs.neednova.com/ for a full explanation of the Nova Credit Passport
            JToken[] scores = creditPassport["scores"].ToArray();
            JToken personal = creditPassport["personal"];

            /*
             * Now that we have this data, you can easily add Nova to your existing underwriting engine.
             * In this example, our underwriting decision is: accept applicants whose NOVA_SCORE_BETA is greater than BORROW_LOAN_DECISION_THRESHOLD
             */
            JToken novaScoreObj = Array.Find(scores, s => s["score_type"].ToString() == "NOVA_SCORE_BETA");

            /*
             * Make our decision:
             */
            String borrowLoanDecision = novaScoreObj != null && (int)novaScoreObj["value"] > BORROW_LOAN_DECISION_THRESHOLD ? "APPROVE" : "DENY";

            /*
             * Finally, store applicant report data - refresh the page at localhost:5000/dashboard to see the results
             *
             * For demo purposes, we'll store the results of a Nova Credit Passport in a cache store
             * Note that production usage should store received data in a database, associated to its respective applicant
             */
            receivedReportData = new Dictionary<string, string> {
                { "userArgs", userArgs },
                { "publicToken" , publicToken },
                { "applicantName" , personal["full_name"].ToString() },
                { "applicantEmail" , personal["email"].ToString() },
                { "novaScore" , novaScoreObj != null ? novaScoreObj["value"].ToString() : "0" },
                { "borrowLoanDecision" , borrowLoanDecision }
            };
        }

        public static async Task HandleNovaWebhook(String publicToken, String userArgs) {
            String novaEnv = Configuration["NOVA_ENV"];
            String novaAccessTokenUrl = Configuration["NOVA_ACCESS_TOKEN_URL"];
            String novaPassportUrl = Configuration["NOVA_PASSPORT_URL"];
            String novaClientId = Configuration["NOVA_CLIENT_ID"];
            String novaSecretKey = Configuration["NOVA_SECRET_KEY"];
            String novaBasicAuthCreds = Base64Encode($"{novaClientId}:{novaSecretKey}");

            /*
             * Get an access token from Nova
             */
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {novaBasicAuthCreds}");
            client.DefaultRequestHeaders.Add("X-ENVIRONMENT", novaEnv);

            HttpResponseMessage response = await client.GetAsync(novaAccessTokenUrl);
            response.EnsureSuccessStatusCode();

            String responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResult = JsonConvert.DeserializeObject<JObject>(responseBody);

            /*
             * Now make a request to Nova to fetch the Credit Passport for the public token provided in the webhook (i.e., unique identifier for the credit file request in Nova's system)
             */
            String accessToken = jsonResult["accessToken"].ToString();
            HttpClient passportClient = new HttpClient();
            passportClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Base64Encode(accessToken)}");
            passportClient.DefaultRequestHeaders.Add("X-PUBLIC-TOKEN", publicToken);
            passportClient.DefaultRequestHeaders.Add("X-ENVIRONMENT", novaEnv);

            HttpResponseMessage passportResponse = await passportClient.GetAsync(novaPassportUrl);
            passportResponse.EnsureSuccessStatusCode();

            String passportResponseBody = await passportResponse.Content.ReadAsStringAsync();
            JObject passportJsonResult = JsonConvert.DeserializeObject<JObject>(passportResponseBody);

            ParseNovaPassport(userArgs, publicToken, passportJsonResult);
        }

        static string Base64Encode(string plainText) {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
