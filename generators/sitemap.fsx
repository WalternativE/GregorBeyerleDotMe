#r "../_lib/Fornax.Core.dll"

open System
open System.IO
open System.Xml
open System.Xml.Linq

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

type Utf8StringWriter() =
  inherit StringWriter()

  override this.Encoding
    with get() = Text.Encoding.UTF8

let inline (!!) arg =
  (^a: (static member op_Implicit: ^b -> ^a) arg)

let generate (ctx: SiteContents) (projectRoot: string) (page: string) =
  let (ns: XNamespace) =
    !! "http://www.sitemaps.org/schemas/sitemap/0.9"

  let root = XElement(ns + "urlset")

  // let siteInfo =
  //   ctx.TryGetValue<SiteInfo>()
  //   |> Option.defaultWith (fun () -> failwith "You didn't configure the site correctly")

  // let pages =
  //   ctx.TryGetValues<Page>()
  //   |> Option.defaultValue Seq.empty
  //   |> Seq.map (fun page ->
  //     { Frequency = Monthly
  //       LastModified = page.lastModified |> Option.defaultValue DateTime.Now
  //       Priority = 0.5
  //       Url = page.link })
  //   |> Seq.iter (fun node ->
  //       let urlElement = XElement(ns + "url")
  //       root.Add(urlElement))

  // let posts =
  //   ctx.TryGetValues<Post>()
  //   |> Option.defaultValue Seq.empty
  //   |> Seq.toList

  use stringWriter = new Utf8StringWriter()

  let document = XDocument(root)
  document.Save(stringWriter)

  stringWriter.ToString()
