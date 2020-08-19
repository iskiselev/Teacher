using System;
using System.Collections.Generic;
using System.Linq;

namespace Teacher.Engine.Modules
{
    public class Arithmetic : ITrainModule
    {
        private class Operation
        {
            public readonly string Sign;
            public readonly Func<int, int, int> GetResult;
            public readonly Func<int, int, bool> IsValid;
            public readonly bool IsSymmetric;

            private Operation(string sign, Func<int, int, int> getResult, Func<int, int, bool> isValid,
                bool isSymmetric)
            {
                GetResult = getResult;
                IsValid = isValid;
                IsSymmetric = isSymmetric;
                Sign = sign;
            }

            public static Operation Addition = new Operation("+", (a, b) => a + b, (a, b) => true, true);
            public static Operation Subtraction = new Operation("-", (a, b) => a - b, (a, b) => true, false);
            public static Operation Multiplication = new Operation("*", (a, b) => a * b, (a, b) => true, true);
            public static Operation Division = new Operation("/", (a, b) => a / b, (a, b) => a % b == 0, false);
            public static Operation Mod = new Operation("mod", (a, b) => a % b, (a, b) => true, false);
        }

        public int MinA { get; set; } = 0;
        public int MaxA { get; set; } = 10;

        public int MinB { get; set; } = 0;
        public int MaxB { get; set; } = 3;
        public int MinResult { get; set; } = 0;
        public int MaxResult { get; set; } = 20;

        public bool EnableSymmetricOperations { get; set; } = true;

        public bool EnableAddition 
        { 
            get => _enabledOperations[Operation.Addition];
            set => _enabledOperations[Operation.Addition] = value;
        }
        public bool EnableSubtraction
        {
            get => _enabledOperations[Operation.Subtraction];
            set => _enabledOperations[Operation.Subtraction] = value;
        }
        public bool EnableMultiplication
        {
            get => _enabledOperations[Operation.Multiplication];
            set => _enabledOperations[Operation.Multiplication] = value;
        }
        public bool EnableDivision
        {
            get => _enabledOperations[Operation.Division];
            set => _enabledOperations[Operation.Division] = value;
        }
        public bool EnableMod
        {
            get => _enabledOperations[Operation.Mod];
            set => _enabledOperations[Operation.Mod] = value;
        }

        private readonly Dictionary<Operation, bool> _enabledOperations = new Dictionary<Operation, bool>
        {
            {Operation.Addition, true},
            {Operation.Subtraction, false},
            {Operation.Multiplication, false},
            {Operation.Division, false},
            {Operation.Mod, false},
        };

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
                    foreach (var op in _enabledOperations.Where(pair=>pair.Value).Select(pair => pair.Key))
                    {
                        if (op.IsValid(i, j))
                        {
                            var result = op.GetResult(i, j);
                            if (result >= MinResult && result <= MaxResult)
                            {
                                quiz.Add(new Quiz($"{i} {op.Sign} {j}", $"{result}"));
                                if (op.IsSymmetric && EnableSymmetricOperations)
                                {
                                    quiz.Add(new Quiz($"{j} {op.Sign} {i}", $"{result}"));

                                }
                            }
                        }
                    }
                }
            }

            return quiz;
        }
    }
}