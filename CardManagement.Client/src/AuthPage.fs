module CardManagement.Client.Pages.AuthPage

open Feliz.Bulma
open Feliz
open System.Text.RegularExpressions
open CardManagement.Client
open CardManagement.Shared.Types
open Inputs
open ErrorComponent
open Types
open WebApi
open Utils

let initialStateUser = { Name = ""; Surname = ""; Patronymic = ""; Password = ""; Age = 0; Salary = 0; Email = "" }

let private validatePassword password repeatPassword =
    password = repeatPassword

let validateEmail (email: string) =
    let pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
    let regex = Regex(pattern)
    regex.IsMatch(email)

let private validateFieldsRegistrationUser (user: InputUser) repeatPassword =
    if user.Name.Length < 4 then Some "Not correct Name field"
    elif user.Surname.Length < 4 then Some "Not correct Surname field"
    elif user.Patronymic.Length < 4 then Some "Not correct Patronymic field"
    elif not (validateEmail user.Email) then Some "Not correct Email field"
    elif user.Surname.Length < 4 then Some "Not correct Surname field"
    elif user.Password.Length < 4 then Some "Not correct Password field"
    elif not (validatePassword user.Password repeatPassword) then Some "Not equal passwords"
    elif user.Age < 18 then Some "Not correct Age field"
    else None
    
let private validateFieldsLoginUser (email: string) (password: string) repeatPassword =
    if not (validateEmail email) then Some "Not correct Email field"
    elif password.Length < 4 then Some "Not correct Password field"
    elif not (validatePassword password repeatPassword) then Some "Not equal passwords"
    else None

