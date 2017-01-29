using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace TaskTester.View
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class ProblemView : Window
    {
        public ProblemView()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;

            LabelVersion.Content = version;
        }
    }
}
