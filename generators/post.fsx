#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html
open System.Text.RegularExpressions

type NavigationItem = { Title: string; Link: string }

type ConnectedPosts =
  { Previous: Postloader.Post option
    Current: Postloader.Post
    Next: Postloader.Post option }

let published (post: Postloader.Post) =
  post.published
  |> Option.defaultValue System.DateTime.Now
  |> fun n -> n.ToString("yyyy-MM-dd")

let parseNavigationStructure (content: string) =
  let pattern =
    """<h[1-6]><a name="(.*)">(.*)<\/a><\/h[1-6]>"""

  Regex.Matches(content, pattern, RegexOptions.IgnoreCase)
  |> Seq.map (fun m ->
       { Link = sprintf "#%s" m.Groups.[1].Value
         Title = m.Groups.[2].Value })
  |> Seq.toList

let constructNavigation (post: ConnectedPosts) (navigations: NavigationItem list) =
  aside [ Class "menu" ] [
    div [ Class "icon-navigation" ] [
      a [ Href "#top"
          Class "icon-navigation__link"
          Custom("aria-label", "to top") ] [
        i [ Class "fas fa-chevron-up"
            Custom("aria-hidden", "true") ] []
      ]
      if post.Previous.IsSome then
        a [ Href post.Previous.Value.link
            Class "icon-navigation__link"
            Custom("aria-label", "previous article") ] [
          i [ Class "fas fa-chevron-left"
              Custom("aria-hidden", "true") ] []
        ]
      if post.Next.IsSome then
        a [ Href post.Next.Value.link
            Class "icon-navigation__link"
            Custom("aria-label", "next article") ] [
          i [ Class "fas fa-chevron-right"
              Custom("aria-hidden", "true") ] []
        ]
    ]
    ul
      [ Class "menu-list" ]
      (navigations
       |> List.map (fun navitation ->
            li [] [
              a [ Href navitation.Link ] [
                !!navitation.Title
              ]
            ]))
  ]

let postLayout (post: Postloader.Post) =
  article [ Class "article" ] [
    figure [ Class "post-hero-figure" ] [
      image [ Class "image"
              Alt post.title
              Src post.large_image ] []
      match post.image_attribution_link, post.image_attribution_text with
      | Some link, Some attributionText ->
          figcaption [] [
            !! "Image by "
            a [ Href link
                Target "_blank"
                Rel "noopener" ] [
              !!attributionText
            ]
          ]
      | _ -> ()
    ]
    hgroup [] [
      h1 [ Class "is-size-1" ] [
        !!post.title
      ]
      h2 [ Class "is-size-5 published-line" ] [
        !!(sprintf "Published %s" (published post))
      ]
    ]
    div [ Class "content" ] [
      !!post.content
    ]
  ]

let extractConnectedPosts (page: string) (posts: Postloader.Post list) =
  let rec matchRemaining (posts: Postloader.Post list) =
    match posts with
    | [] ->
      failwith "I found no matching post :("
    | prev :: current :: next :: _ when current.file = page ->
        { Previous = Some prev
          Current = current
          Next = Some next }
    | prev :: [ current ] when current.file = page ->
        { Previous = Some prev
          Current = current
          Next = None }
    | [ current ] when current.file = page ->
        { Previous = None
          Current = current
          Next = None }
    | current :: next :: _ when current.file = page ->
        { Previous = None
          Current = current
          Next = Some next }
    | _ :: xs -> matchRemaining xs

  matchRemaining posts

let generate' (ctx: SiteContents) (page: string) =
  let posts =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty
    |> Seq.toList

  let post = posts |> extractConnectedPosts page

  let asideMenu =
    post.Current.content
    |> parseNavigationStructure
    |> constructNavigation post

  Layout.layout
    ctx
    (Layout.Post post.Current)
    [ div [ Class "container" ] [
        section [ Class "section" ] [
          div [ Class "columns" ] [
            div [ Class "column is-8 is-offset-2" ] [
              postLayout post.Current
            ]
          ]
        ]
      ]
      div [ Class "side-navigation" ] [
        asideMenu
      ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
