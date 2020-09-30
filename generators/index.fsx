#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"
#load "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) (_: string) =
  let siteInfo =
    match ctx.TryGetValue<Globalloader.SiteInfo>() with
    | Some info -> info
    | None -> failwith "Site info not configured correctly!"

  Layout.layout
    ctx
    "Home"
    [ div [ Class "focus-content-wrapper" ] [
        section [ Class "section focus-content" ] [
          div [ Class "container" ] [
            div [ Class "columns" ] [
              div [ Class "column is-8 is-offset-2" ] [
                div [ Class "columns is-vcentered" ] [
                  div [ Class "column is-narrow" ] [
                    figure [ Class "image landing-profile-pic" ] [
                      img [ Id "profile-image"
                            Src "/images/profile_questioning.jpg"
                            Alt "Photo of Gregor"
                            Class "is-rounded" ]
                    ]
                  ]
                  div [ Class "column" ] [
                    hgroup [] [
                      h1 [ Class "landing-greeting" ] [
                        !! "Hi! I'm Gregor (he/him)"
                      ]
                      h2 [ Class "landing-whoami" ] [
                        !! ".NET Developer"
                      ]
                      h2 [ Class "landing-whoami" ] [
                        !! "F#unctional Programmer"
                      ]
                      h2 [ Class "landing-whoami" ] [
                        !! "Data Wrangler"
                      ]
                      h2 [ Class "landing-whoami" ] [
                        !! "Maker of Web Things"
                      ]
                    ]
                    div [ Class "socials" ] [
                      a [ Class "social-link"
                          Href "https://www.linkedin.com/in/gregor-beyerle"
                          Target "_blank" ] [
                        span [ Class "fab fa-linkedin" ] []
                      ]
                      a [ Class "social-link"
                          Href "https://twitter.com/GBeyerle"
                          Target "_blank" ] [
                        span [ Class "fab fa-twitter-square" ] []
                      ]
                      a [ Class "social-link"
                          Href "https://github.com/WalternativE"
                          Target "_blank" ] [
                        span [ Class "fab fa-github-square" ] []
                      ]
                      a [ Class "social-link"
                          Href "https://stackoverflow.com/users/story/4143281"
                          Target "_blank" ] [
                        span [ Class "fab fa-stack-overflow" ] []
                      ]
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
        PinnedHero.pinnedHero
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
