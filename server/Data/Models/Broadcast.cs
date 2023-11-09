using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Mail.Data.Models;


public class Broadcast
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("slug")]
    public string? Slug { get; set; }
    [Column("status")]
    public string Status { get; set; } = "pending";
    [Column("name")]
    public string? Name { get; set; }
    [Column("message_count")]
    public int MessageCount { get; set; } = 0;
    
    [Column("reply_to")]
    public string ReplyTo { get; set; } = "noreply@tailwindtraders.dev";
    public Email? Email { get; set; }


    //factory goodies
    public static Broadcast Create(string slug, string subject, string markdown, int delayHours=0){
        var email = new Email{
            Slug = slug,
            Subject = subject,
            Markdown = markdown,
            DelayHours = delayHours
        };
        email.Render();
        var broadcast = new Broadcast{
            Slug = slug,
            Name = subject,
            ReplyTo = Environment.GetEnvironmentVariable("DEFAULT_FROM"),
            Email = email,
        };
        return broadcast;
    }


}