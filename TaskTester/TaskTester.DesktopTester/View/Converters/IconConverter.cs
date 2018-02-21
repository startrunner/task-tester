using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskTester.DesktopTester.ViewModel;

namespace TaskTester.DesktopTester.View.Converters
{
    public sealed class IconConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestResultTypeViewModel testResultType)
                return App.Current.Resources[ConvertTestResultType(testResultType)];
            if (value is BatchTestTestResultTypeViewModel batchTestResultType)
                return App.Current.Resources[ConvertBatchTestResultType(batchTestResultType)];


            return App.Current.Resources["StatusOK"];
        }

        private object ConvertBatchTestResultType(BatchTestTestResultTypeViewModel batchTestResultType)
        {
            switch(batchTestResultType)
            {
                case BatchTestTestResultTypeViewModel.CorrectAnswer:
                    return "StatusOK";
                case BatchTestTestResultTypeViewModel.WrongAnswer:
                    return "StatusCriticalError";
                case BatchTestTestResultTypeViewModel.ProgramCrashed:
                case BatchTestTestResultTypeViewModel.CheckerCrashed:
                    return "StatusSecurityWarning";
                case BatchTestTestResultTypeViewModel.TimeLimitExceeded:
                    return "StopTime";
                default:
                    return "StatusOK";
            }
        }

        private string ConvertTestResultType(TestResultTypeViewModel testResultType)
        {
            switch (testResultType)
            {
                case TestResultTypeViewModel.CorrectAnswer:
                    return "StatusOK";
                case TestResultTypeViewModel.WrongAnswer:
                    return "StatusCriticalError";
                case TestResultTypeViewModel.CheckerCrashed:
                case TestResultTypeViewModel.ProgramCrashed:
                    return "StatusSecurityWarning";
                case TestResultTypeViewModel.TimeLimitExceeded:
                    return "StopTime";
                default:
                    return "StatusOK";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
