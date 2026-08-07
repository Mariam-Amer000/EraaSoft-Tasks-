using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Models;

[PrimaryKey(nameof(StudentId), nameof(CourseId))]
internal class StudentCourse
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public Student Student {  get; set; }
}
