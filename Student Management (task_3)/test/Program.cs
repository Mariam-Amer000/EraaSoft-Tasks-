namespace test;

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
    public bool EnrollStudentInCourse(int studentId, int courseId)
    {
        /*
         * check if student is exist 
         * check if course is exist
         * applay the function
         */

        Student student = FindStudentById(studentId);
        Course course = FindCourseById(courseId);

        if (student != null && course != null)
        {
            student.Enroll(course);
            return true;
        }

        return false;
    }

    public void printAllInstructors()
    {
        Console.WriteLine("\n******** Instructors **********\n");
        for (int i = 0; i < instructors.Count; i++)
        {
            Console.Write($"{instructors[i].PrintDetails()}");
            Console.WriteLine("\n===================");

        }
    }
    public void printAllStudents()
    {
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine(i+1);
            Console.Write($"{students[i].PrintDetails()}");
            Console.WriteLine("\n===================");
        }
    }
    public void printAllCourses()
    {
        Console.WriteLine("\n******** Courses **********\n");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.Write($"{courses[i].PrintDetails()}");
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
        Console.WriteLine("9. Fine the course by id");
        Console.WriteLine("10. Check if the student enrolled in specific course");
        Console.WriteLine("11. Return the instructor name by course name");
        Console.WriteLine("0. Exit");
    }
}
internal class Program
{
    static void Main(string[] args)
    {
        School school = new School();
        school.AddStudent(new(1, "mariam", 20));
        school.AddStudent(new(2, "amer", 40));
        school.AddStudent(new(3, "salma", 20));
        school.AddStudent(new(4, "yasmeen", 9));
        school.AddStudent(new(5, "moaaz", 18));
        school.printAllStudents();


        Instructor instructor1 = new(1, "mariam", ".net");
        Instructor instructor2 = new(2, "amer", "c#");
        Instructor instructor3 = new(3, "salme", "cs");
        Instructor instructor4 = new(4, "moaaz", "network");
        Instructor instructor5 = new(5, "amer", "c#");

        school.AddInstructor(instructor1);
        school.AddInstructor(instructor2);
        school.AddInstructor(instructor3);
        school.AddInstructor(instructor4);
        school.AddInstructor(instructor5);
        //school.printAllInstructors();


        school.AddCourse(new(1, "101.net", instructor1));
        school.AddCourse(new(2, "101C#", instructor2));
        school.AddCourse(new(3, "101CS", instructor3));
        school.AddCourse(new(4, "101Network", instructor4));
        //school.printAllCourses();



        //Instructor instructor=school.FindInstructorById(2);
        //Console.WriteLine(instructor.PrintDetails());

        //Course course=school.FindCourseById(3);
        //Console.WriteLine(course.PrintDetails());

        //school.EnrollStudentInCourse(1, 1);

        //Student student = school.FindStudentById(1);
        //Console.WriteLine(student.PrintDetails());

        //Student student2 = school.FindStudentByName("mariam");
        //Console.WriteLine(student2.PrintDetails());




    }
}
