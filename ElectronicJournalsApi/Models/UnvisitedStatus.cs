using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class UnvisitedStatus
{
    public int IdUnvisitedStatus { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}
