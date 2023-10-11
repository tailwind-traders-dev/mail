using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Mail.Data.Models;

public class Email
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    [Column("slug")]
    public string? Slug { get; set; }
    [Column("subject")]
    public string? Subject { get; set; }
    [Column("markdown")]
    public string? Markdown { get; set; }
    [Column("delay_hours")]  
    public int DelayHours { get; set; }
    public Sequence Sequence { get; set; }
    public ICollection<Message> Messages { get; }
}