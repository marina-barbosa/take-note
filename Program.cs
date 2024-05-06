using Microsoft.EntityFrameworkCore;
using Serilog;
using take_note.Domain;
using take_note.Services;

var log = new LoggerConfiguration()
    //.WriteTo.Console()
    .WriteTo.File("serilogs/log.txt", rollingInterval: RollingInterval.Day)
    .Filter.With(new CustomLogFilter())
    .CreateLogger();
log.Information("Inicio da aplicação MXM Sistemas");
Log.Logger = log;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTransient<DatabaseQueryTrackingMiddleware>();

builder.Services.AddSerilog();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MySqlDbContext>(options =>
{
  var defaultConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");
  options.UseMySql(defaultConnectionString, ServerVersion.AutoDetect(defaultConnectionString))
   .EnableSensitiveDataLogging()
   .LogTo(Console.WriteLine, LogLevel.Debug);
});

// builder.Services.AddDbContext<MySqlDbContext>(options =>
// {
//   var defaultConnectionString = builder.Configuration.GetConnectionString("RailWayConnection");
//   options.UseMySql(defaultConnectionString, ServerVersion.AutoDetect(defaultConnectionString))
//    .EnableSensitiveDataLogging()
//    .LogTo(Console.WriteLine, LogLevel.Debug);
// });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ITrackService, TrackService>();

// Configure CORS service
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigins", builder =>
  {
    builder.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
  });
});
// builder.Services.AddCors(options =>
// {
//   options.AddPolicy("AllowSpecificOrigins", builder =>
//   {
//     builder.WithOrigins("https://takenotemxm.netlify.app")
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//   });
// });

var app = builder.Build();

//app.UseMiddleware<DatabaseQueryTrackingMiddleware>();

app.UseCors("AllowSpecificOrigins");


// app.UseSwagger();
// app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

