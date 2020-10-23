#r "../_lib/Fornax.Core.dll"
#r "../_lib/System.ServiceModel.Syndication.dll"
#if !FORNAX
#load "../loaders/postloader.fsx" "../loaders/globalloader.fsx" "../globals.fsx"
#endif

open System
open System.Xml
open System.ServiceModel.Syndication
open Globals

let generate (ctx: SiteContents) (projectRoot: string) (page: string) =
  let siteInfo =
    ctx.TryGetValue<Globalloader.SiteInfo>()
    |> Option.defaultWith (fun () -> failwith "Whyyyy no site info? :(")

  let feed =
    SyndicationFeed(siteInfo.title, siteInfo.description, Uri siteInfo.basePath)

  feed.Language <- siteInfo.language
  feed.LastUpdatedTime <- DateTimeOffset DateTime.Now

  let author =
    SyndicationPerson(siteInfo.authorEmail, siteInfo.author, siteInfo.basePath)

  feed.Authors.Add(author)

  let posts =
    ctx.TryGetValues<Postloader.Post>()
    |> Option.defaultValue Seq.empty

  let items =
    posts
    |> Seq.filter (fun post -> post.published.IsSome)
    |> Seq.map (fun post ->
         let item =
           SyndicationItem(post.title, post.description, Uri(siteInfo.basePath + post.link))

         item.LastUpdatedTime <- DateTimeOffset post.published.Value
         item)
    |> Seq.toArray

  feed.Items <- items

  use stringWriter = new Utf8StringWriter()

  let writeFeed (stringWriter: Utf8StringWriter) =
    use rssWriter = XmlWriter.Create(stringWriter)

    let rssFormatter = Rss20FeedFormatter(feed, false)
    rssFormatter.WriteTo(rssWriter)

  writeFeed stringWriter

  stringWriter.ToString()
