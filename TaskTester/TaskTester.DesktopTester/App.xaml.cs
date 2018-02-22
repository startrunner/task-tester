using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using TaskTester.DesktopTester.View;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester
{
    public partial class App : Application
    {
        public static new App Current =>
            Application.Current as App;

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {

        };

        public static readonly string AppdataDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TaskTester");

        public static readonly string ViewModelPersistenceFile =
            Path.Combine(AppdataDirectory, "ViewModels.json");

        public ViewModelLocator ViewModelLocator { get; } = new ViewModelLocator();

        public App()
        {
            InitializeComponent();
            Directory.CreateDirectory(AppdataDirectory);

            //try
            //{
            //    ViewModelLocator = JsonConvert.DeserializeObject<ViewModelLocator>(
            //        File.ReadAllText(
            //            ViewModelPersistenceFile
            //        ),
            //        JsonSettings
            //    );
            //}
            //catch { }

            new SolutionEvaluationView() {
                DataContext = ViewModelLocator.GetPersistentSingleton<SolutionEvaluationViewModel>()
            }.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //File.WriteAllText(
            //    ViewModelPersistenceFile,
            //    JsonConvert.SerializeObject(ViewModelLocator, JsonSettings)
            //);

            base.OnExit(e);
        }
    }
}
