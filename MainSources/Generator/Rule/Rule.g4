grammar Rule;

//GenRule ����������
tablGen : IDENT '.' OVERTABL EOF    #TablGenOver
			 | tabl ('.' subTabl)* EOF          #TablGenSimple
			 ;

//GenRule �������������
subTablGen : subTabl ('.' subTabl)* EOF   #SubTablGenIdent				  
				  ; 

tabl : IDENT                           #TablIdent
       | IDENT '(' expr ')'          #TablCond
	   //������
	   | IDENT LPAREN expr             #TablParenLost
	   | IDENT '(' expr ')' RPAREN    #TablParenExtra
	   ;

subTabl : SUBTABL                     #SubTablIdent
			 | SUBTABL '(' expr ')'    #SubTablCond
			 //������
			 | SUBTABL LPAREN expr             #SubTablParenLost
			 | SUBTABL '(' expr ')' RPAREN    #SubTablParenExtra
		     ;

//���������
expr : cons                                #ExprCons		
		| '(' expr ')'                     #ExprParen		
		| IDENT                          #ExprIdent		
		| IDENT '(' pars ')'         #ExprFun		
		| MINUS expr                 #ExprUnary 
		| NOT expr                      #ExprUnary
		| expr OPER4 expr           #ExprOper		
		| expr OPER3 expr           #ExprOper		
		| expr OPER2 expr           #ExprOper		
		| expr OPER1 expr            #ExprOper		
		//������		
		| IDENT LPAREN pars              #ExprFunParenLost		
		| IDENT '(' pars ')' RPAREN    #ExprFunParenExtra		
		;

pars : expr (';' expr)*    #ParamsList
       |                             #ParamsEmpty              
	    ;

//���������
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING                #ConsString
	   ;

//------------------------------------------------------------------------------------------------------------
// Lexer 

//������� � �����������
WS  : [' '\n\r\t] -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//�������� �����
OVERTABL : [Oo][Vv][Ee][Rr][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];
SUBTABL : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];

//��������
fragment OR : [Oo][Rr] | [��][��][��];
fragment AND: [Aa][Nn][Dd] | [��];
fragment XOR : [Xx][Oo][Rr] | [��][��][��][��][��][��][��];
fragment LIKE : [Ll][Ii][Kk][Ee] | [��][��][��][��][��][��][��];
fragment MOD : [Mm][Oo][Dd];
fragment DIV : [Dd][Ii][Vv];

NOT : [Nn][Oo][Tt] | [��][��];
MINUS : '-';

OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' | '-' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=' | LIKE);
OPER1 : (AND | OR | XOR);
					   
//�������
LPAREN : '(';
RPAREN : ')';

//���������  �  ��������������
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Z�-��-�];
fragment IDSYMB : (DIGIT | LETTER);

INT : DIGIT+;
REAL : INT ('.' | ',') INT
         | INT (('.' | ',') INT) ? 'e' '-' ? INT
	     ;

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';