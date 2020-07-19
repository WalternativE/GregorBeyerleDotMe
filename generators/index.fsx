#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let generate' (ctx : SiteContents) (_: string) =
  let siteInfo =
    match ctx.TryGetValue<Globalloader.SiteInfo> () with
    | Some info -> info
    | None -> failwith "Site info not configured correctly!"

  Layout.layout ctx "Home" [
    section [Class "container centered"] [
      div [Class "about"] [
        div [ Class "avatar" ] [
          img [ Src "/images/profile.jpeg"; Alt "Photo of Gregor" ]
        ]
        h1 [] [ !! siteInfo.author ]
        h2 [] [ !! siteInfo.description ]
      ]
    ]
  ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
  generate' ctx page
  |> Layout.render ctx