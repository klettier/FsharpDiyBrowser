#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.UserInputHelper
open System
open System.Diagnostics
open System.IO

Target "LiveCode" (fun _ ->
    //let files = !! "LiveCoding/**/*.fsx"
    let files = !! "**/*.fsx" ++ "**/*.fs"
    use watcher = files |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        for a in changes do
            async {
                let processlist = Process.GetProcesses()
                for p in processlist do
                    
                    if p.MainWindowTitle.StartsWith "LiveCoding"
                    then p.Kill()
                let folder = __SOURCE_DIRECTORY__ @@ "LiveCoding"
                let runner = folder @@ "UiRunner.fsx"
                if (Path.GetDirectoryName a.FullPath) = folder
                then 
                    let (succeed, logs) = Fake.FSIHelper.executeFSI __SOURCE_DIRECTORY__ a.FullPath []
                    if not succeed
                    then
                        logs 
                        |> Seq.filter(fun l -> l.IsError)
                        |> Seq.iter(fun l -> traceError l.Message)
            } |> Async.StartAsTask |> ignore
    )

    printfn "Press any key to quit ..."
    ignore <| Console.ReadKey true
    
    watcher.Dispose() // Use to stop the watch from elsewhere, ie another task.
)


//"BuildPackage"
//  ==> "PublishNuget"
//  ==> "Release"

RunTargetOrDefault "LiveCode"

