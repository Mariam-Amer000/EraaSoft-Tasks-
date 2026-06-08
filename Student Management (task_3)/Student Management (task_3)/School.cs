using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Management__task_3_
{
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
                        string newName = Console.ReadLine();
                        student.Name = newName;
                        break;
                    case "3":
                        Console.Write("Enter new age: ");
                        int newAge = Convert.ToInt32(Console.ReadLine());
                        student.Age = newAge;
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
}
}
