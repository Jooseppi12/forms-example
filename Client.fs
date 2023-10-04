namespace test_forms

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.Sitelets
open WebSharper.Forms

[<JavaScript>]
module Client =
    
    type State =
        | Form1
        | Form2 of Form2Case
    
    and Form2Case =
        | Case1 of string
        | Case2 of int

    and Form1Data =
        {
            String: string
            Int: int
        }

        static member Create (s: string) (i: int) = { String = s; Int = i}

    // Alternatively you can create a simple Var of the State if you don't want to create a router that affects 
    let router =
        Router.Infer<State> ()
        |> Router.InstallHash State.Form1
        

    let Form1 (init: Form1Data) =
        Form.Return Form1Data.Create
        <*> (Form.Yield init.String)
        <*> (Form.Yield init.Int)
        |> Form.WithSubmit
        |> Form.Run (fun data ->
            if data.String.Trim() <> "" then
                Case1 data.String |> State.Form2 |> router.Set
            else
                Case2 data.Int |> State.Form2 |> router.Set
        )

    let Form2Case1 (s: string) =
        Form.Return id
        <*> Form.Yield s
        |> Form.WithSubmit
        |> Form.Run (fun s ->
            JS.Alert s
        )

    let Form2Case2 (i: int) =
        Form.Return id
        <*> Form.Yield i
        |> Form.WithSubmit
        |> Form.Run (fun i ->
            JS.Alert <| sprintf "Given: %d" i
        )

    let RenderForm1 (s: Var<string>) (i: Var<int>) (submit: Submitter<Result<_>>) =
        div [] [
            p [] [text "If the string field is not empty, go to state2 with the string input otherwise go to state2 with the int input"]
            Doc.InputType.Text [] s
            Doc.InputType.IntUnchecked [] i
            button [on.click <| fun _ _ -> submit.Trigger()] [text "Go to State 2"]
        ]

    let RenderForm2Case1 (s: Var<string>) (submit: Submitter<Result<_>>) =
        div [] [
            Doc.InputType.Text [] s
            button [on.click <| fun _ _ -> submit.Trigger()] [text "Go"]
        ]

    let RenderForm2Case2 (i: Var<int>) (submit: Submitter<Result<_>>) =
        div [] [
            Doc.InputType.IntUnchecked [] i
            button [on.click <| fun _ _ -> submit.Trigger()] [text "Go"]
        ]


    [<SPAEntryPoint>]
    let Main () =

        router.View.Doc(function
            | State.Form1 -> Form1 { String = ""; Int = 0 } |> Form.Render RenderForm1
            | State.Form2 (Form2Case.Case1 s) -> Form2Case1 s |> Form.Render RenderForm2Case1
            | State.Form2 (Form2Case.Case2 i) -> Form2Case2 i |> Form.Render RenderForm2Case2
        )
        
        |> Doc.RunById "main"
