parser grammar FieldsParsemes;
options { tokenVocab=FieldsLexemes; }

fieldGen : element* EOF;  

element : TEXT        #ElementText
            | prog          #ElementProg			
			;

prog : voidProg                    #ProgVoid
        | valueProg                  #ProgValue        
		;

valueProg : voidProg SEP expr      #ValueProgComplex
               | expr                          #ValueProgSimple
			   ;

voidProg : voidExpr (SEP voidExpr)*;   

//Выражения без значения
voidExpr : IDENT SET expr                                                  #VoidExprVar
              | IF LPAREN ifVoidPars RPAREN                            #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN   #VoidExprWhile
			  ;

ifVoidPars : expr COLON voidProg (COLON expr COLON voidProg)* COLON voidProg?;

//Выражения со значением
expr : cons                                              #ExprCons		
		| LPAREN expr RPAREN                 #ExprParen		
		| IF LPAREN ifPars RPAREN                                                         #ExprIf
		| WHILE LPAREN expr COLON valueProg COLON expr RPAREN   #ExprWhile
		| OVERTABL LPAREN prog RPAREN                                             #ExprOverTabl  
		| SUBTABL LPAREN valueProg (COLON valueProg)* RPAREN        #ExprSubTabl
		| IDENT                                                     #ExprIdent		
		| IDENT LPAREN pars RPAREN                  #ExprFun	
		| MINUS expr                 #ExprUnary 
		| NOT expr                      #ExprUnary	
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

ifPars : expr COLON valueProg (COLON expr COLON valueProg)* valueProg?;

//Константы
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING                #ConsString
	   ;