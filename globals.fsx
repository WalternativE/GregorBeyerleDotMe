open System
open System.IO
open System.Text.RegularExpressions

type Utf8StringWriter() =
  inherit StringWriter()

  override this.Encoding = Text.Encoding.UTF8

let toPostLink (fileName: string) =
  let regex = """(\d{4}-\d{2}-\d{2})-(.*).md"""
  let ``match`` = Regex.Match(fileName, regex)
  if ``match``.Success then
    let datePart =
      ``match``.Groups.[1].Value
      |> fun p -> p.Split('-') |> List.ofArray
      |> function
      | year :: month :: [ day ] -> sprintf "%s/%s/%s" year month day
      | _ -> failwith "The world just exploded"

    #if WATCH
    let suffix = ".html"
    #else
    let suffix = String.Empty
    #endif

    sprintf "/posts/%s/%s%s" datePart ``match``.Groups.[2].Value suffix
  else
    sprintf "The post filename %s was not in the correct format!" fileName
    |> failwith
