using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Tailwind.Mail.Data;

public class Db: DbContext
{
  public DbSet<Contact>? Contacts { get; set; }
  public DbSet<Email>? Emails { get; set; }
  public DbSet<Sequence>? Sequences { get; set; }
  public DbSet<Tag>? Tags { get; set; }
  public DbSet<Message>? Messages { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<Contact>().ToTable("contacts",schema: "mail");
      modelBuilder.Entity<Email>().ToTable("emails",schema: "mail");
      modelBuilder.Entity<Sequence>().ToTable("sequences",schema: "mail");
      modelBuilder.Entity<Tag>().ToTable("tags",schema: "mail");

      modelBuilder.Entity<Sequence>().HasMany(s => s.Emails).WithOne(e => e.Sequence);
      modelBuilder.Entity<Email>().HasOne(e => e.Sequence).WithMany(s => s.Emails);
      modelBuilder.Entity<Email>().HasMany(e => e.Messages).WithOne(e => e.Email);
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