using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class ChooseOneQuestion : Question
{
    public override string Header => "ChooseOneQuestion";
    public ChooseOneQuestion(string body, QuestionLevel level, double mark,List<Answer> choises)
           : base(body, level, mark,choises) { }
}
