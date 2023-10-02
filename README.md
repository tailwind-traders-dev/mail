# Welcome to the Tailwind Mail Service

This project is about sending email, both transactional and newsletter. The following features are planned:

 - **Single transaction**. You need to send `person@example.com` an email as part of your application's service (aka, not marketing or sales related)
 - **Batch transaction**. Same as above, but involves a group of recipients ("there's a new version of our video you can download"). Note: emails are always sent individually or in small batches - for obvious reasons the whole batch can be a transaction.
 - **Broadcast**. Send one or more emails that people can opt out of ("unsub").
 - **Sequence**. Send a series of emails delayed by time that people can opt out of.
 - **Segmentation**. Divide your people up into logical groups that make sense to you.
 - **Tagging**.

Automation isn't planned, but it's definitely something we could think about.

## Powered by Markdown

The goal with this project is to be portable and simple. There are obviously great services out there that do this kind of thing in a much more capable way (ActiveCampaign, MailChimp, Drip, ConvertKit, etc) but they cost a lot of money and sometimes you don't need what they provide.

To that end, I want to use Markdown combined with simple templating (EJS I suppose) so we can do fun things. Not sure how this will play out, but ... we'll see.


## Tests and Such

I'm building out story-based tests (eventually) and there are two ways you can run them:

```
npm run test
```

Will run tests using `NODE_ENV=test`, which means that we won't send actual emails. If you want to do that, you can use [ethereal](https://ethereal.email/), which will intercept your messages and you can see them all in a groovy web interface.

## ENV stuff

The app expects the following to be in your `.env`:

```
SMTP_USER="SOMETHING"
SMTP_PASSWORD="SOMETHING"
SMTP_HOST="SOMETHING"

DEFAULT_FROM="test@tailwind.dev"
DATABASE_URL="postgres://localhost/tailwind"

ETHEREAL_USER="SOMETHING"
ETHEREAL_PW="SOMETHING"
```

