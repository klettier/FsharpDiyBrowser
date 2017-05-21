#I "../packages/FSharp.Data/lib/portable-net45+netcore45"
#r "FSharp.Data.DesignTime.dll"
#r "FSharp.Data.dll"

#load "../FsharpDiyBrowser/Domain.fs"
#load "../FsharpDiyBrowser.HtmlInput/CssParser.fs"
#load "../FsharpDiyBrowser.HtmlInput/HtmlModelConverter.fs"
#load "../FsharpDiyBrowser.Winform/Renderer.fs"
#load "../FsharpDiyBrowser.Winform/UiRunner.fs"

(*
This script should not be run with live coding tool

run > .\build.cmd LiveCode file="LiveCoding/Test2.fsx"

Step 1: render the body with correct children positions
Step 2: parse percents, etc ...
Step 3: try to implement images and hyper links
*)

open System.Drawing
open FsharpDiyBrowser
open FsharpDiyBrowser.HtmlInput
open FsharpDiyBrowser.Winform
open HtmlModelConverter
open Domain
open FSharp.Data
open CssParser
open Renderer
open UiRunner

let source = """<!DOCTYPE html>
<html>
<head>
    <title>Html test 1</title>
    <style type="text/css">
        body
        {
	        font-family: "Times New Roman";
	        font-size: 10px;
	        background-color: #FF0000;
        }
        div.main
        {
            position: absolute;
            background-color: #a0cc9d;
            width: 200px;
            left: 20px;
            font-family: "Arial";
            font-size: 14px;
        }
        div.main span
        {
            color: #00FF00;
        }
    </style>
</head>
<body>
    <div class="main">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Nullam commodo fringilla mollis. 
        Aenean tempor gravida tellus quis elementum. 
        Maecenas finibus lectus id lectus consectetur, id ultricies risus molestie. 
        Nunc vulputate nibh velit, ut posuere arcu hendrerit eget. 
        Cras tincidunt nisl sit amet ultricies dignissim. 
        In consectetur nec odio sollicitudin consequat. 
        Maecenas elit dui, fringilla sed dolor in, scelerisque cursus ipsum. 
        Aliquam risus erat, sollicitudin vel ante vel, tristique venenatis nisl. 
        Morbi in commodo tortor.
    </div>
</body>
</html>"""

let html = HtmlDocument.Parse source
let content = deserialize source

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
   
