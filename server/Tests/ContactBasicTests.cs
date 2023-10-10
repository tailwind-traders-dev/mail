using Xunit;
using Tailwind.Mail.Data;
using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace Tailwind.Mail.Tests;

public class ContactBasicTests {
  Db _db;
  Contact _contact;
  public ContactBasicTests()
  {

    _db = new Db();
    Console.WriteLine("Hi hello");
    _db.Database.EnsureCreated();
    _db.Database.Migrate();
    _contact = new Contact{Email="robconery@gmail.com"};
    _db.Add(_contact);
    var lala = _db.SaveChanges();
    Console.WriteLine(lala);
  }  
  [Fact]
  public void The_contact_is_saved_i_guess()
  {
    //there should be one doc in there
    Assert.Equal(_contact.ID, 1);
  }
}