using Examination_System.Answers;
using Examination_System.Questions;
using Examination_System.Subjects;
namespace Examination_System.Exams;

public class Exam
{
    public Subject Subject { get; set; }
    public TimeOnly StartTime { get; set; }
    public double Score {  get; set; }
    public int NumberOfQuestions { get; set; }
    public  QuestionList Questions { get; set; }     
}
