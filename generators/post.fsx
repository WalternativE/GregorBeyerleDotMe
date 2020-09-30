#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let published (post: Postloader.Post) =
  post.published
  |> Option.defaultValue System.DateTime.Now
  |> fun n -> n.ToString("yyyy-MM-dd")

let postLayout (useSummary: bool) (post: Postloader.Post) =
  article [ Class "article" ] [
    hgroup [] [
      h1 [ Class "is-size-1" ] [
        !!post.title
      ]
      h2 [ Class "is-size-5" ] [
        !!(sprintf "Published %s" (published post))
      ]
    ]
    div [] [
      !!(if useSummary then post.summary else post.content)
    ]
  ]

let generate' (ctx: SiteContents) (page: string) =
  let post =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty
    |> Seq.find (fun n -> n.file = page)

  Layout.layout
    ctx
    post.title
    [ div [ Class "container" ] [
        section [ Class "section" ] [
          div [ Class "columns" ] [
            div [ Class "column is-8 is-offset-2" ] [
              postLayout false post
            ]
          ]
        ]
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
