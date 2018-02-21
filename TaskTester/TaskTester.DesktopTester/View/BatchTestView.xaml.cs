using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for BatchTestSetupView.xaml
    /// </summary>
    public partial class BatchTestView : Window
    {
        public BatchTestView()
        {
            InitializeComponent();
            this.DataContextChanged += HandleDataContextChanged;
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is BatchTestViewModel newViewModel)
            {
                newViewModel.Progress.Starting += (x, args) => UpdateProblemColumns();
            }
            if (e.OldValue is BatchTestViewModel oldViewModel)
            {
                oldViewModel.Progress.Starting += (x, args) => UpdateProblemColumns();
            }
        }

        private void HandleProblemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateProblemColumns();
        }

        private BatchTestViewModel ViewModel => DataContext as BatchTestViewModel;

        private void UpdateProblemColumns()
        {
            while (!ReferenceEquals(XDataGridResults.Columns.Last(), XColumnBeforeProblems))
            {
                XDataGridResults.Columns.RemoveAt(XDataGridResults.Columns.Count - 1);
            }
            if (ViewModel == null) return;

            foreach (BatchTestProblemViewModel problem in ViewModel.Problems)
            {
                var detailsFactory = new FrameworkElementFactory(typeof(BatchTestProblemResultView));
                detailsFactory.SetValue(BatchTestProblemResultView.ProblemIdentifierProperty, problem.Identifier);
                detailsFactory.SetBinding(
                    BatchTestProblemResultView.DataContextProperty,
                    new Binding(nameof(BatchTestCompetitorResultViewModel.ProblemResults))
                );
                var detailsColumn = new DataGridTemplateColumn {
                    Header = problem.Identifier,
                    CellTemplate = new DataTemplate {
                        VisualTree = detailsFactory
                    }
                };

                XDataGridResults.Columns.Add(detailsColumn);
            }
        }
    }
}
