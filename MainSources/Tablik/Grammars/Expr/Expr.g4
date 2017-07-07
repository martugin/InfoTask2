grammar Expr;
//���������� ��� ����� UserExpr

/* Parser Rules */

prog	: voidProg                        #ProgVoid
		| valueProg                      #ProgValue
		;

voidProg : voidExpr (':' voidExpr)*;   

valueProg : (voidExpr ':')* expr;	

//��������� ��� ��������
voidExpr : IDENT ASSIGN expr																   #VoidExprVar
			  | type IDENT ASSIGN expr														   #VoidExprDataType
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)? ')'    #VoidExprIf
			  | WHILE '(' expr ';' voidProg ')'								 		               #VoidExprWhile
			  | FOR '(' IDENT ';' expr ';' voidProg ')'										   #VoidExprFor
			  | SUBPARAMS '(' expr ')'												               #VoidExprSubParams	
			  //������		
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)?   		    #VoidExprIf
			  | IF '(' expr ';' voidProg (';' expr ';' voidProg)* (';' voidProg)? ')' ')'		#VoidExprIf
			  | WHILE '(' expr ';' voidProg	 													        #VoidExprWhile
			  | WHILE '(' expr ';' voidProg ')' ')'										  	 	        #VoidExprWhile
			  | FOR '(' IDENT ';' expr ';' voidProg       										   #VoidExprFor
			  | FOR '(' IDENT ';' expr ';' voidProg ')' ')'										   #VoidExprFor
			  | SUBPARAMS '(' expr       												               #VoidExprSubParams	
			  | SUBPARAMS '(' expr ')' ')'												               #VoidExprSubParams	
			  ;

//��������� �� ���������
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
		//������		
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

//��� ������ ����������
type : DATATYPE                                     #TypeSimple       
	   | ARRAY '(' DATATYPE ')'                #TypeArray
	   | SIGNAL 											#TypeSignal
	   | IDENT ('.' IDENT)*                       #TypeParam
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

ASSIGN : '=';
					   
//�������� �����
VOID: [Vv][Oo][Ii][Dd] | [��][��][��][��][��][��];
CALC: [Cc][Aa][Ll][Cc] | [��][��][��][��][��][��];

OWNER: [Oo][Ww][Nn][Ee][Rr] | [��][��][��][��][��][��][��][��];
SUBPARAMS: [Ss][Uu][Bb][Pp][Aa][Rr][Aa][Mm][Ss] | [��][��][��][��][��][��][��][��][��][��][��][��];
ABSOLUTE : [Aa][Bb][Ss][Oo][Ll][Uu][Tt][Ee] | [��][��][��][��][��][��][��];

IF : [Ii][Ff] 
	 | [��][��][��][��]
	 | [Ii][Ff][Pp][Oo][Ii][Nn][Tt][Ss] 
	 | [��][��][��][��][��][��][��][��][��];
WHILE : [Ww][Hh][Ii][Ll][Ee] 
			| [��][��][��][��]
			| [Ww][Hh][Ii][Ll][Ee][Pp][Oo][Ii][Nn][Tt][Ss] 
			| [��][��][��][��][��][��][��][��][��];
FOR : [Ff][Oo][Rr] | [��][��][��];

GRAPHIC :[Gg][Rr][Aa][Pp][HH][Ii][Cc] | [��][��][��][��][��][��];
TABLC : [Tt][Aa][Bb][Ll][Cc][Oo][Nn][Tt][Aa][Ii][Nn][Ss] 
			| [��][��][��][��][��][��][��][��][��][��][��][��];
TABL : [Tt][Aa][Bb][Ll] 
		 | [��][��][��][��]
		 | [Tt][Aa][Bb][Ll][Ll][Ii][Ss][Tt] 
		 | [��][��][��][��][��][��][��][��][��][��]
		 | [Tt][Aa][Bb][Ll][Dd][Ii][Cc][Nn][Uu][Mm][Bb][Ee][Rr][Ss] 
		 | [��][��][��][��][��][��][��][��][��][��][��][��][��][��][��][��]
		 | [Tt][Aa][Bb][Ll][Dd][Ii][Cc][Ss][Tt][Rr][Ii][Nn][Gg][Ss] 
		 | [��][��][��][��][��][��][��][��][��][��][��][��][��][��][��][��][��]
		 ;

//���� ������
DATATYPE : [Bb][Oo][Oo][Ll] | [��][��][��][��][��] 
				  | [Ii][Nn][Tt] | [��][��][��][��][��]
				  | [Rr][Ee][Aa][Ll] | [��][��][��][��][��][��]
				  | [Tt][Ii][Mm][Ee] | [��][��][��][��][��]
				  | [Ss][Tt][Rr][Ii][Nn][Gg] | [��][��][��][��][��][��]
				  | [Ss][Ee][Gg][Mm][Ee][Nn][Tt][Ss] | [��][��][��][��][��][��][��][��]
				  ;
//���� ��������
ARRAY : [Ll][Ii][Ss][Tt] | [��][��][��][��][��][��]
			| [Dd][Ii][Cc][Nn][Uu][Mm][Bb][Ee][Rr][Ss] | [��][��][��][��][��][��][��][��][��][��][��][��]
			| [Dd][Ii][Cc][Ss][Tt][Rr][Ii][Nn][Gg][Ss] | [��][��][��][��][��][��][��][��][��][��][��][��][��]
			;

//���������  �  ��������������
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Z�-��-�];
fragment IDSYMB : (DIGIT | LETTER);

INT : DIGIT+;
REAL : INT ('.' | ',') INT
		 | INT (('.' | ',') INT) ? 'e' '-' ? INT
		 ;	
TIME : '#' INT '.' INT '.' INT ' '+ INT ':' INT ':' INT '#';

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
SIGNAL: '{' .*? '}';