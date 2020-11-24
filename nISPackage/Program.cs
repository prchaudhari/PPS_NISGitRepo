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
                File schedulerService;
                File archivalService;
                SqlDatabase sqlDatabase;
                SqlScript sqlScript;

                var project =
                new ManagedProject("nIS",
                    new Dir(@"%ProgramFiles%\nIS",
                        new Dir("StatementGenerationService",
                        schedulerService = new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe"),
                        new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe.config")),
                        new Dir("ArchivalService",
                        archivalService = new File(@"..\SchedulerWindowService\bin\Debug\SchedulerWindowService.exe"),
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
                sqlDatabase = new SqlDatabase("NIS", ".", SqlDbOption.CreateOnInstall
                // ,sqlScript = new SqlScript("script", ExecuteSql.OnInstall)
                ));
                sqlDatabase.User = "sa";
                //sqlScript.User = "sa";

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
                project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
                //project.UI = WUI.WixUI_ProgressOnly;
                project.OutFileName = "setup";

                project.ManagedUI = new ManagedUI();
                project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                                .Add<WixSharpSetup.SettingsDialog>()
                                                .Add<ProgressDialog>()
                                                .Add<ExitDialog>();
                project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                               .Add<ExitDialog>();

                project.UILoaded += msi_UILoaded;
                project.BeforeInstall += msi_BeforeInstall;
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
            //If required you can
            // - set the size of the shell view window
            // - scale the whole shell window and its content
            // - reposition controls on the current dialog
            // - subscribe to the current dialog changed event

            //e.ManagedUIShell.SetSize(700, 500);
            //e.ManagedUIShell.OnCurrentDialogChanged += ManagedUIShell_OnCurrentDialogChanged;
            //(e.ManagedUIShell.CurrentDialog as Form).Controls....
        }

        //private static void ManagedUIShell_OnCurrentDialogChanged(IManagedDialog dialog)
        //{
        //}

        static void Project_AfterInstall(SetupEventArgs e)
        {
            ////Debug.Assert(false);
            //MessageBox.Show(e.Data["test"], "Project_AfterInstall");
            //if (e.IsInstalling)
            //{
            //    System.IO.Directory.CreateDirectory(@"C:\Program Files\ttt");
            //    MessageBox.Show($"User '{Defaults.UserName}' with password '{e.Session.Property("PASSWORD")}' has been created");
            //}
        }

        static void msi_BeforeInstall(SetupEventArgs e)
        {

        }
    }
}
