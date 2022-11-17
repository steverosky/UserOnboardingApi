using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserOnboardingApi.EFCore;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<EF_DataContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Ef_Postgres_DB")));

//var emailConfig = builder.Configuration
//        .GetSection("EmailConfiguration")
//        .Get<EmailConfiguration>();
//builder.Services.AddSingleton(emailConfig);

//builder.Services.AddScoped<EMailService.IEmailSender, UserOnboardingApi.Model.DbHelper.EmailSender>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
