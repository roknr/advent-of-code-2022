open System.IO

let input = File.ReadAllText("./input.txt")

let findMarker (datastreamBuffer: string) (markerLength: int) =
    seq { (markerLength - 1) .. (datastreamBuffer.Length - 1) }
    |> Seq.find (fun index -> Set(datastreamBuffer[(index - (markerLength - 1)) .. index]).Count = markerLength)
    |> (fun index -> index + 1)

printfn "%d" (findMarker input 4)
printfn "%d" (findMarker input 14)