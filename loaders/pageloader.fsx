#r "../_lib/Fornax.Core.dll"

type Page =
  { title: string
    link: string
    isInTopNavigation: bool }

let loader (projectRoot: string) (siteContent: SiteContents) =
  siteContent.Add
    ({ title = "Home"
       link = "/"
       isInTopNavigation = true })
  siteContent.Add
    ({ title = "About"
       link = "/about.html"
       isInTopNavigation = true })
  siteContent.Add
    ({ title = "Blog"
       link = "/blog.html"
       isInTopNavigation = true })
  siteContent.Add
    ({ title = "Imprint"
       link = "/imprint.html"
       isInTopNavigation = false })

  siteContent
