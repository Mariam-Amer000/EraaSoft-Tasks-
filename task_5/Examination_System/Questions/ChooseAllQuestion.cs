using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class ChooseAllQuestion : Question
{
    public override string Header => "Choose_All_Question";
    public ChooseAllQuestion( string body, QuestionLevel level, double mark, List<Answer> choices)
           : base( body, level, mark,choices) { }
}
