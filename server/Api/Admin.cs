//API bits for the admin CLI//public endpoints for subscribe/unsubscribe
using Microsoft.AspNetCore.Mvc;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Api;

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
  }

}