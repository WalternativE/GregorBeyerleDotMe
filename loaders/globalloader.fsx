#r "../_lib/Fornax.Core.dll"

type SiteInfo =
  { title: string
    description: string
    language: string
    author: string
    authorEmail: string
    basePath: string }

let loader (projectRoot: string) (siteContent: SiteContents) =
  #if WATCH
  let basePath = "http://localhost:8080"
  #else
  let basePath = "https://www.gregorbeyerle.me"
  #endif

  siteContent.Add
    ({ title = "GregorBeyerleDotMe"
       description = "Functional Programming, Data Science, Machine Learning and more!"
       language = "en"
       author = "Gregor Beyerle"
       authorEmail = "hello@gregorbeyerle.me"
       basePath = basePath })

  siteContent
