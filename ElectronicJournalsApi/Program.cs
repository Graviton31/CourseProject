using ElectronicJournalApi.Classes;
using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавьте CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:7070") // URL клиента
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<ElectronicJournalContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("8.0.19-mysql")));

var app = builder.Build();

// Примените миграции и создайте стандартного пользователя
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ElectronicJournalContext>();
        context.Database.Migrate(); // Применение миграций

        var defaultUserConfig = builder.Configuration.GetSection("DefaultUser");
        string login = defaultUserConfig["Login"];
        string password = defaultUserConfig["Password"];

        // Создание стандартного пользователя
        if (!context.Users.Any(u => u.Login == login)) // Проверка на существование
        {
            var defaultUser = new User
            {
                Login = login,
                Password = PasswordHasher.HashPassword(password, login), 
                Role = "администратор"
            };

            context.Users.Add(defaultUser);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        // Обработка ошибок
        Console.WriteLine($"Ошибка при выполнении миграции базы данных: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Примените CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
