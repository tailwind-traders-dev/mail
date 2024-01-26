using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Queries;
using Tailwind.Mail.Models;

[Collection("Joe Signs Up")]
public class Joe_Signs_Up_Successfully{
    
  [Fact]
  public void When_jim_registers_he_is_given_a_key_and_not_subbed()
  {

    var joe = new Contact{
      Email = "jim@test.com",
      Name="Jim"
    };
    var res = new ContactSignupCommand(joe).Execute();
    Assert.Equal(1, res.Inserted);
    Assert.NotNull(res.Data.Key);
    Assert.False(res.Data.Subscribed);
  }
  [Fact]
  //HACK: I hate writing tests this way but I need things done serially
  //and Xunit won't do that
  public void Joe_confirms_he_is_subbed_then_unsubs()
  {
    var joe = new Contact{
      Email = "joe@test.com",
      Name="Joe"
    };
    var res = new ContactSignupCommand(joe).Execute();
    joe = new ContactQuery{Email="joe@test.com"}.First();
    res = new ContactOptinCommand(joe).Execute();
    Assert.Equal(1, res.Updated);
    Assert.True(res.Data.Success);

    joe = new ContactQuery{Email="joe@test.com"}.First();
    Assert.True(joe.Subscribed);

    res = new ContactOptOutCommand(joe.Key).Execute();
    Assert.Equal(1, res.Updated);
    Assert.True(res.Data.Success); 

    joe = joe = new ContactQuery{Email="joe@test.com"}.First();
    Assert.False(joe.Subscribed);
  }
}
[Collection("Jack Signs Up But Is Already here")]
public class Jack_already_exists{
    
  [Fact]
  public void When_jack_registers_he_we_dont_throw_but_return_false_success()
  {
    var jack = new Contact{
      Email = "test@test.com",
      Name="Jack"
    };
    var res = new ContactSignupCommand(jack).Execute();
    Assert.Equal(0, res.Inserted);
    Assert.False(res.Data.Success);
  }
}