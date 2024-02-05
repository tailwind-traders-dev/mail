//API bits for the admin CLI//public endpoints for subscribe/unsubscribe
using Microsoft.AspNetCore.Mvc;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Api;

public class ValidationResponse{
  public bool Valid { get; set; }
  public string Message { get; set; }
  public MarkdownEmail? Data { get; set; }
  public ValidationResponse()
  {
    Message = "The markdown is valid";
  }
}
public class ValidationRequest{
  public string? Markdown { get; set; }
}

public class Admin{

  //all of these routes will be protected in some way...
  public static void MapRoutes(IEndpointRouteBuilder app)
  {
    //queue up a broadcast
    //CRUD for contacts
    //CRUD for email templates
    //Message queue problems - failed, bounced
    //Message queue pending
    //Broadcast summary
    //Contact stats
    //Tag stats
    //validate a broadcast
    app.MapPost("/admin/validate", ([FromBody] ValidationRequest req) => {
      if(req.Markdown == null){
        return new ValidationResponse{
          Valid = false,
          Message = "The markdown is null"
        };
      }
      var doc = MarkdownEmail.FromString(req.Markdown);
      if(!doc.IsValid()){
        return new ValidationResponse{
          Valid = false,
          Message = "Ensure there is a Subject and Summary in the markdown",
          Data = doc
        };
      }
      //ensure that it has a subject, summary, and slug
      var response = new ValidationResponse{
        Valid = true,
        Data = doc
      };
      return response;
    }).WithOpenApi(op => {
      op.Summary = "Validate the markdown for an email";
      op.Description = "Before you send a broadcast, ping this endpoint to ensure that the markdown is valid";
      op.RequestBody.Description = "The markdown for the email";
      return op;
    });
  }

}