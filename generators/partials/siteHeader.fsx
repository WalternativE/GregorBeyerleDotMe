#r "../../_lib/Fornax.Core.dll"

open Html

let siteHeader menuEntries =
    nav [ Class "navbar"
          Role "navigation"
          Custom("aria-label", "main navigation") ] [
        div [ Class "navbar-brand" ] [
            a [ Role "button"
                Class "navbar-burger burger"
                Custom ("aria-label", "menu")
                Custom ("aria-expanded", "false")
                Custom ("data-target", "blogNavbar")] [
                    span [ Custom ("aria-hidden", "true") ] []
                    span [ Custom ("aria-hidden", "true") ] []
                    span [ Custom ("aria-hidden", "true") ] []
                ]
        ]
        div [ Id "blogNavbar"
              Class "navbar-menu" ] [
            div [ Class "navbar-end" ] menuEntries
        ]
    ]
