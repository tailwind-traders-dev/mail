## Not using Bun...

I want to - love the idea - but there's a bug in the internals that is stopping it from recognizing when Sequelize is done with a query. 

Well, sort of.

If I run `bun sync.js` it will execute just fine, but it's when it's combined with `beforeAll` in the tests that it hangs. Not too sure what's happening but an issue has been raised already.

https://github.com/oven-sh/bun/issues/5902