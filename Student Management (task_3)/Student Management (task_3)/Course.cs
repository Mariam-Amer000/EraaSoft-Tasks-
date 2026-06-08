using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Management__task_3_
{
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
    }
}
