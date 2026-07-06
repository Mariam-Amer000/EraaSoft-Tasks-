using Examination_System.Questions;

namespace Examination_System.Subjects;

public class Subject(int id,string title)
{
    public int Id { get; set; } = id;
    public string Title { get; set; } = title;
    public QuestionList Questions = new QuestionList($"{title}.txt");
}
