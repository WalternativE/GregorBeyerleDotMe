#r "../_lib/Fornax.Core.dll"

type SiteInfo =
  { title: string
    description: string
    language: string
    author: string
    authorEmail: string
    basePath: string }

let loader (projectRoot: string) (siteContent: SiteContents) =
  siteContent.Add
    ({ title = "GregorBeyerleDotMe"
       description = "Functional Programming, Data Science, Machine Learning and more!"
       language = "en"
       author = "Gregor Beyerle"
       authorEmail = "hello@gregorbeyerle.me"
       basePath = "https://www.gregorbeyerle.me" })

  siteContent
