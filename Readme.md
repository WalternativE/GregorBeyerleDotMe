# GregorBeyerleMe - AKA My Blog

Hi y'all 👋 I decided it was time to just go for it and build my own website and blog. If you have any ideas about things I could write about you can always leave an issue. If you found typos or other potential mistakes, please go ahead and tell me. Thanks 🙇‍♂️

## Building the Site

I built the website using [Fornax](https://github.com/ionide/Fornax) a F# static site generator (or static everything generator if you want to see it like that). If you wan to try to build it for yourself you'll have to get some things set up on your machine.

- Install the .NET SDK. I usually pin the version in the `global.json` file in this repository.
- Install Sass (I don't care about the implementation, the generator just needs some `sass` command on the path)
- Restore the dotnet tools using `dotnet tool restore`

You can build the site using `dotnet fornax build`. If you want to add changes and see them hosted locally use `dotnet fornax watch`.

Happy hacking! 😊