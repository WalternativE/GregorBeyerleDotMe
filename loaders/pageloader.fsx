#r "../_lib/Fornax.Core.dll"

open System

type Page =
  { title: string
    link: string
    isInTopNavigation: bool
    changedAt: DateTime option }

let loader (projectRoot: string) (siteContent: SiteContents) =
  siteContent.Add
    ({ title = "Home"
       link = "/"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "About"
       link = "/about.html"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "Blog"
       link = "/blog.html"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "Imprint"
       link = "/imprint.html"
       isInTopNavigation = false
       changedAt = None })

  siteContent
