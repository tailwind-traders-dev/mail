# The Mail Service

Hello! This API is all about sending transactional emails and queueing up batch sends to an email list. For it to work properly, you'll need to set a few ENV variables:

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