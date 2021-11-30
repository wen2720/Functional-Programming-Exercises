(* Exercise 6 *)
(** Exercise from lecture 6 **)
(** Exercise from courese plan **)
(*** Task 1 ***)
let rec collect f = function
    | [] -> []
    | x::xs -> f x @ collect f xs;;
//val ('a -> 'b list) -> 'a list -> 'b list

(**** 1. Reason about the type is indeed the most general type of collect.
            Anyother type for collect is an instance of the above one.
            We may use e:t type assertions and 'a= "Some type" informally.

        1. As the function keword construct the pattern matching expressions, we can label the pattern expressions such as
                [] : T1 list    x::xs : T1 list
           And the we label the matched expressions as
                [] : T2 list    f x @ collect f xs : T2 list
        
        2. Inspect the pattern x::xs is constructed by :: operator, so
                x : T1          xs : T1 list
           Examine the matched expression f x @ collect f xs  is constructed by @ operator, so
                f x : T2 list   collect f xs : T2 list
           Now, we have
                f x : T2 list   x : T1
           Then we can conclude
                f : T1 -> T2 list

        3. There are no further constraints and the type of collect is evaluated as
                collect : f: (T1 -> T2 list) -> T1 list -> T2 list
        
        4. In F#, T1 and T2 are renamed as 'a and 'b. 
****)

(**** 2. Give an evaluation showing that [1;2;3;4;5;6;7;8] is the value of the expression
            collect (fun (a,b) -> [a..b]) [(1,3);(4,7);(8,8)] using e1~>e2 format
        
            collect (fun (a,b) -> [a..b]) [(1,3);(4,7);(8,8)]
        ~>  [1;2;3] @ collect f [(4,7);(8,8)]
        ~>  [1;2;3] @ ([4;5;6;7] @ collect f [(8,8)])
        ~>  [1;2;3] @ ([4;5;6;7] @ ([8] @ collect f []))

        until here are the steps consist anonymous functions

        ~>  [1;2;3] @ ([4;5;6;7] @ ([8] @[]))
        ~>  [1;2;3] @ ([4;5;6;7] @ [8])
        ~>  [1;2;3] @ (4::([5;6;7] @ [8]))
        ~>  [1;2;3] @ (4::(5::([6;7] @ [8])))  
        ~>  [1;2;3] @ (4::(5::(6::([7] @ [8]))))  
        ~>  [1;2;3] @ (4::(5::(6::(7::([] @ [8])))))
        ~>  [1;2;3] @ (4::(5::(6::(7::[8]))))
        ~>  [1;2;3] @ (4::(5::(6::[7;8])))
        ~>  [1;2;3] @ (4::(5::[6;7;8]))
        ~>  [1;2;3] @ (4::[5;6;7;8])
        ~>  [1;2;3] @ [4;5;6;7;8]
        ~>  1::([2;3] @ [4;5;6;7;8])
        ~>  1::(2::([3] @ [4;5;6;7;8]))
        ~>  1::(2::(3::([] @ [4;5;6;7;8])))

        until here are the steps consist consists recursive calls.
        
        ~>  1::(2::(3::[4;5;6;7;8]))
        ~>  1::(2::[3;4;5;6;7;8])
        ~>  1::[2;3;4;5;6;7;8]
        ~>  [1;2;3;4;5;6;7;8]
****)

(**** 3. What is the type used for collect in the expression below?
        collect (fun (a,b) -> [a..b]) [(1,3);(4,7);(8,8)]
        val collect: (int*int -> int list) -> (int*int) list -> int list 
****)

(*** Task 2 Airport-Luggage problem. For the first 4 questions, library functions are forbiden.
***)
type Lid = string
type Flight = string
type Airport = string
type Route = (Flight * Airport) list
type LuggageCatalogue = (Lid * Route) list
[("DL 016-914", [("DL 189","ATL"); ("DL 124","BRU"); ("SN 733","CPH")]);
 ("SK 222-142", [("SK 208","ATL"); ("DL 124","BRU"); ("SK 122","JFK")])];;

(**** 1. Declare a function 
            findRoute: Lid*LuggageCatalogue -> Route
         Exception should be raise if a route is not found.
****)
#load "..\Lecture5\Exercise5.fsx"
open Exercise5


