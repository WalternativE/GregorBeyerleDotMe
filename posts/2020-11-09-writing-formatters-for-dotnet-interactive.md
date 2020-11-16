---
layout: post
title: Writing Fromatter Extensions for .NET Interactive
description: .NET interactive is a pretty new and exiting way to do explorative development with F#. One important thing about exploration is the visual inspection of your outputs. What fields are in those records? What's the content of this list? How would this data look in a bar chart or in a scatter plot? All questiosn we can answer by looking at outputs. But how does .NET interactive know how to display these outputs for us in a form, that tells us what we need to know? In many cases (most cases even when you look at how big the .NET ecosystem is) it simply doesn't. But that's ok because we have the tools to write our own formatters and share them with the rest of the world.
author: @GBeyerle
published: 2020-11-15
pinned: true
large_image: "/images/posts/some-blog-post/BlogHeaderPic.jpg"
---

.NET interactive is a pretty new and exiting way to do explorative development with F#. One important thing about exploration is the visual inspection of your outputs. What fields are in those records? What's the content of this list? How would this data look in a bar chart or in a scatter plot? All questiosn we can answer by looking at outputs. But how does .NET interactive know how to display these outputs for us in a form, that tells us what we need to know? In many cases (most cases even when you look at how big the .NET ecosystem is) it simply doesn't. But that's ok because we have the tools to write our own formatters and share them with the rest of the world.

<!--more-->

## <a name="interactive-programming">Interactive Programming</a>

Many programming languages offer interactive environments for trying out code and looking at simple outputs. Those Read Evaluate Print Loops (REPLs) are staples of languages like Python, Julia, R and F#. In the general .NET ecosystem (which is pretty C# heavy) interactive programming hasn't been a thing, really. With the advent of [.NET interacitve](https://github.com/dotnet/interactive) this has begun to change. Jupyter Notebooks - interactive, web based, coding environments, that allow to mix prose, source code and formatted outputs - have been the defacto standard to communicate experiments within the Data Science and Machine Learning community for a while now.

