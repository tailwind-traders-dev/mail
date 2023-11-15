DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

 
//this serves our Svelte file
//app.UseDefaultFiles().UseStaticFiles();
//here's a comment to unstkick thing
var app = builder.Build();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

//var libPath = Path.Combine(app.Environment.WebRootPath, "Content");

//lock this down as needed
app.UseCors(builder => builder
 .AllowAnyOrigin()
 .AllowAnyMethod()
 .AllowAnyHeader()
);

//load the routes

app.Run();

//this is for tests
public partial class Program { }