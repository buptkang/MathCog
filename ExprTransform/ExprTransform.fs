// Learn more about F# at http://fsharp.net

namespace starPadSDK.MathExpr

open starPadSDK.UnicodeNs
open System
open System.IO
open System.Diagnostics
open System.Collections.Generic
open starPadSDK.MathExpr

/// We maintain this entire duplicate (virtually) of the Expr hierarchy because of F#'s annoying treatment of
/// return values (requiring them to be explicitly cast to the same type even if they're all subtypes of one type). FS stands for F#
type ExprFS =
    | CompositeExprFS of ExprFS * ExprFS list * bool
    | DoubleNumberFS of float * bool
    | IntegerNumberFS of BigInt * bool
    | RationalNumberFS of BigRat * bool
    | ArrayExprFS of ExprFS array array // this is wrong for expr, but matches what MML can say
    | LetterSymFS of char * bool
    | WordSymFS of string * bool
    | WellKnownSymFS of WKSID
    | SubscriptFS of ExprFS * ExprFS
    | ErrorMsgExprFS of string

type public ExprTransform() =
     /// From Expr to ExprFS, a retrograde converter to deal with the output from various Syntax routines that return Exprs
    static let rec fromexpr (e : Expr) = // NB: Force Parentheses annotation not handled yet--but this function is so far only used to back-convert operators
        match e with
            | :? WellKnownSym as wks -> WellKnownSymFS(wks.ID)
            | :? WordSym as ws -> let wsfs = WordSymFS(ws.Word,ws.Annotations.Contains("Factor")) in match ws.Subscript with | :? NullExpr -> wsfs | _ -> SubscriptFS(wsfs, fromexpr ws.Subscript)
            | :? CompositeExpr as ce -> CompositeExprFS(fromexpr ce.Head, ce.Args |> Array.map fromexpr |> Array.toList, ce.Annotations.ContainsKey("Factor"))
            | :? DoubleNumber as dn -> DoubleNumberFS(dn.Num,dn.Annotations.Contains("Factor"))
            | :? IntegerNumber as ine -> IntegerNumberFS(ine.Num,ine.Annotations.Contains("Factor"))
            | :? RationalNumber as rn -> RationalNumberFS(rn.Num,rn.Annotations.Contains("Factor"))
            | :? ArrayExpr as ae -> failwith "Arrays not implemented yet in fromexpr."
            | :? LetterSym as ls -> let lsfs = LetterSymFS(ls.Letter,ls.Annotations.ContainsKey("Factor")) in match ls.Subscript with | :? NullExpr -> lsfs | _ -> SubscriptFS(lsfs, fromexpr ls.Subscript)
            | :? ErrorMsgExpr as eme -> ErrorMsgExprFS(eme.Msg)
            | _ -> failwith "???"
        
    /// From ExprFS to real Expr
    static let rec toexpr efs =
        let addFactor(expr:Expr,factor) =
            expr
        match efs with
            | CompositeExprFS(head, args, factor) -> addFactor(new CompositeExpr(toexpr head, args |> List.map toexpr |> List.toArray), factor)
            | DoubleNumberFS(num,factor) -> addFactor(new DoubleNumber(num), factor)
            | IntegerNumberFS(num,factor) -> addFactor(new IntegerNumber(num) , factor)
            | RationalNumberFS(num,factor) -> addFactor(new RationalNumber(num), factor)
            | ArrayExprFS(arr) -> new ArrayExpr(Array2D.init arr.Length (Array.map Array.length arr |> Array.max) (fun i j -> toexpr arr.[i].[j])) :> Expr
            | LetterSymFS(c,factor) ->  addFactor(new LetterSym(c), factor)
            | WordSymFS(w,factor) -> addFactor(new WordSym(w), factor)
            | WellKnownSymFS(wks) -> new WellKnownSym(wks) :> Expr
            | SubscriptFS(e, sub) -> match e with
                                        | LetterSymFS(c,factor) -> 
                                            let ls = new LetterSym(c, toexpr sub) 
                                            if (factor) then ls.Annotations.Add("Factor", true) 
                                            ls:> Expr
                                        | WordSymFS(w,factor) -> let result = new WordSym(w) in addFactor(result, factor); result.Subscript <- toexpr sub; result :> Expr
                                        | _ -> new CompositeExpr(WellKnownSym.subscript, [|toexpr e; toexpr sub|]) :> Expr
            | ErrorMsgExprFS(msg) -> new ErrorMsgExpr(msg) :> Expr
            
    /// shorthands for matching different types of Exprs
    static let (|SymPlus|_|) (e) = match e with WellKnownSymFS(WKSID.plus) -> Some() | _ -> None
    static let (|SymMinus|_|) (e) = match e with WellKnownSymFS(WKSID.minus) -> Some() | _ -> None
    static let (|SymEq|_|) (e) = match e with WellKnownSymFS(WKSID.equals) -> Some() | _ -> None
    static let (|SymTimes|_|) (e) = match e with WellKnownSymFS(WKSID.times) -> Some() | _ -> None
    static let (|SymDiv|_|) (e) = match e with WellKnownSymFS(WKSID.divide) -> Some() | _ -> None
    static let (|SymPower|_|) (e) = match e with WellKnownSymFS(WKSID.power) -> Some() | _ -> None
    static let (|CEXP|_|) (e) = match e with CompositeExprFS(h,a,f) -> Some(h,a,f) | _ -> None
    
    // shorthands for creating Exprs
    static let rec bint(a) = IntegerNumberFS(BigInt(a.ToString()), false)
    static let rec timesF(args,factor) = CompositeExprFS(WellKnownSymFS(WKSID.times), args, factor)
    static let rec times(args) = 
        if (List.length(args) > 1) then  CompositeExprFS(WellKnownSymFS(WKSID.times), args, false)
        else if (args.IsEmpty) then  bint(1) 
        else   List.head args
    static let rec plus(args) =  CompositeExprFS(WellKnownSymFS(WKSID.plus), args, false)
    static let rec eq(args) =  CompositeExprFS(WellKnownSymFS(WKSID.equals), args, false)
    static let rec power(a,b) =  CompositeExprFS(WellKnownSymFS(WKSID.power), a::[b], false)
    static let rec inverse(b) = CompositeExprFS(WellKnownSymFS(WKSID.divide), [b], false)
    static let rec minus(b) = CompositeExprFS(WellKnownSymFS(WKSID.minus), [b], false)
    static let rec divide(a,b) = times(a :: [inverse(b)])
            
    // some utilties
    static let rec flat(args) = 
        let rec flattened  terms  arg  =
                match arg with
                    | CEXP(SymTimes, moreterms, f) when f <> true ->  terms @ (flat moreterms)
                    | _  as other -> List.append terms [other]
        List.fold (flattened) [] args
                
    static let containsTop(inList, expr) =  List.tryFind (fun arg -> arg=expr) inList <> None
    static let remove(args, expr) = 
        let  found = ref false
        List.filter (fun arg -> if (!found) then true else found  := (arg = expr); !found = false) args
        
    static let containsInSum(inList, expr) = 
        let contains(inExpr,  expr) =
            match inExpr with
                | _ as other when other = expr -> true
                | CompositeExprFS(SymTimes, args, f)  when containsTop(flat(args),expr) -> true // -> let found = false in (args |> List.map (fun arg -> found = (found || contains(arg, thing)))); found
                | _  -> false
        List.length( List.filter (fun arg -> contains(arg,expr)) inList) <> 0
        
    static let containsAnywhere(inList, expr) = 
        let rec contains(inExpr,  expr) =
            match inExpr with
                | _ as other when other = expr  -> true
                | CompositeExprFS(head, args, f)  -> List.tryFind  (fun arg -> contains(arg, expr) ) args <> None // -> let found = false in (args |> List.map (fun arg -> found = (found || contains(arg, thing)))); found
                | _  -> false
        List.length( List.filter (fun arg -> contains(arg,expr)) inList) <> 0
     
            
    // Math Functions
    static let rec _FactorOut (inExpr : ExprFS, factor) =
        match inExpr with
                        // a + bx + c -> x (a/x + b + c/x)
            | CEXP(SymPlus, args, f)                                                                               when containsInSum(args, factor) -> plus(List.map (fun arg -> fromexpr(Engine.Simplify(toexpr(divide(arg,factor))))) args)
                        // 1/(abx)  -> x / abx^2
            | CEXP(SymTimes, args, f)                                                                            when containsTop(flat(args), factor) ->  times(remove(flat(args),factor))
                        // 1/(abx)  -> x / abx^2
            | CEXP(SymMinus, [arg], f)                                                                           when containsTop(flat([arg]), factor) ->  times(bint(-1) :: remove(flat([arg]),factor) )
                        // a / x -> x a / x^2
            | CEXP(SymTimes, numerator ::[CEXP(SymDiv, [arg], f2)], f3)                       when arg=factor ->  divide(numerator,  power(factor,bint(2)))
                        // a/(bx)  -> x a / bx^2
            | CEXP(SymTimes, numerator ::[CEXP(SymDiv, [CEXP(SymTimes, args, f)], f2)], f3)  when containsTop(flat(args), factor) ->  divide(numerator,  times(power(factor,bint(2)) ::  remove(flat(args),factor) ))
                        // 1/(ax)  -> x / ax^2
            | CEXP(SymDiv, [CEXP(SymTimes, args, f)], f2)                                            when containsTop(flat(args), factor) ->  inverse(times(power(factor,bint(2)) :: remove(flat(args),factor)))
                       // a^n -> a * a^(n-1)
            | CEXP(SymPower, bas :: (pow :: rem), f)                                                       when bas = factor ->  fromexpr(Engine.Simplify(toexpr(power(bas, plus(pow :: [minus(bint(1))])))))
                        // leave it
            | _ as other -> other
            
    static let rec _MoveAcross (inExpr : ExprFS, factor) =
        match inExpr with
                        // a + b + x  = z  -> a + b  = z - x
            | CEXP(SymEq, CEXP(SymPlus, args, f) :: rhs, f2)    when containsTop(args,factor) ->  eq(plus(remove(args, factor)) :: [ plus (minus(factor) :: rhs) ] )
                        // lhs  = z  -> 0  = z - lhs
            | CEXP(SymEq, lhs :: rhs, f)                                     when lhs = factor                     ->  eq(bint(0) :: [ plus (minus(factor) :: rhs) ] )
                        // leave it
            | _ as other -> other
            
    static let rec _FlattenTimes (inExpr : ExprFS) =
        let rec flattened  terms  arg  =
            match arg with
                | CEXP(SymTimes, moreterms, f) when f <> true -> List.append terms (List.fold (flattened) [] moreterms)
                | _  -> [arg]
        match inExpr with
            | CEXP(SymTimes, terms,f)    ->  times( List.fold (flattened) [] terms)
                        // leave it
            | _ as other -> other
            
    static member public FactorOut(inexpr, inLet) = toexpr(_FactorOut(fromexpr(inexpr), fromexpr(inLet)))
    static member public MoveAcross(inexpr, inLet) = toexpr(_MoveAcross(fromexpr(inexpr), fromexpr(inLet)))
