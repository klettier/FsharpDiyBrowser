namespace FsharpDiyBrowser

module Domain =

  let findWithDefault (predicate:'t->'v option) (d:'v) (items:'t list) : 'v =
      match items |> List.choose predicate |> List.tryHead with
      | Some v -> v | None -> d

  type PhysicalSize = float32
  type SizeUnits =
      | Px of int //int<px>
      | Cm of int
      | Inch of int
      | Percent of int

  type PositionInfo = | Left | Right | Top | Botton
  type Color =
      | HexaColor  of string
      | RgbColor   of int * int * int
      | RgbaColor  of int * int * int * int

  type FontStyle = 
      | Regular | Bold | Italic | Strikeout | Underline

  type Style =
      | Margin of PositionInfo * SizeUnits
      | Width of SizeUnits
      | Height of SizeUnits
      | X of SizeUnits
      | Y of SizeUnits
      | BackgroundColor of Color
      | TextColor of Color
      | FontSize of SizeUnits
      | FontFamily of string
      | FontWeight of FontStyle
    
  type ImageSource =
      | ImageUrl of string
      | ImageContent of byte array

  type DomElement =
      | Box of Children * Style list
      | Paragraph of Children * Style list
      | Text of string
      | Image of ImageSource
      | Title of degree:int * string
      | BulletedList of BulletedListItem list
      member __.GetChildren () =
          match __ with
          | Box (xs,_)
          | Paragraph (xs,_) -> xs
          | BulletedList xss -> xss |> List.collect(fun xs -> xs)
          | _ -> []
      member __.GetStyles () =
          match __ with
          | Box (_,xs)
          | Paragraph (_,xs) -> xs
          | _ -> []
      member __.GetMargin(p:PositionInfo) =
           __.GetStyles ()
          |> findWithDefault (function | Margin (pos,v) when p = pos -> Some v | _ -> None) (Px 0)
      member __.GetMarginTop() = __.GetMargin Top
      member __.GetMarginLeft() = __.GetMargin Left
      member __.GetMarginRight() = __.GetMargin Right
      member __.GetMarginBotton() = __.GetMargin Botton
    
  and Children = DomElement list
  and BulletedListItem = Children

  let getFont defaultSize defaultFontFace defaultFontWeight (styles:Style list) =
      let size = styles |> findWithDefault (function | FontSize s -> Some s | _ -> None) defaultSize
      let face = styles |> findWithDefault (function | FontFamily face -> Some face | _ -> None) defaultFontFace
      let weight = styles |> findWithDefault (function | FontWeight w -> Some w | _ -> None) defaultFontWeight
      size, face, weight

  type Page = 
      { Title:string
        Header:DomElement list
        Footer:DomElement list
        Content:DomElement list
        DefaultStyles : Style list }
      static member Empty =
          { Title=""; Header=[]; Footer=[]; Content=[]; DefaultStyles=[] }

