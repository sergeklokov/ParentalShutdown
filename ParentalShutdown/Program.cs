using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;

namespace ParentalShutdown
{
    static class Program
    {
        const int OneHour = 60; // one hour by default

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int periodInMinutes = OneHour; 

            if (args != null && args.Length > 0) {
                var isSuccess = Int32.TryParse(args[0], out periodInMinutes);

                if (!isSuccess)
                {
                    EventLog.WriteEntry("Application", $"Parental Shutdown. Can't convert parameter '{args[0]}' to integer minutes.", EventLogEntryType.Error);
                    periodInMinutes = OneHour;
                }

            }

            Thread.Sleep(periodInMinutes * 60 * 1000);

            EventLog.WriteEntry("Application", $"Parental Shutdown. Shutting down after {periodInMinutes} minutes.", EventLogEntryType.Information);
            Shutdown();

        }


        // https://stackoverflow.com/questions/102567/how-to-shut-down-the-computer-from-c-sharp
        // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32shutdown-method-in-class-win32-operatingsystem
        static void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams = mcWin32.GetMethodParameters("Win32Shutdown");


            //mboShutdownParams["Flags"] = "1"; // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "5"; // Forced Shutdown (1 + 4) 
            mboShutdownParams["Reserved"] = "0";

            foreach (ManagementObject manObj in mcWin32.GetInstances())
                mboShutdown = manObj.InvokeMethod("Win32Shutdown", mboShutdownParams, null);
        }
    }
}
