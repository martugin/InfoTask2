grammar CalcExpr;

/* Parser Rules  */

prog 	: vars expr;

vars : 'Vars:' var*;
var : IDENT '!' type ';';
type : DATATYPE                            #TypeF
        | ARRAY '(' DATATYPE ')'      #TypeF
        | IDENT                                  #TypeF
        | IDENT '.' IDENT                  #TypeF
        | SIGNAL                                #TypeF
        ;

exprs : 'Expr:' expr*;

expr : 'ConstBool' '!' INT					#ExprBool
        | 'ConstInt' '!' INT					#ExprInt
        | 'ConstReal' '!' REAL				#ExprReal
        | 'ConstTime' '!' TIME				#ExprTime
        | 'ConstString' '!' STRING		#ExprString 
        | 'Void' info 		   						#ExprVoid
        | 'Param' info   							#ExprParam
        | 'SubParam' info						#ExprSubParam
        | 'Var' info					   		    #ExprVar
        | 'Assign' info							#ExprAssign
        | 'Fun' info								#ExprFun
        | 'Grafic' info							#ExprGrafic
        | 'Signal' info							#ExprSignal
        | 'Object' info							#ExprObject
        | 'MetSignal'	 info						#ExprMetSignal
        | 'Met' info								#ExprMet
        | 'Owner' info							#ExprOwner
        | 'ParamProp' info 						#ExprParamProp
        | 'ObjectProp' info					#ExprObjectProp
        | 'SubParams' info						#ExprSubParams
        | 'Prev' info								#ExprPrev
        | 'If' info									#ExprIf
        | 'While' info							#ExprWhile				
        | 'Tabl' IDENT '.' IDENT '!' INT  #ExprTablField
        | 'Tabl' IDENT '!' INT                  #ExprTabl
        | ' Error' '!'                                   #ExprError       
        ;				

info : '!' IDENT                             #InfoSimple
       | '!' IDENT '!' INT                 #InfoArgs
       | '!' SIGNAL                           #InfoSignal
       | '!' SIGNAL '!' INT               #InfoSignalArgs	    
       ;

/* Lexer Rules */

WS  : [ \n\r\t] -> skip;

DATATYPE : 'Bool' | 'Int' | 'Real' | 'Time' | 'String' | 'Segments';             
ARRAY : 'List' | 'DicNumbers' | 'DicStrings'	;

fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Zà-ÿÀ-ß];
fragment IDSYMB : (DIGIT | LETTER);

IDENT : IDSYMB* LETTER IDSYMB*;

INT : DIGIT+;
REAL : INT '.' INT
         | INT '.' INT ? 'e' '-' ? INT
         ;	
TIME : INT '.' INT '.' INT ' '+ INT ':' INT ':' INT;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';

SIGNAL : '{' .*? '}';