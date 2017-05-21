namespace FsharpDiyBrowser.HtmlInput

module HtmlModelConverter =

  open System
  open System.Collections.Generic
  open FSharp.Data
  open CssParser
  open FsharpDiyBrowser.Domain
  
//  type NodeStyle = 
//      { Node:HtmlNode
//        Styles:CssBlock list }
//  type HtmlStyle = NodeStyle list

  let getStyleSheetsContent (html:HtmlDocument) =
      let contents =
          html.Descendants "style"
          |> Seq.collect (fun node -> node.Elements())
          |> Seq.map(fun n -> n.InnerText())
      String.concat Environment.NewLine contents
  
  let associateStyle (html:HtmlDocument) (style:StyleSheet) = // : HtmlStyle =
        seq {
            for block in style.Blocks do
                let nodes = html.CssSelect block.Selector
                for n in nodes do
                    yield (n,block)
        }
        |> Seq.toList
        |> List.groupBy fst
        |> List.map (
            fun (k,vs) ->
                let b = vs |> List.map snd
                k,b
            )
        |> dict
  
  let parseSizeUnits (text:string) =
      if text.EndsWith "px" && text.Length > 2
      then 
          let value = ref 0.f
          if System.Single.TryParse(text.Substring(0, text.Length-2), value)
          then Some (Px (int !value))
          else None
      else None
      
  let (|IsFontWeight|_|) (p:CssProperty) =
      match p with
      | {Name="font-weight"; Value="bold"} -> Some(TextStyle Bold)
      | {Name="font-weight"; Value="normal"} -> Some(TextStyle Regular)
      | _ -> None
  
  let (|Name|_|) name (p:CssProperty) =
      if p.Name = name then Some() else None
  
  let (|IsMetric|_|) name (p:CssProperty) =
      if p.Name = name then parseSizeUnits p.Value else None
  
  let convertCssProperty (p:CssProperty) : Style option =
      match p with
      | IsMetric "margin-right" v -> Some(Margin(Right, v))
      | IsMetric "margin-left" v -> Some(Margin(Left, v))
      | IsMetric "margin-botton" v -> Some(Margin(Botton, v))
      | IsMetric "margin-top" v -> Some(Margin(Top, v))
      | IsMetric "width" w -> Some(Width w)
      | IsMetric "height" h -> Some(Height h)
      | IsMetric "top" y -> Some(Y y)
      | IsMetric "left" x -> Some(X x)
      | Name "color" -> Some(TextColor (HexaColor p.Value)) //TODO: parse all color syntaxes
      | Name "background-color" -> Some(BackgroundColor (HexaColor p.Value)) //TODO: parse all color syntaxes
      | Name "font-family" -> Some(FontFamily p.Value)
      | IsFontWeight w -> Some w
      | IsMetric "font-size" units -> Some(FontSize units)
      | _ -> None
  
  let convertStyle (styles:CssBlock list) =
      styles 
      |> Seq.collect (fun style -> style.Properties) 
      |> Seq.choose convertCssProperty
      |> Seq.toList
  
  let deserialize (source:string) =
    let rec loop (acc:DomElement list) (styles:IDictionary<HtmlNode, CssBlock list>) (node:HtmlNode) =
      let children = node.Elements() |> Seq.collect(loop [] styles) |> Seq.toList
      let style = if styles.ContainsKey node then convertStyle(styles.Item node) else []
      match node.Name() with
      | "body" -> children
      | "div"
      | "span" -> Box (children, style) :: acc
      | "h1" -> Title (1, node.InnerText()) :: acc
      | name when String.IsNullOrWhiteSpace name ->
          Text (node.InnerText()) :: acc
      | name -> failwithf "cannot handle %s tag yiet !" name //TODO: default could be a box or text node ?
      
    let html = HtmlDocument.Parse source
    let css = html |> getStyleSheetsContent |> parseCss
    let nodesStyles = associateStyle html css
    html.Descendants "body" |> Seq.head |> loop [] nodesStyles

