namespace ArchivalProcessService
{
    #region References

    using System;
    using System.Configuration;
    using Microsoft.Azure.WebJobs;

    #endregion

    /// <summary>
    /// This class represent web job program for archival process to generate PDF statements from HTML statements
    /// </summary>
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();
            config.UseTimers();
            config.DashboardConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            config.StorageConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            config.Queues.MaxDequeueCount = 2;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            config.Queues.BatchSize = 5;
            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
