module CardManagement.Client.CardComponent

open CardManagement.Shared.Types
open Feliz

let convertCardToPoint (code: string) =
    "•••• •••• •••• " + code[-4..]

let addBackSpaceToCardCode (code: string) =
    code[0..3] + " " + code[4..7] + " " + code[8..11] + " " + code[12..code.Length]

[<ReactComponent>]
let CardComponent (card: Card) =
    let code, setCode = React.useState ""
    
    React.useEffect((fun _ -> setCode(card.Code.ToString() |> convertCardToPoint); printfn "%A" card.Code), [|box card|])
    
    let className =
        match card.TypeCard with
        | Basic -> "card_basic"
        | Priority -> "card_priority"
    
    let time =
        let year = card.LifeTime.Year.ToString()[2..]
        let stringMonth = card.LifeTime.Month.ToString()
        let updateMonth =
            match stringMonth.Length > 1 with
            | true -> stringMonth
            | false -> "0" + stringMonth
        updateMonth + "/" + year
    
    let convertBalance balance =
        "$" + balance.ToString() + ",00" + " USD"
    
    let onMouseMove _ =
        card.Code.ToString() |> addBackSpaceToCardCode |> setCode
    
    let onMouseLeave _ =
        card.Code.ToString() |> convertCardToPoint |> setCode
    
    Html.div [
        prop.onMouseMove onMouseMove
        prop.onMouseLeave onMouseLeave
        prop.className className
        prop.style [
            style.position.relative
        ]
        prop.children [
            Html.div [
                prop.style [
                    style.position.absolute
                    style.left 24
                    style.top 44
                ]
                prop.children [
                    Html.h1 [
                        prop.text "Salary card"
                        prop.style [
                            style.color "#A2C8FB"
                        ]
                    ]
                    Html.h1 [
                        prop.text (convertBalance card.Balance)
                        prop.style [
                            style.color "#FFF"
                        ]
                    ]
                ]
            ]
            Html.img [
                prop.src "/img/Icon_Visa.svg"
                prop.style [
                    style.position.absolute
                    style.bottom 24
                    style.left 24
                ]
            ]
            Html.div [
                prop.className "rectangle"
                prop.style [
                    match card.TypeCard with
                    | Basic -> style.backgroundColor "#49B1FF"
                    | Priority -> style.backgroundColor "gold"
                ]
                prop.children [
                    Html.h1 [
                        prop.text time
                        prop.style [
                            style.position.absolute
                            style.right 24
                            style.bottom 49
                            style.color "#FFF"
                        ]
                    ]
                    Html.h1 [
                        prop.text code
                        prop.style [
                            style.position.absolute
                            style.right 24
                            style.bottom 20
                            style.color "#FFF" 
                        ]
                    ]
                ]
            ]
        ]
    ]