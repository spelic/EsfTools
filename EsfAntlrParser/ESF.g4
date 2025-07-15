//  ESF.g4  – ANTLR-v4 grammar for IBM VisualAge Generator ESF files
//--------------------------------------------------------------
grammar ESF;

/* ============================================================
 *  PARSER RULES
 * ========================================================== */

file        : (statement | textLine)* EOF ;

/* Every construct that starts with a colon in column 1 is a tag.
 * A tag may nest other tags or plain text lines until the (optional)
 * explicit end-tag  appears (the book says you “should” use one).
 */
statement   : tag ;

tag
    : tagStart                 #openOnly            // unpaired (rare)
    | tagStart (statement      // nested tags
               | textLine)* 
      tagEnd?                  #paired              // well-formed pair
    ;

/* Leading colon, tag-name, 0-N attributes, optional period, then EOL */
tagStart    : COLON ID attribute* PERIOD? NL ;

/* End-tags look like “:eTAGNAME.” (case-insensitive) */
tagEnd      : COLON E ID attribute* PERIOD? NL ;

/* key = value         (value may be quoted or bare) */
attribute   : ID EQUALS value ;

/* The book allows:
 *   – single-quoted or double-quoted strings (with doubled quotes inside)
 *   – unquoted literals made up of non-blank, non-control chars
 *   – numbers  (DB2GEN=430 etc.)
 */
value       : STRING
            | NUMBER
            | ID ;

/* Any physical line whose *first* non-blank char is *not* a colon
 * is treated as part of a tag’s free-format “content”.
 */
textLine    : TEXT NL ;

/* ============================================================
 *  LEXER RULES
 * ========================================================== */

COLON       : ':' ;
E           : [eE] ;            // to match :eTAG or :ETAG equally
PERIOD      : '.' ;
EQUALS      : '=' ;

/* quoted strings -------------------------------------------------- */
STRING      : SQSTR | DQSTR ;
fragment
SQSTR       : '\'' ( ~['\r\n] | '\'\'' )* '\'' ;
fragment
DQSTR       : '"'  ( ~["\r\n] | '""'  )* '"'  ;

/* bare tokens ----------------------------------------------------- */
NUMBER      : [0-9]+ ('.' [0-9]+)? ;
ID          : [A-Za-z_][A-Za-z0-9_]* ;

/* a “text” line — everything from first non-blank up to (not incl.) NL
 * so long as that first non-blank is NOT ':'  (i.e. not a tag line)
  */

// lexer rule that should match TEXT when the ':' is *not* the first char
TEXT
    :   ~[\r\n]+               // whatever you use now
        {TokenStartColumn > 0}?   // <-- this compiles
    ;

/* line terminator */
NL          : '\r'? '\n' ;

/* throw-away stuff */
WS          : [ \t]+    -> skip ;

