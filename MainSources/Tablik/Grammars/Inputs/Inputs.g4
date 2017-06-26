grammar Inputs;
//√рамматика дл€ пол€ Inputs

/* Parser Rules */

prog : param (';' param)*;

param : arg                                      #ParamArg
          | arg ASSIGN constVal          #ParamConst
          | SIGNAL IDENT					#ParamSignal
          | identChain IDENT               #ParamClass
          ;
               
arg : DATATYPE IDENT   #ArgDataType 
      | IDENT                     #ArgIdent 
      ;

identChain : IDENT ('.' IDENT)*;

signalChain : IDENT ('.' IDENT)* '.' STRING;

constVal : INT 	         #ConsInt
             | STRING     #ConsString 
             | REAL          #ConsReal 
             | TIME         #ConsTime
             ;
                    
/* Lexer Rules */

//ѕробелы и комментарии
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//“ипы данных
DATATYPE : [Bb][Oo][Oo][Ll] | [Ћл][ќо][√г][»и][„ч] 
                  | [Ii][Nn][Tt] | [÷ц][≈е][Ћл][ќо][≈е]
                  | [Rr][Ee][Aa][Ll] | [ƒд][≈е][…й][—с][“т][¬в]
                  | [Tt][Ii][Mm][Ee] | [¬в][–р][≈е][ћм][я€]
                  | [Ss][Tt][Rr][Ii][Nn][Gg] | [—с][“т][–р][ќо][ к][ја]
                  | [Ss][Ee][Gg][Mm][Ee][Nn][Tt][Ss] | [—с][≈е][√г][ћм][≈е][Ќн][“т][џы] ;

// онстанты  и  идентификаторы
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Zа-€ј-я];
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

