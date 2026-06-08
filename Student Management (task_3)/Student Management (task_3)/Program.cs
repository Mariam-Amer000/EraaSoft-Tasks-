using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace Student_Management__task_3_;
internal class Program
{
    static void Main(string[] args)
    {
        string selection=string.Empty;
        School sc1 = new School();
        do
        {
            sc1.menu();

            Console.Write("Enter your selection: ");
            selection = Console.ReadLine().ToUpper();

            switch (selection)
            {
                case "1":
                    Console.Write("Enter Student name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter student age: ");
                    int age = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Enter student id: "); //make it random
                    int id = Convert.ToInt32(Console.ReadLine());

                    if (sc1.AddStudent(new Student(id, name, age)))
                        Console.WriteLine("************ Student added *************");
                    else 
                        Console.WriteLine("************ Student not added ************");
                    break;//Add Student
                case "2":
                    Console.Write("Enter Instructor name: ");
                    name = Console.ReadLine();

                    Console.Write("Enter Instructor specialization: ");
                    string specialization= Console.ReadLine();

                    Console.Write("Enter Instructor id: "); //make it random
                     id = Convert.ToInt32(Console.ReadLine());

                    if (sc1.AddInstructor(new Instructor(id, name, specialization)))
                        Console.WriteLine("************ Instructor added ************");
                    else
                        Console.WriteLine("************ Instructor not added ************");
                    break;//Add Instructor
                case "3":
                    if (sc1.instructors.Count == 0)
                    {
                        Console.WriteLine("****************************");
                        Console.WriteLine("there is no instructors yet");
                        Console.WriteLine("you can't add course right now");
                        Console.WriteLine("****************************");
                        break;
                    }
                    Console.Write("Enter course title: ");
                    string title= Console.ReadLine();

                    Console.Write("Enter course id: ");
                    id=Convert.ToInt32(Console.ReadLine());

                    Console.Write("Choose an instructor: ");
                    sc1.printAllInstructors();
                    int choise = Convert.ToInt32(Console.ReadLine());
                    //if there is no instructors back to the main menu 
                    // i will try to  make it 
                    Course course = new Course(id, title, sc1.instructors[choise - 1]);
                    if (sc1.AddCourse(course) == true)
                        Console.WriteLine("************ Course added ************");
                    else
                        Console.WriteLine("************ Course not added ************");
                    break;//Add Course
                case "4":
                    Console.Write("Enter student id: ");
                    int studentId=Convert.ToInt32(Console.ReadLine());

                    Console.Write("Enter course Id:");
                    int courseId=Convert.ToInt32(Console.ReadLine());

                    if(sc1.EnrollStudentInCourse(studentId, courseId))
                        Console.WriteLine("********** Enrolled done **********");
                    else
                        Console.WriteLine("********** Enrolled falied **********");
                    break;//Enroll Student in Course
                case "5":
                    sc1.printAllStudents();
                    break;//Show All Students
                case "6":
                    sc1.printAllCourses();
                    break;//Show All Courses
                case "7":
                    sc1.printAllInstructors();
                    break;//Show All Instructors
                case "8":
                    Console.Write("Enter Student Id: ");
                    id= Convert.ToInt32(Console.ReadLine());

                    Student student = sc1.FindStudentById(id);

                    if(student!= null)
                        Console.WriteLine($"{student.PrintDetails()}");
                    else
                        Console.WriteLine("********* Student not found *********");
                    break;//Find the student by id
                case "9":
                    Console.Write("Enter student Name: ");
                    name = Console.ReadLine();

                    student = sc1.FindStudentByName(name);

                    if (student != null)
                        Console.WriteLine($"{student.PrintDetails()}");
                    else
                        Console.WriteLine("********* Student not found *********");
                    break;//Find the student by name
                case "10":
                    Console.Write("Enter Course Id: ");
                    id = Convert.ToInt32(Console.ReadLine());

                     course = sc1.FindCourseById(id);

                    if (course != null)
                        Console.WriteLine(course.PrintDetails());
                    else
                        Console.WriteLine("********** Course not found **********");
                    break;//Fine the course by id
                case "11":
                    Console.Write("Enter course Name: ");
                    name = Console.ReadLine();

                    course = sc1.FindCourseByName(name);

                    if (course != null)
                        Console.WriteLine(course.PrintDetails());
                    else
                        Console.WriteLine("********** Course not found **********");
                    break;//Fine the course by name
                case "12":
                    Console.Write("Enter instrucotr Id: ");
                    id = Convert.ToInt32(Console.ReadLine());

                    Instructor instructor = sc1.FindInstructorById(id);

                    if (instructor != null)
                        Console.WriteLine(instructor.PrintDetails());
                    else
                        Console.WriteLine("********** Instrucotr not found **********");
                    break;// Find the Instructor by id
                case "13":
                    Console.Write("Enter course Name: ");
                    name = Console.ReadLine();

                    instructor = sc1.FindInstructorByName(name);

                    if (instructor != null)
                        Console.WriteLine(instructor.PrintDetails());
                    else
                        Console.WriteLine("********** Instrucotr not found **********");
                    break;// Find the Instructor by name
                case "14":
                    Console.Write("Enter Studnet id:");
                    studentId = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Enter course id : ");
                    courseId=Convert.ToInt32(Console.ReadLine());

                    student = sc1.FindStudentById(studentId);
                    course = (sc1.FindCourseById(courseId));
                    if (course != null && student != null) 
                    {
                        if (student.isEnrolled(course))
                            Console.WriteLine("enrolled ");
                        else
                            Console.WriteLine("not enrolled");

                    }
                    else
                    {
                        Console.WriteLine("Error either student or course id is worng or not found");
                    }
                    break;//Check if the student enrolled in specific course
                case "15":
                    Console.Write("Enter course name: ");
                    name = Console.ReadLine();

                    if (sc1.FindCourseByName(name) != null)
                    {
                        course = sc1.FindCourseByName(name);
                        Console.WriteLine(course.Instructor.Name);
                    }
                        
                    break;//Return the instructor name by course name
                case "16":
                    Console.Write("Enter id of the student: ");
                    id=Convert.ToInt32(Console.ReadLine());
                    if (sc1.UpdateStudent(id))
                        Console.WriteLine("************* Student updated *************");
                    else
                        Console.WriteLine("************* not updated *************");
                    break;//update student
                case "17":
                    Console.Write("Enter id of the student: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    if (sc1.DeleteStudent(id))
                        Console.WriteLine("************* Student deleted *************");
                    else
                        Console.WriteLine("************* not deleted *************");
                    break;//delete update
            }
        } while (selection != "0");
    }
}
