grammar Expr;
//√рамматика дл€ полей UserExpr

/* Parser Rules */

prog	: voidProg                        #ProgVoid
		| valueProg                      #ProgValue
		;

voidProg : voidExpr (':' voidExpr)*;   

valueProg : (voidExpr ':')* expr;	

//¬ыражени€ без значени€
voidExpr : IDENT ASSIGN expr																   #VoidExprVar
			  | type IDENT ASSIGN expr														   #VoidExprDataType
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)? ')'    #VoidExprIf
			  | WHILE '(' expr ';' voidProg ')'								 		               #VoidExprWhile
			  | FOR '(' IDENT ';' expr ';' voidProg ')'										   #VoidExprFor
			  | SUBPARAMS '(' expr ')'												               #VoidExprSubParams	
			  //ќшибки		
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)?   		    #VoidExprIf
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)? ')' ')'		#VoidExprIf
			  | WHILE '(' expr ';' voidProg	 													        #VoidExprWhile
			  | WHILE '(' expr ';' voidProg ')' ')'										  	 	        #VoidExprWhile
			  | FOR '(' IDENT ';' expr ';' voidProg       										   #VoidExprFor
			  | FOR '(' IDENT ';' expr ';' voidProg ')' ')'										   #VoidExprFor
			  | SUBPARAMS '(' expr       												               #VoidExprSubParams	
			  | SUBPARAMS '(' expr ')' ')'												               #VoidExprSubParams	
			  ;

//¬ыражени€ со значением
expr : cons                                               #ExprCons		
		| SIGNAL                                        #ExprSignal
		| '(' expr ')'					                   #ExprParen		
		| IF '(' expr ';' valueProg (';' expr ';' valueProg)* (';' valueProg)? ')'    #ExprIf        
		| ABSOLUTE '(' expr (';' expr)? ')'  #ExprAbsolute		
		| GRAPHIC '(' IDENT ';' pars ')'     #ExprGraphic
		| TABL '(' IDENT ';' IDENT ';' pars ')'     #ExprTabl
		| TABLC '(' IDENT ';' pars ')'          #ExprTablC
		| IDENT                                          #ExprIdent		
		| IDENT '(' pars ')'			               #ExprFun	
		| expr '.' IDENT		                       #ExprMet
		| expr '.' IDENT '(' pars ')'             #ExprMetFun
		| expr '.' SIGNAL		                       #ExprMetSignal
		| MINUS expr								   #ExprUnary		
		| expr OPER5 expr						   #ExprOper		
		| expr OPER4 expr						   #ExprOper		
		| expr (OPER3 | MINUS) expr         #ExprOper		
		| expr OPER2 expr						   #ExprOper		
		| NOT expr									   #ExprUnary
		| expr OPER1 expr							   #ExprOper				
		//ќшибки		
		| '(' expr						   		           #ExprParen		
		| '(' expr ')' ')'							       #ExprParen		
		| IDENT '(' pars                             #ExprFun
		| IDENT '(' pars ')' ')'				       #ExprFun
		| expr '.' IDENT '(' pars                 #ExprMetFun
		| expr '.' IDENT '(' pars ')' ')'        #ExprMetFun
		| IF '(' expr ';' valueProg (';' expr ';' valueProg)* (';' valueProg)?             #ExprIf
		| IF '(' expr ';' valueProg (';' expr ';' valueProg)* (';' valueProg)? ')' ')'    #ExprIf
		| ABSOLUTE '(' expr (';' expr)?                     #ExprAbsolute		
		| ABSOLUTE '(' expr (';' expr)? ')' ')'            #ExprAbsolute		
		| GRAPHIC '(' IDENT ';' pars                         #ExprGraphic
		| GRAPHIC '(' IDENT ';' pars ')' ')'                #ExprGraphic
		| TABL '(' IDENT (';' IDENT)? ';' pars           #ExprTabl
		| TABL '(' IDENT (';' IDENT)? ';' pars ')' ')'  #ExprTabl
		| TABLC '(' IDENT (';' IDENT)? ';' pars           #ExprTablC
		| TABLC '(' IDENT (';' IDENT)? ';' pars ')' ')'  #ExprTablC
		;

pars : expr (';' expr)*    #ParamsList
	   |                            #ParamsEmpty              
	   ;

//“ип данных переменной
type : DATATYPE                                     #TypeSimple       
	   | ARRAY '(' DATATYPE ')'                #TypeArray
	   | SIGNAL 											#TypeSignal
	   | IDENT ('.' IDENT)*                       #TypeParam
	   ; 

// онстанты
cons : INT                      #ConsInt
	   | REAL                    #ConsReal 
	   | STRING				  #ConsString
	   | TIME					  #ConsTime
	   ;

/* Lexer Rules */
//ѕробелы и комментарии
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//ќперации
fragment OR : [Oo][Rr] | [»и][Ћл][»и];
fragment AND: [Aa][Nn][Dd] | [»и];
fragment XOR : [Xx][Oo][Rr] | [»и][—с][ к][Ћл][»и][Ћл][»и];
fragment LIKE : [Ll][Ii][Kk][Ee] | [ѕп][ќо][ƒд][ќо][Ѕб][Ќн][ќо];
fragment MOD : [Mm][Oo][Dd];
fragment DIV : [Dd][Ii][Vv];

