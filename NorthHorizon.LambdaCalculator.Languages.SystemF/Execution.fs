module NorthHorizon.LambdaCalculator.Languages.SystemF.Execution

open NorthHorizon.LambdaCalculator.Languages.SystemF.Types
open NorthHorizon.LambdaCalculator.Languages.SystemF.Lexing
open NorthHorizon.LambdaCalculator.Languages.SystemF.Parsing
open Microsoft.FSharp.Text.Lexing;

// isValid checks an Expression for free variables. Free type variable are checked in typeCheckExpression
[<CompiledName "IsValid">]
let isValid e =
    let rec examine e bv = 
        match e with
          Var v -> List.exists (fun x -> x = v) bv
        | Abs ((v, t), e) | FixPoint ((v, t), e)  -> examine e (v :: bv)
        | Case (e, (v1, e1), (v2, e2)) -> (examine e bv) && (examine e1 (v1 :: bv)) && (examine e2 (v2 :: bv))
        | App (e1, e2) | ArithOp (e1, _, e2) 
        | BoolOp (e1, _, e2) | Comparison (e1, _, e2) 
        | Pair (e1, e2) -> (examine e1 bv) && (examine e2 bv)
        | Proj (_, e) | Inj (_, _, e) | PolyAbs (_, e) | PolyInst (e, _) -> examine e bv
        | IntVal _ | True | False | UnitVal -> true
    examine e []

type private ExprVal = Expr of Expr | Unbound
type private VarBind = VarName -> ExprVal

type TypeCheckResult = TypeExpr of TypeExpr | Unbound
type private TypeVarBind = TypeVarName -> TypeCheckResult

type private TypeContext = VarName -> TypeCheckResult

type TypeCheckVal = Type of TypeExpr | Err of string

let private update f v i = fun x -> if x = v then i else f x

[<CompiledName "TypeCheckExpression">]
let typeCheckExpression e =
    let rec typeCheck (tc:TypeContext) e : TypeCheckVal =
        let typeCheckArgs (a, b) t tret =
            match (typeCheck tc a, typeCheck tc b) with
              (Type t1, Type t2) when t1 = t && t = t2 -> Type tret
            | (Err s, _) | (_, Err s) -> Err s
            | (Type t1, _) when t1 <> t -> Err (sprintf "Parameter 1 was type %s, expected type %s" (t1.ToString()) (t.ToString()))
            | (_, Type t2) -> Err (sprintf "Parameter 2 was type %s, expected type %s" (t2.ToString()) (t.ToString()))
        match e with
          IntVal n -> Type Int
        | Var v -> match tc v with
                     TypeExpr t -> Type t
                   | Unbound -> Err (sprintf "Use of unbound type variable '%s'" v)
        | Abs ((arg, argtyp), e) -> 
            let argtyp = if argtyp = Void then TypeQuant("α", TypeVar "α") else argtyp
            match typeCheck (update tc arg (TypeExpr argtyp)) e with
              Type t -> Type (Func (argtyp, t))
            | err -> err
        | App (e1, e2) ->
            match (typeCheck tc e1, typeCheck tc e2) with
              (Type (Func (t1, tp)), Type t2) when t1 = t2 -> Type tp
            | (Type _, Type _) -> Err "Invalid expression application."
            | (Err e, _) | (_, Err e) -> Err e
            | (Type (Func (t1, tp)), Type t2) -> Err (sprintf "Expected type %s, received type %s" (t1.ToString()) (t2.ToString()))
        | True | False -> Type Bool
        | ArithOp (e1, op, e2) -> typeCheckArgs (e1, e2) Int Int
        | BoolOp (e1, op, e2) -> typeCheckArgs (e1, e2) Bool Bool
        | Comparison (e1, op, e2) -> typeCheckArgs (e1, e2) Int Bool
        | Pair (e1, e2) -> match (typeCheck tc e1, typeCheck tc e2) with
                             (Type t1, Type t2) -> Type (Prod (t1, t2))
                           | (Err e, _) | (_, Err e) -> Err e
        | UnitVal -> Type Unit
        | Proj (n, e) ->
            match (n, typeCheck tc e) with
              (1, Type (Prod (t, _))) | (2, Type (Prod (_, t))) -> Type t
            | (_, Err e) -> Err e
            | _ -> Err "Invalid projection index."
        | Inj (n, (t1, t2), e) ->
            match (n, typeCheck tc e) with
              (1, Type t) when t1 = t -> Type (Sum (t1, t2))
            | (2, Type t) when t2 = t -> Type (Sum (t1, t2))
            | (_, Err e) -> Err e
            | (1, _) | (2, _) -> Err "Injection type mismatch."
            | _ -> Err "Invalid injection index."
        | Case (e, (v1, e1), (v2, e2)) ->
            match typeCheck tc e with
              Type (Sum (t1, t2)) ->
                let tc1 = update tc v1 (TypeExpr t1)
                let tc2 = update tc v2 (TypeExpr t2)
                match (typeCheck tc1 e1, typeCheck tc2 e2) with
                  (ta, tb) when ta = tb -> ta
                | _ -> Err "Cases return type mismatch."
            | Type _ -> Err "Case type is not a sum."
            | Err s -> Err s
        | FixPoint ((v, t), e) -> typeCheck (update tc v (TypeExpr t)) e
        | PolyAbs (v, e) -> match typeCheck tc e with
                              Type t -> Type (TypeQuant (v, t))
                            | Err s -> Err s
        | PolyInst (e, t) ->
            match typeCheck tc e with
              Type (TypeQuant (v, tp)) ->
                  let rec sub e vRep tRep =
                      let subRec e = sub e vRep tRep
                      match e with
                        TypeVar v when v = vRep -> tRep
                      | TypeQuant (v, e) when v <> vRep -> subRec e
                      | Func (a, b) -> Func (subRec a, subRec b)
                      | Prod (a, b) -> Prod (subRec a, subRec b)
                      | Sum (a, b) -> Sum (subRec a, subRec b)
                      | x -> x
                  Type (sub tp v t)
            | Type _ -> Err "Instantiated type is not polymorphic."
            | Err s -> Err s
    typeCheck (fun _ -> Unbound) e

