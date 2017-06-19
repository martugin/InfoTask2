lexer grammar FieldsLexemes;

//����� ������ �� ��������� (DEFAULT_MODE)

TLSQUARE : '['  -> pushMode(EXPRESSION) ;
TRSQUARE : ']';
TEXT: ~('[' | ']')+;

//����� ����������� ������ 
mode INNER_TEXT;
ILSQUARE : '['  -> pushMode(EXPRESSION) ;
IRSQUARE : ']'  -> popMode;
ITEXT: ~('[' | ']')+;

//����� ���������
mode EXPRESSION;
ELSQUARE : '['  -> pushMode(INNER_TEXT) ;
ERSQUARE : ']'  -> popMode;

//������� � �����������
WS  : [' '\n\r\t] -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//�������� �����
OVERTABL : [Oo][Vv][Ee][Rr][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];
SUBTABL : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];

IF : [Ii][Ff] | [��][��][��][��];
WHILE : [Ww][Hh][Ii][Ll][Ee] | [��][��][��][��];

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