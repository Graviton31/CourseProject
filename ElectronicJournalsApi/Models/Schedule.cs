using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class Schedule
{
    public int IdSchedule { get; set; }

    public string WeekDay { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int IdGroup { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;
}
