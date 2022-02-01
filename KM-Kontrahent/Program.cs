using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using KM_Kontrahent.Managers;
using itDesk.Business.Debuggers;
using itDesk.Business.Debuggers.Enums;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;

namespace KM_Kontrahent
{

    static class Program
    {

        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        static void Main(string[] args)
        {


            if (!Environment.UserInteractive)
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new KMKontrahent(),

            };

                ServiceBase.Run(ServicesToRun);

            }
            else
            {
                KMKontrahent service1 = new KMKontrahent();
                service1.StartAndStop(args);

            }


        }
    }
}
