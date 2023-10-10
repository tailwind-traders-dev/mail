using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Tailwind.Mail.Data;

public class Db: DbContext
{
  public DbSet<Contact> Contacts { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<Contact>().ToTable("contacts",schema: "mail");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {

    var config= new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();
    var connectionString = config.GetConnectionString("MailApp");
    optionsBuilder.UseNpgsql(connectionString);
  }
}