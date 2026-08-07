using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Models;

[Table(name: "Courses")]
internal class Course
{
    public int CourseId { get; set; }
    [Unicode]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;
    [Unicode]
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [Precision(10,2)]
    public decimal Price { get; set; }
    public ICollection<StudentCourse> StudentCourses { get; } = new List<StudentCourse>();
    public ICollection<Resource> Resources { get; } = new List<Resource>();
    public ICollection<Homework> Homeworks { get; } = new List<Homework>();
}