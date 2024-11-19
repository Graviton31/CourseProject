using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class Student
{
    public int IdStudent { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Phone { get; set; } = null!;

    public string ParentPhone { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual ICollection<Group> IdGroups { get; set; } = new List<Group>();
}
