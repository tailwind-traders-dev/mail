using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Mail.Data.Models;


public class Broadcast
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("slug")]
    public string Slug { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    
    [Column("reply_to")]
    public string ReplyTo { get; set; } = "noreply@tailwindtraders.dev";
    public Email? Email { get; set; }

}