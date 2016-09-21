grammar Rule;

//GenRule параметров
tablGen : IDENT '.' OVERTABL EOF						  #TablGenOver
             | tabl ('.' subTabl)* EOF						     #TablGenSimple			 
			 ;

//GenRule подпараметров
subTablGen : subTabl ('.' subTabl)* EOF 
				  ; 

tabl : IDENT                           #TablIdent
       | IDENT '(' expr ')'          #TablCond
	   //ќшибки
	   | IDENT '(' expr?             #TablCond
	   | IDENT '(' expr? ')' ')'    #TablCond
	   ;

subTabl : SUBTABL                     #SubTablIdent
			 | SUBTABL '(' expr ')'    #SubTablCond
			 //ќшибки
			 | SUBTABL '(' expr?             #SubTablCond
			 | SUBTABL '(' expr? ')' ')'    #SubTablCond
		     ;

//¬ыражени€
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
		//ќшибки		
		| '(' expr                          #ExprParen	
		| '(' expr ')' ')'                 #ExprParen		
		| IDENT '(' pars		        #ExprFun
		| IDENT '(' pars ')' ')'		#ExprFun		
		;

pars : expr (';' expr)*    #ParamsList
       |                             #ParamsEmpty              
	    ;

// онстанты
cons : INT                      #ConsInt
       | REAL                    #ConsReal 
	   | STRING                #ConsString
	   | TIME					  #ConsTime
	   ;

//------------------------------------------------------------------------------------------------------------
// Lexer 

//ѕробелы и комментарии
WS  : [' '\n\r\t] -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

// лючевые слова
OVERTABL : [Oo][Vv][Ee][Rr][Tt][Aa][Bb][Ll] | [Ќн][ја][ƒд][“т][ја][Ѕб][Ћл];
SUBTABL : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [ѕп][ќо][ƒд][“т][ја][Ѕб][Ћл];

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

//‘ункции - константы
FUNCONST : ([Tt][Rr][Uu][Ee] | [ѕп][–р][ја][¬в][ƒд][ја])
                    | ([Ff][Aa][Ll][Ss][Ee] | [Ћл][ќо][∆ж][№ь])
					| [Pp][Ii]
					| [Nn][Ee][Ww][Ll][Ii][Nn][Ee]
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