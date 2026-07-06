namespace Examination_System.Answers
{
    public class Answer(int symbol, string value, bool isCorrect = false)
    {
        public int Symbol { get; set; } = symbol;
        public string Value { get; set; } = value;
        public bool IsCorrect { get; set; } = isCorrect;

        public override string ToString()
        {
            return $"{Symbol}: {Value}";
        }
    }
}
