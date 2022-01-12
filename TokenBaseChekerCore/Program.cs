using System;

namespace TokenBaseChekerCore
{
    internal class Program
    {


        static void Main(string[] args)
        {
            Checker checker = new Checker("base.txt", "new.txt");

            var tokens = checker.GetNewTokensAsync().GetAwaiter().GetResult();
            Console.WriteLine(checker.CheckResult);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