let rec fFindrouteP lid = 
    function
    | [] -> failwith "Could not find the route."
    | (x,r)::lcgt when x=lid -> r
    | _::lcgt -> fFindrouteP lid lcgt;;

let findRoute lid lcgs = fFindrouteP lid lcgs;;

(**** 2. Declare a function 
            inRoute: Filight -> Route -> bool 
         that asserts whether a given flight exists in a route.
****)
let rec pOccur p =
    function
    | [] -> false
    | x::_ when p x -> true
    | _::xt -> pOccur p xt;;

let inRoute fl rs = pOccur (fun (x,_) -> x=fl) rs;; 

(**** 3. Declare a function 
            withFlight f lc 
         returns the laguages belong to the flight. 
****)

let rec fWithflightP fl =
    function
    | [] -> []
    | (lid,rs)::lct when inRoute fl rs  -> lid::fWithflightP fl lct
    | _::lct -> fWithflightP fl lct

let withFlight f lc = fWithflightP f lc;;

type ArrivalCatalogue = (Airport * Lid list) list
[("ATL", ["DL 016-914"; "SK 222-142"]);
("BRU", ["DL 016-914"; "SK 222-142"]);
("JFK", ["SK 222-142"]);
("CPH", ["DL 016-914"])];;

(**** 4. Declare a function 
            extend: Lid*Route*ArrivalCatalogue -> ArrivalCatalogue
         so that extend(lid,r,ac) will extend the lugague ID to the airport if the route of luagguage ID was contains the airport.
****)
let rec fFold f acc xs =
    match xs with
    | [] -> acc
    | x::xt -> fFold f (f acc x) xt;;
let rec fFoldB f xs acc =
    match xs with
    | [] -> acc
    | x::xt -> f x (fFoldB f xt acc);;

fFold (fun acc x -> x::acc) [] [1;2;3];;
fFoldB (fun x acc -> x::acc) [1;2;3] [];;

let rec fAddLua lid an ac =
    match ac with
    | [] -> [(an,[lid])]
    | (x,lids)::act -> if x=an 
                       then (x,lid::lids)::act
                       else (x,lids)::fAddLua lid an act;;
let rec fExtendP lid r ac =
    match r with
    | [] -> ac
    | (_,an)::rt -> fExtendP lid rt (fAddLua lid an ac)

let extend(lid,r,ac) = fExtendP lid r ac;;

(**** 5. Declare a function 
            toArrivalCatalogue: LuggageCatalogue -> ArrivalCatalogue
         so it creates an arrival catalogue from a given luggage catalogue 
            with using function extend in combination with either List.fold or List.foldBack.
****)
//Implementation without using library functions. //equivalent to foldBack
let rec fToacP = 
    function
    | [] -> []
    | (lid,r)::lct -> extend(lid,r,fToacP lct);;
let toArrivalCatalogueP lcs = fToacP lcs;; 

//Using List.fold.
let toArrivalCatalogueF lcs = 
    List.fold (fun acc (lid,r) ->
        extend(lid,r,acc)) [] lcs;;
//Using List.foldBack
let toArrivalCatalogueFB lcs = 
    List.foldBack (fun (lid,r) acc ->
        extend(lid,r,acc)) lcs [];;


toArrivalCatalogueP [("DL 016-914", [("DL 189","ATL"); ("DL 124","BRU"); ("SN 733","CPH")]);
("SK 222-142", [("SK 208","ATL"); ("DL 124","BRU"); ("SK 122","JFK")])];;

toArrivalCatalogueF [("DL 016-914", [("DL 189","ATL"); ("DL 124","BRU"); ("SN 733","CPH")]);
("SK 222-142", [("SK 208","ATL"); ("DL 124","BRU"); ("SK 122","JFK")])];;

toArrivalCatalogueFB [("DL 016-914", [("DL 189","ATL"); ("DL 124","BRU"); ("SN 733","CPH")]);
("SK 222-142", [("SK 208","ATL"); ("DL 124","BRU"); ("SK 122","JFK")])];;

(*** Task 3. Task 2 Airport-Luggage problem. For the question 2 and 3 Use library functions .***)

(**** inRoute:Flight -> Route -> bool
****)
let inRouteE fl r = 
    List.exists (fun (x,_) ->
        x=fl) r;;
(**** withFlight f lc 
****)

let withFlightF f lc =
    List.fold (fun acc (lid,r) ->
        if inRouteE f r 
        then lid::acc
        else acc) [] lc