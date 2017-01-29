using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using TaskTester.DesktopTester.Model;
using TaskTester.DesktopTester.View;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            new ProblemView() { DataContext = new ProblemViewModel(new Problem()) }.Show();
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
