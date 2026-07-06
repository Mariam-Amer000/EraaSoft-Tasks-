using Examination_System.Questions;

namespace Examination_System.Subjects;

public class Subject(string title)
{
    public string Title { get; set; } = title;
    public QuestionList Questions = new QuestionList($"{title}.txt");
}
