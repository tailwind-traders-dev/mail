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
      modelBuilder.Entity<Broadcast>().ToTable("broadcasts",schema: "mail");

      
      // modelBuilder.Entity<Email>().HasMany(e => e.Messages).WithOne(e => e.Email);

      modelBuilder.Entity<Email>().Property<int?>("SequenceId").HasColumnName("sequence_id");
      modelBuilder.Entity<Broadcast>().Property<int?>("EmailId").HasColumnName("email_id");

      // modelBuilder.Entity<Broadcast>().HasMany(e => e.Recipients)
      //                               .WithMany(e => e.Broadcasts)
      //                               .UsingEntity<Dictionary<string, object>>(
      //                                   "broadcast_recipients",
      //                                   r => r.HasOne<Contact>().WithMany().HasForeignKey("contact_id"),
      //                                   l => l.HasOne<Broadcast>().WithMany().HasForeignKey("broadcast_id")
      //                               );

      // modelBuilder.Entity<Message>().Property<int?>("EmailId").HasColumnName("email_id");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {

    // var config= new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    // .AddJsonFile("appsettings.json")
    // .Build();
    // var connectionString = config.GetConnectionString("MailApp");
    DotEnv.Load();
    
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    optionsBuilder.UseNpgsql(connectionString);//.LogTo(Console.WriteLine, LogLevel.Information);
  }
}