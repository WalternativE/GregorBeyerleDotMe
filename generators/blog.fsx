#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"
#load "./partials/pinnedHero.fsx"

open Html

let generate' (ctx: SiteContents) (_: string) =
  let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>()

  let posts =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty<Postloader.Post>

  let postLinks =
    posts
    |> Seq.map (fun post ->
         li [] [
           a [ Href post.link ] [ !!post.title ]
         ])
    |> Seq.toList

  Layout.layout
    ctx
    "Blog"
    [ PinnedHero.pinnedHero false
      div [ Class "container" ] [
        div [ Class "columns" ] [
          div [ Class "column is-8 is-offset-2" ] [
            section [ Class "section" ] [
              ul [] postLinks
            ]
          ]
        ]
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
