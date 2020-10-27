#r "../_lib/Fornax.Core.dll"

open System.IO
open System.Diagnostics

let generate (ctx: SiteContents) (projectRoot: string) (page: string) =
  let inputPath = Path.Combine(projectRoot, page)
  let outputPath = Path.GetTempFileName()

#if WATCH
  let extraSassArgs = "--embed-source-map"
#else
  let extraSassArgs = "--style=compressed --no-source-map"
#endif

  let psi = ProcessStartInfo()
  psi.FileName <- "sass"
  psi.Arguments <- sprintf "%s %s %s" extraSassArgs inputPath outputPath
  psi.CreateNoWindow <- true
  psi.WindowStyle <- ProcessWindowStyle.Hidden
  psi.UseShellExecute <- true

  try
    let proc = Process.Start psi
    proc.WaitForExit()
    let output = File.ReadAllText outputPath
    File.Delete outputPath
    output
  with ex ->
    printfn "EX: %s" ex.Message

    printfn
      "Please check you have installed the Sass compiler if you are going to be using files with extension .scss. https://sass-lang.com/install"

    System.String.Empty
