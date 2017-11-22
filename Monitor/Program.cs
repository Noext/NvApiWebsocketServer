using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Monitor
{
    
    class Program
    {
        static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = true;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }

        static void Main(string[] args)
        {


            AppDomain.CurrentDomain.AssemblyResolve += (Object sender, ResolveEventArgs eventArgs) =>
            {
                String thisExe = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                System.Reflection.AssemblyName embeddedAssembly = new System.Reflection.AssemblyName(eventArgs.Name);
                String resourceName = thisExe + "." + embeddedAssembly.Name + ".dll";

                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return System.Reflection.Assembly.Load(assemblyData);
                }
            };


            notifyIcon.DoubleClick += (s, e) =>
            {
                Visible = !Visible;
                SetConsoleWindowVisibility(Visible);
            };
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });
            notifyIcon.ContextMenuStrip = contextMenu;

            Console.WriteLine("Running!");

            var th = new ServerThread();

            Thread ServerThread = new Thread(th.Run);
            ServerThread.IsBackground = true;
            ServerThread.Start();

            Application.Run();
            notifyIcon.Visible = false;

        }

       


    }
}
