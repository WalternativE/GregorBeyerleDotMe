#r "../_lib/Fornax.Core.dll"

open System

type Page =
  { title: string
    description: string
    link: string
    isInTopNavigation: bool
    changedAt: DateTime option }

let link (fragment: string) =
#if WATCH
  let suffix = ".html"
#else
  let suffix = String.Empty
#endif

  fragment + suffix

let loader (projectRoot: string) (siteContent: SiteContents) =
  siteContent.Add
    ({ title = "Home"
       description =
         "Hi! I'm a software developer, functional programmar and a data scientist in training. This is my site."
       link = "/"
       isInTopNavigation = true
       changedAt = None })

  siteContent.Add
    ({ title = "About"
       description =
         "Want to know more about me? This is the right place to see how I'd describe myself and maybe get in contact with me."
       link = link "/about"
       isInTopNavigation = true
       changedAt = None })

  siteContent.Add
    ({ title = "Blog"
       description =
         "I try to write down things I learned and show people some projects I'm working on. Hope you like it!"
       link = link "/blog"
       isInTopNavigation = true
       changedAt = None })

  siteContent.Add
    ({ title = "Imprint"
       description =
         "The legal stuff. Boring, right? Nevertheless it is important! Just trying my best to abide the laws of the internet."
       link = link "/imprint"
       isInTopNavigation = false
       changedAt = None })

  siteContent
