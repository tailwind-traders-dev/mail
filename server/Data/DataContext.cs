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
      modelBuilder.Entity<Message>().ToTable("messages",schema: "mail");

      
      // modelBuilder.Entity<Email>().HasMany(e => e.Messages).WithOne(e => e.Email);
      modelBuilder.Entity<Email>().Property<int?>("SequenceId").HasColumnName("sequence_id");
      modelBuilder.Entity<Message>().Property<int?>("EmailId").HasColumnName("email_id");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {

    // var config= new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    // .AddJsonFile("appsettings.json")
    // .Build();
    // var connectionString = config.GetConnectionString("MailApp");
    DotEnv.Load();
    
    var host = Environment.GetEnvironmentVariable("DB_HOST");
    var user = Environment.GetEnvironmentVariable("DB_USER");
    var pw = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var db = Environment.GetEnvironmentVariable("DB_NAME");
    var connectionString = $"Host={host};Username={user};Password={pw};Database={db}";

    optionsBuilder.UseNpgsql(connectionString).LogTo(Console.WriteLine, LogLevel.Information);;
  }
}