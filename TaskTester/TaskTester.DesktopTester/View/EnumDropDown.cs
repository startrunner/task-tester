using System;
using System.Windows;
using System.Windows.Controls;

namespace TaskTester.DesktopTester.View
{
    class EnumDropDown:ComboBox
    {
        public static DependencyProperty EnumValueProperty = DependencyProperty.Register(nameof(EnumValue), typeof(object), typeof(EnumDropDown), new PropertyMetadata(OnEnumValueChanged));

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is EnumDropDown)
            {
                (d as EnumDropDown).EnumValChanged(e);
            }
        }

        private void EnumValChanged(DependencyPropertyChangedEventArgs args)
        {
            var type = args.NewValue?.GetType();
            var oldType = args.OldValue?.GetType();
            if(type!=null && type.IsEnum && type!=oldType)
            {
                this.Items.Clear();
                foreach(var v in Enum.GetValues(type))
                {
                    this.Items.Add(v);
                }
                this.SelectedItem = args.NewValue;
            }
        }

        public object EnumValue
        {
            get { return GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }

        public EnumDropDown():base()
        {
            this.SelectionChanged += EnumDropDown_SelectionChanged;
        }

        private void EnumDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnumValue = this.SelectedItem;
        }
    }
}
