using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class ChooseAllQuestion : Question
{
    public override string Header => "ChooseAllQuestion";
    public ChooseAllQuestion( string body, QuestionLevel level, double mark, List<Answer> choises)
           : base( body, level, mark,choises) { }
}
