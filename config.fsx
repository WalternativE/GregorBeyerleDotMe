#r "_lib/Fornax.Core.dll"
#load "globals.fsx"

open Config
open System.IO
open Globals

let postPredicate (projectRoot: string, page: string) =
  let fileName = Path.Combine(projectRoot, page)
  let ext = Path.GetExtension page

  if ext = ".md" then
    let ctn = File.ReadAllText fileName
    ctn.Contains("layout: post")
    ctn.Contains("published:")
  else
    false

let staticPredicate (projectRoot: string, page: string) =
  let ext = Path.GetExtension page

  (page.Contains "_public"
   || page.Contains "_bin"
   || page.Contains "_lib"
   || page.Contains "_data"
   || page.Contains "_settings"
   || page.Contains "_config.yml"
   || page.Contains ".sass-cache"
   || page.Contains ".git"
   || page.Contains ".ionide"
   || page.Contains "node_modules"
   || page.Contains ".scss"
   || page.Contains ".config"
   || page.Contains ".editorconfig"
   || page.Contains "global.json"
   || page.Contains "package.json"
   || page.Contains "package-lock.json"
   || ext = ".fsx")
  |> not

let scssPredicate (projectRoot: string, page: string) = page.Contains "main.scss"

let config =
  { Generators =
      [ { Script = "sass.fsx"
          Trigger = OnFilePredicate scssPredicate
          OutputFile = ChangeExtension "css" }
        { Script = "post.fsx"
          Trigger = OnFilePredicate postPredicate
          OutputFile = Custom(fun s -> s |> toPostLink |> fun s -> s.TrimStart('/')) }
        { Script = "staticfile.fsx"
          Trigger = OnFilePredicate staticPredicate
          OutputFile = SameFileName }
        { Script = "index.fsx"
          Trigger = Once
          OutputFile = NewFileName "index.html" }
        { Script = "about.fsx"
          Trigger = Once
          OutputFile = NewFileName "about.html" }
        { Script = "blog.fsx"
          Trigger = Once
          OutputFile = NewFileName "blog.html" }
        { Script = "imprint.fsx"
          Trigger = Once
          OutputFile = NewFileName "imprint.html" }
        { Script = "sitemap.fsx"
          Trigger = Once
          OutputFile = NewFileName "sitemap.xml" }
        { Script = "rss.fsx"
          Trigger = Once
          OutputFile = NewFileName "blogrssfeed.xml" } ] }
