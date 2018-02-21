using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace TaskTester.DesktopTester.ViewModel
{
    public class PrimitiveViewModel<TPrimitive> : ViewModelBase
    {
        public static implicit operator PrimitiveViewModel<TPrimitive>(TPrimitive value) =>
            new PrimitiveViewModel<TPrimitive>(value);

        public event EventHandler RemoveRequested;
        //public event EventHandler ValueSet;
        public ICommand Remove { get; }

        private TPrimitive mValue;
        public TPrimitive Value
        {
            get => mValue;
            set
            {
                mValue = value;
                RaisePropertyChanged(nameof(Value));
                //ValueSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public PrimitiveViewModel(TPrimitive value = default(TPrimitive))
        {
            mValue = value;
            Remove = new RelayCommand(RemoveExecute);
        }

        private void RemoveExecute() => RemoveRequested?.Invoke(this, EventArgs.Empty);
    }
}
