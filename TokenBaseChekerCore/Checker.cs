using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TokenBaseChekerCore
{
    public class Checker
    {

        private CheckInfo checkResult;

        public CheckInfo CheckResult
        {
            get { return checkResult; }
            //set { checkResult = value; }
        }

        public string DataFileName { get; }
        public string LogsFileName { get; }

        public Checker(string dataFilePath, string logsFilePath)
        {
            DataFileName = dataFilePath;
            LogsFileName = logsFilePath;
        }


        private static async Task<(List<string>, TokensInfo)> GetValidTokensAsync(string fileName)
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

            TokensInfo info = new TokensInfo()
            {
                RemovedEmptyLinesCount = removedEmptyLinesCount,
                RemovedBrokenLinesCount = removedInvalidLines,
                RemovedDublicatesCount = RemovedDublicatesLines,
            };

            return (UniqLines.ToList(), info);
        }



        public async Task<List<string>> GetNewTokensAsync()
        {
            (List<string>, TokensInfo) dataTokensResult = await GetValidTokensAsync(DataFileName);
            (List<string>, TokensInfo) logsTokensResult = await GetValidTokensAsync(LogsFileName);

            List<string> data = dataTokensResult.Item1.ToList();
            List<string> logs = logsTokensResult.Item1.ToList();

            List<string> resulTokenList = logs.Except(data).ToList();

            int newTokensInLogsCount = resulTokenList.Count;
            int matchesInDataCount = logs.Count - resulTokenList.Count;

            checkResult = new CheckInfo()
            {
                dataInfo = dataTokensResult.Item2,
                logsInfo = logsTokensResult.Item2,
                MatchesInDataCount = matchesInDataCount,
                NewTokensInLogsCount = newTokensInLogsCount,
            };

            return resulTokenList;
        }

    }


    public class StaticChecker
    {
        private CheckInfo checkResult;

        public CheckInfo CheckResult
        {
            get { return checkResult; }
            //set { checkResult = value; }
        }

        public List<string> Data { get; }
        public List<string> Logs { get; }

        public StaticChecker(List<string> data, List<string> logs)
        {
            Data = data;
            Logs = logs;
        }


        public async Task<ICollection<string>> GetNewTokensAsync()
        {
            (List<string>, TokensInfo) dataTokensResult = await GetValidTokensAsync(Data);
            (List<string>, TokensInfo) logsTokensResult = await GetValidTokensAsync(Logs);

            List<string> data = dataTokensResult.Item1.ToList();
            List<string> logs = logsTokensResult.Item1.ToList();

            List<string> resulTokenList = logs.Except(data).ToList();

            int newTokensInLogsCount = resulTokenList.Count;
            int matchesInDataCount = logs.Count - resulTokenList.Count;

            checkResult = new CheckInfo()
            {
                dataInfo = dataTokensResult.Item2,
                logsInfo = logsTokensResult.Item2,
                MatchesInDataCount = matchesInDataCount,
                NewTokensInLogsCount = newTokensInLogsCount,
            };

            return resulTokenList;
        }

        public static async Task<(List<string>, TokensInfo)> GetValidTokensAsync(List<string> lines)
        {
            int removedEmptyLinesCount = lines.RemoveAll(x => string.IsNullOrWhiteSpace(x));//Удаление пустых строк

            lines = lines.Select(outer => outer.Trim()).ToList();
            Regex regex = new Regex(@"^[A-Za-z0-9]{24}\.[A-Za-z0-9_-]{6}\.[A-Za-z0-9_-]{27}$");
            //lines = lines.Where(outer => regex.Match(outer).Success).ToList();
            int removedInvalidLines = lines.RemoveAll(outer => !regex.Match(outer).Success); //Удалить невалидные строки

            var UniqLines = lines.Distinct();
            var RemovedDublicatesLines = lines.Count - UniqLines.Count();

            TokensInfo info = new TokensInfo()
            {
                RemovedEmptyLinesCount = removedEmptyLinesCount,
                RemovedBrokenLinesCount = removedInvalidLines,
                RemovedDublicatesCount = RemovedDublicatesLines,
            };

            return (UniqLines.ToList(), info);
        }


    }
}
