#r "../_lib/Fornax.Core.dll"
#load "layout.fsx" "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) (_: string) =
  Layout.layout
    ctx
    "Imprint"
    [ div [ Class "filling-with-hero-content" ] [
        div [ Class "filling-with-hero-content__content-wrapper" ] [
          div [ Class "container" ] [
            div [ Class "columns" ] [
              div [ Class "column is-8 is-offset-2" ] [
                section [ Class "section" ] [
                  h2 [ Class "is-size-3" ] [
                    !! "Contact"
                  ]
                  address [ Class "contact-address" ] [
                    span [] [ !! "Gregor Beyerle" ]
                    span [] [
                      !! "hello (at) gregorbeyerle (dot) me"
                    ]
                  ]
                ]
                section [ Class "section" ] [
                  h2 [ Class "is-size-3" ] [
                    !! "Data Protection"
                  ]
                  div [ Class "data-protection-block" ] [
                    h3 [ Class "is-size-5" ] [
                      !! "Email request"
                    ]
                    p [] [
                      !! @"If you contact me by e-mail, your data will be stored for processing the
request and in case of follow-up questions. I will not share this information
without your consent.
"
                    ]
                  ]
                  div [ Class "data-protection-block" ] [
                    h3 [ Class "is-size-5" ] [
                      !! "Logfiles"
                    ]
                    p [] [
                      !! """The provider automatically collects data about accesses to this website
and saves them as "server log files". The following data is logged: IP
address, host name, date/time of access, called URL, transferred bytes,
referrer URL, browser and operating system.
"""
                    ]
                    p [] [
                      !! @"There is no merge of this data with other data sources. However, I reserve
the right to check this data retrospectively, if concrete evidence of illegal
use is known."
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
        div [ Class "filling-with-hero-content__hero-wrapper" ] [
          PinnedHero.pinnedHero false
        ]
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
