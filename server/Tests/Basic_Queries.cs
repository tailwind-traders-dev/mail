using Xunit;
using Tailwind.Mail.Queries;
using Tailwind.Mail.Commands;
namespace Tailwind.Mail.Tests;

public class Basic_Queries:TestBase
{
  public Basic_Queries()
  {
    var cmd = new Command();
    var vals =     new Dictionary<string,object>{
      {"id", 999},
      {"name", "Jill Jones"},
      {"email", "jill@test.com"}
    };
    cmd.Delete("mail.contacts", vals);
    cmd.Insert("mail.contacts", vals);
  }
  [Fact]
  public async Task A_contact_is_found_using_first()
  {
    var qry = new Query();
    var res = await qry.First("mail.contacts", new Dictionary<string,object>{{"id", 999}});

    Assert.Equal("Jill Jones", res.name);  

  }
  [Fact]
  public async Task A_list_is_returned_using_select()
  {
    var qry = new Query();
    var res = await qry.Select("mail.contacts");
    Assert.True(res.Count > 0);
  }
  [Fact]
  public async Task A_list_is_returned_using_straight_sql()
  {
    var qry = new Query();
    var res = await qry.Run("select * from mail.contacts where id > 0");
    Assert.True(res.Count > 0);
  }
  [Fact]
  public async Task A_list_is_returned_using_straight_sql_with_params()
  {
    var qry = new Query();
    var res = await qry.Run("select * from mail.contacts where id > @id", new Dictionary<string,object>{{"id", 0}});
    Assert.True(res.Count > 0);
  }
}