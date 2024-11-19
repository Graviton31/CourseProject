using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public string Name { get; set; } = null!;

    public sbyte? StudentCount { get; set; }

    public string Classroom { get; set; } = null!;

    public int IdUsers { get; set; }

    public int IdSubject { get; set; }

    public virtual Subject IdSubjectNavigation { get; set; } = null!;

    public virtual User IdUsersNavigation { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual ICollection<ScheduleChange> ScheduleChanges { get; set; } = new List<ScheduleChange>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Student> IdStudents { get; set; } = new List<Student>();
}
