using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TokenBaseChekerCore
{
    struct CheckInfo
    {
        public int DuplicateTokenCount;
        public int NewTokensCount;

        public override string ToString()
        {
            return $"Дубли:{DuplicateTokenCount} Новые:{NewTokensCount}";
        }
    }

    internal class Checker
    {

        private ICollection<string> newTokens = new HashSet<string>();

        public ICollection<string> NewTokens
        {
            get { return newTokens; }
        }
        public string BaseName { get; }
        public string NewName { get; }

        public Checker(string baseName, string newName)
        {
            BaseName = baseName;
            NewName = newName;
        }

        public async Task<CheckInfo> CheckAsync()
        {
            var baseLines = await File.ReadAllLinesAsync(BaseName);
            var newLines = await File.ReadAllLinesAsync(NewName);

            newTokens = (ICollection<string>)newLines.Except(baseLines);
            return new CheckInfo();
        }
        
        public CheckInfo Check()
        {
            var baseLines = File.ReadAllLines(BaseName);
            var newLines = File.ReadAllLines(NewName);

            newTokens = (ICollection<string>)newLines.Except(baseLines);
            return new CheckInfo();
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


            Checker checker = new Checker("base.txt", "new.txt");
            Console.WriteLine(checker.Check());
        }
    }
}
