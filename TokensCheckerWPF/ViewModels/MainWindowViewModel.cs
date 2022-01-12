using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using TokenBaseChekerCore;
using TokensCheckerWPF.Commands;
using TokensCheckerWPF.ViewModels.Base;

namespace TokensCheckerWPF.ViewModels
{
    class MainWindowViewModel : ViewModel
    {

        /// <summary>/// Title/// </summary>
		private string title = "ПРИВАТ24";
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }



        /// <summary>/// Токены базы /// </summary>
		private ObservableCollection<string> dataTokens = new ObservableCollection<string>();

        public ObservableCollection<string> DataTokens
        {
            get { return dataTokens; }
            set { Set(ref dataTokens, value); }
        }

        /// <summary>/// Токены для чека /// </summary>
        private ObservableCollection<string> logsTokens = new ObservableCollection<string>();

        public ObservableCollection<string> LogsTokens
        {
            get { return logsTokens; }
            set { Set(ref logsTokens, value); }
        }



        /// <summary>/// Новые токены /// </summary>
        private ObservableCollection<string> newTokens = new ObservableCollection<string>();

        public ObservableCollection<string> NewTokens
        {
            get { return newTokens; }
            set { Set(ref newTokens, value); }
        }




        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region OpenDataCommand
        public ICommand OpenDataCommand { get; }
        private bool CanOpenDataCommandExecute(object p) => true;

        private async void OnOpenDataCommandExecuted(object p)
        {
            DataTokens.Clear();
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var stream = dialog.OpenFile();

                using (StreamReader sr = new StreamReader(stream))
                {
                    string token;
                    while ((token = await sr.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            DataTokens.Add(token);
                        }
                    }
                    MessageBox.Show($"Загружено {DataTokens.Count} строк", "Загружено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (dataTokens.Count > 0 && logsTokens.Count > 0)
                {
                    StaticChecker checker = new StaticChecker(DataTokens.ToList(), LogsTokens.ToList());

                    var newTokens = await checker.GetNewTokensAsync();

                    NewTokens = new ObservableCollection<string>(newTokens);
                    MessageBox.Show(checker.CheckResult.ToString(), "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        #endregion


        #region OpenLogsCommand
        public ICommand OpenLogsCommand { get; }
        private bool CanOpenLogsCommandExecute(object p) => true;

        private async void OnOpenLogsCommandExecuted(object p)
        {
            LogsTokens.Clear();
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var stream = dialog.OpenFile();

                using (StreamReader sr = new StreamReader(stream))
                {
                    string token;
                    while ((token = await sr.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            LogsTokens.Add(token);
                        }
                    }
                    MessageBox.Show($"Загружено {LogsTokens.Count} строк", "Загружено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (dataTokens.Count > 0 && logsTokens.Count > 0)
                {
                    StaticChecker checker = new StaticChecker(DataTokens.ToList(), LogsTokens.ToList());

                    var newTokens = await checker.GetNewTokensAsync();

                    NewTokens = new ObservableCollection<string>(newTokens);

                    MessageBox.Show(checker.CheckResult.ToString(), "Результат", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }
        #endregion


        #region SaveNewTokensCommand
        public ICommand SaveNewTokensCommand { get; }
        private bool CanSaveNewTokensCommandExecute(object p) => true;

        private async void OnSaveNewTokensCommandExecuted(object p)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.FileName = "newTokens.txt";
            if (dialog.ShowDialog() == true)
            {
                if (File.Exists(dialog.FileName)) File.Delete(dialog.FileName);
                using (FileStream fstream = new FileStream(dialog.FileName, FileMode.OpenOrCreate))
                {
                    foreach (var line in newTokens)
                    {
                        byte[] array = Encoding.Default.GetBytes($"{line}\n");
                        // запись массива байтов в файл
                        await fstream.WriteAsync(array, 0, array.Length);
                    }
                }
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            OpenDataCommand = new LabmdaCommand(OnOpenDataCommandExecuted, CanOpenDataCommandExecute);
            OpenLogsCommand = new LabmdaCommand(OnOpenLogsCommandExecuted, CanOpenLogsCommandExecute);
            SaveNewTokensCommand = new LabmdaCommand(OnSaveNewTokensCommandExecuted, CanSaveNewTokensCommandExecute);

            DataTokens = new ObservableCollection<string>();
        }
    }
}
