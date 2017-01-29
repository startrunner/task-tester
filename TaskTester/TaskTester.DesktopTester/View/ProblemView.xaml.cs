using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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
            DataGrid sender = s as DataGrid;
            if (sender.SelectedCells.Count != 0)
            {
                sender.UnselectAll();
            }
        }
    }
}
