using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tailwind.Mail.Data;

public class Db: DbContext
{

  public DbSet<Contact> Contacts { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<Contact>().ToTable("contacts");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    //var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MailApp"].ConnectionString;
    //var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
    //var connectionString = "Host=localhost;Database=tailwind;Username=rob;";
    var config= new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    //.AddJsonFile($"appsettings.{envName}.json", optional: false)
    .Build();
    var connectionString = config.GetConnectionString("MailApp");
Console.WriteLine(connectionString);
    optionsBuilder.UseNpgsql(connectionString);
  }
}