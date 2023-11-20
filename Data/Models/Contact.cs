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
    [Column("subscribed")]
    public bool Subscribed { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public static IList<Contact> ByTag(Tag tag){
      var _db = new Db();
      var res =  _db.Contacts.Where(c => c.Subscribed).Where(c => c.Tags.Contains(tag)).ToList();
      _db.Dispose();
      return res;
    }
    public static IList<Contact> AllSubscribed(Tag tag){
      var _db = new Db();
      var res =  _db.Contacts.Where(c => c.Subscribed).ToList();
      _db.Dispose();
      return res;
    }



}