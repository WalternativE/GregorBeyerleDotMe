#r "../_lib/Fornax.Core.dll"

open System

type Page =
  { title: string
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
       link = "/"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "About"
       link = link "/about"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "Blog"
       link = link "/blog"
       isInTopNavigation = true
       changedAt = None })
  siteContent.Add
    ({ title = "Imprint"
       link = link "/imprint"
       isInTopNavigation = false
       changedAt = None })

  siteContent
