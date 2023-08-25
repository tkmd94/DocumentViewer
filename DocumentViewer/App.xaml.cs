using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DocumentViewer
{

    public partial class App : Application
    {
        private static readonly string mutexName = "DocumentViewer";
        private static readonly Mutex mutex = new Mutex(false, mutexName);
        private static bool hasHandle = false;


        protected override void OnStartup(StartupEventArgs e)
        {
            hasHandle = mutex.WaitOne(0, false);
            if (!hasHandle)
            {
                ; MessageBox.Show(DocumentViewer.Properties.Resources.MutexMesseage);

                this.Shutdown();
                return;
            }

            base.OnStartup(e);

        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (hasHandle)
            {
                mutex.ReleaseMutex();
            }

            mutex.Close();
        }


        private async void ApplicationStartup(object sender, StartupEventArgs e)
        {
            string patid = "";
            //patid = "99990003";
            if (e.Args.Length == 1)
            {
                patid = e.Args[0];
            }

            var windows = new MainWindow(patid);
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string scriptVersion = fvi.FileVersion;
            string Title = string.Format("{0} v{1}", assembly.GetName().Name, assembly.GetName().Version);
            windows.Title = Title;

            var checkWebView2 = CheckWebView2Runtime();
            if (!checkWebView2)
            {
                windows.documentDataGrid.Width = 1200;
            }


            windows.Show();

            if (windows.PatientIdTextBox.Text != "")
            {
                windows.ProcessRingExecute();
                bool result = await Task.Run(() => windows.LoadList());
                windows.ProcessRingExecute();
            }
        }
        private bool CheckWebView2Runtime()
        {
            var version = "";
            var error = "";
            try
            {
                version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (WebView2RuntimeNotFoundException e)
            {
                error = e.Message;
            }
            if (version == "")
                return false;
            else
                return true;
        }
    }
}