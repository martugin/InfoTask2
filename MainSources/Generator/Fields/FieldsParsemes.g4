parser grammar FieldsParsemes;
options { tokenVocab=FieldsLexemes; }

fieldGen : textGen EOF;  

textGen : element*;

element : TEXT          #ElementText            
            | TLSQUARE voidProg RSQUARE     #ElementVoid			
			| TLSQUARE valueProg RSQUARE    #ElementValue						
			;

voidProg : voidExpr (SEP voidExpr)*;   

valueProg : (voidExpr SEP)* expr;			   

//Выражения без значения
voidExpr : IDENT SET expr															 #VoidExprVar
              | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN   #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN	 		 #VoidExprWhile
			  | OVERTABL LPAREN voidProg RPAREN							 #VoidExprOver			  
			  | SUBTABL LPAREN (expr COLON)? voidProg RPAREN    #VoidExprSub
			  ;

//Выражения со значением
expr : cons                                              #ExprCons		
		| LPAREN expr RPAREN                 #ExprParen		
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)? RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN					     #ExprWhile
		| OVERTABL LPAREN valueProg RPAREN														     #ExprOver
		| SUBTABL LPAREN (expr COLON)? valueProg (COLON valueProg)? RPAREN     #ExprSub		
		| IDENT                                                  #ExprIdent		
		| FUNCONST ('(' ')')?								#ExprFunConst
		| IDENT LPAREN pars RPAREN               #ExprFun	
		| MINUS expr                 #ExprUnary		
		| expr OPER5 expr           #ExprOper		
		| expr OPER4 expr           #ExprOper		
		| expr  (OPER3 | MINUS) expr   #ExprOper		
		| expr OPER2 expr           #ExprOper		
		| NOT expr		               #ExprUnary
		| expr OPER1 expr           #ExprOper		
		| LSQUARE textGen TRSQUARE     #ExprTextGen			 
		//Ошибки		
		| IDENT LPAREN pars                                #ExprFunParenLost		
		| IDENT LPAREN pars RPAREN RPAREN    #ExprFunParenExtra		
		;

pars : expr (COLON expr)*    #ParamsList
       |                                     #ParamsEmpty              
	   ;

//Константы
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING				  #ConsString
	   | TIME					  #ConsTime
	   ;