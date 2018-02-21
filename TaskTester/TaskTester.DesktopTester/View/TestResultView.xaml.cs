using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for TestResultView.xaml
    /// </summary>
    public partial class TestResultView : Window
    {
        public TestResultView(object viewModel)
        {
            InitializeComponent();
            this.KeyUp += TestResultView_KeyUp;
            this.DataContext = viewModel ?? DataContext;
        }

        void TestResultView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
