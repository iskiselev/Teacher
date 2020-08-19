using System.Threading.Tasks;

namespace Teacher.Engine.Ui
{
    public interface IConsole
    {
        public Task<string> ReadLine();
        public void WriteLine(string line = "");
        public void Clear();
    }
}