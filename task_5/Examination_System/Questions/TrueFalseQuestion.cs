using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class TrueFalseQuestion : Question
{
    public override string Header => "True_ False_Question";
    public TrueFalseQuestion(string body, QuestionLevel level, double mark, List<Answer> choices)
            : base( body, level, mark,choices) { }
     
}
