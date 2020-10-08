#r "../_lib/Fornax.Core.dll"
#load "layout.fsx" "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) =
  let posts =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty

  let pinnedPost = Layout.pinnedPost ctx

  let postLinks =
    posts
    |> Seq.map (fun post ->
         li [ Class "bloglist-entry" ] [
           div [] [
             div [] [
               a [ Href post.link ] [ !!post.title ]
             ]
             if post.published.IsSome then
               div [ Class "bloglist-entry__subline" ] [
                 em [ Class "bloglist-entry__publish-date" ] [
                   !!(post.published
                      |> Option.map (fun p -> p.ToString("yyyy-MM-dd"))
                      |> Option.defaultValue "1970-01-01")
                 ]
                 div [ Class "inline-divider" ] [ hr [] ]
               ]
           ]
         ])
    |> Seq.toList

  Layout.layout
    ctx
    (Layout.Page "Blog")
    [ PinnedHero.pinnedHero false pinnedPost.title pinnedPost.link
      section [ Class "section" ] [
        div [ Class "container" ] [
          div [ Class "columns" ] [
            div [ Class "column is-8 is-offset-2" ] [
              ul [] postLinks
            ]
          ]
        ]
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx |> Layout.render ctx
