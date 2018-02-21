using System;
using System.Diagnostics;
using System.Windows;
using TaskTester.DesktopTester.View;

namespace TaskTester.DesktopTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            new SolutionEvaluationView()
            .Show();
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
