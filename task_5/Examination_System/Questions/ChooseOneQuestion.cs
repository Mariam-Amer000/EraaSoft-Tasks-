using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class ChooseOneQuestion : Question
{
    public override string Header => "Choose_One_Question";
    public ChooseOneQuestion(string body, QuestionLevel level, double mark,List<Answer> choices)
           : base(body, level, mark,choices) { }
}
