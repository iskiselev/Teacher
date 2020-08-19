using System.Collections.Generic;
using System.Linq;

namespace Teacher.Engine.Modules
{
    public class Debug : ITrainModule
    {
        public int Count { get; set; } = 1;

        public IEnumerable<Quiz> CreateQuizzes() =>
            Enumerable.Range(0, Count).Select(i => new Quiz($"{i}", $"{i}"));
    }
}