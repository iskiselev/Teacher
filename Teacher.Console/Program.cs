using System.Threading.Tasks;
using Teacher.Engine.Ui;

namespace Teacher.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new ConsoleUi(new ConsoleWrapper()).Run();
        }
    }
}
