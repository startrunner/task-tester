using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class SolutionEvaluationView : Window
    {
        public SolutionEvaluationView()
        {
            InitializeComponent();
        }


        private void DataGrid_SelectedCellsChanged(object s, System.Windows.Controls.SelectedCellsChangedEventArgs e) { }

        private void HyperlinkViewDetail_Click(object s, RoutedEventArgs e)
        {
            var sender = s as Hyperlink;
            var view = new TestResultView(sender.DataContext);
            view.ShowDialog();
        }

        private void XButtonTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void XLinkOpenBatchEvaluator_Click(object sender, RoutedEventArgs e)
        {
            new BatchTestView() { DataContext = new BatchTestViewModel() }.Show();
        }
    }
}
