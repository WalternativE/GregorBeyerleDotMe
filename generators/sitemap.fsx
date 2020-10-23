#r "../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../loaders/postloader.fsx" "../loaders/pageloader.fsx" "../loaders/globalloader.fsx" "../globals.fsx"
#endif

open System
open System.Xml.Linq
open System.Globalization
open Globals

type SitemapFrequency =
  | Never
  | Yearly
  | Monthly
  | Weekly
  | Daily
  | Hourly
  | Always

type SitemapNode =
  { Frequency: SitemapFrequency
    LastModified: DateTime
    Priority: float
    Url: string }

let inline (!!) arg =
  (^a: (static member op_Implicit: ^b -> ^a) arg)

let generate (ctx: SiteContents) (projectRoot: string) (page: string) =
  let (ns: XNamespace) =
    !! "http://www.sitemaps.org/schemas/sitemap/0.9"

  let root = XElement(ns + "urlset")

  let siteInfo =
    ctx.TryGetValue<Globalloader.SiteInfo>()
    |> Option.defaultWith (fun () -> failwith "No siteinfo found :(")

  let toUrlElement (node: SitemapNode) =
    XElement
      (ns + "url",
       XElement(ns + "loc", Uri.EscapeUriString(node.Url)),
       XElement(ns + "lastmod", node.LastModified.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
       XElement(ns + "changefreq", node.Frequency.ToString().ToLowerInvariant()),
       XElement(ns + "priority", node.Priority.ToString("F1", CultureInfo.InvariantCulture)))

  let addNodeToRoot (node: SitemapNode) =
    let toUrlElement = toUrlElement node
    root.Add(toUrlElement)

  let pages =
    ctx.TryGetValues<Pageloader.Page>()
    |> Option.defaultValue Seq.empty

  pages
  |> Seq.map (fun page ->
       { Frequency = Monthly
         LastModified = None |> Option.defaultValue DateTime.Now
         Priority = 0.5
         Url = siteInfo.basePath + page.link })
  |> Seq.iter addNodeToRoot

  let posts =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty

  posts
  |> Seq.filter (fun post -> post.published.IsSome)
  |> Seq.map (fun post ->
       { Frequency = Monthly
         LastModified = post.published.Value
         Priority = 0.5
         Url = siteInfo.basePath + post.link })
  |> Seq.iter addNodeToRoot

  use stringWriter = new Utf8StringWriter()

  let document = XDocument(root)
  document.Save(stringWriter)

  stringWriter.ToString()
