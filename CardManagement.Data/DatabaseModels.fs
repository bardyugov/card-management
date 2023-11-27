namespace CardManagement.Data

open CardManagement.Infrastructure.DomainModels

module DatabaseModels =
    open System
    
    [<CLIMutable>] 
    type DBTransaction = {
        Id: Guid
        Card: DBCard
        CreateDate: DateTime
        Sum: int
        ToUserId: Guid
    }
        
    and [<CLIMutable>] DBCard = {
        Id: Guid
        Code: int32
        CVV: int
        User: DBUser
        Balance: int
        TypeCard: TypeOfCard
        Status: TypeOfActivation
        Transactions: DBTransaction ResizeArray
        LifeTime: DateTime
    }
    
    and [<CLIMutable>] DBUser = {
        Id: Guid
        Name: string
        Surname: string
        Patronymic: string
        Age: int
        Salary: int
        Email: string
        Cards: DBCard ResizeArray
    }
    