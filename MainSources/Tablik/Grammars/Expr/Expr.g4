grammar Expr;
//���������� ��� ����� UserExpr

/* Parser Rules */

prog	: voidProg
		| valueProg
		;

voidProg : voidExpr (SEP voidExpr)*;   

valueProg : (voidExpr SEP)* expr;	

//��������� ��� ��������
voidExpr : IDENT SET expr															 #VoidExprVar
              | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN   #VoidExprIf
			  | WHILE LPAREN expr COLON voidProg RPAREN	 		 #VoidExprWhile
			  | OVERTABL LPAREN voidProg RPAREN						     #VoidExprOver
			  | SUBTABL LPAREN voidProg RPAREN							 #VoidExprSub
			  | SUBTABL LPAREN expr COLON voidProg RPAREN		 #VoidExprSub
			  //������		
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)?									#VoidExprIf
			  | IF LPAREN expr COLON voidProg (COLON expr COLON voidProg)* (COLON voidProg)? RPAREN RPAREN		#VoidExprIf
			  | WHILE LPAREN expr COLON voidProg	 								#VoidExprWhile
			  | WHILE LPAREN expr COLON voidProg RPAREN RPAREN	 	#VoidExprWhile
			  | OVERTABL LPAREN voidProg  												#VoidExprOver			  
			  | OVERTABL LPAREN voidProg RPAREN RPAREN   					#VoidExprOver			  
			  | SUBTABL LPAREN (expr COLON)? voidProg									#VoidExprSub
			  | SUBTABL LPAREN (expr COLON)? voidProg RPAREN RPAREN		#VoidExprSub
			  ;

//��������� �� ���������
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
		//������		
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

//���������
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING				  #ConsString
	   | TIME					  #ConsTime
	   ;

/* Lexer Rules */
//������� � �����������
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//��������
fragment OR : [Oo][Rr] | [��][��][��];
fragment AND: [Aa][Nn][Dd] | [��];
fragment XOR : [Xx][Oo][Rr] | [��][��][��][��][��][��][��];
fragment LIKE : [Ll][Ii][Kk][Ee] | [��][��][��][��][��][��][��];
fragment MOD : [Mm][Oo][Dd];
fragment DIV : [Dd][Ii][Vv];

NOT : [Nn][Oo][Tt] | [��][��];
MINUS : '-';

OPER5 : ('^');
OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=' | LIKE);
OPER1 : (AND | OR | XOR);
					   
//�������
SET : '=';
LPAREN : '(';
RPAREN : ')';
COLON : ';';
DOT : '.';
SEP : ':';

//�������� �����
IF : [Ii][Ff] | [��][��][��][��];
WHILE : [Ww][Hh][Ii][Ll][Ee] | [��][��][��][��];
VOID: [Vv][Oo][Ii][Dd] | [��][��][��][��][��][��];
CALC: [Cc][Aa][Ll][Cc] | [��][��][��][��][��][��];
OWNER: [Oo][Ww][Nn][Ee][Rr] | [��][��][��][��][��][��][��][��];
SUBPARAMS: [Ss][Uu][Bb][Pp][Aa][Rr][Aa][Mm][Ss] | [��][��][��][��][��][��][��][��][��][��][��][��];

//���������  �  ��������������
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Z�-��-�];
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