#load "../FsharpDiyBrowser/Domain.fs"
#load "../FsharpDiyBrowser.Winform/Renderer.fs"
#load "../FsharpDiyBrowser.Winform/UiRunner.fs"

open System
open System.Windows.Forms
open System.Drawing
open FsharpDiyBrowser
open FsharpDiyBrowser.Winform
open Domain
open Renderer
open UiRunner

let content = [
        Box (
            [
                Text "Hello, I am rendering HTML in a custom engine implementation"
                Text "Twitter"
                Box (
                    [Text "@rflechner"],
                    [
                        BackgroundColor (HexaColor "#00CC00")
                        Width (Percent 100)
                        Height (Px 10)
                        FontFamily "Arial"
                        FontSize (Px 12)
                        Margin(Left, (Px 5))
                        Margin(Right, (Px 5))
                    ]
                )
            ],
            [
                BackgroundColor (HexaColor "#FF0000")
                Width (Percent 40)
                FontFamily "Comic Sans MS"
                FontSize (Px 20)
                Margin(Top, (Px 15))
                Margin(Left, (Px 10))
                Y (Px 20)
            ]
            )
        Box ([], [
                    BackgroundColor (HexaColor "#0000CC")
                    Height (Px 10)
                    Width (Px 100)
                    Margin(Top, (Px 20)) // TODO: fix the bug
                 ])
         // TODO: implement padding, floating rigth and left, lists (ul, li), images, hyperlinks
    ]

let page = 
   { Page.Empty with 
       Title="Test 1"; 
       Content=content
       DefaultStyles = 
           [
               FontFamily "Arial"
               FontSize (Px 10)
               BackgroundColor (HexaColor "#FFFFFF")
           ]
   }
   
let font = Renderer.Defaults.DefaultFont page
page
|> renderPageWith (
    fun g size ->
        
        let parentRect = new RectangleF(PointF.Empty, new SizeF(float32 size.Width, float32 size.Height))
        render page.Content g parentRect font |> ignore
   )
   

