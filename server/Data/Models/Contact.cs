

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Mail.Data.Models;

public class Contact
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    [Column("email")]
    public string? Email { get; set; }
}