namespace Teacher.Engine
{
    public class TrainingSettings
    {
        public int CorrectRepeats { get; set; } = 2;
        //public float Coverage { get; set; } = 1;
        //public float ValidFactor { get; set; } = 1;
        public int NullifyOnAttempt { get; set; } = 2;
        //public bool ShowCorrectAnswer { get; set; } = true;
        public int Part { get; set; } = 1;
        public int QuestionsLimit { get; set; } = 0;
    }
}