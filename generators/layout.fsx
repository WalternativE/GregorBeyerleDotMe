#r "../_lib/Fornax.Core.dll"
#load "partials/siteHeader.fsx"
#load "partials/siteFooter.fsx"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/pageloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html

let injectWebsocketCode (webpage:string) =
    let websocketScript =
        """
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
    webpage.Insert ( (index + head.Length + 1),websocketScript)

let layout (ctx : SiteContents) (active : string) bodyCnt =
    printfn "Called with %s" active

    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo =
      match ctx.TryGetValue<Globalloader.SiteInfo> () with
      | Some info -> info
      | None -> failwith "Site info not correctly configured!"

    let menuEntries =
      pages
      |> Seq.map (fun p ->
          let isActive = p.title = active
          let navClasses = "navbar-item is-tab"
          a [ Class (if isActive then navClasses + " is-active" else navClasses)
              Href p.link ]
            [ !! (p.title.ToUpper()) ])
      |> Seq.toList

    html [ Lang siteInfo.language ] [
        head [] [
            meta [ CharSet "utf-8" ]
            meta [ Name "viewport"; Content "width=device-width, initial-scale=1" ]
            meta [ HttpEquiv "Content-Language"; Content siteInfo.language ]
            meta [ Name "author"; Content siteInfo.author ]
            meta [ Name "description"; Content siteInfo.description ]
            // TODO: keywords
            // TODO: twitter card
            // TODO: open graph
            title [] [!! siteInfo.title]
            link [Rel "icon"; Type "image/png"; Sizes "32x32"; Href "/images/favicon.png"]
            link [
              Rel "stylesheet"
              Href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.14.0/css/all.min.css"
              Integrity "sha512-1PKOgIY59xJ8Co8+NE6FZ+LOAZKjy+KY8iq0G4B3CyeY6wYHN3yt9PW0XpSriVlkMXe40PTKnXrLnZ9+fkDaog=="
              CrossOrigin "anonymous"
            ]
            link [
              Rel "stylesheet"
              Href "https://cdnjs.cloudflare.com/ajax/libs/normalize/8.0.1/normalize.min.css"
              Integrity "sha256-l85OmPOjvil/SOvVt3HnSSjzF1TUMyT9eV0c2BzEGzU="
              CrossOrigin "anonymous"
            ]
            link [
              Rel "stylesheet"
              Type "text/css"
              Href "/style/main.css"
              Media "screen"
            ]
        ]
        body [] [
          SiteHeader.siteHeader menuEntries
          main [] [
              div [ Class "content" ] bodyCnt
          ]
          SiteFooter.siteFooter
          script [ Type "text/javascript"; Src "scripts/main.js" ] []
        ]
    ]

let render (ctx : SiteContents) cnt =
  let disableLiveRefresh =
    ctx.TryGetValue<Postloader.PostConfig> ()
    |> Option.map (fun n -> n.disableLiveRefresh)
    |> Option.defaultValue false

  cnt
  |> HtmlElement.ToString
  |> fun n -> if disableLiveRefresh then n else injectWebsocketCode n

let published (post: Postloader.Post) =
    post.published
    |> Option.defaultValue System.DateTime.Now
    |> fun n -> n.ToString("yyyy-MM-dd")

let postLayout (useSummary: bool) (post: Postloader.Post) =
    div [Class "card article"] [
        div [Class "card-content"] [
            div [Class "media-content has-text-centered"] [
                p [Class "title article-title"; ] [ a [Href post.link] [!! post.title]]
                p [Class "subtitle is-6 article-subtitle"] [
                a [Href "#"] [!! (defaultArg post.author "")]
                !! (sprintf "on %s" (published post))
                ]
            ]
            div [Class "content article-body"] [
                !! (if useSummary then post.summary else post.content)
            ]
        ]
    ]
