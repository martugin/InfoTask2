grammar Inputs;
//���������� ��� ���� Inputs

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
//������� � �����������
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

//�������� �����
SIGNALS : [Ss][Ii][Gg][Nn][Aa][Ll][Ss] | [��][��][��][��][��][��][��];
PARAMS : [Pp][Aa][Rr][Aa][Mm][Ee][Tt][Ee][Rr][Ss] | [��][��][��][��][��][��][��][��][��] ;

DATATYPE : [Bb][Oo][Oo][Ll] | [��][��][��][��][��] 
				  | [Ii][Nn][Tt] | [��][��][��][��][��]
				  | [Rr][Ee][Aa][Ll] | [��][��][��][��][��][��]
				  | [Tt][Ii][Mm][Ee] | [��][��][��][��][��]
				  | [Ss][Tt][Rr][Ii][Nn][Gg] | [��][��][��][��][��][��]
				  | [Ss][Ee][Gg][Mm][Ee][Nn][Tt][Ss] | [��][��][��][��][��][��][��][��] ;

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

