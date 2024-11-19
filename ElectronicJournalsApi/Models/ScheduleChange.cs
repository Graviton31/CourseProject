using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class ScheduleChange
{
    public int IdScheduleChange { get; set; }

    public DateOnly? NewDate { get; set; }

    public DateOnly? OldDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int IdGroup { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;
}
