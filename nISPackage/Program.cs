
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;

namespace nISPackage
{
    class Program
    {
        static void Main()
        {
            try
            {
                File service;
                SqlDatabase sqlDatabase;
                SqlScript sqlScript;

                var project =
                new Project("nIS",
                    new Dir(@"%ProgramFiles%\nIS",
                        new Dir("Service",
                        service = new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe"),
                        new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe.config")),
                        new Dir("Application",
                            new Dir("API",
                                    new IISVirtualDir
                                    {
                                        Name = "nis-api",
                                        AppName = "nis-api",
                                        WebSite = new WebSite("nis-api", "*:8082") { InstallWebSite = true },
                                        WebAppPool = new WebAppPool("nis-api", "Identity=applicationPoolIdentity")
                                    },
                                    new Files(@"..\API\bin\app.publish\*")),
                             new Dir("APP",
                                    new IISVirtualDir
                                    {
                                        Name = "nis-app",
                                        AppName = "nis-app",
                                        WebSite = new WebSite("nis-app", "*:8083") { InstallWebSite = true },
                                        WebAppPool = new WebAppPool("nis-app", "Identity=applicationPoolIdentity")
                                    },
                                    new Files(@"..\APP\ClientApp\dist\*"))
                                )
                        ),
                    new User(new Id("sa"), "sa") { CreateUser = false, Password = "Admin@123" },
                    new Binary(new Id("script"), "script.sql"),
                        sqlDatabase = new SqlDatabase("NIS", ".", SqlDbOption.CreateOnInstall,
                            sqlScript = new SqlScript("script", ExecuteSql.OnInstall)));
                //CustomActionRef customActionRef = new CustomActionRef("DBName",When.Before,Step.WriteEnvironmentStrings);
                //customActionRef.

                sqlDatabase.User = "sa";
                sqlScript.User = "sa";

                service.ServiceInstaller = new ServiceInstaller
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

                project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
                //project.UI = WUI.WixUI_ProgressOnly;
                project.OutFileName = "setup";
               
                project.PreserveTempFiles = true;
                project.BuildMsi();
              
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
