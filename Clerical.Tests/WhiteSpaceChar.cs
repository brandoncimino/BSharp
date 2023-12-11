namespace Clerical.Tests;

public enum WhiteSpaceChar {
    Space                   = ' ',      //U+0020	SPACE		
    NonBreakingSpace        = '\u00a0', //U+00A0	NO-BREAK SPACE	 	
    OghamSpaceMark          = '\u1680', //U+1680	OGHAM SPACE MARK	 	
    EnQuad                  = '\u2000', //U+2000	EN QUAD	 	
    EmQuad                  = '\u2001', //U+2001	EM QUAD	 	
    EnSpace                 = '\u2002', //U+2002	EN SPACE	 	
    EmSpace                 = '\u2003', //U+2003	EM SPACE	 	
    ThreePerEmSpace         = '\u2004', //U+2004	THREE-PER-EM SPACE	 	
    FourPerEmSpace          = '\u2005', //U+2005	FOUR-PER-EM SPACE	 	
    SixPerEmSpace           = '\u2006', //U+2006	SIX-PER-EM SPACE	 	
    FigureSpace             = '\u2007', //U+2007	FIGURE SPACE	 	
    PunctuationSpace        = '\u2008', //U+2008	PUNCTUATION SPACE	 	
    ThinSpace               = '\u2009', //U+2009	THIN SPACE	 	
    HairSpace               = '\u200A', //U+200A	HAIR SPACE	 	
    NarrowNoBreakSpace      = '\u202F', //U+202F	NARROW NO-BREAK SPACE	 	
    MediumMathematicalSpace = '\u205F', //U+205F	MEDIUM MATHEMATICAL SPACE	 	
    IdeographicSpace        = '\u3000', //U+3000	IDEOGRAPHIC SPACE	　	

    LineBreak      = '\n',
    CarriageReturn = '\r',
    Tab            = '\t',
}