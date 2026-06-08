using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Management__task_3_
{
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
}
