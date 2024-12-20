﻿using System;
using System.Collections.Generic;

namespace ElectronicJournalsApi.Models;

public partial class Subject
{
    public int IdSubject { get; set; }

    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    public sbyte Duration { get; set; }

    public sbyte LessonLenght { get; set; }

    public sbyte LessonsCount { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
