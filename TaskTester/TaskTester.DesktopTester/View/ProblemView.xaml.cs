using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class ProblemView : Window
    {
        public ProblemView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectedCellsChanged(object s, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
           
        }

        private void xHyperlinkViewDetail_Click(object s, RoutedEventArgs e)
        {
            var sender = s as Hyperlink;
            var view = new TestResultView()
            {
                DataContext = sender.DataContext
            };
            view.ShowDialog();
        }

        private void xButtonEditChecker_Click(object s, RoutedEventArgs e)
        {
            var sender = s as Button;
            var view = new CheckerView()
            {
                DataContext = sender.DataContext
            };
            view.ShowDialog();
        }
    }
}
