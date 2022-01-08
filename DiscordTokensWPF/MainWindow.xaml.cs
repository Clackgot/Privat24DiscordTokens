using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DiscordTokensWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> parsedTokens = new List<string>();
        private List<string> loadedTokens = new List<string>();


        public MainWindow()
        {
            InitializeComponent();
            docBox.Document.PageWidth = 400;
        }
        private void SaveTokens_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    if (Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                }
            }
        }

        private async Task<string> GetToken(string filename)
        {
            var text = await File.ReadAllLinesAsync(filename);//Содержимое файла cookies
            var tokenString = text.FirstOrDefault(x => x.Contains(".twitch.tv") && x.Contains("auth-token"));//Поиск подходящей строки
            if (!(tokenString is null))
            {
                string token = tokenString.Split().Last();//Получение токена
                return token;
            }
            else
            {
                return null;
            }
        }

        private async Task<List<string>> GetDiscordTokens(string filename)
        {
            List<string> tokens = new List<string>();
            using (StreamReader fIn = new StreamReader(filename))
            {
                //// Метод Peek() - определить, конец ли файла
                //while (fIn.Peek() != -1)
                //{
                //    string s = await fIn.ReadLineAsync(); // прочитать строку
                //    Regex regex = new Regex(@"^\S{24}\.\S{6}\.\S{27}\r");
                //    var b = regex.Match(s).Success;

                //    if(b)
                //    {
                //        docBox.AppendText(s);
                //    }
                //    //bool isContains = s.Contains(".twitch.tv") && s.Contains("auth-token");//Поиск подходящей строки
                //    //if (isContains)
                //    //{
                //    //    fIn.Close();
                //    //    return s.Split().Last();
                //    //}
                //}

                string allText = await fIn.ReadToEndAsync();
                Regex regex = new Regex(@"^\S{24}\.\S{6}\.\S{27}\r");
                var match = regex.Match(allText);
                foreach (var item in match.Groups)
                {
                    if (!string.IsNullOrEmpty(item.ToString()))
                    {
                        tokens.Add(item.ToString());
                    }
                }
            }
            return tokens;
        }


        private async void LoadLogs_Click(object sender, RoutedEventArgs e)
        {
            TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
            doc.Text = "";
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                List<string> allfiles = new List<string>();


                foreach (var dir in dialog.SelectedPaths)
                {
                    await Task.Run(() => {
                        allfiles.AddRange(Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories).ToList());
                        });
                    statusTextBlock.Text = $"Загружено {allfiles.Count.ToString()} файлов";
                }

                int count = allfiles.Count;
                int index = 1;
                foreach (string filename in allfiles)
                {
                    statusTextBlock.Text = $"[{index}/{count}]";
                    var tokens = await GetDiscordTokens(filename);
                    foreach (var token in tokens)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            //resultTokens += $"{token}\n";
                            parsedTokens.Add(token.Trim());
                        }
                    }
                    index++;
                    progressBar.Value = ((double)index / count)*100;
                }
                if(parsedTokens.Count == 0)
                {
                    MessageBox.Show("Discord токенов не найдено");
                }
                foreach (var token in parsedTokens)
                {
                    docBox.AppendText($"{token}\n");
                }
                //doc.Text = resultTokens;
            }
        }

        private async void SaveNewTokens_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.FileName = "netTokens.txt";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(docBox2.Document.ContentStart, docBox2.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    if (Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                }
            }

            //TextRange doc = new TextRange(docBox2.Document.ContentStart, docBox2.Document.ContentEnd);
            //doc.Text = "";
            //newTokens = parsedTokens.Except(loadedTokens) as List<string>;
            //string text = "";
            //foreach (var token in newTokens)
            //{
            //    text += token + "\n";
            //}
            
        }

        private async void LoadTokens_Click(object sender, RoutedEventArgs e)
        {
            //TextRange doc = new TextRange(docBox2.Document.ContentStart, docBox2.Document.ContentEnd);
            //doc.Text = "";
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "txt|*txt";
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                var stream = dialog.OpenFile();
                //StreamReader streamReader = new StreamReader(stream);
                //var text = await streamReader.ReadToEndAsync();
                //doc.Text = text;

                using (StreamReader sr = new StreamReader(stream))
                {
                    string token;
                    while ((token = await sr.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            loadedTokens.Add(token);
                        }
                    }
                }
            }

        }


        private async void GetNewTokens_Click(object sender, RoutedEventArgs e)
        {
            #region Test1
            //TextRange doc = new TextRange(docBox2.Document.ContentStart, docBox2.Document.ContentEnd);
            //doc.Text = "";

            //System.Console.WriteLine($"С логов: {parsedTokens.Count}");
            //System.Console.WriteLine($"С базы: {loadedTokens.Count}");

            //List<string> newTokens = loadedTokens.Except(parsedTokens).ToList();
            //System.Console.WriteLine($"Новых: {newTokens.Count}");
            //string text = "";
            //foreach (var token in newTokens)
            //{
            //    text += token + "\n";
            //}
            //doc.Text = text; 
            #endregion



            TextRange doc = new TextRange(docBox2.Document.ContentStart, docBox2.Document.ContentEnd);
            doc.Text = "";

            List<string> newTokens = parsedTokens.Except(loadedTokens).ToList();


            //System.Console.WriteLine($"Новых: {result.Count}");
            string text = "";
            foreach (var token in newTokens)
            {
                text += token + "\n";
            }
            doc.Text = text;


        }
    }
}
