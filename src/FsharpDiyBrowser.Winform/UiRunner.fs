namespace FsharpDiyBrowser.Winform

module UiRunner =
  open System
  open System.Windows.Forms
  open System.Drawing
  open FsharpDiyBrowser
  open FsharpDiyBrowser.Winform
  open Domain
    
  let renderPageWith (fg:Graphics->Size->unit) (page:Page) =
      printfn "UiRunner"
      let f = new Form()
      f.Text <- ("LiveCoding - " + page.Title)
      f.Width <- 500
      f.Height <- 500
      f.Left <- 0
      f.Top <- 0
      f.BackColor <- Color.White
      f.Paint.Add (fun p -> fg p.Graphics f.Size)
      f.ShowDialog() |> ignore
