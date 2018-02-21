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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskTester.DesktopTester.ViewModel;
using Xceed.Wpf.Toolkit;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for ProblemView.xaml
    /// </summary>
    public partial class ProblemView : UserControl
    {
        public ProblemView()
        {
            InitializeComponent();
        }

        private void XBrowseChecker_OverrideClicked(object sender, RoutedEventArgs e)
        {
            new CheckerView(XCheckerViewModel.DataContext)
#if DEBUG
            .Show();
#else
            .ShowDialog();
#endif
        }

        private void XLinkOpenBatchEvaluator_Click(object sender, RoutedEventArgs e)
        {
            new BatchTestView().Show();
        }

        private void XSpinner_Spin(object sender, SpinEventArgs e)
        {
            var spinner = sender as ButtonSpinner;
            var textBox = spinner.Content as TextBox;

            double delta = e.Direction == SpinDirection.Increase ? 0.3 : -0.3;

            try
            {
                double oldValue = double.Parse(textBox.GetValue(TextBox.TextProperty).ToString());
                double newValue = (!double.IsNaN(oldValue) ? oldValue : 0) + delta;
                if (newValue < 0) newValue = double.NaN;

                textBox.SetValue(TextBox.TextProperty, newValue.ToString());
            }
            catch { }
        }
    }
}
