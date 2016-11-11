parser grammar FieldsParsemes;
options { tokenVocab=FieldsLexemes; }

fieldGen : textGen EOF;  

textGen : element*;

element : TEXT            #ElementText            
			| ITEXT          #ElementText
            | TLSQUARE voidProg ERSQUARE     #ElementVoid			
			| ILSQUARE voidProg ERSQUARE     #ElementVoid			
			| TLSQUARE valueProg ERSQUARE    #ElementValue						
			| ILSQUARE valueProg ERSQUARE    #ElementValue						
			//Ошибки		
			| TLSQUARE voidProg						  #ElementVoid			
			| TLSQUARE voidProg ERSQUARE TRSQUARE   #ElementVoid			
			| TLSQUARE valueProg					      #ElementValue					
			| TLSQUARE valueProg ERSQUARE TRSQUARE   #ElementVoid			
			;

voidProg : voidExpr (SEP voidExpr)*;   

valueProg : (voidExpr SEP)* expr;			   

//Выражения без значения
voidExpr : IDENT SET expr															 #VoidExprVar
              | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN   #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN	 		 #VoidExprWhile
			  | OVERTABL LPAREN voidProg RPAREN						     #VoidExprOver
			  | SUBTABL LPAREN voidProg RPAREN							 #VoidExprSub
			  | SUBTABL LPAREN expr COLON voidProg RPAREN		 #VoidExprSub
			  //Ошибки		
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)?    #VoidExprIf
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN RPAREN  #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg	 								#VoidExprWhile
			  | WHILE LPAREN expr COLON voidProg RPAREN RPAREN	 	#VoidExprWhile
			  | OVERTABL LPAREN voidProg  												#VoidExprOver			  
			  | OVERTABL LPAREN voidProg RPAREN RPAREN   					#VoidExprOver			  
			  | SUBTABL LPAREN (expr COLON)? voidProg									#VoidExprSub
			  | SUBTABL LPAREN (expr COLON)? voidProg RPAREN RPAREN		#VoidExprSub
			  ;

//Выражения со значением
expr : cons                                              #ExprCons		
		| LPAREN expr RPAREN                 #ExprParen		
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)? RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN					     #ExprWhile
		| OVERTABL LPAREN valueProg RPAREN														     #ExprOver
		| SUBTABL LPAREN valueProg (COLON valueProg)? (COLON valueProg)? RPAREN     #ExprSub		
		| IDENT                                                  #ExprIdent		
		| FUNCONST (LPAREN RPAREN)?		   #ExprFunConst
		| IDENT LPAREN pars RPAREN               #ExprFun	
		| MINUS expr                 #ExprUnary		
		| expr OPER5 expr           #ExprOper		
		| expr OPER4 expr           #ExprOper		
		| expr  (OPER3 | MINUS) expr   #ExprOper		
		| expr OPER2 expr           #ExprOper		
		| NOT expr		               #ExprUnary
		| expr OPER1 expr           #ExprOper		
		| ELSQUARE textGen IRSQUARE     #ExprTextGen			 
		//Ошибки		
		| LPAREN expr										      #ExprParen		
		| LPAREN expr RPAREN RPAREN               #ExprParen		
		| IDENT LPAREN pars                                #ExprFun
		| IDENT LPAREN pars RPAREN RPAREN    #ExprFun
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)?                                #ExprIf
		| IF LPAREN expr COLON valueProg (COLON expr COLON valueProg)* (COLON valueProg)? RPAREN RPAREN    #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr					              #ExprWhile
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN RPAREN   #ExprWhile
		| OVERTABL LPAREN valueProg      														      #ExprOver
		| OVERTABL LPAREN valueProg RPAREN	RPAREN  								      #ExprOver
		| SUBTABL LPAREN (valueProg COLON)? valueProg (COLON valueProg)?								  #ExprSub		
		| SUBTABL LPAREN (valueProg COLON)? valueProg (COLON valueProg)? RPAREN RPAREN     #ExprSub		
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