NOT : [Nn][Oo][Tt] | [Ќн][≈е];
MINUS : '-';

OPER5 : ('^');
OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=' | LIKE);
OPER1 : (AND | OR | XOR);

ASSIGN : '=';
					   
// лючевые слова
VOID: [Vv][Oo][Ii][Dd] | [ѕп][”у][—с][“т][ќо][…й];
CALC: [Cc][Aa][Ll][Cc] | [–р][ја][—с][„ч][≈е][“т];

OWNER: [Oo][Ww][Nn][Ee][Rr] | [¬в][Ћл][ја][ƒд][≈е][Ћл][≈е][÷ц];
SUBPARAMS: [Ss][Uu][Bb][Pp][Aa][Rr][Aa][Mm][Ss] | [ѕп][ќо][ƒд][ѕп][ја][–р][ја][ћм][≈е][“т][–р][џы];
ABSOLUTE : [Aa][Bb][Ss][Oo][Ll][Uu][Tt][Ee] | [ја][Ѕб][—с][ќо][Ћл][ёю][“т];

IF : [Ii][Ff] 
	 | [≈е][—с][Ћл][»и]
	 | [Ii][Ff][Pp][Oo][Ii][Nn][Tt][Ss] 
	 | [≈е][—с][Ћл][»и][“т][ќо][„ч][ к][»и];
WHILE : [Ww][Hh][Ii][Ll][Ee] 
			| [ѕп][ќо][ к][ја]
			| [Ww][Hh][Ii][Ll][Ee][Pp][Oo][Ii][Nn][Tt][Ss] 
			| [ѕп][ќо][ к][ја][“т][ќо][„ч][ к][»и];
FOR : [Ff][Oo][Rr] | [ƒд][Ћл][я€];

GRAPHIC :[Gg][Rr][Aa][Pp][HH][Ii][Cc] | [√г][–р][ја][‘ф][»и][ к];
TABLC : [Tt][Aa][Bb][Ll][Cc][Oo][Nn][Tt][Aa][Ii][Nn][Ss] 
			| [“т][ја][Ѕб][Ћл][—с][ќо][ƒд][≈е][–р][∆ж][»и][“т];
TABL : [Tt][Aa][Bb][Ll] 
		 | [“т][ја][Ѕб][Ћл]
		 | [Tt][Aa][Bb][Ll][Ll][Ii][Ss][Tt] 
		 | [“т][ја][Ѕб][Ћл][—с][ѕп][»и][—с][ќо][ к]
		 | [Tt][Aa][Bb][Ll][Dd][Ii][Cc][Nn][Uu][Mm][Bb][Ee][Rr][Ss] 
		 | [“т][ја][Ѕб][Ћл][—с][Ћл][ќо][¬в][ја][–р][№ь][„ч][»и][—с][Ћл][ја]
		 | [Tt][Aa][Bb][Ll][Dd][Ii][Cc][Ss][Tt][Rr][Ii][Nn][Gg][Ss] 
		 | [“т][ја][Ѕб][Ћл][—с][Ћл][ќо][¬в][ја][–р][№ь][—с][“т][–р][ќо][ к][»и]
		 ;

//“ипы данных
DATATYPE : [Bb][Oo][Oo][Ll] | [Ћл][ќо][√г][»и][„ч] 
				  | [Ii][Nn][Tt] | [÷ц][≈е][Ћл][ќо][≈е]
				  | [Rr][Ee][Aa][Ll] | [ƒд][≈е][…й][—с][“т][¬в]
				  | [Tt][Ii][Mm][Ee] | [¬в][–р][≈е][ћм][я€]
				  | [Ss][Tt][Rr][Ii][Nn][Gg] | [—с][“т][–р][ќо][ к][ја]
				  | [Ss][Ee][Gg][Mm][Ee][Nn][Tt][Ss] | [—с][≈е][√г][ћм][≈е][Ќн][“т][џы]
				  ;
//“ипы массивов
ARRAY : [Ll][Ii][Ss][Tt] | [—с][ѕп][»и][—с][ќо][ к]
			| [Dd][Ii][Cc][Nn][Uu][Mm][Bb][Ee][Rr][Ss] | [—с][Ћл][ќо][¬в][ја][–р][№ь][„ч][»и][—с][Ћл][ја]
			| [Dd][Ii][Cc][Ss][Tt][Rr][Ii][Nn][Gg][Ss] | [—с][Ћл][ќо][¬в][ја][–р][№ь][—с][“т][–р][ќо][ к][»и]
			;

// онстанты  и  идентификаторы
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Zа-€ј-я];
fragment IDSYMB : (DIGIT | LETTER);

INT : DIGIT+;
REAL : INT ('.' | ',') INT
		 | INT (('.' | ',') INT) ? 'e' '-' ? INT
		 ;	
TIME : '#' INT '.' INT '.' INT ' '+ INT ':' INT ':' INT '#';

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
SIGNAL: '{' .*? '}';