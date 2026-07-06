using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class TrueFalseQuestion : Question
{
    public override string Header => "TrueFalseQuestion";
    public TrueFalseQuestion(string body, QuestionLevel level, double mark, List<Answer> choises)
            : base( body, level, mark,choises) { }
     
}
