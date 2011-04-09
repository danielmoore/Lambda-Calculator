module NorthHorizon.LambdaCalculator.Languages.SystemF.Types

type VarName = string
type TypeVarName = string

type ArithOp =
    Add | Subtract | Multiply | Divide
    override o.ToString() =
        match o with
          Add -> "+"
        | Subtract -> "-"
        | Multiply -> "×"
        | Divide -> "÷"

type BoolOp =
    Conjunction | Disjunction
    override o.ToString() =
        match o with
          Conjunction -> "&&"
        | Disjunction -> "||"

type CompareOp = 
    LessThan | LessThanOrEqual
    | GreaterThan | GreaterThanOrEqual
    | Equal | NotEqual
    override o.ToString() =
        match o with
          LessThan -> "<"
        | LessThanOrEqual -> "≤"
        | GreaterThan -> ">"
        | GreaterThanOrEqual -> "≥"
        | Equal -> "="
        | NotEqual -> "≠"

type TypeExpr = 
    Int | Bool | Unit | Void
    | Func of (TypeExpr * TypeExpr)
    | Prod of (TypeExpr * TypeExpr)
    | Sum of (TypeExpr * TypeExpr)
    | TypeVar of TypeVarName
    | TypeQuant of (TypeVarName * TypeExpr)
    override t.ToString() =
        match t with
          Int -> "int"
        | Bool -> "bool"
        | Func (t1, t2) -> sprintf "(%s → %s)" (t1.ToString()) (t2.ToString())
        | Prod (t1, t2) -> sprintf "(%s × %s)" (t1.ToString()) (t2.ToString())
        | Sum (t1, t2) -> sprintf "(%s + %s)" (t1.ToString()) (t2.ToString())
        | Unit -> "unit"
        | Void -> "void"
        | TypeVar v -> v
        | TypeQuant (v, t) -> 
            match t with
            | TypeVar v -> "void"
            | _ -> sprintf "(∀%s.%s)" v (t.ToString())

(* To be implemented later: type inferencing *)
 
type TypedParam = (VarName * TypeExpr)

type Expr =
    IntVal of int | Var of VarName | Abs of (TypedParam * Expr)
    | App of (Expr * Expr) | True | False | UnitVal
    | ArithOp of (Expr * ArithOp * Expr) | BoolOp of (Expr * BoolOp * Expr)
    | Comparison of (Expr * CompareOp * Expr) | Pair of (Expr * Expr)
    | Proj of (int * Expr) 
    | Inj of (int * (TypeExpr * TypeExpr) * Expr)
    | Case of (Expr * (VarName * Expr) * (VarName * Expr))
    | FixPoint of (TypedParam * Expr)
    | PolyAbs of (TypeVarName * Expr)
    | PolyInst of (Expr * TypeExpr)
    override e.ToString() =
        let GetSubscript n =
            match n with
              0 -> "₀" | 1 -> "₁" | 2 -> "₂" | 3 -> "₃"
            | 4 -> "₄" | 5 -> "₅" | 6 -> "₆" | 7 -> "₇"
            | 8 -> "₈" | 9 -> "₉" | n -> sprintf "_%d" n
        match e with
          IntVal n -> n.ToString()
        | Var v -> v
        | Abs ((v, t), e) -> sprintf "(λ%s:%s.%s)" v (t.ToString()) (e.ToString())
        | App (e1, e2) -> sprintf "(%s %s)" (e1.ToString()) (e2.ToString())
        | True -> "true" | False -> "false"
        | ArithOp (e1, op, e2) -> sprintf "%s %s %s" (e1.ToString()) (op.ToString()) (e2.ToString())
        | BoolOp (e1, op, e2) -> sprintf "%s %s %s" (e1.ToString()) (op.ToString()) (e2.ToString())
        | Comparison (e1, op, e2) -> sprintf "%s %s %s" (e1.ToString()) (op.ToString()) (e2.ToString())
        | Pair (e1, e2) -> sprintf "(%s, %s)" (e1.ToString()) (e2.ToString())
        | Proj (n, e) -> sprintf "π%s %s" (GetSubscript n) (e.ToString())
        | UnitVal -> "()"
        | Inj (n, (t1, t2), e) -> sprintf "in%s^(%s + %s) %s" (GetSubscript n) (t1.ToString()) (t2.ToString()) (e.ToString())
        | Case (e, (v1, e1), (v2, e2)) -> 
            sprintf "case %s of in₁(%s) -> %s | in₂(%s) → %s"
                (e.ToString()) v1 (e1.ToString()) v2 (e2.ToString())
        | FixPoint ((v, t), e) -> sprintf "μ%s:%s.%s" v (t.ToString()) (e.ToString())
        | PolyAbs (tv, e) -> sprintf "Λ%s.%s" tv (e.ToString())
        | PolyInst (e, te) -> sprintf "%s[%s]" (e.ToString()) (te.ToString())

