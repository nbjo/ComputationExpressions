open System.IO
open System.Net.Http
open ElevationLib
open System

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

let divide ifZero ifSuccess top bottom =
    if (bottom = 0)
    then ifZero()
    else ifSuccess (top/bottom)

type UpDownEvent = Incr | Decr
type View = IObservable<UpDownEvent>
let subject = Event<UpDownEvent>()
type Model = { mutable State : int }
type Controller = Model -> UpDownEvent -> unit

let model = { State = 6 }

type Mvc = Controller -> Model -> View -> IDisposable

let raiseEvents xs = List.iter subject.Trigger xs
let view = subject.Publish

let controller model event = 
    match event with
    | Incr -> model.State <- model.State + 1
    | Decr -> model.State <- model.State - 1

let mvc : Mvc = fun controller model view -> 
    view.Subscribe (fun event ->
        controller model event
        printfn "Model: %A" model)

let subscription = view |> mvc controller model
raiseEvents [ Incr; Decr; Incr ]
subscription.Dispose()

let ifZero() = None
let ifSuccess x = Some (x)


let divideWithZeroHandling = divide ifZero ifSuccess

let divWithZero = divideWithZeroHandling 10 0
let divWithNonZero = divideWithZeroHandling 12 3


let (|DivisibleBy|_|) by n =
    if n % by = 0 then Some DivisibleBy else None

let tellMeIfDivisibleBy by n = 
    match n with
    | DivisibleBy by -> printfn $"Yes, {n} is divisible by {by}"
    | _ -> printfn $"No, no, {n} is NOT divisible by {by}"

tellMeIfDivisibleBy 6 12
