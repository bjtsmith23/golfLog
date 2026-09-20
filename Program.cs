using System;
using System.Windows.Forms;
using GolfTracker.Forms;

namespace GolfTracker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
