open System.Reflection
open System.Collections.Generic
open System

let asm = Assembly.GetExecutingAssembly()

let referencedAssemblyMap (assembly:Assembly)  = 
    let map = new Dictionary<Assembly, Assembly[]>()
    let rec getAssem (ass:Assembly) = 
        let assemblies = 
            ass.GetReferencedAssemblies()
            |> Array.map (fun name -> Assembly.ReflectionOnlyLoad name.FullName) 
        map.Add (ass, assemblies)
        for assembly in assemblies |> Seq.filter (fun x -> not (map.ContainsKey x)) do 
            getAssem assembly
    getAssem assembly
    map

let referenced = referencedAssemblyMap asm

let outputLines (input:Dictionary<Assembly, Assembly[]>) = 
    let linesForEntry (entry:KeyValuePair<Assembly, Assembly[]>) =
        entry.Value 
        |> Array.map (fun value -> sprintf "\"%s\" -> \"%s\";" entry.Key.FullName value.FullName)
    let allLines = Seq.map linesForEntry input
    seq {
    yield "digraph G {"
    for lines in allLines do
        yield! lines
    yield "}"    }

let output = outputLines referenced

output |> Seq.iter (fun x -> printfn "%s" x)



