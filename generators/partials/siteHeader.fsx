#r "../../_lib/Fornax.Core.dll"

open Html

let siteHeader title menuEntries =
    nav [Class "navigation"] [
        section [Class "container"] [
            a [Class "navigation-title"] [ !! title ]
            input [Type "checkbox"; Id "menu-toggle" ]
            label [ Class "menu-button float-right"; Custom ("for", "menu-toggle") ] [
                i [ Class "fas fa-bars menu-toggle-icon"] []
            ]
            ul [ Class "navigation-list" ]
                menuEntries
        ]
    ]