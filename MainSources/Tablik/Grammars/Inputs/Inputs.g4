grammar Inputs;
//√рамматика дл€ пол€ Inputs

/* Parser Rules */
prog : param (';' param)*        #ParamList ;

param : arg ASSIGN constVal                      #ParamConst
		  |	valueType                                        #ParamType
		  | valueType PARAMS '(' classList ')'  #ParamParams ;

valueType : arg			                             #TypeGet
		  | arg SIGNALS '(' signalsList ')'  #TypeSignals		  
		  | identChain IDENT                      #TypeClass ;

classList : identChain (';' identChain)*  #ClassListGet ;

identChain : IDENT ('.' IDENT)*        #IdentChainGet ;

signalsList : argSignal? (';' argSignal)*     #SignalsListGet ;

argSignal : arg                             # ArgSignalArg
			   | DATATYPE SIGNAL  # ArgSignalDataType 	
			   | SIGNAL                    # ArgSignalSignal
				;
			   
arg : DATATYPE IDENT   #ArgDataType 
	  | IDENT                     #ArgIdent 
	  ;

constVal : BOOL         #ConstBool	
			| INT 	         #ConstInt
			| STRING     #ConstString 
			| REAL          #ConstReal 
			| TIME         #ConstTime
			;
					
/* Lexer Rules */
//ѕробелы и комментарии
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

// лючевые слова
SIGNALS : [Ss][Ii][Gg][Nn][Aa][Ll][Ss] | [—с][»и][√г][Ќн][ја][Ћл][џы];
PARAMS : [Pp][Aa][Rr][Aa][Mm][Ee][Tt][Ee][Rr][Ss] | [ѕп][ја][–р][ја][ћм][≈е][“т][–р][џы] ;

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

