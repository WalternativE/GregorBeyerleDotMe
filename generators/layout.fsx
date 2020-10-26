#r "../_lib/Fornax.Core.dll"
#r "../_lib/Newtonsoft.Json.dll"
#load "partials/siteHeader.fsx" "partials/siteFooter.fsx"
#if !FORNAX
#load "../loaders/postloader.fsx" "../loaders/pageloader.fsx" "../loaders/globalloader.fsx"
#endif

open Html
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

type Active =
  | Page of pagename: string
  | Post of post: Postloader.Post

type CommonMetadata =
  { Title: string
    Description: string
    Image: string
    Url: string }

  static member FromPage (siteInfo: Globalloader.SiteInfo) (url: string) (page: Pageloader.Page) =
    { Title = page.title
      Description = page.description
      Image =
        siteInfo.basePath
        + "/images/profile_questioning.jpg"
      Url = url }

  static member FromPost (siteInfo: Globalloader.SiteInfo) (url: string) (post: Postloader.Post) =
    { Title = post.title
      Description = post.description
      Image = siteInfo.basePath + post.large_image
      Url = url }

type PostMetadata =
  { PublishedAt: System.DateTime
    ModifiedAt: System.DateTime }

  static member FromPost(post: Postloader.Post) =
    { PublishedAt = post.published.Value
      ModifiedAt =
        match post.last_modified with
        | Some v -> v
        | None -> post.published.Value }

type Person =
  { [<JsonProperty("@type")>]Type: string
    Name: string }

  static member Create name =
    { Type = "Person"
      Name = name }

type WebSite =
  { [<JsonProperty("@context")>]Context: string
    [<JsonProperty("@type")>]Type: string
    Author: Person
    Publisher: Person
    Headline: string
    SameAs: string []
    Description: string
    Image: string
    Name: string
    Url: string }

  static member Create headline sameAs description image name url authorName =
    let authoringPerson = Person.Create authorName
    { Context = "https://schema.org"
      Type = "WebSite"
      Headline = headline
      SameAs = sameAs
      Description = description
      Image = image
      Name = name
      Url = url
      Author = authoringPerson
      Publisher = authoringPerson }

type WebPageEntity =
  { [<JsonProperty("@type")>]Type: string
    [<JsonProperty("@id")>]Id: string }

  static member Create (uri: string) =
    { Type = "WebPage"
      Id = uri }

type BlogPosting =
  { [<JsonProperty("@context")>]Context: string
    [<JsonProperty("@type")>]Type: string
    Headline: string
    DateModified: System.DateTime
    DatePublished: System.DateTime
    Author: Person
    Description: string
    Image: string
    Url: string
    Publisher: Person
    MainEntityOfPage: WebPageEntity }

    static member Create headline dateModified datePublished authorName description image url =
      let authoringPerson = Person.Create authorName
      let mainEntity = WebPageEntity.Create url
      { Context = "https://schema.org"
        Type = "BlogPosting"
        Headline = headline
        DateModified = dateModified
        DatePublished = datePublished
        Author = authoringPerson
        Description = description
        Image = image
        Url = url
        Publisher = authoringPerson
        MainEntityOfPage = mainEntity }

let jsonSerializationSettings =
  JsonSerializerSettings(
    ContractResolver = CamelCasePropertyNamesContractResolver()
  )

let semanticContent (siteInfo: Globalloader.SiteInfo) (commonMeta: CommonMetadata) (postMeta: PostMetadata option) =
  // TODO: match if there is post meta or not and return other jsonld doc
  let webSite =
    WebSite.Create commonMeta.Title
                   [| "https://www.linkedin.com/in/gregor-beyerle"
                      "https://twitter.com/GBeyerle"
                      "https://github.com/WalternativE"
                      "https://stackoverflow.com/users/story/4143281" |]
                   commonMeta.Description
                   commonMeta.Image
                   siteInfo.author
                   commonMeta.Url
                   siteInfo.author

  JsonConvert.SerializeObject(webSite, jsonSerializationSettings)

let siteInfo (ctx: SiteContents) =
  ctx.TryGetValue<Globalloader.SiteInfo>()
  |> Option.defaultWith (fun () -> failwith "Oh no, all that failure :(")

