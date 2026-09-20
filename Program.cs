using System;
using System.Windows;
using GolfTracker.Forms;

namespace GolfTracker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();
            app.Run(new MainForm());
        }
    }
}
