#r "../../_lib/Fornax.Core.dll"

open Html

let siteFooter =
  footer [ Class "footer" ] [
    div [ Class "container" ] [
      div [ Class "columns" ] [
        div [ Class "column is-offset-2 content" ] [
          p [] [
            !! "© 2020 Gregor Beyerle | "
            a [ Href "/imprint.html" ] [ !! " Imprint " ]
            !! " | Made with 🧡 and "
            a [ Href "https://github.com/ionide/Fornax"
                Target "_blank" ] [
              !! "Fornax"
            ]
          ]
        ]
      ]
    ]
  ]
