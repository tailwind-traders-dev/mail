using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;
//load up the config from env and appsettings
Viper.Config();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "0.0.1",
        Title = "Tailwind Traders Mail Services API",
        Description = "Transactional and bulk email sending services for Tailwind Traders.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

//this serves our Svelte file
//app.UseDefaultFiles().UseStaticFiles();
//here's a comment to unstkick thing
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

//public routes
app.MapGet("/about", () => "Tailwind Traders Mail Services API").WithOpenApi(op => {
  op.Summary = "Information about the API";
  op.Description = "This is the API for the Tailwind Traders Mail Services API";
  return op;
});

app.MapGet("/unsubscribe/{key}", (string key) => {
  var cmd = new ContactOptOutCommand(key);
  var result = cmd.Execute();
  return result;
}).WithOpenApi(op => {
  op.Summary = "Unsubscribe from the mailing list";
  op.Description = "This is the API for the Tailwind Traders Mail Services API";
  op.Parameters[0].Description = "This is the contact's unique key";
  return op;
});


app.MapPost("/signup", ([FromBody] SignUpRequest req) => {
  var contact = new Contact{
    Email = req.Email,
    Name = req.Name
  };
  var cmd = new ContactSignupCommand(contact);
  var result = cmd.Execute();
  return result;
}).WithOpenApi(op => {
  op.Summary = "Sign up for the mailing list";
  op.Description = "This is the form endpoint for signing up for the mail list";
  op.RequestBody.Description = "This is the contact's information";
  return op;
});

app.Run();

//this is for tests
public partial class Program { }