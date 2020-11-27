using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;
using IO = System.IO;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace nISPackage
{
    class Program
    {
        static void Main()
        {
            try
            {
                File schedulerService;
                File archivalService;
                SqlDatabase sqlDatabase;
                SqlScript sqlScript;
                string PASSWORD = "[PASSWORD]";
                string USERNAME = "[USERNAME]";
                string INSTANCENAME = "[INSTANCENAME]";
                string DBNAME = "[DBNAME]";
                var project =
                new ManagedProject("nISApp",
                   new Dir(@"%ProgramFiles%\nISApp",
                       new Dir("SchedulerWindowService",
                       schedulerService = new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe"),
                       new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe.config")),
                       new Dir("ArchivalProcessWindowsService",
                       archivalService = new File(@"..\ArchivalProcessWindowsService\bin\Debug\ArchivalProcessWindowsService.exe"),
                       new File(@"..\ArchivalProcessWindowsService\bin\Debug\ArchivalProcessWindowsService.exe.config")),
                       new Dir("Application",
                           new Dir("API",
                                   new IISVirtualDir
                                   {
                                       Name = "nis-api",
                                       AppName = "nis-api",
                                       WebSite = new WebSite("nis-api", "*:[APIPORT]") { InstallWebSite = true },
                                       WebAppPool = new WebAppPool("nis-api", "Identity=applicationPoolIdentity")
                                   },
                                   new Files(@"..\API\bin\app.publish\*")),
                            new Dir("APP",
                                   new IISVirtualDir
                                   {
                                       Name = "nis-app",
                                       AppName = "nis-app",
                                       WebSite = new WebSite("nis-app", "*:[APPPORT]") { InstallWebSite = true },
                                       WebAppPool = new WebAppPool("nis-app", "Identity=applicationPoolIdentity")
                                   },
                                   new Files(@"..\APP\ClientApp\dist\*"))
                               ),
                       new File("script.sql")
                       ),

                    new ElevatedManagedAction(CustomActions.OnInstall, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                    {
                        UsesProperties = "INSTANCENAME=[INSTANCENAME], DBNAME=[DBNAME], USERNAME=[USERNAME],PASSWORD=[PASSWORD], APPPORT=[APPPORT], APIPORT=[APIPORT] "
                    },
                new WixSharp.User(new Id("dbUser"), USERNAME) { CreateUser = false, Password = PASSWORD },
                // new Binary(new Id("script"), "script.sql"),
                sqlDatabase = new SqlDatabase(DBNAME, INSTANCENAME, SqlDbOption.CreateOnInstall
                // , sqlScript = new SqlScript("script", ExecuteSql.OnInstall)
                )
                );
                sqlDatabase.User = "dbUser";
                //sqlScript.User = USERNAME;

                schedulerService.ServiceInstaller = new ServiceInstaller
                {
                    Name = "Statment Generation Service",
                    StartOn = SvcEvent.Install, //set it to null if you don't want service to start as during deployment
                    StopOn = SvcEvent.InstallUninstall_Wait,
                    RemoveOn = SvcEvent.Uninstall_Wait,
                    DelayedAutoStart = true,
                    ServiceSid = ServiceSid.none,
                    FirstFailureActionType = FailureActionType.restart,
                    SecondFailureActionType = FailureActionType.restart,
                    ThirdFailureActionType = FailureActionType.runCommand,
                    ProgramCommandLine = "SchedulerWindowService -run",
                    RestartServiceDelayInSeconds = 30,
                    ResetPeriodInDays = 1,
                    PreShutdownDelay = 1000 * 60 * 3,
                    RebootMessage = "Failure actions do not specify reboot",
                    DependsOn = new[]
                    {
                    new ServiceDependency("Dnscache"),
                    new ServiceDependency("Dhcp"),
                    },
                };
                archivalService.ServiceInstaller = new ServiceInstaller
                {
                    Name = "Archival Process Service",
                    StartOn = SvcEvent.Install, //set it to null if you don't want service to start as during deployment
                    StopOn = SvcEvent.InstallUninstall_Wait,
                    RemoveOn = SvcEvent.Uninstall_Wait,
                    DelayedAutoStart = true,
                    ServiceSid = ServiceSid.none,
                    FirstFailureActionType = FailureActionType.restart,
                    SecondFailureActionType = FailureActionType.restart,
                    ThirdFailureActionType = FailureActionType.runCommand,
                    ProgramCommandLine = "ArchivalProcessService -run",
                    RestartServiceDelayInSeconds = 30,
                    ResetPeriodInDays = 1,
                    PreShutdownDelay = 1000 * 60 * 3,
                    RebootMessage = "Failure actions do not specify reboot",
                    DependsOn = new[]
                   {
                    new ServiceDependency("Dnscache"),
                    new ServiceDependency("Dhcp"),
                    },
                };
                project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25989c");
                project.OutFileName = "setup";

                project.ManagedUI = new ManagedUI();
                project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                                .Add<LicenceDialog>()
                                                .Add<WixSharpSetup.SettingsDialog>()
                                                .Add<ProgressDialog>()
                                                .Add<ExitDialog>();
                project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                               .Add<ExitDialog>();
                project.UILoaded += msi_UILoaded;
                project.AfterInstall += Project_AfterInstall;

                project.PreserveTempFiles = true;

                project.BuildMsi();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void msi_UILoaded(SetupEventArgs e)
        {
            e.Session.Log("Project before install method call");
        }


        static void Project_AfterInstall(SetupEventArgs e)
        {
            try
            {
                //e.Session.Log("Project after install method call");

                //var INSTANCENAME = e.Session.Property("INSTANCENAME");
                //var DBNAME = e.Session.Property("DBNAME");
                //var USERNAME = e.Session.Property("USERNAME");
                //var PASSWORD = e.Session.Property("PASSWORD");
                //var APPPORT = e.Session.Property("APPPORT");
                //var APIPORT = e.Session.Property("APIPORT");

                //e.Session.Log("Session Data = " + e.Data.ToString());
                //e.Session.Log(INSTANCENAME);
                //e.Session.Log(DBNAME);
                //e.Session.Log(USERNAME);
                //e.Session.Log(PASSWORD);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult OnInstall(Session session)
        {
            session.Log("------------- " + session.Property("INSTALLDIR"));
            session.Log("------------- " + session.Property("CONFIG_FILE"));
            session.Log("------------- " + session.Property("APP_FILE"));

            string PASSWORD = session.Property("PASSWORD");
            string USERNAME = session.Property("USERNAME");
            string INSTANCENAME = session.Property("INSTANCENAME");
            string DBNAME = session.Property("DBNAME");
            string APPPORT = session.Property("APPPORT");
            string APIPORT = session.Property("APIPORT");
            //MessageBox.Show("PASSWORD = " + PASSWORD + "USERNAME = " + USERNAME + " \n INSTANCENAME = " + INSTANCENAME
            //    + "\n DBNAME = " + DBNAME + "\n APPPORT = " + APPPORT + " APIPORT = " + APIPORT);
            string dbConnectionString = "Data Source={{InstanceName}};Initial Catalog={{DataBaseName}};User ID={{UserName}};Password={{Password}}";
            dbConnectionString = dbConnectionString.Replace("{{InstanceName}}", INSTANCENAME);
            dbConnectionString = dbConnectionString.Replace("{{DataBaseName}}", DBNAME);
            dbConnectionString = dbConnectionString.Replace("{{UserName}}", USERNAME);
            dbConnectionString = dbConnectionString.Replace("{{Password}}", PASSWORD);

            return session.HandleErrors(() =>
            {
                string configFile = session.Property("INSTALLDIR") + @"Application\API\Web.config";
                UpdateAsAppConfig(session.Property("INSTALLDIR"), configFile, dbConnectionString, APPPORT, APIPORT);
                session.Log("UpdateAsAppConfig done");

                string envJsonFile = session.Property("INSTALLDIR") + @"Application\APP\assets\env-specific.json";
                UpdateAppEnvJSON(envJsonFile, APIPORT);
                session.Log("UpdateAppEnvJSON done");

                string dbScript = session.Property("INSTALLDIR") + "script.sql";
                UpdateDBScript(dbScript, dbConnectionString);
                session.Log("UpdateDBScript done");

            });
        }
        static public void UpdateAsAppConfig(string baseURL, string configFile, string connectionString, string appPort, string apiport)
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);
            var section = config.ConnectionStrings;
            section.ConnectionStrings["TenantManagerConnectionString"].ConnectionString = connectionString;
            string changePWDLink = config.AppSettings.Settings["ChangePasswordLink"].Value;
            changePWDLink = changePWDLink.Replace("{{APPPORTNO}}", appPort);
            config.AppSettings.Settings["ChangePasswordLink"].Value = changePWDLink;
            config.Save();
            //Update API base url in Archival process windows service
            string AppConfig = baseURL + @"ArchivalProcessWindowsService\ArchivalProcessWindowsService.exe.config";
            string script = IO.File.ReadAllText(AppConfig);
            script = script.Replace("{{APIPORTNO}}", apiport);
            IO.File.WriteAllText(AppConfig, script); 

            //Update API base url in Scheduler windows service
            AppConfig = baseURL + @"SchedulerWindowService\SchedulerWindowService.exe.config";
            script = IO.File.ReadAllText(AppConfig);
            script = script.Replace("{{APIPORTNO}}", apiport);
            IO.File.WriteAllText(AppConfig, script);

            //Update API base url in script.js API->Resources->js->script.js
            AppConfig = baseURL + @"Application\API\Resources\js\script.js";
            script = IO.File.ReadAllText(AppConfig);
            script = script.Replace("{{APIPORTNO}}", apiport);
            IO.File.WriteAllText(AppConfig, script);
        }

        static public void UpdateAppEnvJSON(string configFile, string apiPort)
        {
            string configuration = IO.File.ReadAllText(configFile);
            configuration = configuration.Replace("{{APIPORTNO}}", apiPort);
            IO.File.WriteAllText(configFile, configuration);
        }
        static public void UpdateDBScript(string configFile, string connectionString)
        {
            // session.Log("In UpdateDBScript ");

            string script = IO.File.ReadAllText(configFile);
            script = script.Replace("{{DataBaseConnectionString}}", connectionString);
            IO.File.WriteAllText(configFile, script);

            //Microsoft.Data.SqlClient.SqlConnection conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);

            //Server server = new Server(new ServerConnection(conn));

            //server.ConnectionContext.ExecuteNonQuery(script);
        }

    }
}
