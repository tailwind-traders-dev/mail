using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Api.Admin;

public class BulkTagRequest{
  public string Tag { get; set; }
  public IEnumerable<string> Emails { get; set; }
}

public class BulkOperationRoutes{
  private BulkOperationRoutes()
  {
    
  }
  public static void MapRoutes(IEndpointRouteBuilder app)
  {
    app.MapPost("/admin/bulk/contacts/tag", ([FromBody] BulkTagRequest request, [FromServices] IDb db) => {
      using var conn = db.Connect();
      var tx = conn.BeginTransaction();
      var updated =0;
      var inserted = 0;
      try{
        var tag = conn.GetList<Tag>(new {name=request.Tag}, tx).FirstOrDefault();
        if(tag == null){
          tag = new Tag(request.Tag);
          tag.ID = conn.Insert(tag, tx);
        }
        foreach(var email in request.Emails){
          var contact = conn.GetList<Contact>(new {Email = email}).FirstOrDefault();
          if(contact == null){
            //create the contact
            contact = new Contact{
              Email = email
            };
            inserted++;
            contact.ID = conn.Insert(contact, tx);
          }else{
            updated++;
          }
          var sql = @"
          insert into mail.tagged (contact_id, tag_id) 
          values (@contactId, @tagId) 
          on conflict do nothing"; //upsert if already tagged
          conn.Execute(sql, new {contactId=contact.ID, tagId=tag.ID}, tx);
        }
        tx.Commit();
        return new CommandResult{Inserted = inserted, Updated = updated};
      }catch(Exception e){
        tx.Rollback();
        return new CommandResult{Data = new {error = e.Message}};
      }

    }).WithOpenApi(op => {
      op.Summary = "Tag a set of contacts";
      op.Description = "Tag a set of contacts";
      return op;
    }).Produces<CommandResult>()
    .Produces(500);
  }
}