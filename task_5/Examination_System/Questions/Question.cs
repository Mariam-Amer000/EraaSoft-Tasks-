using Examination_System.Answers;
using Examination_System.Enums;
namespace Examination_System.Questions;

public abstract class Question(string body, QuestionLevel level, double mark, List<Answer> choices)
{
    public abstract string Header { get; } // question type
    public string Body { get; set; } = body;
    public double Mark { get; set; } = mark;
    public QuestionLevel Level { get; set; } = level;
    public List<Answer> Choices { get; set; } = choices;


    public override string ToString()
    {
        string text = $"{Header}\n{Body}\n";

        foreach (Answer answer in Choices)
        {
            text += answer + "\n";
        }

        return text;
    }


    public void Display()
    {
        Console.WriteLine(Header);
        Console.WriteLine(Body);

        foreach (Answer answer in Choices)
        {
            Console.WriteLine(answer);
        }

        Console.WriteLine(Level);
        Console.WriteLine($"Degree: {Mark}");
    }
}
