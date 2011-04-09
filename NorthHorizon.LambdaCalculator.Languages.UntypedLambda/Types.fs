module NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Types

type VarName = string;;

type Expr =
    Abs of (VarName * Expr) | App of (Expr * Expr) | Var of VarName
    override e.ToString () =
        match e with
          Abs (v, e2) -> sprintf "(λ%s.%s)" v (e2.ToString())
        | Var v -> v
        | App (e1, e2) -> sprintf "(%s %s)" (e1.ToString()) (e2.ToString())