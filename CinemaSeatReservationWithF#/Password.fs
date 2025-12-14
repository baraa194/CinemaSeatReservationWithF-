namespace CinemaSeatReservationWithFSharp

open System
open System.Security.Cryptography
open System.Text

module Password =

  
    let private iterateCount = 100_000
    let private saltSize = 16
    let private hashSize = 32

    /// generate random salt
    let createSalt () =
        let salt = Array.zeroCreate<byte> saltSize
        use rng = RandomNumberGenerator.Create()
        rng.GetBytes(salt)
        salt

    /// hash password -> returns (hash, salt)
    let hashPassword (password:string) : byte[] * byte[] =
        if String.IsNullOrEmpty(password) then
            invalidArg "password" "password cannot be empty"
        let salt = createSalt()
        // Note: Using overload compatible with many frameworks (PBKDF2 using HMAC-SHA1)
        use pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterateCount)
        let hash = pbkdf2.GetBytes(hashSize)
        (hash, salt)

    /// verify password against stored hash+salt
    let verifyPassword (password:string) (storedHash:byte[]) (storedSalt:byte[]) : bool =
        if isNull storedHash || isNull storedSalt then false
        else
            use pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, iterateCount)
            let computed = pbkdf2.GetBytes(hashSize)
            if computed.Length <> storedHash.Length then false
            else
                // constant-time comparison
                let mutable equal = true
                for i in 0 .. computed.Length - 1 do
                    equal <- equal && (computed.[i] = storedHash.[i])
                equal

