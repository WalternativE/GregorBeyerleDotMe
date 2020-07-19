#r "../../_lib/Fornax.Core.dll"

open Html

let siteFooter =
    footer [ Class "footer" ] [
        section [ Class "container" ] [
            p [] [
                !! "© 2020 Gregor Beyerle · Powered by "
                a [ Href "https://github.com/ionide/Fornax"; Target "_blank" ] [ !! "Fornax" ]
                !! " & "
                a [ Href "https://github.com/luizdepra/hugo-coder"; Target "_blank" ] [ !! "Coder Theme" ]
            ]
        ]
    ]