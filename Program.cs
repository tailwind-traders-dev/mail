DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
 
//this serves our Svelte file
//app.UseDefaultFiles().UseStaticFiles();
//here's a comment to unstkick thing
var app = builder.Build();

//lock this down as needed
// app.UseCors(builder => builder
//  .AllowAnyOrigin()
//  .AllowAnyMethod()
//  .AllowAnyHeader()
// );

//load the routes
app.MapGet("/", (HttpContext context) => 
{
  var links = new List<Dictionary<string,string>>();
  links.Add(new Dictionary<string,string>(){
    {"rel","self"},
    {"href","/"}
  });
  return Results.Json(links);
});

app.Run();

//this is for tests
public partial class Program { }