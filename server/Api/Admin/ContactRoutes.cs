using Microsoft.AspNetCore.Mvc;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Api.Admin;

public class ContactSearchResponse{
  public string? Term { get; set; }
  public IList<Contact> Contacts { get; set; } = new List<Contact>();
}

public class ContactRoutes{
  private ContactRoutes()
  {
    
  }
  public static void MapRoutes(IEndpointRouteBuilder app)
  {
    //CRUD for contacts
    //Tagging
    //Search
    app.MapGet("/admin/contacts/search", ([FromQuery] string term) => {
      //searches by both email and name
      var response = new ContactSearchResponse{Term = term};
      var sql = "select * from mail.contacts where email ~* @term or name ~* @term";
      var contacts = new Query().Raw(sql);
      foreach(var contact in contacts){
        response.Contacts.Add(new Contact{
          ID = contact.id,
          Name = contact.name,
          Email = contact.email,
          Subscribed = contact.subscribed,
          CreatedAt = contact.created_at
        });
      }
      return response;
    }).WithOpenApi(op => {
      op.Summary = "Find one or more contacts using a fuzzy match on email or name";
      op.Description = "Find a set of contacts using a search term";
      return op;
    }).Produces<ContactSearchResponse>()
    .Produces(500);
  }
}