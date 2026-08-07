using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Models;

[Table(name:"Students")]
internal class Student
{
    public int StudentId { get; set; }
    [Unicode]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Unicode(false)]
    [MaxLength(10)]
    public  string? PhoneNumber { get; set; }
    public DateTime RegisteredOn { get; set; }
    public DateOnly? Birthday { get; set; }
    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    public ICollection<Homework> Homeworks { get; set; }= new List<Homework>();
}
