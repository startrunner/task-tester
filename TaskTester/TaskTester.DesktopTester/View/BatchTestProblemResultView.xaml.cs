using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for BatchTestProblemResultView.xaml
    /// </summary>
    public partial class BatchTestProblemResultView : UserControl
    {
        public static readonly DependencyProperty ProblemIdentifierProperty = DependencyProperty.Register(
            nameof(ProblemIdentifier),
            typeof(string),
            typeof(BatchTestProblemResultView)
        );

        public string ProblemIdentifier
        {
            get => GetValue(ProblemIdentifierProperty) as string;
            set => SetValue(ProblemIdentifierProperty, value);
        }

        Dictionary<string, BatchTestProblemResultViewModel> ViewModel =>
            DataContext as Dictionary<string, BatchTestProblemResultViewModel>;

        public BatchTestProblemResultView()
        {
            InitializeComponent();
            this.DataContextChanged += HandleDataContextChanged;
        }


        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BatchTestProblemResultViewModel problemResult = ViewModel?[ProblemIdentifier];
            XGridMain.DataContext = problemResult;
        }
    }
}
