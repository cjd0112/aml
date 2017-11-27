
grammar GraphQL;

document
   : definition+
   ;

definition
   : operationDefinition | fragmentDefinition
   ;

operationDefinition
   : selectionSet | operationType NAME variableDefinitions? directives? selectionSet
   ;

selectionSet
   : '{' selection ( ','? selection )* '}'
   ;

operationType
   : 'query' | 'mutation'
   ;

selection
   : field | fragmentSpread | inlineFragment
   ;

field
   : fieldName arguments? directives? selectionSet?
   ;

fieldName
   : alias | NAME
   ;

alias
   : NAME ':' NAME
   ;

arguments
   : '(' argument ( ',' argument )* ')'
   ;

argument
   : NAME ':' valueOrVariable
   ;

fragmentSpread
   : '...' fragmentName directives?
   ;

inlineFragment
   : '...' 'on' typeCondition directives? selectionSet
   ;

fragmentDefinition
   : 'fragment' fragmentName 'on' typeCondition directives? selectionSet
   ;

fragmentName
   : NAME
   ;

directives
   : directive+
   ;

directive
   : '@' NAME ':' valueOrVariable | '@' NAME | '@' NAME '(' argument ')'
   ;

typeCondition
   : typeName
   ;

variableDefinitions
   : '(' variableDefinition ( ',' variableDefinition )* ')'
   ;

variableDefinition
   : variable ':' type defaultValue?
   ;

variable
   : '$' NAME
   ;

defaultValue
   : '=' value
   ;

valueOrVariable
   : value | variable
   ;

value
   : STRING # stringValue | NUMBER # numberValue | BOOLEAN # booleanValue | array # arrayValue | object # objectValue | NAME # enumValue
   ;

type
   : typeName nonNullType? | listType nonNullType?
   ;

typeName
   : NAME
   ;

listType
   : '[' type ']'
   ;

nonNullType
   : '!'
   ;

array
   : '[' value ( ',' value )* ']' | '[' ']'
   ;

object 
   : '{' NAME ':' value ( ',' NAME ':' value )* '}'
   ;

BOOLEAN
   : 'true' | 'false'
   ;


NAME
   : [_A-Za-z] [_0-9A-Za-z]*
   ;

STRING
   : '"' ( ESC | ~ ["\\] )* '"'
   ;


fragment ESC
   : '\\' ( ["\\/bfnrt] | UNICODE )
   ;

fragment UNICODE
   : 'u' HEX HEX HEX HEX
   ;

fragment HEX
   : [0-9a-fA-F]
   ;

NUMBER
   : '-'? INT '.' [0-9]+ EXP? | '-'? INT EXP | '-'? INT
   ;

fragment INT
   : '0' | [1-9] [0-9]*
   ;

// no leading zeros
fragment EXP
   : [Ee] [+\-]? INT
   ;

// \- since - means "range" inside [...]
WS
   : [ \t\n\r]+ -> skip
   ;

COMMENT
  :  '#' ~( '\r' | '\n' )* 
  ;