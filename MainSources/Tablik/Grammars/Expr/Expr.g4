grammar Expr;
//√рамматика дл€ полей UserExpr

/* Parser Rules */

prog	: voidProg
		| valueProg
		;

voidProg : voidExpr (SEP voidExpr)*;   

valueProg : (voidExpr SEP)* expr;	

//¬ыражени€ без значени€
voidExpr : IDENT SET expr															 #VoidExprVar
              | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN   #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN	 		 #VoidExprWhile
			  | OVERTABL LPAREN voidProg RPAREN						     #VoidExprOver
			  | SUBTABL LPAREN voidProg RPAREN							 #VoidExprSub
			  | SUBTABL LPAREN expr COLON voidProg RPAREN		 #VoidExprSub
			  //ќшибки		
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)?									#VoidExprIf
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN RPAREN		#VoidExprIf
			  | WHILE LPAREN expr COLON voidProg	 								#VoidExprWhile
			  | WHILE LPAREN expr COLON voidProg RPAREN RPAREN	 	#VoidExprWhile
			  | OVERTABL LPAREN voidProg  												#VoidExprOver			  
			  | OVERTABL LPAREN voidProg RPAREN RPAREN   					#VoidExprOver			  
			  | SUBTABL LPAREN (expr COLON)? voidProg									#VoidExprSub
			  | SUBTABL LPAREN (expr COLON)? voidProg RPAREN RPAREN		#VoidExprSub
			  ;

//¬ыражени€ со значением
expr : cons                                              #ExprCons		
		| LPAREN expr RPAREN                 #ExprParen		
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)? RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN					     #ExprWhile		
		| IDENT                                                  #ExprIdent		
		| FUNCONST (LPAREN RPAREN)?		   #ExprFunConst
		| IDENT LPAREN pars RPAREN               #ExprFun	
		| MINUS expr                 #ExprUnary		
		| expr OPER5 expr           #ExprOper		
		| expr OPER4 expr           #ExprOper		
		| expr (OPER3 | MINUS) expr   #ExprOper		
		| expr OPER2 expr           #ExprOper		
		| NOT expr		               #ExprUnary
		| expr OPER1 expr           #ExprOper				
		//ќшибки		
		| LPAREN expr										      #ExprParen		
		| LPAREN expr RPAREN RPAREN               #ExprParen		
		| IDENT LPAREN pars                                #ExprFun
		| IDENT LPAREN pars RPAREN RPAREN    #ExprFun
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)?                                #ExprIf
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)? RPAREN RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr					              #ExprWhile
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN RPAREN   #ExprWhile
		;

pars : expr (COLON expr)*    #ParamsList
       |                                    #ParamsEmpty              
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
					   
//—имволы
SET : '=';
LPAREN : '(';
RPAREN : ')';
COLON : ';';
DOT : '.';
SEP : ':';

// лючевые слова
IF : [Ii][Ff] | [≈е][—с][Ћл][»и];
WHILE : [Ww][Hh][Ii][Ll][Ee] | [ѕп][ќо][ к][ја];
VOID: [Vv][Oo][Ii][Dd] | [ѕп][”у][—с][“т][ќо][…й];
CALC: [Cc][Aa][Ll][Cc] | [–р][ја][—с][„ч][≈е][“т];
OWNER: [Oo][Ww][Nn][Ee][Rr] | [¬в][Ћл][ја][ƒд][≈е][Ћл][≈е][÷ц];
SUBPARAMS: [Ss][Uu][Bb][Pp][Aa][Rr][Aa][Mm][Ss] | [ѕп][ќо][ƒд][ѕп][ја][–р][ја][ћм][≈е][“т][–р][џы];

// онстанты  и  идентификаторы
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Zа-€ј-я];
fragment IDSYMB : (DIGIT | LETTER);

BOOL : [01];
INT : DIGIT+;
REAL : INT ('.' | ',') INT
		 | INT (('.' | ',') INT) ? 'e' '-' ? INT
		 ;	
TIME : '#' INT '.' INT '.' INT ' '+ INT ':' INT ':' INT '#';

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
SIGNAL: '{' .*? '}';

ASSIGN : '=';