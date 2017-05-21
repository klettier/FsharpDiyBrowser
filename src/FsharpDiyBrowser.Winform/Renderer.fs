namespace FsharpDiyBrowser.Winform

module Renderer =

  open System
  open System.Windows.Forms
  open System.Drawing
  open FsharpDiyBrowser
  open Domain

  type RenderColor = System.Drawing.Color

  let convertFontStyle =
      function
      | Regular -> System.Drawing.FontStyle.Regular 
      | Bold -> System.Drawing.FontStyle.Bold
      | Italic -> System.Drawing.FontStyle.Italic
      | Strikeout -> System.Drawing.FontStyle.Strikeout
      | Underline -> System.Drawing.FontStyle.Underline

  let reverseFontStyle =
      function
      | System.Drawing.FontStyle.Regular -> Regular 
      | System.Drawing.FontStyle.Bold -> Bold
      | System.Drawing.FontStyle.Italic -> Italic
      | System.Drawing.FontStyle.Strikeout -> Strikeout
      | System.Drawing.FontStyle.Underline -> Underline
      | _ -> Regular

  let getFont defaultSize defaultFontFace defaultFontWeight (styles:Style list) =
      let size, face, weight = Domain.getFont defaultSize defaultFontFace defaultFontWeight styles
      let s = 
          match size with
          | Px px -> px
          | _ -> failwith "TODO !"
      new Font(face, float32 s, (convertFontStyle weight))

  module Defaults =
    let DefaultFont page = getFont (Px 12) "Times New Roman" Domain.FontStyle.Regular page.DefaultStyles
  
  let overrideFont (font:Font) =
      getFont (Px (int font.Size)) font.FontFamily.Name (reverseFontStyle font.Style)

  let convertToPhysicalSize (containerSize:PhysicalSize) =
      function
      | Px w -> float32 w
      | Percent p ->
          if p <= 0 || containerSize <= 0.f
          then 0.f
          else containerSize * (float32 p) / 100.f
      | _ -> failwith "not yiet implemented !"

  let getPhysicalWith (item:DomElement) (containerWidth:PhysicalSize) =
      item.GetStyles() 
      |> findWithDefault (function | Width w -> Some w | _ -> None) (Percent 100)
      |> convertToPhysicalSize containerWidth

  let computeTextHeight (g:Graphics) (text:string) (w:float32) font =
      let s = g.MeasureString(text, font, int w)
      s.Height

  let computeBoxingHeight (item:DomElement) containerHeight =
      item.GetStyles() 
      |> findWithDefault (function | Height w -> Some w | _ -> None) (Px 0)
      |> convertToPhysicalSize containerHeight

  let calulateHeight (g:Graphics) (item:DomElement) (w:float32) font (parentRect:RectangleF) =
      let rec loop (n:DomElement) h font =
          let bh = computeBoxingHeight n parentRect.Height
          let th =
              match n with
              | Text text -> computeTextHeight g text w font
              | _ -> 0.f
          let ch = n.GetChildren() |> List.sumBy (fun c -> loop c h font) 
          bh + th + ch
      loop item 0 font

  let convertColor (color:Domain.Color) : RenderColor =
      match color with
      | HexaColor c -> ColorTranslator.FromHtml c
      | RgbColor (r,g,b) -> Color.FromArgb(r,g,b)
      | RgbaColor (r,g,b,a) -> Color.FromArgb(a,r,g,b)

  let drawText (g:Graphics) text (rect:RectangleF) font color =
      use brush = new SolidBrush(color)
      g.DrawString(text, font, brush, rect)
      let s = g.MeasureString(text, font, rect.Size)
      RectangleF(rect.Location, s)
    
  let getBackgroundColor (item:DomElement) =
      item.GetStyles() |> findWithDefault (function | BackgroundColor c -> Some (convertColor c) | _ -> None) (Color.Transparent)
    
  let getTop (item:DomElement) (containerSize:PhysicalSize) =
      item.GetStyles() |> findWithDefault (function | Y y -> Some y | _ -> None) (Px 0)
      |> convertToPhysicalSize containerSize
    
  let getLeft (item:DomElement) (containerSize:PhysicalSize) =
      item.GetStyles() |> findWithDefault (function | X x -> Some x | _ -> None) (Px 0)
      |> convertToPhysicalSize containerSize

  let getMargin pi (item:DomElement) (containerSize:PhysicalSize) =
      item.GetMargin(pi) |> convertToPhysicalSize containerSize
        
  let rec renderItem (element:DomElement) (g:Graphics) (mainRect:RectangleF) (minY:float32) font : RectangleF =
      let render (g:Graphics) (parentRect:RectangleF) font (elements:DomElement list) =
          elements
          |> List.fold (fun (r:RectangleF) e ->
              let r = renderItem e g r r.Height font
              RectangleF(parentRect.X, r.Bottom, parentRect.Width, parentRect.Height)
              ) parentRect
      use brush = new SolidBrush(Color.Black)
      use pen = new Pen(Color.Black, 1.0f)
      let ew = getPhysicalWith element mainRect.Width
      let bgColor = getBackgroundColor element
      let nfont = element.GetStyles() |> overrideFont font
      match element with
      | Text text ->
          let r = new RectangleF(float32 mainRect.Left, float32 mainRect.Top, float32 mainRect.Width, float32 mainRect.Height)
          drawText g text r nfont (Color.Black)
      | _ ->
          brush.Color <- bgColor
          let h = calulateHeight g element ew nfont mainRect
          let left = getLeft element mainRect.Width
          let top = getTop element mainRect.Height
          let marginTop = getMargin Top element mainRect.Height
          let marginLeft = getMargin Left element mainRect.Width
          let marginRight = getMargin Right element mainRect.Width
          let x = left + marginLeft + mainRect.X
          let y = top + marginTop + minY
          let w = min mainRect.Right (ew - marginLeft - marginRight)   
          let r = new RectangleF(x,y,w,h)
          g.FillRectangle(brush, r)
          element.GetChildren() |> render g r nfont

  let render (elements:DomElement list) (g:Graphics) (parentRect:RectangleF) defaultFont =
      elements
      |> List.fold (fun (r:RectangleF) e ->
          renderItem e g r r.Height defaultFont
          ) (RectangleF(parentRect.Location, new SizeF(parentRect.Width, 0.f)))
