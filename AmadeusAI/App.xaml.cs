// File: App.xaml.cs
// Created: 20.02.2018
// 
// See <summary> tags for more information.

using System.Runtime.InteropServices;
using System.Windows;


namespace AmadeusAI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("SHCore.dll", SetLastError = false)]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        protected override void OnStartup(StartupEventArgs e)
        {
       //     if (AmadeusAISettings.Default.DpiWorkaround)
      //      {
                // Magic, possibly
           //     App.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
         //   }

            base.OnStartup(e);
            var trayIcon = new Trayicon(); //creation of the TrayIcon instance upon StartUp
        }

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }
    }
}
