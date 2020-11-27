using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ArchivalProcessWindowsService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer _timer;
        DateTime _scheduleTime;
        static string ApiBaseAddress;
        static string TenantCode;

        public Service1()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer();
            _scheduleTime = DateTime.Today.AddDays(1).AddHours(7); // Schedule to run once a day at 7:00 a.m.
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);

            ApiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
            TenantCode = ConfigurationManager.AppSettings["TenantCode"];

            // For first time, set amount of seconds between current time and schedule time
            _timer.Enabled = true;
            _timer.Interval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void Timer_Elapsed(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
            RunArchivalProcessSchedule();
            if (_timer.Interval != 24 * 60 * 60 * 1000)
            {
                _timer.Interval = 24 * 60 * 60 * 1000;
            }
        }
        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ArchiveLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
        public static void RunArchivalProcessSchedule()
        {
            WriteToFile("RunArchivalProcessSchedule is called" + DateTime.Now);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("TenantCode", TenantCode);
            WriteToFile("App Base Address: " + ApiBaseAddress + " TenantCode: " + TenantCode);
            try
            {
                var response = client.PostAsync("ArchivalProcess/RunArchivalProcess", new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json")).Result;
                WriteToFile("Response Content from Run Archival Process API: " + response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                WriteToFile("Exception at Run Archival Process Schedule: " + ex);
                throw ex;
            }
        }
    }
}
