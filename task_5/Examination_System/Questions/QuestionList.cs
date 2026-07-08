using Examination_System.Answers;
using Examination_System.Enums;

namespace Examination_System.Questions;

public class QuestionList(string filePath) : List<Question>
{
    public string FilePath { get;} = filePath;

    public new void Add(Question question)
    {
        WriteQuestionToFile(question);
        base.Add(question);
    }

    public void WriteQuestionToFile(Question question)
    {
        using StreamWriter writer = new StreamWriter(FilePath, true);

        writer.WriteLine(question.Header);
        writer.WriteLine(question.Body);
        writer.WriteLine(question.Level);
        writer.WriteLine(question.Mark);

        writer.WriteLine(question.Choices.Count);

        foreach (Answer answer in question.Choices)
        {
            writer.WriteLine($"{answer.Symbol}|{answer.Value}|{answer.IsCorrect}");
        }
    }

    public void ReadQuestionsFromFile()
    {
        if (!File.Exists(FilePath))
            return;

        using StreamReader reader = new StreamReader(FilePath);

        while (!reader.EndOfStream)
        {
            string type = reader.ReadLine()!;
            string body = reader.ReadLine()!;

            QuestionLevel level =
                Enum.Parse<QuestionLevel>(reader.ReadLine()!);

            double mark =
                Convert.ToDouble(reader.ReadLine());

            int choicesCount =
                Convert.ToInt32(reader.ReadLine());

            List<Answer> choices = [];

            for (int i = 0; i < choicesCount; i++)
            {
                string line = reader.ReadLine()!;
                string[] parts = line.Split('|');

                Answer answer = new Answer(
                   Convert.ToInt32(parts[0]),
                    parts[1],
                    Convert.ToBoolean(parts[2]));

                choices.Add(answer);
            }

            Question question = type switch
            {
                "TrueFalse" => new TrueFalseQuestion(body, level, mark, choices),
                "ChooseOne" => new ChooseOneQuestion(body, level, mark, choices),
                "ChooseAll" => new ChooseAllQuestion(body, level, mark, choices),

                _ => throw new Exception("Unknown Question Type")
            };

            base.Add(question);
        }
    }

    public void DisplayAllQuestions()
    {
        foreach (Question question in this)
        {
            question.Display();
            Console.WriteLine();
        }
    }

    public void DisplayQuestionsByLevel(QuestionLevel level)
    {
        foreach (Question question in this)
        {
            if (question.Level == level)
            {
                question.Display();
                Console.WriteLine();
            }
        }
    }
}