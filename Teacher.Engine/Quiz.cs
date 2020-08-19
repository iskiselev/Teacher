namespace Teacher.Engine
{
    public class Quiz
    {
        public Quiz(string question, string correctAnswer)
        {
            Question = question;
            CorrectAnswer = correctAnswer;
        }

        public string Question { get; }
        public string CorrectAnswer { get; }

        protected bool Equals(Quiz other)
        {
            return Question == other.Question;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Quiz) obj);
        }

        public override int GetHashCode()
        {
            return (Question != null ? Question.GetHashCode() : 0);
        }
    }
}