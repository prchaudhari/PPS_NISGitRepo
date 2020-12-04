using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;
using IO = System.IO;

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
                SqlString sqlString;
                Feature featureB = new Feature("Update") { Condition = new FeatureCondition(Condition.Installed, 1) };

                string PASSWORD = "[PASSWORD]";
                string USERNAME = "[USERNAME]";
                string INSTANCENAME = "[INSTANCENAME]";
                string DBNAME = "[DBNAME]";
                string tenantSchemaQuery = "CREATE SCHEMA TenantManager ";
                string tenantCreate = "CREATE TABLE TenantManager.Tenant(Id int IDENTITY(1,1) NOT NULL,TenantCode nvarchar(max) NOT NULL,TenantName nvarchar(max) NOT NULL,TenantDescription nvarchar(max) NULL,TenantType nvarchar(max) NULL,TenantImage nvarchar(max) NULL,TenantDomainName nvarchar(max) NOT NULL,FirstName nvarchar(max) NULL,LastName nvarchar(max) NULL,ContactNumber nvarchar(max) NOT NULL,EmailAddress nvarchar(max) NOT NULL,SecondaryContactName nvarchar(max) NULL,SecondaryContactNumber nvarchar(max) NULL,SecondaryEmailAddress nvarchar(max) NULL,AddressLine1 nvarchar(max) NULL,AddressLine2 nvarchar(max) NULL,TenantCity nvarchar(max) NULL,TenantState nvarchar(max) NULL,TenantCountry nvarchar(max) NULL,PinCode nvarchar(max) NULL,StartDate date NULL,EndDate date NULL,StorageAccount nvarchar(max) NOT NULL,AccessToken nvarchar(max) NOT NULL,ApplicationURL nvarchar(max) NULL,ApplicationModules nvarchar(max) NULL,BillingEmailAddress nvarchar(max) NULL,SecondaryLastName nvarchar(max) NULL,BillingFirstName nvarchar(max) NULL,BillingLastName nvarchar(max) NULL,BillingContactNumber nvarchar(max) NULL,PanNumber nvarchar(max) NULL,ServiceTax nvarchar(max) NULL,IsPrimaryTenant bit NULL,ManageType nvarchar(max) NULL,ExternalCode nvarchar(max) NULL,AutheticationMode nvarchar(max) NULL,IsActive bit NOT NULL CONSTRAINT DF__Tenant__IsActive__6C190EBB  DEFAULT ((1)),IsDeleted bit NOT NULL CONSTRAINT DF__Tenant__IsDelete__6D0D32F4  DEFAULT ((0)),ParentTenantCode nvarchar(max) NULL,IsTenantConfigured bit NOT NULL,CONSTRAINT PK__Tenant__3214EC073F2DE561 PRIMARY KEY CLUSTERED (Id ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) )";
                string tenantInsertQuery = "INSERT TenantManager.Tenant VALUES ( N'00000000-0000-0000-0000-000000000000', N'nIS SuperAdmin', N'', N'Instance', N'', N'default.com', N'Super', N'Admin', N'+91-1234567890', N'instancemanager@nis.com', N'', N'', N'', N'Mumbai', N'', N'1', N'1', N'1', N'123456', CAST(N'2015-12-31' AS Date), CAST(N'9999-12-31' AS Date), N'', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'Self', NULL, NULL, 1, 0, NULL, 1)" +
                                    "INSERT TenantManager.Tenant VALUES ( N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', N'Tenant Group 05102020', N'', N'Group', N'', N'', N'pramod', N'shinde', N'+91-9876567834', N'pramod.shinde45123@gmail.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-10-05' AS Date), CAST(N'9999-12-31' AS Date), N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, NULL, 1)" +
                                    "INSERT TenantManager.Tenant VALUES ( N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Tenant UK', N'', N'Tenant', N'', N'domain.com', N'tenant', N'UK', N'+44-7867868767', N'tenantuk@demo.com', N'', N'', N'', N'test tenant', N'', N'London', N'London', N'18', N'545342', CAST(N'2020-10-06' AS Date), CAST(N'9999-12-31' AS Date), N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 1)" +
                                    "INSERT TenantManager.Tenant VALUES ( N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', N'SS_Group', N'Group Created for Testing', N'Group', N'', N'', N'SSGroup', N'manager', N'+91-1254632589', N'ss_group@mailinator.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'', 1)" +
                                    "INSERT TenantManager.Tenant VALUES ( N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'SS_Websym1', N'', N'Tenant', N'', N'websym1.com', N'ss', N'websym', N'+91-2342342321', N'sswebsym@mailinator.com', N'', N'', N'', N'123', N'', N'PN', N'MH', N'36', N'57', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 1)";

                var project =
                new ManagedProject("nISApp1",
                   new Dir(@"%ProgramFiles%\nISApp1",
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
                                   new Files(@"..\APP\ClientApp\dist\*"))),
                      new File(@"readme.txt")
                       ),
                    new ElevatedManagedAction(CustomActions.OnInstall, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                    {
                        UsesProperties = "INSTANCENAME=[INSTANCENAME], DBNAME=[DBNAME], USERNAME=[USERNAME],PASSWORD=[PASSWORD], APPPORT=[APPPORT], APIPORT=[APIPORT] "
                    },
                    new WixSharp.User(new Id("dbUser"), USERNAME) { CreateUser = false, Password = PASSWORD },
                    new Binary(new Id("script"), "script.sql"),
                    sqlDatabase = new SqlDatabase(DBNAME, INSTANCENAME, SqlDbOption.CreateOnInstall
                 , sqlScript = new SqlScript("script", ExecuteSql.OnInstall)
                 , sqlString = new SqlString("CREATE SCHEMA ConfigurationManager", ExecuteSql.OnInstall)
                 , sqlString = new SqlString("CREATE TABLE ConfigurationManager.ConfigurationManager(    Id bigint IDENTITY(1,1) NOT NULL,    PartionKey nvarchar(100) NOT NULL,    RowKey nvarchar(100) NOT NULL,    Value nvarchar(500) NOT NULL,PRIMARY KEY CLUSTERED (    Id ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))", ExecuteSql.OnInstall)
                 , sqlString = new SqlString("SET IDENTITY_INSERT ConfigurationManager.ConfigurationManager ON INSERT ConfigurationManager.ConfigurationManager (Id, PartionKey, RowKey, Value) VALUES (1, N'nIS', N'nISConnectionString', N'Data Source=INSTANCENAME;Initial Catalog=DBNAME;User ID=USERNAME;Password=PASSWORD') INSERT ConfigurationManager.ConfigurationManager (Id, PartionKey, RowKey, Value) VALUES (2, N'EntityManager', N'EntityManagerConnectionString', N'Data Source=INSTANCENAME;Initial Catalog=DBNAME;User ID=USERNAME;Password=PASSWORD') INSERT ConfigurationManager.ConfigurationManager (Id, PartionKey, RowKey, Value) VALUES (3, N'EventManager', N'EventManagerConnectionString', N'Data Source=INSTANCENAME;Initial Catalog=DBNAME;User ID=USERNAME;Password=PASSWORD') INSERT ConfigurationManager.ConfigurationManager (Id, PartionKey, RowKey, Value) VALUES (4, N'SubscriptionManager', N'SubscriptionManagerConnectionString', N'Data Source=INSTANCENAME;Initial Catalog=DBNAME;User ID=USERNAME;Password=PASSWORD') INSERT ConfigurationManager.ConfigurationManager (Id, PartionKey, RowKey, Value) VALUES (5, N'TemplateManager', N'NotificationEngineConnectionString', N'Data Source=INSTANCENAME;Initial Catalog=DBNAME;User ID=USERNAME;Password=PASSWORD') ", ExecuteSql.OnInstall)
                 , sqlString = new SqlString("Update ConfigurationManager.ConfigurationManager set Value='Data Source=" + INSTANCENAME + ";Initial Catalog=" + DBNAME + ";User ID=" + USERNAME + ";Password=" + PASSWORD + "'; ", ExecuteSql.OnInstall)
                 , sqlString = new SqlString(tenantSchemaQuery, ExecuteSql.OnInstall)
                 , sqlString = new SqlString(tenantCreate, ExecuteSql.OnInstall)
                 , sqlString = new SqlString(tenantInsertQuery, ExecuteSql.OnInstall)
                 , sqlString = new SqlString("Update TenantManager.Tenant set StorageAccount='Data Source=" + INSTANCENAME + ";Initial Catalog=" + DBNAME + ";User ID=" + USERNAME + ";Password=" + PASSWORD + "'; ", ExecuteSql.OnInstall)
                ),
                    new ManagedAction(CustomActions.MyAction, Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                      new CloseApplication(new Id("notepad"), "notepad.exe", true, false)
                      {
                          Timeout = 15
                      });
                sqlDatabase.User = "dbUser";

                sqlScript.User = "dbUser";
                sqlString.User = "dbUser";

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
                project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba259855");
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
                e.Session.Log("Project after install method call");
                if (e.Session.IsInstalled())
                {
                    string url = @"http://localhost:" + e.Session.Property("APPPORT") + "/login";
                    MessageBox.Show(url);
                    System.Diagnostics.Process.Start(url);
                }
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

                //string dbScript = session.Property("INSTALLDIR") + "script.sql";
                //UpdateDBScript(dbScript, dbConnectionString);
                //session.Log("UpdateDBScript done");

            });
        }
        [CustomAction]
        public static ActionResult MyAction(Session session)
        {
            System.Diagnostics.Process.Start("Notepad.exe", session["INSTALLDIR"] + @"\readme.txt");
            return ActionResult.Success;
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

            //update readme.text.
            string readMeFilePath = baseURL + @"readme.txt";
            string readMeFile = IO.File.ReadAllText(readMeFilePath);
            readMeFile = readMeFile.Replace("{{APPURL}}", appPort);
            IO.File.WriteAllText(readMeFilePath, readMeFile);
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
