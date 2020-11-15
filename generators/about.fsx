#r "../_lib/Fornax.Core.dll"
#load "layout.fsx" "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) =
  let pinnedPost = Layout.pinnedPost ctx

  Layout.layout
    ctx
    (Layout.Page "About")
    [ div [ Class "filling-with-hero-content" ] [
        div [ Class "filling-with-hero-content__content-wrapper" ] [
          section [ Class "section" ] [
            div [ Class "container" ] [
              div [ Class "columns" ] [
                div [ Class "column is-8 is-offset-2" ] [
                  h1 [ Class "is-size-1" ] [
                    !! "Hello there! Glad you found me!"
                  ]
                  p [ Class "about-paragraph" ] [
                    !! @"My name is Gregor (he/him), I'm from beautiful Austria 🌄 where I work, study and live. I'm a software developer (or software engineer, or coder, or programmer or full-stack-engineer - you choose) and I like to build stuff 👨‍💻"
                  ]
                  p [ Class "about-paragraph" ] [
                    !! @"My most recent learning journey focuses on all things Data Science 🔬 and Machine Learning 🧠, especially where those topics can be used to create fun and useful applications (favorably on the web)."
                  ]
                  p [ Class "about-paragraph" ] [
                    !! "Things I like"
                  ]
                  ul [ Class "about-list" ] [
                    li [] [ !! "Functional Programming" ]
                    li [] [ !! "Data Science/ML" ]
                    li [] [
                      !! "Thinking about Ethics in Tech"
                    ]
                    li [] [
                      !! "Being nice to my fellow human beings (and animals 🦋🐶🐱)"
                    ]
                  ]
                  p [ Class "about-paragraph" ] [
                    !! """Interested in the things I do? Having an "out of this world" idea and needing someone to talk about it? Some other reason to message me? Never hesitate to reach out 😊"""
                  ]
                  p [ Class "about-paragraph" ] [
                    !! "Mail me at"
                    b [] [
                      !! "gregor_beyerle (at) outlook (dot) com"
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
        if pinnedPost.IsSome then
          let pinnedPost = pinnedPost.Value
          div [ Class "filling-with-hero-content__hero-wrapper" ] [
            PinnedHero.pinnedHero false pinnedPost.title pinnedPost.link
          ]
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx |> Layout.render ctx
