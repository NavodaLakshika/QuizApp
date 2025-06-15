using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!dbContext.Questions.Any())
    {
        var questions = new List<Question>();

        void AddQuestion(string text, string[] options, int correctIndex)
        {
            var correctId = Guid.NewGuid();
            var optionList = options.Select((opt, i) => new Option
            {
                Id = i == correctIndex ? correctId : Guid.NewGuid(),
                Text = opt
            }).ToList();

            questions.Add(new Question
            {
                Id = Guid.NewGuid(),
                Text = text,
                Options = optionList,
                CorrectOption = correctId
            });
        }

        AddQuestion("What is the capital of Sri Lanka?", new[] { "Colombo", "Galle", "Sri Jayawardenepura Kotte", "Kandy" }, 2);
        AddQuestion("Which planet is known as the Red Planet?", new[] { "Mars", "Earth", "Venus", "Jupiter" }, 0);
        AddQuestion("Who wrote 'Romeo and Juliet'?", new[] { "William Shakespeare", "Leo Tolstoy", "Charles Dickens", "Mark Twain" }, 0);
        AddQuestion("What is the boiling point of water?", new[] { "50°C", "100°C", "75°C", "120°C" }, 1);
        AddQuestion("Which language is primarily spoken in Brazil?", new[] { "Portuguese", "Spanish", "English", "French" }, 0);
        AddQuestion("What is H2O commonly known as?", new[] { "Oxygen", "Hydrogen", "Water", "Salt" }, 2);
        AddQuestion("Who is the current President of the United States? (2024)", new[] { "Joe Biden", "Donald Trump", "Barack Obama", "George Bush" }, 0);
        AddQuestion("Which ocean is the largest?", new[] { "Pacific Ocean", "Atlantic Ocean", "Indian Ocean", "Arctic Ocean" }, 0);
        AddQuestion("Which country is famous for the Great Wall?", new[] { "China", "Japan", "India", "Thailand" }, 0);
        AddQuestion("How many continents are there on Earth?", new[] { "5", "6", "7", "8" }, 2);

        foreach (var question in questions)
        {
            foreach (var option in question.Options)
            {
                option.QuestionId = question.Id;
            }
        }

        dbContext.Questions.AddRange(questions);
        dbContext.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quiz}/{action=Index}/{id?}");

app.Run();