let pinnedPost (ctx: SiteContents) =
  ctx.TryGetValues<Postloader.Post>()
  |> Option.defaultValue Seq.empty
  |> Seq.sortByDescending (fun post -> post.published)
  |> Seq.filter (fun post -> post.pinned)
  |> Seq.head

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

let openGraph (siteInfo: Globalloader.SiteInfo) (commonMeta: CommonMetadata) (postMeta: PostMetadata option) =
  [ meta [ Property "og:locale"
           Content siteInfo.language ]
    meta [ Property "og:title"
           Content commonMeta.Title ]
    meta [ Property "og:description"
           Content commonMeta.Description ]
    meta [ Property "og:url"
           Content commonMeta.Url ]
    meta [ Property "og:site_name"
           Content siteInfo.title ]
    meta [ Property "og:image"
           Content commonMeta.Image ]
    yield!
      match postMeta with
      | None ->
          [ meta [ Property "og:type"
                   Content "website" ] ]
      | Some postMeta ->
          [ meta [ Property "og:type"
                   Content "article" ]
            meta [ Property "article:published_time"
                   Content(postMeta.PublishedAt.ToString("o")) ]
            meta [ Property "article:modified_time"
                   Content(postMeta.ModifiedAt.ToString("o")) ] ] ]

let twitter (commonMeta: CommonMetadata) =
  [ meta [ Name "twitter:card"
           Content "summary_large_image" ]
    meta [ Name "twitter:url"
           Content commonMeta.Url ]
    meta [ Name "twitter:title"
           Content commonMeta.Title ]
    meta [ Name "twitter:description"
           Content commonMeta.Description ]
    meta [ Name "twitter:image"
           Content commonMeta.Image ]
    meta [ Name "twitter:site"
           Content "@GBeyerle" ]
    meta [ Name "twitter:creator"
           Content "@GBeyerle" ] ]

let layout (ctx: SiteContents) (active: Active) bodyCnt =
  let pages =
    ctx.TryGetValues<Pageloader.Page>()
    |> Option.defaultValue Seq.empty

  let siteInfo = siteInfo ctx

  let menuEntries =
    pages
    |> Seq.distinct
    |> Seq.filter (fun p -> p.isInTopNavigation)
    |> Seq.map (fun p ->
         let isActive =
           match active with
           | Page title -> p.title = title
           | Post _ -> false

         let navClasses = "navbar-item is-tab"
         a [ Class(if isActive then navClasses + " is-active" else navClasses)
             Href p.link ] [
           !!(p.title.ToUpper())
         ])
    |> Seq.toList

  let toCanonical link = siteInfo.basePath + link

  let (canonicalUrl, commonMeta, postMeta) =
    let (link, commonMeta, postMeta) =
      match active with
      | Page pageName ->
          let page =
            pages
            |> Seq.filter (fun p -> p.title = pageName)
            |> Seq.head

          page.link, CommonMetadata.FromPage siteInfo (toCanonical page.link) page, None
      | Post post ->
          post.link, CommonMetadata.FromPost siteInfo (toCanonical post.link) post, PostMetadata.FromPost post |> Some

    toCanonical link, commonMeta, postMeta

  html [ Lang siteInfo.language ] [
    head [] [
      meta [ CharSet "utf-8" ]
      meta [ Name "viewport"
             Content "width=device-width, initial-scale=1" ]
      meta [ HttpEquiv "Content-Language"
             Content siteInfo.language ]
      title [] [
        !!(sprintf "%s | %s" commonMeta.Title siteInfo.title)
      ]
      meta [ Name "generator"
             Content "Fornax" ]
      meta [ Name "author"
             Content siteInfo.author ]
      meta [ Name "description"
             Content commonMeta.Description ]
      yield! twitter commonMeta
      yield! openGraph siteInfo commonMeta postMeta
      script [ Type "application/ld+json" ] [ !!(semanticContent siteInfo commonMeta postMeta) ]
      link [ Rel "canonical"
             Href canonicalUrl ]
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
               Src "/scripts/prism.js" ] []
      script [ Defer true
               Type "text/javascript"
               Src "/scripts/main.js" ] []
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
