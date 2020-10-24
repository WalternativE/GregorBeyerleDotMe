#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "../globals.fsx"

open Markdig
open System

type PostConfig = { disableLiveRefresh: bool }

type Post =
  { file: string
    link: string
    title: string
    description: string
    author: string option
    published: DateTime option
    last_modified: DateTime option
    tags: string list
    content: string
    summary: string
    pinned: bool
    large_image: string
    image_attribution_link: string option
    image_attribution_text: string option }

let contentDir = "posts"

let markdownPipeline =
  MarkdownPipelineBuilder()
    .UsePipeTables()
    .UseGridTables()
    .UseGenericAttributes()
    .UseEmphasisExtras()
    .UseListExtras()
    .UseCitations()
    .UseCustomContainers()
    .UseFigures()
    .Build()

let isSeparator (input: string) = input.StartsWith "---"

let isSummarySeparator (input: string) = input.Contains "<!--more-->"

///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns content of config that should be used for the page
let getConfig (fileContent: string) =
  let fileContent = fileContent.Split '\n'
  let fileContent = fileContent |> Array.skip 1 //First line must be ---

  let indexOfSeperator =
    fileContent |> Array.findIndex isSeparator

  let splitKey (line: string) =
    let seperatorIndex = line.IndexOf(':')
    if seperatorIndex > 0 then
      let key =
        line.[..seperatorIndex - 1].Trim().ToLower()

      let value = line.[seperatorIndex + 1..].Trim()
      Some(key, value)
    else
      None

  fileContent
  |> Array.splitAt indexOfSeperator
  |> fst
  |> Seq.choose splitKey
  |> Map.ofSeq

///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns HTML version of content of the page
let getContent (fileContent: string) =
  let fileContent = fileContent.Split '\n'
  let fileContent = fileContent |> Array.skip 1 //First line must be ---

  let indexOfSeperator =
    fileContent |> Array.findIndex isSeparator

  let _, content =
    fileContent |> Array.splitAt indexOfSeperator

  let summary, content =
    match content |> Array.tryFindIndex isSummarySeparator with
    | Some indexOfSummary ->
        let summary, _ = content |> Array.splitAt indexOfSummary
        summary, content
    | None -> content, content

  let summary =
    summary |> Array.skip 1 |> String.concat "\n"

  let content =
    content |> Array.skip 1 |> String.concat "\n"

  Markdown.ToHtml(summary, markdownPipeline), Markdown.ToHtml(content, markdownPipeline)

let trimString (str: string) = str.Trim().TrimEnd('"').TrimStart('"')

let loadFile n =
  let text = System.IO.File.ReadAllText n

  let config = getConfig text
  let summary, content = getContent text

  let file =
    System.IO.Path.Combine(contentDir,
                           (n |> System.IO.Path.GetFileNameWithoutExtension)
                           + ".md").Replace("\\", "/")

  let link = file |> Globals.toPostLink

  let title = config |> Map.find "title" |> trimString

  let description =
    config |> Map.find "description" |> trimString

  let author =
    config
    |> Map.tryFind "author"
    |> Option.map trimString

  let published =
    config
    |> Map.tryFind "published"
    |> Option.map (trimString >> System.DateTime.Parse)

  let lastModified =
    config
    |> Map.tryFind "last_modified"
    |> Option.map (trimString >> System.DateTime.Parse)

  let pinned =
    config
    |> Map.tryFind "pinned"
    |> Option.map (fun _ -> true)
    |> Option.defaultValue false

  let tags =
    let tagsOpt =
      config
      |> Map.tryFind "tags"
      |> Option.map
           (trimString
            >> fun n -> n.Split ',' |> Array.toList)

    defaultArg tagsOpt []

  let largeImage =
    config
    |> Map.find "large_image"
    |> trimString

  let imageAttributionLink =
    config
    |> Map.tryFind "image_attribution_link"
    |> Option.map trimString

  let imageAttributionText =
    config
    |> Map.tryFind "image_attribution_text"
    |> Option.map trimString

  { file = file
    link = link
    title = title
    description = description
    author = author
    published = published
    last_modified = lastModified
    tags = tags
    content = content
    summary = summary
    pinned = pinned
    large_image = largeImage
    image_attribution_link = imageAttributionLink
    image_attribution_text = imageAttributionText }

let loader (projectRoot: string) (siteContent: SiteContents) =
  let postsPath =
    System.IO.Path.Combine(projectRoot, contentDir)

  System.IO.Directory.GetFiles postsPath
  |> Array.filter (fun n -> n.EndsWith ".md")
  |> Array.map loadFile
  |> Array.sortByDescending (fun post -> post.published)
  |> Array.iter siteContent.Add

#if WATCH
  let disableLiveRefresh = false
#else
  let disableLiveRefresh = true
#endif

  siteContent.Add({ disableLiveRefresh = disableLiveRefresh })
  siteContent
