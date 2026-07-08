using Examination_System.Questions;
using Examination_System.Subjects;
namespace Examination_System.Exams;

public class Exam(Subject subject,int numberOfQuestons,QuestionList questions)
{
    public Subject Subject { get; set; } = subject;
    public DateTime StartTime { get; set; } = System.DateTime.Now;
    public double Score { get; set; } = 0;
    public int NumberOfQuestions { get; set; } = numberOfQuestons;
    public QuestionList Questions { get; set; } = questions;   
}
