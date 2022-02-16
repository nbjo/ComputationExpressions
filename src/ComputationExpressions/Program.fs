open System.IO
open System.Net.Http
open ElevationLib

// For more information see https://aka.ms/fsharp-console-apps
printfn "Understanding async computation expressinons in F#"

let museums =
    [ "MOMA", "http://moma.org/";
    "British Museum", "http://www.thebritishmuseum.ac.uk/";
    "Prado", "http://www.museodelprado.es/" ]

let tprintfn fmt =
    printf "[Thread %d]" System.Threading.Thread.CurrentThread.ManagedThreadId;
    printfn fmt

let fetchAsync(nm, url : string) =
    async {
        tprintfn "Creating request for %s..." nm
        let htmlClient = new HttpClient()
        let! response = htmlClient.GetAsync(url) |> Async.AwaitTask
        
        tprintfn "Getting response stream for %s..." nm
        let stream = response.Content.ReadAsStream()
        
        tprintfn "Reading response for %s..." nm
        let reader = new StreamReader(stream)
        let html = reader.ReadToEnd()
        
        tprintfn "Read %d characters for %s..." html.Length nm
    }

// Async.Parallel [for nm, url in museums -> fetchAsync(nm, url)]
// |> Async.Ignore
// |> Async.RunSynchronously

let f x = x + 2
let result = Option.apply (Some (fun x -> x + 2)) (Some 8)
let get = function
    | Some x -> x
    | None -> 0

printfn $"result = {get result}"; printfn ""

let fList = [(fun x -> x + 2); (fun x -> 2 * x)]
let xList = [4; 8]
let resultList = List.apply fList xList
resultList |> Seq.iter (fun res -> printfn $"res = {res}")


let add x y = x + y

let add4 = add 4

let add4Plus6 = add4  6

let (<+>) = Option.apply

let intermediateRes = Some add <+> Some 4 <+> Some 6

let resultOption =
    (Some add) <+> (Some 2) <+> (Some 3)

printfn ""; printfn $"resultOption = {get resultOption}"

let (<++>) = List.apply
let resultApplyList =
    [add] <++> [1;2] <++> [10;20]

printfn ""; resultApplyList |> Seq.iter (fun resultApply -> printfn $"resultApply = {resultApply}")

let resultOption2 =
    let (<!>) = Option.map
    let (<*>) = Option.apply

    add <!> (Some 2) <*> (Some 3)

printfn ""; printfn $"result = {get resultOption2}"

