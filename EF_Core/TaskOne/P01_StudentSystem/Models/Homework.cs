using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static P01_StudentSystem.Enums;

namespace P01_StudentSystem.Models;

[Table(name: "Homeworks")]
internal class Homework
{
    public int HomeworkId { get; set; }
    [Unicode(false)]
    public string Content { get; set; }
    public ContentType ContentType { get; set; }
    public DateTime SubmissionTime { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
}
