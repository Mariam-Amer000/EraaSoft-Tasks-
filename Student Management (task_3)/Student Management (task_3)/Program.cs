using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace Student_Management__task_3_;

class Instructor(int instructorId, string name, string specialization)
{
    public int InstructorId = instructorId;
    public string Name = name;
    public string Specialization = specialization;

    public string PrintDetails()
    {
        string result = string.Empty;
        result = $"Instructor Name: {Name}" +
                 $"\nInstructor Id: {InstructorId}" +
                 $"\nInstructor Specialization: {Specialization}";
        return result;
    }
}
class Course(int courseId, string title, Instructor instructor)
{
    public int CourseId = courseId;
    public string Title = title;
    public Instructor Instructor = instructor;

    public string PrintDetails()
    {
        string result = string.Empty;
        result = $"Title: {Title}" +
                 $"\nCourse Id : {CourseId}" +
                 $"\nInstructor Info\n{Instructor.PrintDetails()}";
        return result;
    }
};
class Student(int studentId, string name, int age)
{
    public int StudentId = studentId;
    public string Name = name;
    public int Age = age;
    public List<Course> Courses = [];
    public bool Enroll(Course course)
    {
        if (isEnrolled(course))
            return false;

        Courses.Add(course);
        return true;
    }
    public bool isEnrolled(Course course)//check if student enrolle in the course or not 
    {
        foreach (Course item in Courses)
        {
            if (item.CourseId == course.CourseId)
                return true;
        }
        return false;
    }

    public string PrintDetails()
    {
        string info = string.Empty;
        if (Courses.Count == 0)
            info = "No Courses enrolled yet";
        else
        {
            info = string.Empty;
            foreach (Course item in Courses)
            {
                info += item.Title;
            }
        }
        string result = string.Empty;
        result = $"Student Name: {Name}" +
                 $"\nStudent Id : {StudentId}" +
                 $"\nStudent age : {Age}" +
                 $"\nCourses \n{info}";
        return result;

    }
}
class School
{
    public List<Instructor> instructors = [];
    public List<Student> students = [];
    public List<Course> courses = [];

    //student 
    public bool AddStudent(Student student)
    {
        if (students.Count == 0)
        {
            students.Add(student);
            return true;
        }

        if (FindStudentById(student.StudentId) == null) // student not exist
        {
            students.Add(student);
            return true;
        }
        return false;
    }
    public Student? FindStudentById(int studentId)
    {
        if (students.Count > 0)
        {
            foreach (Student student in students)
            {
                if (student.StudentId == studentId)
                    return student;
            }
        }
        return null;
    }
    public Student? FindStudentByName(string name)
    {
        if (students.Count > 0)
        {
            foreach (Student student in students)
            {
                if (student.Name.ToLower() == name.ToLower())
                    return student;
            }
        }
        return null;
    }
    public void printAllStudents()
    {
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine(i + 1);
            Console.Write($"{students[i].PrintDetails()}");
            Console.WriteLine("\n===================");
        }
    }
    public bool EnrollStudentInCourse(int studentId, int courseId)
    {
        /*
         * check if student is exist 
         * check if course is exist
         * applay the function
         */

        Student? student = FindStudentById(studentId);
        Course? course = FindCourseById(courseId);

        if (student != null && course != null)
        {
            student.Enroll(course);
            return true;
        }

        return false;
    }
    public bool UpdateStudent(int id) 
    {
        Student? student = FindStudentById(id);
        // when i make update for any thing in the student it will appears in the original object ;=
        if (student != null) 
        {

            Console.WriteLine("1- id");
            Console.WriteLine("2- name");
            Console.WriteLine("3- age");
            Console.Write("Choose what you want to update: ");
            string choise = Console.ReadLine();
            switch (choise)
            {
                case "1":
                    Console.Write("Enter new id: ");
                    int newId = Convert.ToInt32(Console.ReadLine());
                    student.StudentId = newId;
                    break;
                case "2":
                    Console.Write("Enter new name: ");
                    string newName= Console.ReadLine();
                    student.Name = newName;
                    break;
                case "3":
                    Console.Write("Enter new age: ");
                    int newAge = Convert.ToInt32(Console.ReadLine());
                    student.Age= newAge;
                    break;
            }
            return true;
        }
        return false;
 
    }
    public bool DeleteStudent(int id)
    {
        Student? student = FindStudentById(id);
        if (student != null)
        {
            students.Remove(student); //new thing to me 
            return true;
        }
        return false;
    }

    //course
    public bool AddCourse(Course course)
    {
        if (courses.Count == 0)
        {
            courses.Add(course);
            return true;
        }


        if (FindCourseById(course.CourseId) == null) // course not exist
        {
            courses.Add(course);
            return true;
        }

        return false;
    }
    public Course? FindCourseById(int courseId)
    {
        if (courses.Count > 0)
        {
            foreach (Course course in courses)
            {
                if (course.CourseId == courseId)
                    return course;
            }
        }
        return null;
    }
    public Course? FindCourseByName(string name)
    {
        if (courses.Count > 0)
        {
            foreach (Course course in courses)
            {
                if (course.Title.ToLower() == name.ToLower())
                    return course;
            }
        }
        return null;
    }
    public void printAllCourses()
    {
        Console.WriteLine("\n******** Courses **********\n");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine(i + 1);
            Console.Write($"{courses[i].PrintDetails()}");
            Console.WriteLine("\n===================");
        }
    }


    //instructor
    public bool AddInstructor(Instructor instructor)
    {
        if (instructors.Count == 0)
        {
            instructors.Add(instructor);
            return true;
        }

        if (FindInstructorById(instructor.InstructorId) == null) // course not exist
        {
            instructors.Add(instructor);
            return true;
        }

        return false;
    }
    public Instructor? FindInstructorById(int instructorId)
    {
        if (instructors.Count > 0)
        {
            foreach (Instructor instructor in instructors)
            {
                if (instructor.InstructorId == instructorId)
                    return instructor;
            }
        }
        return null;
    }
    public Instructor? FindInstructorByName(string name)
    {
        if (instructors.Count > 0)
        {
            foreach (Instructor instructor in instructors)
            {
                if (instructor.Name.ToLower() == name.ToLower())
                    return instructor;
            }
        }
        return null;
    }
    public void printAllInstructors()
    {
        Console.WriteLine("\n******** Instructors **********\n");
        for (int i = 0; i < instructors.Count; i++)
        {
            Console.WriteLine(i + 1);
            Console.Write($"{instructors[i].PrintDetails()}");
            Console.WriteLine("\n===================");

        }
    }

    public void menu()
    {
        Console.WriteLine("1. Add Student");
        Console.WriteLine("2. Add Instructor");
        Console.WriteLine("3. Add Course");
        Console.WriteLine("4. Enroll Student in Course");
        Console.WriteLine("5. Show All Students");
        Console.WriteLine("6. Show All Courses");
        Console.WriteLine("7. Show All Instructors");
        Console.WriteLine("8. Find the student by id");
        Console.WriteLine("9. Find the student by name");
        Console.WriteLine("10.Find the course by id");
        Console.WriteLine("11.Find the course by name");
        Console.WriteLine("12.Find the Instructor by id");
        Console.WriteLine("13.Find the Instructor by name");
        Console.WriteLine("14.Check if the student enrolled in specific course");
        Console.WriteLine("15.Return the instructor name by course name");
        Console.WriteLine("16.Update student");
        Console.WriteLine("17.Delete student");
        Console.WriteLine("0. Exit");
    }
}
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
