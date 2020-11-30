open System
open System.IO
open System.Text.RegularExpressions

type Utf8StringWriter() =
  inherit StringWriter()

  override this.Encoding = Text.Encoding.UTF8

let extractUrlContent (fileName: string) =
  let regex = """(\d{4}-\d{2}-\d{2})-(.*).md"""
  let ``match`` = Regex.Match(fileName, regex)

  let result =
    if ``match``.Success then
        let datePart =
          ``match``.Groups.[1].Value
          |> fun p -> p.Split('-') |> List.ofArray
          |> function
          | year :: month :: [ day ] -> sprintf "%s/%s/%s" year month day
          | _ -> failwith "The world just exploded"

        (datePart, ``match``.Groups.[2].Value)
    else
      sprintf "The post filename %s was not in the correct format!" fileName
      |> failwith

  result

let toPostLink (fileName: string) =
#if WATCH
  let suffix = ".html"
#else
  let suffix = String.Empty
#endif

  let (datePart, postName) = extractUrlContent fileName

  sprintf "/posts/%s/%s%s" datePart postName suffix

let toHtmlFileName (fileName: string) =
  let (datePart, postName) = extractUrlContent fileName

  sprintf "/posts/%s/%s%s" datePart postName ".html"
