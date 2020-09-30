#r "../_lib/Fornax.Core.dll"
#load "partials/siteHeader.fsx"
#load "partials/siteFooter.fsx"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/pageloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html

let injectWebsocketCode (webpage: string) =
  let websocketScript = """
    <script type="text/javascript">
      var wsUri = "ws://localhost:8080/websocket";
      function init()
      {
        websocket = new WebSocket(wsUri);
        websocket.onclose = function(evt) { onClose(evt) };
      }
      function onClose(evt)
      {
        console.log('closing');
        websocket.close();
        document.location.reload();
      }
      window.addEventListener("load", init, false);
    </script>
      """

  let head = "<head>"
  let index = webpage.IndexOf head
  webpage.Insert((index + head.Length + 1), websocketScript)

let layout (ctx: SiteContents) (active: string) bodyCnt =
  let pages =
    ctx.TryGetValues<Pageloader.Page>()
    |> Option.defaultValue Seq.empty

  let siteInfo =
    match ctx.TryGetValue<Globalloader.SiteInfo>() with
    | Some info -> info
    | None -> failwith "Site info not correctly configured!"

  let menuEntries =
    pages
    |> Seq.filter (fun p -> p.isInTopNavigation)
    |> Seq.map (fun p ->
         let isActive = p.title = active
         let navClasses = "navbar-item is-tab"
         a [ Class(if isActive then navClasses + " is-active" else navClasses)
             Href p.link ] [
           !!(p.title.ToUpper())
         ])
    |> Seq.toList

  html [ Lang siteInfo.language ] [
    head [] [
      meta [ CharSet "utf-8" ]
      meta [ Name "viewport"
             Content "width=device-width, initial-scale=1" ]
      meta [ HttpEquiv "Content-Language"
             Content siteInfo.language ]
      meta [ Name "author"
             Content siteInfo.author ]
      meta [ Name "description"
             Content siteInfo.description ]
      // TODO: keywords
      // TODO: twitter card
      // TODO: open graph
      title [] [ !!siteInfo.title ]
      link [ Rel "icon"
             Type "image/png"
             Sizes "32x32"
             Href "/images/favicon.png" ]
      link [ Rel "stylesheet"
             Type "text/css"
             Href "/style/main.css"
             Media "screen" ]
      link [ Rel "preload"
             Href "/webfonts/raleway/raleway-v17-latin-ext_latin-regular.woff2"
             Custom("as", "font")
             Type "font/woff2"
             CrossOrigin "anonymous" ]
      link [ Rel "preload"
             Href "/webfonts/font_awesome/fa-brands-400.woff2"
             Custom("as", "font")
             Type "font/woff2"
             CrossOrigin "anonymous" ]
      link [ Rel "preload"
             Href "/webfonts/font_awesome/fa-solid-900.woff2"
             Custom("as", "font")
             Type "font/woff2"
             CrossOrigin "anonymous" ]
      script [ Defer true
               Type "text/javascript"
               Src "scripts/main.js" ] []
    ]
    body [] [
      SiteHeader.siteHeader menuEntries
      main [] bodyCnt
      SiteFooter.siteFooter
    ]
  ]

let render (ctx: SiteContents) cnt =
  let disableLiveRefresh =
    ctx.TryGetValue<Postloader.PostConfig>()
    |> Option.map (fun n -> n.disableLiveRefresh)
    |> Option.defaultValue false

  cnt
  |> HtmlElement.ToString
  |> fun n -> if disableLiveRefresh then n else injectWebsocketCode n
  |> fun page -> "<!DOCTYPE html>\n" + page

let published (post: Postloader.Post) =
  post.published
  |> Option.defaultValue System.DateTime.Now
  |> fun n -> n.ToString("yyyy-MM-dd")

let postLayout (useSummary: bool) (post: Postloader.Post) =
  div [ Class "card article" ] [
    div [ Class "card-content" ] [
      div [ Class "media-content has-text-centered" ] [
        p [ Class "title article-title" ] [
          a [ Href post.link ] [ !!post.title ]
        ]
        p [ Class "subtitle is-6 article-subtitle" ] [
          a [ Href "#" ] [
            !!(defaultArg post.author "")
          ]
          !!(sprintf "on %s" (published post))
        ]
      ]
      div [ Class "content article-body" ] [
        !!(if useSummary then post.summary else post.content)
      ]
    ]
  ]