exception ExecutionFailure

[<CompiledName "Execute">]
let rec execute e =
    let bool_of_expr e = match e with True -> true | False -> false | _ -> raise ExecutionFailure
    let expr_of_bool p = if p then True else False
    let rec sub e vRep eRep =
        let subRec e = sub e vRep eRep
        match e with
          Var v when vRep = v -> eRep
        | Abs ((v, t), e) when v <> vRep -> Abs ((v, t), subRec e)
        | Case (e, c1, c2) ->
            let subCasePair (v, e) = if v = vRep then (v, e) else (v, subRec e)
            Case (subRec e, subCasePair c1, subCasePair c2)
        | App (e1, e2) -> App (subRec e1, subRec e2)
        | ArithOp (a, o, b) -> ArithOp (subRec a, o, subRec b)
        | BoolOp (a, o, b) -> BoolOp (subRec a, o, subRec b)
        | Comparison (a, o, b) -> Comparison (subRec a, o, subRec b)
        | Pair (a, b) -> Pair (subRec a, subRec b)
        | Proj (n, e) -> Proj (n, subRec e)
        | Inj (n, t, e) -> Inj (n, t, subRec e)
        | FixPoint (v, e) -> FixPoint (v, subRec e)
        | PolyAbs (v, e) -> PolyAbs (v, subRec e)
        | PolyInst (e, t) -> PolyInst (subRec e, t)
        | x -> x
    match e with
      App (e1, e2) ->
        match execute e1 with
          Abs ((v, _), e) -> execute (sub e v e2)
        | _ -> raise ExecutionFailure
    | ArithOp (a, op, b) ->
        let performOp = match (execute a, execute b) with
                          (IntVal a, IntVal b) -> fun op -> IntVal (op a b)
                        | _ -> raise ExecutionFailure
        match op with
          Add -> performOp (+)
        | Subtract -> performOp (-)
        | Multiply -> performOp (*)
        | Divide -> performOp (/)
    | BoolOp (a, op, b) ->
        let performOp op = expr_of_bool (op (bool_of_expr (execute a)) (bool_of_expr (execute b)))
        match op with
          Conjunction -> performOp (&&)
        | Disjunction -> performOp (||)
    | Comparison (a, op, b) ->
        let performOp = match (execute a, execute b) with
                          (IntVal a, IntVal b) -> fun op -> expr_of_bool (op a b)
                        | _ -> raise ExecutionFailure
        match op with
          LessThan -> performOp (<)
        | LessThanOrEqual -> performOp (<=)
        | GreaterThan -> performOp (>)
        | GreaterThanOrEqual -> performOp (>=)
        | Equal -> performOp (=)
        | NotEqual -> performOp (<>)
    | Pair (e1, e2) -> Pair (execute e1, execute e2)
    | Proj (n, e) -> match (n, execute e) with
                       (1, Pair (e, _)) | (2, Pair (_, e)) -> execute e
                     | _ -> raise ExecutionFailure
    | Inj (n, t, e) -> Inj (n, t, execute e)
    | Case (e, c1, c2) ->
        match (execute e, c1, c2) with
          (Inj (1, (t, _), e), (v, caseExpr), _)
        | (Inj (2, (_, t), e), _, (v, caseExpr)) -> execute (sub caseExpr v e)
        | _ -> raise ExecutionFailure
    | FixPoint ((v, t), e) -> execute (sub e v (FixPoint ((v, t), e)))
    | PolyInst (e, t) ->
        let rec typesub e vRep tRep =
            let subRec e = typesub e vRep (tRep:TypeExpr)
            match e with
              PolyAbs (v, e) when v <> vRep -> PolyAbs (v, subRec e)
            | PolyInst (e, t) -> PolyInst (subRec e, if t = tRep then tRep else t)
            | Abs ((v, t), e) -> Abs ((v, if t = tRep then tRep else t), subRec e)
            | Case (e, c1, c2) ->
                let subCasePair (v, e) = if v = vRep then (v, e) else (v, subRec e)
                Case (subRec e, subCasePair c1, subCasePair c2)
            | App (e1, e2) -> App (subRec e1, subRec e2)
            | ArithOp (a, o, b) -> ArithOp (subRec a, o, subRec b)
            | BoolOp (a, o, b) -> BoolOp (subRec a, o, subRec b)
            | Comparison (a, o, b) -> Comparison (subRec a, o, subRec b)
            | Pair (a, b) -> Pair (subRec a, subRec b)
            | Proj (n, e) -> Proj (n, subRec e)
            | Inj (n, t, e) -> Inj (n, t, subRec e)
            | FixPoint (v, e) -> FixPoint (v, subRec e)
            | x -> x
        match execute e with
          PolyAbs (tparam, e) -> typesub e tparam t
        | _ -> raise ExecutionFailure
    | x -> x

let private lexeme = LexBuffer<char>.FromString
    
[<CompiledName "GetTokenList">]
let getTokenList s =
    let lexbuf = lexeme s
    Seq.initInfinite (fun _ -> tokenize lexbuf) |> Seq.takeWhile (fun t -> t <> EOF) |> Seq.toArray
    
[<CompiledName "GetExpression">]
let getExpr s = start tokenize (lexeme s)