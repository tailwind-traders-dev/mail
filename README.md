# Welcome to the Tailwind Mail Service

This project is about sending email, both transactional and newsletter. The following features are planned:

 - **Single transaction**. You need to send `person@example.com` an email as part of your application's service (aka, not marketing or sales related)
 - **Batch transaction**. Same as above, but involves a group of recipients ("there's a new version of our video you can download"). Note: emails are always sent individually or in small batches - for obvious reasons the whole batch can be a transaction.
 - **Broadcast**. Send one or more emails that people can opt out of ("unsub").
 - **Sequence**. Send a series of emails delayed by time that people can opt out of.
 - **Segmentation**. Divide your people up into logical groups that make sense to you.
 - **Tagging**.

Automation isn't planned, but it's definitely something we could think about.

## Powered by Markdown and the File System

The goal with this project is to be portable and simple. There are obviously great services out there that do this kind of thing in a much more capable way (ActiveCampaign, MailChimp, Drip, ConvertKit, etc) but they cost a lot of money and sometimes you don't need what they provide.

To that end, I want to use Markdown document with intelligent meta data and a cron job 🥸 to do 20% of the same stuff (which is 80% of what you'll probably need).

For instance, in the `/sequences` directory you can see a subdirectory called `/sequences/welcome`. In there are some markdown documents that are self-contained in terms of their content, subject, and when they should be sent.

When someone is added to a sequence, individual jobs will be queued with an execution time - all built on the file name (the numeric prefix is the hour delay). They can be removed from the queue easily by deleting the job (more on that as I figure out the jobs interface).

## Tests and Such

I decided to have a little fun and use Bun (sorry for the pun) but I like to get things done! Bun is insanely fast so if you want to run these tests try [installing it](https://bun.sh/docs/installation) and see for yourself.

I like the idea of this tool! Reminds me of Elixir's "all in one" approach. Saves you tons of time.

Anyway - you can run:

```
npm run test
```

And it will use Bun to do the needful