With its latest push to make .NET a workable target for Machine Learning projects, Microsoft has shown great commitment to make all common .NET languages work well in Jupyter Notebooks. It even went a step beoyond and started building great tooling for VSCode, that makes it possible to run and edit .NET interactive notebooks directly in the editor. Their [latest blog post](https://devblogs.microsoft.com/dotnet/net-interactive-preview-3-vs-code-insiders-and-polyglot-notebooks/) - as of writng this - shows how to get started with .NET interactive in VSCode Insiders. I highly encourage you to try it out! While you're at it you can also check out the [nteract desktop application](https://nteract.io/applications) which was one of the first apps I'm aware of, that allowed people to have a more integrated development exprience while working with interactive notebooks. All examples in this blog post can be found in [this repo](https://github.com/WalternativE/WritingDotNetInteractiveFormatters) in case you might want to experiment with the code yourself.

## <a name="default-formatting">Default Formatting</a>

So when you write code, what do you usually do? I would guess that you write some functions, apply them to some values and get something back from those expression. As a F# developer those things are usually records, discriminated unions or Plain Old CLR Objects (POCOs). Because these types are so commonly used .NET interactive includes some sensible default formatting strategies. Let's look at a simple record:

```fsharp
type Person =
    { Name: string
      Age: int }

let alice = { Name = "Alice"
              Age = 33 }

alice
```

If a value is returned without being bound to a name (or without being ignored) .NET interactive will display it. We could also use the `display` function to get to the same output.

![The standard format of a simple record](/images/posts/writing-formatters-for-dotnet-interactive/simple_record_formatted.jpg)

This tabular view makes complete sense for records. It puts all the member names in the header and displays the values as a row. Even if you're not used to look at tabular data the whole day it is still understandable. So what about nested records? In the F# world we are pretty fond of composing more complicated data structures out of small and simple records. How would .NET interactive handle the following?

```fsharp
type Car =
    { Make: string
      Owner: Person }

let alicesCar = { Make = "Toyota"; Owner = alice }

alicesCar
```
![The standard format of a nested record](/images/posts/writing-formatters-for-dotnet-interactive/nested_record_formatted.jpg)

Nice! Displaying nested tables would be odd so .NET interactive was so kind to display the nested record similar to how we constructed it in code. So what about POCOs? In F# it sometimes makes sense to define a plain old class or struct. How does .NET interactive handle those kind of objects?

```fsharp
type Dog(name: string, isGoodBoy: bool) =
    member _.Name = name
    member _.IsGoodBoy = isGoodBoy

let henry = Dog("Henry", true)

henry
```

![The standard format of a plain old clr object](/images/posts/writing-formatters-for-dotnet-interactive/simple_poco_formatted.jpg)

This looks exactly as the output for the simple record, doesn't it? Well it does because it uses the same formatter. Records are just POCOs with a bit of compiler magic sprinkeled on top. As we'll see with the next example of a nested POCO this extra compiler magic pays of in interactive programming environments (besides being great in general).


```fsharp
type DogHotel(name: string, inhabitants: Dog list) =
    member _.Name = name
    member _.Inhabitant = inhabitants

let hiltonForDogs = DogHotel("Hilton For Dogs", [ henry ])

hiltonForDogs
```

![The standard format of a nested clr object](/images/posts/writing-formatters-for-dotnet-interactive/nested_poco_formatted.jpg)

That doesn't look super nice, does it? Well it looks like this because .NET interactive doesn't try to be extremely smart about displaying objects. It just goes through the top level properties, displays them in the table and basically calls `ToString` on the values. This works well for F# records - the compiler magic - but not for POCOs. This can be resolved at the type level, though, by overriding `ToString` no need for a new formatter. Let's look at Discriminated Unions next.

```fsharp
type Fruit =
    | Orange
    | Banana
    | Apple

Apple
```

For simple case identifiers without any data .NET interacitve just displays its name. In this case we will read `Apple`. How would that differ for more complicated discriminated unions?

```fsharp
type GiftBasket =
    | EmptyBasket
    | FruitBasket of Fruits: Fruit list
    | SpoiledFruitBasket of Fruit list

let aNiceGiftBasket = FruitBasket [ Orange; Orange; Banana; Banana; Banana; Apple ]
let aNotVeryNiceGiftBasket = SpoiledFruitBasket [ Orange; Orange; Banana; Banana; Banana; Apple ]

display aNiceGiftBasket
display aNotVeryNiceGiftBasket
display [ aNiceGiftBasket; aNotVeryNiceGiftBasket ]
```

![Different ways to format discriminated unions with data using the dotnet interactive standard formatter](/images/posts/writing-formatters-for-dotnet-interactive/du_withdata_formatted.jpg)

There's a bit to unpack here. The first line displays the data of the nice gift basket which is a `FruitBasket`. We get the descriptive `Fruits` table header because that's the name we gave to the field. For the `SpoiledFruitBasket` we did not specify this field name so we'll get the standard `Item` label. It seems a bit odd to me, that we don't get to see which case identifier we're currently looking at. It gets even more odd when we see that the standard formatter dispalys the case identifier types correctly for lists. I'm not entirely sure why that's the case but I'll use this oddity to show how to register custom formatters.

## <a name="simple-plain-text-formatter">Simple Plain Text Formatter</a>

For me it should be super obvious whether we're looking at `FruitBasket` or a `SpoiledFruitBasket`. So - only for this very special discriminated union - we're going to register a formatter, that displays its (very sensible) standard F# string representation. .NET interactive comes out-of-the-box with all the tools to achieve this. Let's take a look.

```fsharp
module GiftBasketFormatter =

    open System.Text

    Formatter.SetPreferredMimeTypeFor(typeof<GiftBasket> ,"text/plain")

    Formatter.Register<GiftBasket>(Func<FormatContext, GiftBasket, TextWriter, bool>(fun context basket writer ->
        let formatted = sprintf "%A" basket
        writer.Write(formatted)

        true), "text/plain")

    // alternatively much simpler
    // Formatter.Register<GiftBasket>((fun basket -> sprintf "%A" basket), "text/plain")
```

We don't really need to create a module here but it makes sense to use one if you don't want to pollute your notebook scope with the extra `open` statement. We can access the `Formatter` class because .NET interactive loads a couple of assemblies in the background which make sense to have in an interactive session. We can use it to set the preferred MIME type for the type we wish to format. It works kind of like content negotiation: .NET interactive gets the request to display a value, looks at its type, checks its default MIME type and selects the fitting formatter for the MIME type.

Now that we specified, that `GiftBasket` values should be formatted as plaintext we can register a formatter. The `Register` method offers different overloads, one taking an `ITextFormatter` instace while the other allows us to specify a `Func` delegate with the signature `FormatContext -> 'T -> TextWriter -> bool` where `'T` is the type we wan to format. The syntax for specifying this in F# is a bit verbose for this overload but I default to it because it gives me the most control over formatting. We could also use the simpler `'T -> string` delegate in this case. In most cases we are also fine by not explicitly creating the function delegate and defaulting to the much nicer F# lambda expression. I had problems with type inferences in some edge cases so your mileage may vary 🤷‍♂️

With the new formatter in place we can try out to display the different `GiftBasket` values again.

```fsharp
display aNiceGiftBasket
display aNotVeryNiceGiftBasket
```

![Discriminated unions formatted with our custom formatter](/images/posts/writing-formatters-for-dotnet-interactive/du_withdata_formatted_custom.jpg)

Much better now! This example was - to be totally honest - not very useful, though. With the basics out of the way we can look at a more useful example.

## <a name="charting-with-plotly-net">Charting with Plotly.NET</a>

On my search for a plotting library, that actually allows subplots (which `XPlot.Plotly` doesn't) I stumbled upon `Plotly.NET`. It started out (as many great sciency F# libraries) at the institute for [Computational Systems Biology - CSB Kaiserslautern](https://github.com/CSBiology) but was moved to the [offical Plotly organization](https://github.com/plotly/Plotly.NET) some time ago. The current "approved" version is still in alpha but in general it feels much more mature than that.

I often use plots for looking at the distribution of a dataset. Let's say we roll a 'random' die in .NET and want to see how many ones and twos and threes and fours and sixes we have rolled and take in the data in a visual appealing way I'd go and look at a histogram for that. In `Plotly.NET`, that's pretty easy to do.

```fsharp
open System
open Plotly.NET

let rollUniformDice (rnd: Random) =
    rnd.Next(1, 7)
    |> int

let rnd = Random(Seed = 1)
let diceRolls = List.init 1000 (fun _ -> rollUniformDice rnd)

diceRolls
|> Chart.Histogram
```

If we would be using `XPlot.Plotly` we'd be looking at a nice histogram by now. That's the case because the .NET Interactive developers wrote a formatter for us. They obviously didn't do so for `Plotly.NET` and so we only get to see some object properties and nothing else. What a shame! Good, that we learned how to write a custom formatter! Similar to the implementation for `XPlot.Plotly` the library has basically already done all the leg work for us. To get the whole thing working I basically had to call a single method - that's it. At least it would be if there weren't a bug, that - as of writing this - breaks a piece of JavaScript in VSCode. That's also not too hard to monkeypatch, though.

```fsharp
module PlotlyFormatter =
    open System.Text
    open GenericChart

    Formatter.Register<GenericChart>((fun chart writer ->
        let html = toChartHTML chart
        let hackedHtml =
            html.Replace(
                """var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}})""",
                """var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}}) || require;"""
            )
        writer.Write(hackedHtml)), HtmlFormatter.MimeType)
```

When we try to display the same plotly chart from before we'll see a nice interactive histogram. Neat!

![The output for a Plotly.NET chart with a custom formatter](/images/posts/writing-formatters-for-dotnet-interactive/plotly_output_with_formatter.jpg)
