using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;

[Collection("Joe Signs Up")]
public class Joe_Signs_Up_Successfully{
    
  [Fact]
  public void When_joe_registers_he_is_given_a_key_and_not_subbed()
  {
    var res = new ContactSignupCommand("joe@test.com", "Joe").Execute();
    Assert.Equal(1, res.Inserted);
    Assert.NotNull(res.Data.Key);
    Assert.False(res.Data.Subscribed);
  }
  [Fact]
  //HACK: I hate writing tests this way but I need things done serially
  //and Xunit won't do that
  public void Joe_confirms_he_is_subbed_then_unsubs()
  {
    dynamic joe = new Query().First("mail.contacts", new{
      email = "joe@test.com"
    });
    var res = new ContactOptinCommand(joe.key).Execute();
    Assert.Equal(1, res.Updated);
    Assert.True(res.Data.Success);

    joe = new Query().First("mail.contacts", new{
      email = "joe@test.com"
    });
    Assert.True(joe.subscribed);

    res = new ContactOptOutCommand("joe@test.com").Execute();
    Assert.Equal(1, res.Updated);
    Assert.True(res.Data.Success); 

    joe = new Query().First("mail.contacts", new{
      email = "joe@test.com"
    });
    Assert.False(joe.subscribed);
  }
}
[Collection("Jack Signs Up But Is Already here")]
public class Jack_already_exists{
    
  [Fact]
  public void When_jack_registers_he_we_dont_throw_but_return_false_success()
  {
    var res = new ContactSignupCommand("test@test.com", "Jack").Execute();
    Assert.Equal(0, res.Inserted);
    Assert.False(res.Data.Success);
  }
}