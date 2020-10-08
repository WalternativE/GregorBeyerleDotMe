#r "../../_lib/Fornax.Core.dll"

open Html

let pinnedHero isMedium title link =
  let addedClass = if isMedium then " is-medium" else ""
  section [ Class(sprintf "hero is-info%s" addedClass) ] [
    div [ Class "hero-body" ] [
      div [ Class "container" ] [
        div [ Class "columns" ] [
          div [ Class "column is-8 is-offset-2" ] [
            div [ Class "pinned-item" ] [
              div [ Class "pin-container is-1by1" ] [
                div [ Class "pin has-ratio" ] [
                  i [ Class "pin__icon fas fa-thumbtack fa-4x" ] []
                ]
              ]
              div [ Class "pin-text" ] [
                hgroup [] [
                  h4 [ Class "pin-text__sub" ] [
                    !! "Pinned for you:"
                  ]
                  a [ Class "pin-text__main-link"
                      Href link ] [
                    h3 [ Class "is-size-3" ] [
                      !! title
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
  ]
