

namespace StatementGeneratorService
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;

    #endregion

    public class Functions
    {
        static string ApiBaseAddress;
        static string TenantCode;

        // This function will get triggered/executed immediately on startup, and then every 30 minutes thereafter.
        public static void ProcessQueueBasedOnTimer([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo timer)
        {
            Console.WriteLine("This should run every 30 minutes: " + DateTime.UtcNow);
            ApiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
            TenantCode = ConfigurationManager.AppSettings["TenantCode"];
            Console.WriteLine("App Base Address: "+ ApiBaseAddress + " TenantCode: "+ TenantCode);
            RunStatementGenerationSchedule();
            Console.WriteLine("Execution done..!! ");
        }

        public static void RunStatementGenerationSchedule()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("TenantCode", TenantCode);

            try
            {
                var response = client.PostAsync("Schedule/RunSchedule", new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json")).Result;
                Console.WriteLine("Response Contenat from RunSchedule: " + response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RunSchedule: " + ex);
                throw ex;
            }
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
