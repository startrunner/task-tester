using GalaSoft.MvvmLight;
using TaskTester.DesktopTester.Model;

namespace TaskTester.DesktopTester.ViewModel
{
    class ArgumentViewModel:ViewModelBase
    {
        public Argument Model { get; private set; }

        public ArgType Type
        {
            get { return Model.Type; }
            set { Model.Type = value; }
        }

        public ArgumentViewModel(Argument model=null)
        {
            Model = model;
        }
    }
}