using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace SchedulerWindowService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        static string ApiBaseAddress;
        static string TenantCode;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);

            ApiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
            TenantCode = ConfigurationManager.AppSettings["TenantCode"];

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000 * 60 * 30; //number in milisecinds  -- 30 minutes
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall start at " + DateTime.Now);
            RunStatementGenerationSchedule();
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
        public static void RunStatementGenerationSchedule()
        {
            WriteToFile("RunStatementGenerationSchedule is called " + DateTime.Now);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("TenantCode", TenantCode);
            WriteToFile("App Base Address: " + ApiBaseAddress + " TenantCode: " + TenantCode);

            try
            {
                var response = client.PostAsync("Schedule/RunSchedule", new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json")).Result;
                WriteToFile("Response Content from RunSchedule API: " + response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                WriteToFile("Exception at RunSchedule: " + ex);
                throw ex;
            }
            WriteToFile("RunStatementGenerationSchedule is completed " + DateTime.Now);
        }
    }
}
