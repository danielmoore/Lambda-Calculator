module NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Execution

open NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Types
open NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Lexing
open NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Parsing
open Microsoft.FSharp.Text.Lexing;

// isValid checks an Expression for free variables.
let isValid e =
    let rec Examine e bv = 
        match e with
          Var v -> List.exists (fun x -> x = v) bv
        | Abs (v, e2) -> Examine e2 (v :: bv)
        | App (e1, e2) -> (Examine e1 bv) && (Examine e2 bv)
    Examine e []

let rec execute e =
    let rec sub e vRep eRep =
        let subRec e = sub e vRep eRep
        match e with
          Var v when v = vRep -> eRep
        | Abs (v, e) when v <> vRep -> Abs (v, subRec e)
        | App (e1, e2) -> App (subRec e1, subRec e2)
        | x -> x
    match e with
      App (Abs (v, e1), e2) -> execute (sub e1 v e2)
    | App(e1, e2) -> match execute e1 with
                       Abs a -> execute (App (Abs a, e2))
                     | _ -> e
    | _ -> e

let lexeme = LexBuffer<char>.FromString

let getTokenList s = 
    let rec lex arr =
        let token = tokenize (lexeme s)
        let ret = token :: arr
        match token with
          EOF -> ret
        | _ -> lex ret
    List.rev (lex [])
    
let getExpr s = start tokenize (lexeme s)