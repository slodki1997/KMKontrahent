using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using KM_Kontrahent.Managers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Threading;
using KM_Kontrahent.Controllers;
using Microsoft.Owin.Hosting;
using itDesk.Business.Debuggers;
using itDesk.Business.Debuggers.Enums;
using System.Reflection;

namespace KM_Kontrahent
{
    public partial class KMKontrahent : ServiceBase
    {
        public static Logger Logger { get; private set; }
        public static String DirectoryPath { get; private set; }

        public static SyncManager SyncManager { get; private set; }
        internal void StartAndStop(string[] args)
        {
            this.OnStart(args);
            this.OnStop();
        }

        public KMKontrahent()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            this.AutoLog = false;

            if (!System.Diagnostics.EventLog.SourceExists("Kraina Marzeń"))
                System.Diagnostics.EventLog.CreateEventSource("Kraina Marzeń", "KMKontrahenci");

            eventLog1.Source = "Kraina Marzeń";
            eventLog1.Log = "KMKontrahenci";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Rozpoczęto udostępnianie danych.");

            string baseAddress = "http://*:9000/";

            WebApp.Start<Startup>(url: baseAddress);


            DirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Logger = new Logger(DirectoryPath, 7)
            {
                ConsoleLogVisibility = LogType.Error | LogType.Information | LogType.Warning,
                ExceptionExtendedMode = false
            };

            Modules.enova365.ModuleLoader.Load();

            SyncManager = new SyncManager();

            SyncManager.Launch(false);


        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Zakończono udostępnianie danych.");

        }


    }
}
