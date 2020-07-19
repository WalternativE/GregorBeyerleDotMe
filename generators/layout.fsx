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

let layout (ctx : SiteContents) active bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo =
      match ctx.TryGetValue<Globalloader.SiteInfo> () with
      | Some info -> info
      | None -> failwith "Site info not correctly configured!"

    let menuEntries =
      pages
      |> Seq.map (fun p ->
        li [ Class "navigation-item" ] [
          a [ Class ""; Href p.link ] [ !! p.title ]
        ])
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
              Href "https://fonts.googleapis.com/css?family=Lato:400,700%7CMerriweather:300,700%7CSource+Code+Pro:400,700&display=swap"
            ]
            link [
              Rel "stylesheet"
              Href "https://use.fontawesome.com/releases/v5.13.0/css/all.css"
              Integrity "sha384-Bfad6CLCknfcloXFOyFnlgtENryhrpZCe29RTifKEixXQZ38WheV+i/6YWSzkz3V"
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
        body [Class "colorscheme-dark"] [
          main [Class "wrapper"] [
              SiteHeader.siteHeader siteInfo.title menuEntries

              div [ Class "content" ] bodyCnt

              SiteFooter.siteFooter
          ]
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
