using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class StaffLog
{
    public int Id { get; set; }

    public int? StaffId { get; set; }

    public string? StaffFirstName { get; set; }

    public string? StaffLastName { get; set; }

    public string? StaffPhoneNumber { get; set; }

    public DateTime? DeleatedAt { get; set; }
}
