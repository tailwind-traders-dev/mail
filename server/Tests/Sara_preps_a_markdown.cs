using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Markdig;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Tests;

[Collection("Markdown")]
public class Sara_preps_a_markdown_email:TestBase
{
  public string? _markdown { get; set; }
  public MarkdownEmail _doc { get; set; }
  public Broadcast _broadcast { get; set; }
  public Sara_preps_a_markdown_email()
  {
    //get the markdown file in the helpers directory
    //I suck at .NET... how do I get the path to the test file?
    var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Helpers", "test_broadcast.md");
    _doc = new MarkdownEmail(path);
  }
  [Fact]
  public void Given_a_markdown_file()
  {
    Assert.NotNull(_doc);
  }
  [Fact]
  public void A_valid_markdown_doc_is_produced()
  {
    Assert.NotNull(_doc.Html);
    Assert.NotNull(_doc.Data);
    Assert.Equal("Test Broadcast", _doc.Data.Subject);
    Assert.Equal("test-broadcast", _doc.Data.Slug);
  }
  [Fact]
  public void A_valid_email_is_produced()
  {
    var email = new Email(_doc);
    Assert.NotNull(email);
    Assert.Equal("Test Broadcast", email.Subject);
    Assert.Equal("test-broadcast", email.Slug);
    Assert.NotNull(email.Html);
  }
  [Fact]
  public void A_valid_broadcast_is_produced()
  {
    var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Helpers", "test_broadcast.md");
    _broadcast = new Broadcast(path);
    Assert.NotNull(_broadcast);
    Assert.NotNull(_broadcast.Email);
    Assert.Equal("Test Broadcast", _broadcast.Email.Subject);
    Assert.Equal("test-broadcast", _broadcast.Email.Slug);
    Assert.NotNull(_broadcast.Email.Html);
    Assert.Equal("pending", _broadcast.Status);
    Assert.Equal("Test Broadcast", _broadcast.Name);
  }
  [Fact]
  public void A_valid_broadcast_is_saved_to_the_db()
  {
    var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Helpers", "test_broadcast.md");
    _broadcast = new Broadcast(path);
    var cmd = new CreateBroadcast(_broadcast);
    var res = cmd.Execute();
    Assert.True(res.Inserted >= 10000);
    Assert.Equal(true, res.Data.Notified);
    
  }
  [Fact]
  public void A_valid_tagged_broadcast_is_saved_to_the_db()
  {
    var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Helpers", "test_broadcast_tagged.md");
    _broadcast = new Broadcast(path);
    var cmd = new CreateBroadcast(_broadcast);
    var res = cmd.Execute();
    Console.WriteLine(res.Inserted);
    Assert.True(res.Inserted >= 10000);
    Assert.Equal(true, res.Data.Notified);

  }
}