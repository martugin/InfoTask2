grammar Rule;

//GenRule ����������
tablGen : IDENT '.' OVERTABL EOF						  #TablGenOver
             | tabl ('.' subTabl)* EOF						     #TablGenSimple			 
			 ;

//GenRule �������������
subTablGen : subTabl ('.' subTabl)* EOF 
				  ; 

tabl : IDENT                           #TablIdent
       | IDENT '(' expr ')'          #TablCond
	   //������
	   | IDENT '(' expr?             #TablCond
	   | IDENT '(' expr? ')' ')'    #TablCond
	   ;

subTabl : SUBTABL                     #SubTablIdent
			 | SUBTABL '(' expr ')'    #SubTablCond
			 //������
			 | SUBTABL '(' expr?             #SubTablCond
			 | SUBTABL '(' expr? ')' ')'    #SubTablCond
		     ;

//���������
expr : cons                                #ExprCons		
		| '(' expr ')'                     #ExprParen		
		| IDENT                          #ExprIdent		
		| FUNCONST ('(' ')')?     #ExprFunConst
		| IDENT '(' pars ')'         #ExprFun		
		| MINUS expr                 #ExprUnary 
		| expr OPER5 expr           #ExprOper		
		| expr OPER4 expr           #ExprOper		
		| expr (OPER3 | MINUS) expr  #ExprOper		
		| expr OPER2 expr           #ExprOper	
		| NOT expr                      #ExprUnary	
		| expr OPER1 expr            #ExprOper		
		//������		
		| '(' expr                          #ExprParen	
		| '(' expr ')' ')'                 #ExprParen		
		| IDENT '(' pars		        #ExprFun
		| IDENT '(' pars ')' ')'		#ExprFun		
		;

pars : expr (';' expr)*    #ParamsList
       |                             #ParamsEmpty              
	    ;

//���������
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING                #ConsString
	   | TIME					  #ConsTime
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

OPER5 : ('^');
OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=' | LIKE);
OPER1 : (AND | OR | XOR);

//������� - ���������
FUNCONST : ([Tt][Rr][Uu][Ee] | [��][��][��][��][��][��])
                    | ([Ff][Aa][Ll][Ss][Ee] | [��][��][��][��])
					| [Pp][Ii]
					| [Nn][Ee][Ww][Ll][Ii][Nn][Ee]
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