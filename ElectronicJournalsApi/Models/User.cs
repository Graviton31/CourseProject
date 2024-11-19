using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class User
{
    public int IdUsers { get; set; }

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Subject> IdSubjects { get; set; } = new List<Subject>();
}
