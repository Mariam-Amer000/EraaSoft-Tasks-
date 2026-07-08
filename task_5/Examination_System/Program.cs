using Examination_System.Answers;
using Examination_System.Enums;
using Examination_System.Questions;
using Examination_System.Subjects;
namespace Examination_System;
internal class Program
{
    #region Menus
    public static void MainMenu()
    {
        Console.WriteLine("Examination System ");
        Console.WriteLine("1- Teacher mode");
        Console.WriteLine("2- Student mode");
        Console.WriteLine("0- Exit");
    }
    public static void Subjects()
    {
        Console.WriteLine("****** Subjects ******");
        Console.WriteLine("1- Physics");
        Console.WriteLine("2- Chemistry");
        Console.WriteLine("3- Math");
        Console.WriteLine("4- Biology");
        Console.WriteLine("5- English");
        Console.WriteLine("0- Back");
    }
    public static void SubjectMenu()
    {
        Console.WriteLine("****** Subject Menu ******");
        Console.WriteLine("1- Add question");
        Console.WriteLine("2- edit question");
        Console.WriteLine("3- remove question");
        Console.WriteLine("4- show all questions");
        Console.WriteLine("0- Back");
    }
    public static void QuestionTypesMenu()
    {
        Console.WriteLine("Question types");
        Console.WriteLine("1- True and false");
        Console.WriteLine("2- Chose one");
        Console.WriteLine("3- Chose more than one");
    }
    public static void ExamTypes()
    {
        Console.WriteLine("****** Exams ******");
        Console.WriteLine("1- Final Exam");
        Console.WriteLine("2- Practical Exam");
        Console.WriteLine("0- Back");
    }
    #endregion

    #region Input Helpers
    public static int TakeChoice()
    {
        Console.WriteLine("Enter your choise: ");
        int num = Convert.ToInt32(Console.ReadLine());
        return num;
    }

    public static QuestionLevel TakeQuestionLevel()
    {
        Console.WriteLine("Enter level of the question:");
        Console.WriteLine("1- Easy");
        Console.WriteLine("2- Medium");
        Console.WriteLine("3- Hard");

        int choice = Convert.ToInt32(Console.ReadLine());

        return choice switch
        {
            1 => QuestionLevel.Easy,
            2 => QuestionLevel.Medium,
            3 => QuestionLevel.Hard,
            _ => throw new Exception("Invalid Level")
        };
    }
    public static string ChoseSubjectTitle() 
    {
        string title = string.Empty;
        
        Subjects();
        int SubjectChoise;
        SubjectChoise = TakeChoice();

        switch (SubjectChoise)
        {
            case 1:
                title = "Physics";
                break;
            case 2:
                title = "Chemistry";
                break;
            case 3:
                title = "Math";
                break;
            case 4:
                title = "Biology";
                break;
            case 5:
                title = "English";
                break;
            case 0:
                break;
            default:
                Console.WriteLine("invalid choise");
                break;
        }
        return title;
    }
    #endregion

    #region Factory Methods
    public static Answer MakeAnswer()
    {
        char symbol;
        string value;
        bool IsCorrect = false;

        Console.Write("Enter symbol: ");
        symbol = Convert.ToChar(Console.ReadLine());

        Console.Write("Enter Value: ");
        value = Console.ReadLine();

        Console.Write("Value Correct [True / False]: ");
        IsCorrect = Convert.ToBoolean(Console.ReadLine()); 
        return new(symbol, value, IsCorrect);
    }
    public static Question MakeQuestion(int ChooisesNumber, int questionType)
    {
        string body = string.Empty;
        double degree;
        QuestionLevel level;
        Console.WriteLine("Enter question body: ");
        body = Console.ReadLine();

        Console.WriteLine("Enter Mark of The question: ");
        degree = Convert.ToDouble(Console.ReadLine());

        level = TakeQuestionLevel();

        List<Answer> Choices = [];
        for (int i = 0; i < ChooisesNumber; i++)
        {
            Answer answer = MakeAnswer();
            Choices.Add(answer);
        }
        return questionType switch
        {
            1 => new TrueFalseQuestion(body, level, degree, Choices),
            2 => new ChooseOneQuestion(body, level, degree, Choices),
            3 => new ChooseAllQuestion(body, level, degree, Choices),

        };
    }
    public static Subject MakeSubject(string title) => new(title);
    
