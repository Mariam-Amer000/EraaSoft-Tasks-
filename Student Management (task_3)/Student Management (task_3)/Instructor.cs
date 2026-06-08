using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Management__task_3_
{
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
}
