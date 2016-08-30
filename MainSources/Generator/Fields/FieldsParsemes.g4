parser grammar FieldsParsemes;
options { tokenVocab=FieldsLexemes; }

fieldGen : element* EOF;  

element : TEXT          #ElementText
            | voidProg      #ElementVoid			
			| valueProg     #ElementValue			
			;

voidProg : voidExpr (SEP voidExpr)*;   

valueProg : (voidExpr SEP)* expr;			   

//Выражения без значения
voidExpr : IDENT SET expr														 #VoidExprVar
              | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* COLON voidProg? RPAREN   #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN	 	 #VoidExprWhile
			  | OVERTABL LPAREN voidProg RPAREN						 #VoidExprOver			  
			  | SUBTABL LPAREN (expr COLON)? voidProg RPAREN    #VoidExprSub
			  ;

//Выражения со значением
expr : cons                                              #ExprCons		
		| LPAREN expr RPAREN                 #ExprParen		
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* valueProg? RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN					     #ExprWhile
		| OVERTABL LPAREN valueProg RPAREN														     #ExprOver
		| SUBTABL LPAREN (expr COLON)? valueProg (COLON valueProg)? RPAREN     #ExprSub		
		| IDENT                                                  #ExprIdent		
		| IDENT LPAREN pars RPAREN               #ExprFun	
		| UNARY expr                 #ExprUnary		
		| expr OPER4 expr           #ExprOper		
		| expr OPER3 expr           #ExprOper		
		| expr OPER2 expr           #ExprOper		
		| expr OPER1 expr            #ExprOper		
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
	   | STRING               #ConsString
	   ;