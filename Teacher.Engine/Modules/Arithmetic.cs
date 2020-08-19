using System.Collections.Generic;

namespace Teacher.Engine.Modules
{
    public class Arithmetic : ITrainModule
    {
        public int MinA { get; set; } = 0;
        public int MaxA { get; set; } = 10;

        public int MinB { get; set; } = 0;
        public int MaxB { get; set; } = 3;
        public int MinResult { get; set; } = 0;
        public int MaxResult { get; set; } = 20;

        public IEnumerable<Quiz> CreateQuizzes()
        {
            if (MaxA < MinA || MaxB < MinB || MaxResult < MinResult)
            {
                return new List<Quiz>();
            }

            var quiz = new HashSet<Quiz>();
            for (int i = MinA; i <= MaxA; i++)
            {
                for (int j = MinB; j <= MaxB; j++)
                {
                    if (i + j >= MinResult && i + j <= MaxResult)
                    {
                        quiz.Add(new Quiz($"{i} + {j}", $"{i + j}"));
                        quiz.Add(new Quiz($"{j} + {i}", $"{i + j}"));
                    }
                }
            }

            return quiz;
        }
    }
}