    #endregion

    #region Teacher Mode
    public static void TeacherMode()
    {
        
        int InnerSubjectChoise;
        int questionType;

        Console.WriteLine("****** Teacher Mode ******");
        //Chose and creat subject and make question list
        Subject subject = MakeSubject(ChoseSubjectTitle());


        SubjectMenu();
        InnerSubjectChoise = TakeChoice();
        switch (InnerSubjectChoise)
        {
            case 1://add question
                Console.WriteLine("Enter numer of questions: ");
                int numberOfQuestions = Convert.ToInt32(Console.ReadLine());

                while (numberOfQuestions > 0)
                {
                    QuestionTypesMenu();
                    questionType = TakeChoice();
                    switch (questionType)
                    {
                        case 1:
                            subject.Questions.Add(MakeQuestion(2,1));
                            numberOfQuestions--;
                            break;
                        case 2:
                            subject.Questions.Add(MakeQuestion(4, 2));
                            numberOfQuestions--;
                            break;
                        case 3:
                            subject.Questions.Add(MakeQuestion(4,3));
                            numberOfQuestions--;
                            break;
                    }
                    
                }
                
                break;
            case 2:
                Console.WriteLine("Edit Question is not implemented yet.");
                break;

            case 3:
                Console.WriteLine("Remove Question is not implemented yet.");
                break;

            case 4:
                Console.WriteLine("Show Questions is not implemented yet.");
                break;
        }
    }
    #endregion

    #region StudentMode
    public static void StudentMode()
    {
        int answerSymbol;
        double Score = 0;

        Console.Clear();
        Console.WriteLine("****** Student Mode ******");

        Subject subject = MakeSubject(ChoseSubjectTitle());
        subject.Questions.ReadQuestionsFromFile();

        ExamTypes();
        int examType = TakeChoice();
        QuestionLevel ExamLevel = TakeQuestionLevel();

        List<Question> examQuestions = [];

        foreach (Question question in subject.Questions)
        {
            if (question.Level == ExamLevel)
                examQuestions.Add(question);
        }

        Console.Write("Enter Number of questions: ");
        int numberOfQuestoins = Convert.ToInt32(Console.ReadLine());

        if (numberOfQuestoins > examQuestions.Count)
        {
            Console.WriteLine("Not enough questions.");
            return;
        }

        for (int i = 0; i < numberOfQuestoins; i++)
        {
            Console.WriteLine(examQuestions[i]);

            Console.Write("Enter Your Choise: ");
            answerSymbol = Convert.ToInt32(Console.ReadLine());


            if (examQuestions[i].Choices.ElementAt(answerSymbol - 1).IsCorrect)
                Score += examQuestions[i].Mark;
        }
        switch (examType)
        {
            case 1:
                Console.WriteLine("Final Exam");
                Console.WriteLine($"Your score is: {Score}");
                break;
            case 2:
                Console.WriteLine("Practical Exam: ");
                Console.WriteLine("Questions wih corrcet answer");
                int correct = 0 ;
                for (int i = 0; i < numberOfQuestoins; i++)
                {
                    Console.WriteLine(examQuestions[i]);

                    for (int k = 0; k < examQuestions[i].Choices.Count; k++)
                    {
                        if (examQuestions[i].Choices[k].IsCorrect) { correct = k;  break; }
                           
                    }
                    Console.WriteLine($"Your correct answer is:{examQuestions[i].Choices[correct]}");
                    
                }
                break;
            default:
                Console.WriteLine("invalid choies");
                break;
        }

    }         
    #endregion

    #region Program Entry
    static void Main(string[] args)
    {
        int MainChoise;

        do
        {
            MainMenu();
            MainChoise = TakeChoice();

            switch (MainChoise)
            {
                case 1:
                    Console.Clear();
                    TeacherMode();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    break;
                case 2:
                    Console.Clear();
                    StudentMode();
                    break;
                case 0:
                    Console.WriteLine("Exit");
                    break;
                default:
                    Console.WriteLine("invalid choise");
                    break;
            }
        } while (MainChoise != 0);
    }
    #endregion
}