using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Mail.Data.Models;

public class Tag
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    [Column("slug")]
    public string? Slug { get; set; }
    
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}