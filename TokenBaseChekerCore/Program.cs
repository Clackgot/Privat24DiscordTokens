using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TokenBaseChekerCore
{

    /// <summary>
    /// Результат проверки
    /// </summary>
    struct CheckInfo2
    {
        /// <summary>
        /// Найдено дублей в базе
        /// </summary>
        public int DublicateTokensInDataCount;
        /// <summary>
        /// Найдено дублей в логах
        /// </summary>
        public int DublicateTokensInLogsCount;


        /// <summary>
        /// Токенов, которые уже есть в базе
        /// </summary>
        public int MatchesInDataCount;

        /// <summary>
        /// Новых токенов
        /// </summary>
        public int NewTokensInLogsCount;

        public override string ToString()
        {
            return $"Дублей в базе:{DublicateTokensInDataCount} Дублей в логах:{DublicateTokensInLogsCount}\n" +
                $"Токенов из логов в базе:{MatchesInDataCount} Новых токенов:{NewTokensInLogsCount}";
        }
    }


    struct CheckInfo
    {
        public TokensDataInfo dataInfo;
        public TokensDataInfo logsInfo;


        /// <summary>
        /// Токенов, которые уже есть в базе
        /// </summary>
        public int MatchesInDataCount;

        /// <summary>
        /// Новых токенов
        /// </summary>
        public int NewTokensInLogsCount;

        public override string ToString()
        {
            return $"База: {dataInfo}\n" +
                $"Логи: {logsInfo}\n" +
                $"Результат проверки: Найдено дублей:{MatchesInDataCount} Уникальных токенов:{NewTokensInLogsCount}";
        }

    }

    struct TokensDataInfo
    {
        /// <summary>
        /// Удалено дублей
        /// </summary>
        public int RemovedDublicatesCount;

        /// <summary>
        /// Удалено "битых" строк
        /// </summary>
        public int RemovedBrokenLinesCount;


        /// <summary>
        /// Удалено пустых строк
        /// </summary>
        public int RemovedEmptyLinesCount;

        public override string ToString()
        {
            return $"Удалено дублей:{RemovedDublicatesCount} Удалено битых строк:{RemovedBrokenLinesCount} Удалено пустых строк:{RemovedEmptyLinesCount}";
        }
    }

    internal class Checker
    {

        private CheckInfo checkResult;

        public CheckInfo CheckResult
        {
            get { return checkResult; }
            //set { checkResult = value; }
        }


        //private ICollection<string> newTokens = new HashSet<string>();

        //public IReadOnlyCollection<string> NewTokens
        //{
        //    get { return newTokens.ToList(); }
        //}
        public string BaseName { get; }
        public string NewName { get; }

        public Checker(string baseName, string newName)
        {
            BaseName = baseName;
            NewName = newName;
        }

        //public async Task<CheckInfo> CheckAsync()
        //{
        //    var baseLines = await File.ReadAllLinesAsync(BaseName);
        //    var newLines = await File.ReadAllLinesAsync(NewName);

        //    newTokens = (ICollection<string>)newLines.Except(baseLines);
        //    return new CheckInfo();
        //}

        //public CheckInfo Check()
        //{
        //    HashSet<string> baseLines = File.ReadAllLines(BaseName).ToHashSet();
        //    HashSet<string> newLines = File.ReadAllLines(NewName).ToHashSet();
        //    baseLines = (HashSet<string>)baseLines.Select(outer => outer.Trim());

        //    newTokens = (ICollection<string>)newLines.Except(baseLines);
        //    return new CheckInfo();
        //}


        public List<string> LoadDataTokens()
        {
            HashSet<string> baseLines = File.ReadAllLines(BaseName).ToHashSet();
            baseLines = baseLines.Select(outer => outer.Trim()).ToHashSet();

            Regex regex = new Regex(@"^\S{24}\.\S{6}\.\S{27}$");
            baseLines = baseLines.Where(outer => regex.Match(outer).Success).ToHashSet();
            baseLines.Remove("");
            return baseLines.ToList();
        }

        public async Task<ICollection<string>> GetDataTokensAsyncOld()
        {

            var lines = await File.ReadAllLinesAsync(BaseName);//Все строки файла
            int dataLinesCount = lines.Count();//Всего строк
            HashSet<string> dataLines = lines.ToHashSet();
            dataLines = dataLines.Select(outer => outer.Trim()).ToHashSet();

            int dataLinesCountAfterRemoveddDublicate = dataLines.Count();

            //CheckingResult.dataInfo.RemovedDublicatesCount = dataLinesCount - dataLinesCountAfterRemoveddDublicate;

            dataLines.Remove("");
            Regex regex = new Regex(@"^\S{24}\.\S{6}\.\S{27}$");
            dataLines = dataLines.Where(outer => regex.Match(outer).Success).ToHashSet();

            int dataLinesCountAfterRemoveInvalidLines = dataLines.Count();

            checkResult.dataInfo = new TokensDataInfo()
            {
                RemovedDublicatesCount = dataLinesCount - dataLinesCountAfterRemoveddDublicate,
                RemovedBrokenLinesCount = dataLinesCountAfterRemoveddDublicate - dataLinesCountAfterRemoveInvalidLines,
            };

            return dataLines.ToList();
        }

        private async Task<(ICollection<string>, TokensDataInfo)> GetValidTokensAsync(string fileName)
        {

            string[] linesArray = await File.ReadAllLinesAsync(fileName); //Все строки файла в виде массива
            List<string> lines = linesArray.ToList(); //Все строки файла в виде списка
            int removedEmptyLinesCount = lines.RemoveAll(x => string.IsNullOrWhiteSpace(x));//Удаление пустых строк

            lines = lines.Select(outer => outer.Trim()).ToList();
            Regex regex = new Regex(@"^[A-Za-z0-9]{24}\.[A-Za-z0-9_-]{6}\.[A-Za-z0-9_-]{27}$");
            //lines = lines.Where(outer => regex.Match(outer).Success).ToList();
            int removedInvalidLines = lines.RemoveAll(outer => !regex.Match(outer).Success); //Удалить невалидные строки


            var UniqLines = lines.Distinct();
            var RemovedDublicatesLines = lines.Count - UniqLines.Count();

            //checkResult.dataInfo = new TokensDataInfo()
            //{
            //    RemovedEmptyLinesCount = removedEmptyLinesCount,
            //    RemovedBrokenLinesCount = removedInvalidLines,
            //    RemovedDublicatesCount = RemovedDublicatesLines,
            //};

            checkResult.dataInfo = new TokensDataInfo()
            {
                RemovedEmptyLinesCount = removedEmptyLinesCount,
                RemovedBrokenLinesCount = removedInvalidLines,
                RemovedDublicatesCount = RemovedDublicatesLines,
            };

            return (UniqLines.ToList(), TokensDataInfo);
        }



        public async Task<ICollection<string>> GetNewTokens()
        {
            var baseTokens = await GetValidTokensAsync(BaseName);
            var baseTokens = await GetValidTokensAsync(BaseName);
        }

    }
    internal class Program
    {


        static void Main(string[] args)
        {
            #region Trash
            //List<string> vs1 = new List<string>()
            //{
            //    "test1",
            //    "test2",
            //    "test3",
            //    "test4",
            //    "test5",
            //    "test6",
            //};
            //List<string> vs2 = new List<string>()
            //{
            //    "test3",
            //    "test4",
            //};


            //ICollection<string> NewTokens = new HashSet<string>();
            //NewTokens.Add("123");
            //NewTokens.Add("123");

            //foreach (var item in NewTokens)
            //{
            //    Console.WriteLine(item);
            //} 
            #endregion


            Checker checker = new Checker("baseWithBrokenWithoutDublicate.txt", "new.txt");
            //Console.WriteLine(checker.Check());
            Console.WriteLine(checker.GetValidTokensAsync().GetAwaiter().GetResult().Count());

            Console.WriteLine(checker.CheckResult);

            //foreach (var item in checker.GetDataTokensAsync().GetAwaiter().GetResult())
            //{
            //    Console.WriteLine($"[{item}]");
            //}
        }
    }
}
