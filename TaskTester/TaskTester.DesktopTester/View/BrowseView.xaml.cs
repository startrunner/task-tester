using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace TaskTester.DesktopTester.View
{
    /// <summary>
    /// Interaction logic for BrowseView.xaml
    /// </summary>
    public partial class BrowseView : UserControl
    {
        public enum BrowseType { File, Directory, SaveFile };

        public BrowseType Type { get; set; } = BrowseView.BrowseType.File;

        public static readonly DependencyProperty SelectedPathsProperty = DependencyProperty.Register(
            nameof(SelectedPaths),
            typeof(string[]),
            typeof(BrowseView),
            new PropertyMetadata(new string[0], OnSelectedFilesValueChanged)
        );

        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
            nameof(FileFilter),
            typeof(string),
            typeof(BrowseView),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(BrowseView),
            new PropertyMetadata("Browse View", OnLabelValueChanged)
        );

        public static readonly DependencyProperty MultipleSelectProperty = DependencyProperty.Register(
            nameof(MultipleSelect),
            typeof(bool),
            typeof(BrowseView),
            new PropertyMetadata(false)
        );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(BrowseView),
            new PropertyMetadata("", OnTextValueChanged)
        );

        public static readonly DependencyProperty DefaultFilenameProperty = DependencyProperty.Register(
            nameof(DefaultFilename),
            typeof(string),
            typeof(BrowseView)
        );

        private static void OnTextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d as BrowseView).OnTextValueChanged(e);
        private static void OnLabelValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d as BrowseView).OnLabelValueChanged(e);
        private static void OnSelectedFilesValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d as BrowseView).OnSelectedFilesValueChanged(e);

        public BrowseView()
        {
            InitializeComponent();
            XLabel.Text = this.Label;
            XTextBox.Text = this.Text;
        }

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        public string[] SelectedPaths
        {
            get => GetValue(SelectedPathsProperty) as string[];
            set => SetValue(SelectedPathsProperty, value);
        }


        public string Label
        {
            get => GetValue(LabelProperty) as string;
            set => SetValue(LabelProperty, value);
        }

        public bool MultipleSelect
        {
            get => (bool)GetValue(MultipleSelectProperty);
            set => SetValue(MultipleSelectProperty, value);
        }

        public string FileFilter
        {
            get => GetValue(FileFilterProperty) as string;
            set => SetValue(FileFilterProperty, value);
        }

        public string DefaultFilename
        {
            get => GetValue(DefaultFilenameProperty) as string;
            set => SetValue(DefaultFilenameProperty, value);
        }

        public bool ShowFullPath { get; set; } = false;

        public event EventHandler<RoutedEventArgs> OverrideClicked;

        private void OnTextValueChanged(DependencyPropertyChangedEventArgs e) => XTextBox.Text = e.NewValue?.ToString();
        private void OnLabelValueChanged(DependencyPropertyChangedEventArgs e) => XLabel.Text = e.NewValue?.ToString();

        private void OnSelectedFilesValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string[] files)
            {
                this.XTextBox.Text = String.Join(", ", files.Select(Path.GetFileName));
            }
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.OverrideClicked != null) OverrideClicked?.Invoke(this, e);
            else
            {
                if (this.Type == BrowseType.File)
                {
                    OpenFileDialog dialog = new OpenFileDialog() {
                        Multiselect = this.MultipleSelect,
                        Filter = FileFilter,
                        CheckFileExists = true,
                        CheckPathExists = true,
                    };
                    dialog.ShowDialog();
                    this.SelectedPaths = dialog.FileNames;
                }
                else if (this.Type == BrowseType.SaveFile)
                {
                    SaveFileDialog dialog = new SaveFileDialog() {
                        Filter = FileFilter,
                        CheckPathExists = true,
                        FileName = DefaultFilename ?? "Spreadsheet",
                        AddExtension = true
                    };
                    dialog.ShowDialog();
                    this.SelectedPaths = dialog.FileNames;
                }
                else
                {
                    var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.ShowDialog();
                    this.SelectedPaths = new[] { dialog.SelectedPath };
                }


                if (!ShowFullPath) this.Text = string.Join(", ", SelectedPaths.Select(Path.GetFileName));
                else this.Text = string.Join(", ", SelectedPaths);
            }
        }
    }
}
