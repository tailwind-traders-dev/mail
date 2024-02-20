using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Markdig;
using Markdig.Syntax;
using Tailwind.Mail.Models;
using Dapper;

namespace Tailwind.Mail.Tests;

//probably end up mocking this at some point
//for now, just use the test db


[Collection("Jill Queues a Broadcast")]
public class Jill_Queues_a_Broadcast:TestBase
{
  public MarkdownEmail _doc { get; set; }
  public Jill_Queues_a_Broadcast()
  {
        _doc = MarkdownEmail.FromString(@"
---
Subject: Test Test Jack
Summary: This is a test broadcast
---
## Test Test\n\nThis is a test broadcast
");
  }
  [Fact]
  public void A_When_a_broadcast_is_created_for_everyone_there_should_be_100_messages()
  {
    //this tag id will exist if you run the seed.sql file which is what you should be doing
    //the Makefile does it for you just run make
    //That's right. I said Make.
    for(var i = 0; i < 100; i++){
      Conn.Insert(new Contact{
        Name = "Test User",
        Email = $"test-zz{i}@test.com",
        Subscribed = true
      });
    }
    var result = new CreateBroadcast(_doc).Execute(Conn);
    Console.WriteLine(result.Inserted);
    Assert.Equal(true, result.Data.Notified);
    Assert.Equal(true, result.Inserted >= 100);
  }
  [Fact]
  public void A_When_a_broadcast_is_created_for_a_tag_there_should_be_10000_messages()
  {
    var tagId = Conn.Insert(new Tag{
      Name = "Test Tag",
      Slug = "test-tag-z"
    });
    for(var i = 0; i < 50; i++){
      var id = Conn.Insert(new Contact{
        Name = "Test User",
        Email = $"test-yy{i}@test.com",
        Subscribed = true
      });
      Conn.Execute("insert into mail.tagged (contact_id, tag_id) values (@contact_id, @tag_id)", new{
        contact_id = id,
        tag_id = tagId
      });
    }

    _doc = MarkdownEmail.FromString(@"
---
Subject: Test Test Jill
Summary: This is a test broadcast
SendToTag: test-tag
---
## Test Test\n\nThis is a test broadcast
");

    var result = new CreateBroadcast(_doc).Execute(Conn);
    Console.WriteLine(result.Inserted);
    Assert.True(result.Inserted >= 10);
  }

} 
