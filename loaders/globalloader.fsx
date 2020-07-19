#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title : string
    description : string
    language : string
    author : string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add(
        { title = "GregorBeyerleDotMe";
          description = "Functional Programmer, Data Enthusiast"
          language = "en"
          author = "Gregor Beyerle" })

    siteContent
