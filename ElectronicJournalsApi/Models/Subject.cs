using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElectronicJournalsApi.Models;

public partial class Subject
{
    public int IdSubject { get; set; }

    [Display(Name = "Название")]
    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    [Display(Name = "Длительность")]
    public sbyte Duration { get; set; }

    [Display(Name = "Академических часов")]
    public sbyte LessonLength { get; set; }

    [Display(Name = "Количество уроков")]
    public sbyte LessonsCount { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
