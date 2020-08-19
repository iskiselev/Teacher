using System.Collections.Generic;

namespace Teacher.Engine.Modules
{
    public interface ITrainModule
    {
        public IEnumerable<Quiz> CreateQuizzes();
    }
}