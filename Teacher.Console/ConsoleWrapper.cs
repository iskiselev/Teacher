using System.Threading.Tasks;
using Teacher.Engine.Ui;

namespace Teacher.Console
{
    public class ConsoleWrapper : IConsole
    {
        public Task<string> ReadLine()
        {
            return Task.FromResult(System.Console.ReadLine());
        }

        public void WriteLine(string line = "")
        {
            System.Console.WriteLine(line);
        }

        public void Clear()
        {
            System.Console.Clear();
        }
    }
}