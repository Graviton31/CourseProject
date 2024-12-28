using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class Journal
{
    public int IdJournal { get; set; }

    public DateOnly? LessonDate { get; set; }

    public int IdGroup { get; set; }

    public int IdStudent { get; set; }

    public int? IdUnvisitedStatus { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual Student IdStudentNavigation { get; set; } = null!;

    public virtual UnvisitedStatus IdUnvisitedStatusNavigation { get; set; } = null!;
}
