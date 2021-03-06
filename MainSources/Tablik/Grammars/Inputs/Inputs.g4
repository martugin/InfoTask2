grammar Inputs;
//���������� ��� ���� Inputs

/* Parser Rules */

prog : param (';' param)*;

param : arg                                                  #ParamArg
          | arg ASSIGN constVal                      #ParamConst
		  | ARRAY '(' DATATYPE ')' IDENT   #ParamArray
          | SIGNAL IDENT				              #ParamSignal
          | identChain IDENT                           #ParamClass
          ;
               
arg : DATATYPE IDENT   #ArgDataType 
      | IDENT                     #ArgIdent 
      ;

identChain : IDENT ('.' IDENT)?;

constVal : INT 	         #ConsInt
             | STRING     #ConsString 
             | REAL          #ConsReal 
             | TIME         #ConsTime
             ;
                    
/* Lexer Rules */

//������� � �����������
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

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
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
SIGNAL : '{' .*? '}';

IDENT : IDSYMB* LETTER IDSYMB*;
ASSIGN : '=';

