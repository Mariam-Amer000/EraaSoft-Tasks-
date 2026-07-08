using Examination_System.Questions;

namespace Examination_System.Subjects;

public class Subject(string title)
{
    public string Title { get;} = title;
    public QuestionList Questions { get; } = new($"{title}.txt");
}
