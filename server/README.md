# The Mail Service API

This is the .NET Minimal API backend service for the Tailwind Traders list server.

This API is all about sending transactional emails and queueing up batch sends to an email list. For it to work properly, you'll need to set a few ENV variables:

```
ASPNETCORE_ENVIRONMENT="Development"
DATABASE_URL="postgres://user:pw@host/db_name:port"

SMTP_USER=""
SMTP_PASSWORD=""
SMTP_HOST=""

DEFAULT_FROM="test@tailwind.dev"

ETHEREAL_USER="easy and free to set up at ethereal.email"
ETHEREAL_PASSWORD=""
```

Will fill this out more as we go...

## To Do...

 - Models and such
 - COPY command

## Stories

Here are the scenarios we're trying to hit with the first release:

 - [x] **Jill queues a broadcast to 10K contacts**. Jill has 10,001 contacts in her database with one opting out. The app should queue up 10K messages for send.
 - [ ] **Jim wants to signup to Jill's list**. He submits a form and is sent a double opt-in link to confirm his signup.
 - [ ] **Kim gets too much email** and decides to opt out using an unsub link.
 - [ ] **Jill sends a transactional email** to people who buy her book. These are sent individually without an unsub link.