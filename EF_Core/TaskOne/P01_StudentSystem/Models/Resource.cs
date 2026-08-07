using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static P01_StudentSystem.Enums;

namespace P01_StudentSystem.Models;

[Table(name: "Resources")]
internal class Resource
{
    public int ResourceId { get; set; }
    [Unicode]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Unicode(false)]
    public string Url { get; set; }
    public ResourceType ResourceType { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
}
