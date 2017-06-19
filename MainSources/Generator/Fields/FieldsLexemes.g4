lexer grammar FieldsLexemes;

//–ежим текста по умолчению (DEFAULT_MODE)

TLSQUARE : '['  -> pushMode(EXPRESSION) ;
TRSQUARE : ']';
TEXT: ~('[' | ']')+;

//–ежим внутреннего текста 
mode INNER_TEXT;
ILSQUARE : '['  -> pushMode(EXPRESSION) ;
IRSQUARE : ']'  -> popMode;
ITEXT: ~('[' | ']')+;

//–ежим выражени€
mode EXPRESSION;
ELSQUARE : '['  -> pushMode(INNER_TEXT) ;
ERSQUARE : ']'  -> popMode;

//ѕробелы и комментарии
WS  : [' '\n\r\t] -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

// лючевые слова
OVERTABL : [Oo][Vv][Ee][Rr][Tt][Aa][Bb][Ll] | [Ќн][ја][ƒд][“т][ја][Ѕб][Ћл];
SUBTABL : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [ѕп][ќо][ƒд][“т][ја][Ѕб][Ћл];

IF : [Ii][Ff] | [≈е][—с][Ћл][»и];
WHILE : [Ww][Hh][Ii][Ll][Ee] | [ѕп][ќо][ к][ја];

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