[<ReactComponent>]
let RegistrationForm setTypeAuthorization =
    let user, setUser = React.useState initialStateUser
    let repeatPassword, setRepeatPassword = React.useState ""
    let error, setError = React.useState ""
    
    let register() = async {
        try
            let! result = userStore.Register user
            match result with
            | Ok (_, token) ->
                Browser.WebStorage.localStorage.setItem("token", token.Token)
                navigate [ "home" ]
                Browser.Dom.window.location.reload()
            | Error err -> setError err.Message
        with
            | ex -> printfn "%A" ex; setError "Server error"
    }
    
    let next (e:Browser.Types.MouseEvent) =
        e.preventDefault()
        match validateFieldsRegistrationUser user repeatPassword with
        | None -> register() |> Async.StartImmediate
        | Some msg -> setError msg
    
    Html.div [
        prop.style [
            style.height (length.vh 100)
            style.display.flex
            style.flexDirection.column
            style.justifyContent.center
            style.alignItems.center
            style.position.relative
        ]
        prop.children [
            if error.Length > 0 then ErrorComponent error 20 20 "#f14668"
            Html.h1 [
                prop.text "Registration"
                prop.style [
                    style.color "black"
                    style.marginTop 10
                    style.fontSize 25
                ]
            ]
            Html.form [
                prop.style [
                    style.width 457
                    style.fontSize 12
                ]
                prop.children [
                    InputText "Your name" "Name" Text (fun v -> setUser { user with Name = v }) (Some user.Name)
                    InputText "Your surname" "Surname" Text (fun v -> setUser { user with Surname = v }) (Some user.Surname)
                    InputText "Your patronymic" "Patronymic" Text (fun v -> setUser { user with Patronymic =  v }) (Some user.Patronymic)
                    InputText "Your email" "Email" Text (fun v -> setUser { user with Email = v }) (Some user.Email)
                    InputText "Your password" "Password" Password (fun v -> setUser { user with Password = v }) (Some user.Password)
                    InputText "Repeat your password" "Repeat Password" Password setRepeatPassword (Some repeatPassword)
                    InputNumber "Your age" "Age" (fun v -> setUser {user with Age = v }) (Some user.Age)
                    InputNumber "Your salary" "Salary" (fun v -> setUser { user with Salary = v }) (Some user.Salary)
                    Html.div [
                        prop.style [
                            style.display.flex
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            Bulma.button.button [
                                prop.text "Next"
                                prop.style [
                                    style.backgroundColor "#3D70FF"
                                    style.color "white"
                                ]
                                prop.onClick next
                            ]
                            Bulma.button.button [
                                prop.text "Login Form"
                                prop.onClick (fun e -> e.preventDefault(); setTypeAuthorization Login )
                                prop.style [
                                    style.backgroundColor "#3D70FF"
                                    style.color "white"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
    
[<ReactComponent>]
let LoginForm setTypeAuthorization =
    let error, setError = React.useState ""
    let email, setEmail = React.useState ""
    let password, setPassword = React.useState ""
    let repeatPassword, setRepeatPassword = React.useState ""
    
    let login() = async {
        try
            let! result = userStore.Login email password
            match result with
            | Ok (_, token) ->
                Browser.WebStorage.localStorage.setItem("token", token.Token)
                navigate [ "home" ]
                Browser.Dom.window.location.reload()
            | Error error -> setError error.Message 
        with
            | ex -> printfn "%A" ex; setError "Server error"
    }
    
    let next (e:Browser.Types.MouseEvent) =
        e.preventDefault()
        setError ""
        match validateFieldsLoginUser email password repeatPassword with
        | None -> login() |> Async.StartImmediate
        | Some msg -> setError msg
    
    Html.div [
        prop.style [
            style.display.flex
            style.flexDirection.column
            style.justifyContent.center
            style.alignItems.center
            style.position.relative
            style.flexGrow 1
        ]
        prop.children [
            if error.Length > 0 then ErrorComponent error 20 20 "#f14668"
            Html.h1 [
                prop.text "Login"
                prop.style [
                    style.color "black"
                    style.marginTop 10
                    style.fontSize 25
                ]
            ]
            Html.form [
                prop.style [
                    style.width 457
                    style.fontSize 12
                ]
                prop.children [
                    InputText "Your email" "Email" Text setEmail (Some email)
                    InputText "Your password" "Password" Password setPassword (Some password)
                    InputText "Repeat your password" "Repeat Password" Password setRepeatPassword (Some repeatPassword)
                    Html.div [
                        prop.style [
                            style.display.flex
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            Bulma.button.button [
                                prop.text "Next"
                                prop.onClick next
                                prop.style [
                                    style.backgroundColor "#3D70FF"
                                    style.color "white"
                                ]
                            ]
                            Bulma.button.button [
                                prop.text "Registration Form"
                                prop.onClick (fun e -> e.preventDefault(); setTypeAuthorization Registration)
                                prop.style [
                                    style.backgroundColor "#3D70FF"
                                    style.color "white"
                                ]
                            ]
                        ]
                    ]
                ]
            ] 
        ]
    ]

[<ReactComponent>]
let AuthPage() =
    let typeAuthorization, setTypeAuthorization = React.useState Registration
    
    let switchAuthorization value =
        setTypeAuthorization value
    
    Html.div [
        prop.style [
            style.height (length.vh 100)
            style.display.flex
            style.flexDirection.column
        ]
        prop.children [
            Html.header [
                prop.children [
                    Html.h1 [
                        prop.text "Bank"
                        prop.style [
                            style.color "white"
                            style.fontWeight 400
                            style.fontSize 26
                            style.marginLeft 10
                        ]
                    ]
                    Html.img [
                        prop.src "./img/Icon_Visa.svg"
                        prop.style [
                            style.marginRight 10
                        ]
                    ]
                    
                ]
                prop.style [
                    style.height 50
                    style.backgroundColor "#3D70FF"
                    style.display.flex
                    style.alignItems.center
                    style.justifyContent.spaceBetween
                ]
            ]
            match typeAuthorization with
            | Registration -> RegistrationForm switchAuthorization
            | Login -> LoginForm switchAuthorization
        ]
    ]    