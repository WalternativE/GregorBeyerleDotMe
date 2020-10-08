#r "../_lib/Fornax.Core.dll"
#load "layout.fsx" "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) =
  let siteInfo = Layout.siteInfo ctx
  let pinnedPost = Layout.pinnedPost ctx

  Layout.layout
    ctx
    (Layout.Page "Home")
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
                          Custom ("aria-label", "To my LinkedIn")
                          Href "https://www.linkedin.com/in/gregor-beyerle"
                          Target "_blank"
                          Rel "noopener" ] [
                        i [ Class "fab fa-linkedin"
                            Custom ("aria-hidden", "true") ] []
                      ]
                      a [ Class "social-link"
                          Custom ("aria-label", "To my Twitter")
                          Href "https://twitter.com/GBeyerle"
                          Target "_blank"
                          Rel "noopener" ] [
                        i [ Class "fab fa-twitter-square"
                            Custom ("aria-hidden", "true") ] []
                      ]
                      a [ Class "social-link"
                          Custom ("aria-label", "To my GitHub")
                          Href "https://github.com/WalternativE"
                          Target "_blank"
                          Rel "noopener" ] [
                        i [ Class "fab fa-github-square"
                            Custom ("aria-hidden", "true") ] []
                      ]
                      a [ Class "social-link"
                          Custom ("aria-label", "To my StackOverflow Developer Story")
                          Href "https://stackoverflow.com/users/story/4143281"
                          Target "_blank"
                          Rel "noopener" ] [
                        i [ Class "fab fa-stack-overflow"
                            Custom ("aria-hidden", "true") ] []
                      ]
                      a [ Class "social-link"
                          Custom ("aria-label", "My Blog RSS Feed")
                          Href (siteInfo.basePath + "/blogrssfeed.xml")
                          Target "_blank"
                          Rel "noopener" ] [
                        i [ Class "fas fa-rss-square"
                            Custom ("aria-hidden", "true") ] []
                      ]
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
        PinnedHero.pinnedHero true pinnedPost.title pinnedPost.link
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx |> Layout.render ctx
