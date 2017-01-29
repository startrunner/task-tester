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
    /// Interaction logic for CheckerView.xaml
    /// </summary>
    public partial class CheckerView : Window
    {
        public CheckerView()
        {
            InitializeComponent();
            this.KeyUp += CheckerView_KeyUp;
            this.Closing += CheckerView_Closing;
        }

        private void CheckerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void CheckerView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }
    }
}
