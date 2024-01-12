module CardManagement.Server.UserStore

open CardManagement.Data.UsersRepository
open CardManagement.Shared.Types
open CardManagement.Shared.Core
open CardManagement.Infrastructure.UserActions
open CardManagement.Server.JWT
open CardManagement.Server.Password

let register inputUser = async {
    try
        let! isExistUser = tryFindUserByEmail inputUser.Email |> Async.AwaitTask
        if isExistUser.IsSome then return Error { Message = "User with this email already exist" }
        else
            let user = buildUser inputUser
            let hashPassword = createHash user.Password
            let userWithUpdateHashPassword = { user with Password = hashPassword }
            let token = userToToken userWithUpdateHashPassword
            saveUser userWithUpdateHashPassword |> Async.AwaitTask |> ignore
            return Ok (user, token)
    with
        | ex -> printfn "%A" ex; return Error { Message = "Server error" }
}

let private login email password: Async<ResponseResult<RegistrationResponse>> = async {
    try
        let! isExistUser = tryFindUserByEmail email |> Async.AwaitTask
        if isExistUser.IsNone then return Error { Message = $"User with %s{email} not found" }
        else
            let user = isExistUser.Value
            let isValidPassword = verifyPassword user.Password password
            match isValidPassword with
            | false -> return Error { Message = "Not valid password" }
            | true ->
                let token = userToToken user
                return Ok (user, token)
    with
        | ex -> printfn "%A" ex; return Error { Message = "Server error" }
}

let usersStoreImplementation: IUsersStore = {
    Register = register
    Login = login
}
