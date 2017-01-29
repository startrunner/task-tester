using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TaskTester.CheckerCore.OutputVerification;
using TaskTester.DesktopTester.Model;

namespace TaskTester.DesktopTester.ViewModel
{
    class CheckerBindingViewModel:ViewModelBase
    {
        public CheckerBinding Model { get; set; }

        

        public OutputVerificationResultType Type
        {
            get
            {
                return Model.Type;
            }
            set { Model.Type = value; }
        }

        public string SearchString
        {
            get { return Model?.SearchString??"@@@@"; }
            set { Model.SearchString = value;  }
        }

        public string Score
        {
            get { return Model.Score.ToString(); }
            set
            {
                double val;
                if(double.TryParse(value, out val))
                {
                    Model.Score = val;
                }
            }
        }

        public CheckerBindingViewModel(CheckerBinding model)
        {
            Model = model;
        }
    }
}
