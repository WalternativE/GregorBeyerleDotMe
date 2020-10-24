#r "../../_lib/Fornax.Core.dll"

open Html

let link (fragment: string) =
  #if WATCH
  let suffix = ".html"
  #else
  let suffix = System.String.Empty
  #endif

  fragment + suffix

let siteFooter =
  footer [ Class "footer" ] [
    div [ Class "container" ] [
      div [ Class "columns" ] [
        div [ Class "column is-offset-2 content" ] [
          p [] [
            !! "© 2020 Gregor Beyerle | "
            a [ Href (link "/imprint") ] [
              !! " Imprint "
            ]
            !! " | Made with 🧡 and "
            a [ Href "https://github.com/ionide/Fornax"
                Target "_blank"
                Rel "noopener" ] [
              !! "Fornax"
            ]
            !! " | "
            a [ Href "#top" ] [ !! "To Top" ]
          ]
        ]
      ]
    ]
  ]
