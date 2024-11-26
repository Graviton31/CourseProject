using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElectronicJournalsApi.Models;

public partial class UserResponse
{
    [Display(Name = "Логин")]
    [Required(ErrorMessage = "Ведите логин")]
    public string Login { get; set; } = null!;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Ведите пароль")]
    public string Password { get; set; } = null!;
}
