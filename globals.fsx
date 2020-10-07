open System.Text.RegularExpressions

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

    sprintf "/posts/%s/%s.html" datePart ``match``.Groups.[2].Value
  else
    sprintf "The post filename %s was not in the correct format!" fileName
    |> failwith
