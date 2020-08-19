using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Teacher.Engine.Modules;
using Teacher.Engine.Util;

namespace Teacher.Engine
{
    public class Training
    {
        public class TrainingItem
        {
            private readonly TrainingSettings _trainingSettings;

            public TrainingItem(Quiz quiz, TrainingSettings trainingSettings)
            {
                _trainingSettings = trainingSettings;
                Quiz = quiz;
            }

            public Quiz Quiz { get; }

            public List<Trial> Trials { get; } = new List<Trial>();
        }

        public class Trial
        {
            public List<TrainingItemAttemptAnswer> Answers { get; } = new List<TrainingItemAttemptAnswer>();

            [JsonIgnore]
            public bool IsFinished => Answers.LastOrDefault()?.Correct ?? false;
            [JsonIgnore]
            public bool IsSuccesfullyFinished => Answers.Count > 0 && Answers.All(item => item.Correct);
        }

        public class TrainingItemAttemptAnswer
        {
            [JsonConverter(typeof(TimeSpanJsonConverter))]
            public TimeSpan Duration { get; }
            public string Answer { get; }

            [JsonIgnore]
            public bool Correct { get; }

            public TrainingItemAttemptAnswer(TimeSpan duration, string answer, bool correct)
            {
                Duration = duration;
                Answer = answer;
                Correct = correct;
            }
        }

        private readonly TrainingSettings _settings;

        public readonly List<TrainingItem> QuizItems;

        public Training(TrainingSettings settings, ITrainModule module)
        {
            _settings = settings;
            var items = module.CreateQuizzes();
            if (settings.QuestionsLimit > 0)
            {
                if (settings.Part <= 0)
                {
                    items = items.Shuffle();
                }
                else
                {
                    items = items.Shuffle(new Random(666)).Skip(settings.QuestionsLimit * (settings.Part - 1));
                }

                items = items.Take(settings.QuestionsLimit);
            }

            QuizItems = items.Select(item => new TrainingItem(item, settings)).ToList();
            NextItem();
        }

        private bool IsSatisfying(Training.TrainingItem item)
        {
            return ScoredAttempts(item) >= _settings.CorrectRepeats;
        }

        private int ScoredAttempts(Training.TrainingItem item)
        {
            var trials = item.Trials.Select(i => i);
            if (_settings.NullifyOnAttempt > 0)
            {
                int badAnswers = 0;
                trials = trials.Reverse()
                    .TakeWhile(attempt =>
                    {
                        badAnswers += attempt.Answers.Count(answer => !answer.Correct);
                        return badAnswers < _settings.NullifyOnAttempt;
                    });
            }

            return Math.Min(trials.Count(answer => answer.IsSuccesfullyFinished), _settings.CorrectRepeats);
        }

        public Quiz Current => CurrentItem?.Quiz;
        private TrainingItem CurrentItem { get; set; }

        public Reaction RegisterAnswer(string answer)
        {
            var result = new TrainingItemAttemptAnswer(TimeSpan.Zero, answer,
                answer == CurrentItem.Quiz.CorrectAnswer);
            CurrentItem.Trials.Last().Answers.Add(result);
            if (result.Correct)
            {
                NextItem();
                return Reaction.Correct;
            }

            return Reaction.Incorrect;
        }

        private void NextItem()
        {
            var questions = QuizItems.Where(item =>
                !IsSatisfying(item)).ToList();

            if (questions.Count > 0)
            {
                var quiz = questions.RandomElement();
                for (int i = 0; i < 10; i++)
                {
                    if (quiz == CurrentItem)
                    {
                        quiz = questions.RandomElement();
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    if (quiz == CurrentItem)
                    {
                        quiz = QuizItems.RandomElement();
                    }
                    else
                    {
                        break;
                    }
                }
                quiz.Trials.Add(new Trial());
                CurrentItem = quiz;
            }
            else
            {
                CurrentItem = null;
            }
        }

        public int ExpectedQuestions => _settings.CorrectRepeats * QuizItems.Count;
        public int CompletedQuestions => QuizItems.Sum(ScoredAttempts);
    }
}