using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicJournalsApi.Models;

public partial class User
{
    public int IdUsers { get; set; }

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    [Display(Name = "Логин")]
    public string Login { get; set; } = null!;

    [Display(Name = "Пароль")]
    public byte[] Password { get; set; } = null!;

    [Display(Name = "Номер телефона")]
    public string? Phone { get; set; }

    [Display(Name = "Дата рождения")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Роль")]
    public string Role { get; set; } = null!;

    public bool IsDelete { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Subject> IdSubjects { get; set; } = new List<Subject>();
}
