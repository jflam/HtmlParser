/*
 * Copyright (c) 2008-2011 Mozilla Foundation
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

using System;
public class ElementName
// uncomment when regenerating self
//        implements Comparable<ElementName> 
{

    /**
     * The mask for extracting the dispatch group.
     */
    public const int GROUP_MASK = 127;

    /**
     * Indicates that the element is not a pre-interned element. Forbidden 
     * on preinterned elements.
     */
    public const int CUSTOM = (1 << 30);

    /**
     * Indicates that the element is in the "special" category. This bit
     * should not be pre-set on MathML or SVG specials--only on HTML specials.
     */
    public const int SPECIAL = (1 << 29);

    /**
     * The element is foster-parenting. This bit should be pre-set on elements
     * that are foster-parenting as HTML.
     */
    public const int FOSTER_PARENTING = (1 << 28);

    /**
     * The element is scoping. This bit should be pre-set on elements
     * that are scoping as HTML.
     */
    public const int SCOPING = (1 << 27);

    /**
     * The element is scoping as SVG.
     */
    public const int SCOPING_AS_SVG = (1 << 26);

    /**
     * The element is scoping as MathML.
     */
    public const int SCOPING_AS_MATHML = (1 << 25);

    /**
     * The element is an HTML integration point.
     */
    public const int HTML_INTEGRATION_POINT = (1 << 24);

    /**
     * The element has an optional end tag.
     */
    public const int OPTIONAL_END_TAG = (1 << 23);

    public readonly ElementName NULL_ELEMENT_NAME = new ElementName(null);

    public readonly string name;

    public readonly string camelCaseName;

    /**
     * The lowest 7 bits are the dispatch group. The high bits are flags.
     */
    public readonly int flags;

    public int getFlags() {
        return flags;
    }
    
    public int getGroup() {
        return flags & GROUP_MASK;
    }
    
    // [NOCPP[
    
    public bool isCustom() {
        return (flags & CUSTOM) != 0;
    }
    
    // ]NOCPP]

    static ElementName elementNameByBuffer(char[] buf, int offset, int length, Interner interner) {
        int hash = ElementName.bufToHash(buf, length);
        int index = Array.BinarySearch(ElementName.ELEMENT_HASHES, hash);
        if (index < 0) {
            return new ElementName(Portability.newLocalNameFromBuffer(buf, offset, length, interner));
        } else {
            ElementName elementName = ElementName.ELEMENT_NAMES[index];
            string name = elementName.name;
            if (!Portability.localEqualsBuffer(name, buf, offset, length)) {
                return new ElementName(Portability.newLocalNameFromBuffer(buf,
                        offset, length, interner));
            }
            return elementName;
        }
    }

    /**
     * This method has to return a unique integer for each well-known
     * lower-cased element name.
     * 
     * @param buf
     * @param len
     * @return
     */
    private static int bufToHash(char[] buf, int len) {
        int hash = len;
        hash <<= 5;
        hash += buf[0] - 0x60;
        int j = len;
        for (int i = 0; i < 4 && j > 0; i++) {
            j--;
            hash <<= 5;
            hash += buf[j] - 0x60;
        }
        return hash;
    }

    private ElementName(string name, string camelCaseName, int flags) {
        this.name = name;
        this.camelCaseName = camelCaseName;
        this.flags = flags;
    }

    protected ElementName(string name) {
        this.name = name;
        this.camelCaseName = name;
        this.flags = TreeBuilderBase.OTHER | CUSTOM;
    }
    
    void release() {
        // No-op in Java. 
        // Implement as delete this in subclass.
        // Be sure to release the local name
    }
    
    private void destructor() {
    }

    public ElementName cloneElementName(Interner interner) {
        return this;
    }
    
    // START CODE ONLY USED FOR GENERATING CODE uncomment and run to regenerate

//    /**
//     * @see java.lang.Object#tostring()
//     */
//    @Override public string tostring() {
//        return "(\"" + name + "\", \"" + camelCaseName + "\", " + decomposedFlags() + ")";
//    }
//
//    private string decomposedFlags() {
//        stringBuilder buf = new stringBuilder("TreeBuilder.");
//        buf.append(treeBuilderGroupToName());
//        if ((flags & SPECIAL) != 0) {
//            buf.append(" | SPECIAL");
//        }
//        if ((flags & FOSTER_PARENTING) != 0) {
//            buf.append(" | FOSTER_PARENTING");
//        }
//        if ((flags & SCOPING) != 0) {
//            buf.append(" | SCOPING");
//        }        
//        if ((flags & SCOPING_AS_MATHML) != 0) {
//            buf.append(" | SCOPING_AS_MATHML");
//        }
//        if ((flags & SCOPING_AS_SVG) != 0) {
//            buf.append(" | SCOPING_AS_SVG");
//        }
//        if ((flags & OPTIONAL_END_TAG) != 0) {
//            buf.append(" | OPTIONAL_END_TAG");
//        }
//        return buf.tostring();
//    }
//    
//    private string constName() {
//        char[] buf = new char[name.length()];
//        for (int i = 0; i < name.length(); i++) {
//            char c = name.charAt(i);
//            if (c == '-') {
//                buf[i] = '_';
//            } else if (c >= '0' && c <= '9') {
//                buf[i] = c;
//            } else {
//                buf[i] = (char) (c - 0x20);
//            }
//        }
//        return new string(buf);
//    }
//
//    private int hash() {
//        return bufToHash(name.toCharArray(), name.length());
//    }
//
//    public int compareTo(ElementName other) {
//        int thisHash = this.hash();
//        int otherHash = other.hash();
//        if (thisHash < otherHash) {
//            return -1;
//        } else if (thisHash == otherHash) {
//            return 0;
//        } else {
//            return 1;
//        }
//    }
//
//    private string treeBuilderGroupToName() {
//        switch (getGroup()) {
//            case TreeBuilder.OTHER:
//                return "OTHER";
//            case TreeBuilder.A:
//                return "A";
//            case TreeBuilder.BASE:
//                return "BASE";
//            case TreeBuilder.BODY:
//                return "BODY";
//            case TreeBuilder.BR:
//                return "BR";
//            case TreeBuilder.BUTTON:
//                return "BUTTON";
//            case TreeBuilder.CAPTION:
//                return "CAPTION";
//            case TreeBuilder.COL:
//                return "COL";
//            case TreeBuilder.COLGROUP:
//                return "COLGROUP";
//            case TreeBuilder.FONT:
//                return "FONT";
//            case TreeBuilder.FORM:
//                return "FORM";
//            case TreeBuilder.FRAME:
//                return "FRAME";
//            case TreeBuilder.FRAMESET:
//                return "FRAMESET";
//            case TreeBuilder.IMAGE:
//                return "IMAGE";
//            case TreeBuilder.INPUT:
//                return "INPUT";
//            case TreeBuilder.ISINDEX:
//                return "ISINDEX";
//            case TreeBuilder.LI:
//                return "LI";
//            case TreeBuilder.LINK_OR_BASEFONT_OR_BGSOUND:
//                return "LINK_OR_BASEFONT_OR_BGSOUND";
//            case TreeBuilder.MATH:
//                return "MATH";
//            case TreeBuilder.META:
//                return "META";
//            case TreeBuilder.SVG:
//                return "SVG";
//            case TreeBuilder.HEAD:
//                return "HEAD";
//            case TreeBuilder.HR:
//                return "HR";
//            case TreeBuilder.HTML:
//                return "HTML";
//            case TreeBuilder.KEYGEN:
//                return "KEYGEN";
//            case TreeBuilder.NOBR:
//                return "NOBR";
//            case TreeBuilder.NOFRAMES:
//                return "NOFRAMES";
//            case TreeBuilder.NOSCRIPT:
//                return "NOSCRIPT";
//            case TreeBuilder.OPTGROUP:
//                return "OPTGROUP";
//            case TreeBuilder.OPTION:
//                return "OPTION";
//            case TreeBuilder.P:
//                return "P";
//            case TreeBuilder.PLAINTEXT:
//                return "PLAINTEXT";
//            case TreeBuilder.SCRIPT:
//                return "SCRIPT";
//            case TreeBuilder.SELECT:
//                return "SELECT";
//            case TreeBuilder.STYLE:
//                return "STYLE";
//            case TreeBuilder.TABLE:
//                return "TABLE";
//            case TreeBuilder.TEXTAREA:
//                return "TEXTAREA";
//            case TreeBuilder.TITLE:
//                return "TITLE";
//            case TreeBuilder.TR:
//                return "TR";
//            case TreeBuilder.XMP:
//                return "XMP";
//            case TreeBuilder.TBODY_OR_THEAD_OR_TFOOT:
//                return "TBODY_OR_THEAD_OR_TFOOT";
//            case TreeBuilder.TD_OR_TH:
//                return "TD_OR_TH";
//            case TreeBuilder.DD_OR_DT:
//                return "DD_OR_DT";
//            case TreeBuilder.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6:
//                return "H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6";
//            case TreeBuilder.OBJECT:
//                return "OBJECT";
//            case TreeBuilder.OUTPUT_OR_LABEL:
//                return "OUTPUT_OR_LABEL";
//            case TreeBuilder.MARQUEE_OR_APPLET:
//                return "MARQUEE_OR_APPLET";
//            case TreeBuilder.PRE_OR_LISTING:
//                return "PRE_OR_LISTING";
//            case TreeBuilder.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U:
//                return "B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U";
//            case TreeBuilder.UL_OR_OL_OR_DL:
//                return "UL_OR_OL_OR_DL";
//            case TreeBuilder.IFRAME:
//                return "IFRAME";
//            case TreeBuilder.NOEMBED:
//                return "NOEMBED";
//            case TreeBuilder.EMBED_OR_IMG:
//                return "EMBED_OR_IMG";
//            case TreeBuilder.AREA_OR_WBR:
//                return "AREA_OR_WBR";
//            case TreeBuilder.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU:
//                return "DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU";
//            case TreeBuilder.FIELDSET:
//                return "FIELDSET";
//            case TreeBuilder.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY:
//                return "ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY";
//            case TreeBuilder.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR:
//                return "RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR";
//            case TreeBuilder.RT_OR_RP:
//                return "RT_OR_RP";
//            case TreeBuilder.COMMAND:
//                return "COMMAND";
//            case TreeBuilder.PARAM_OR_SOURCE_OR_TRACK:
//                return "PARAM_OR_SOURCE_OR_TRACK";
//            case TreeBuilder.MGLYPH_OR_MALIGNMARK:
//                return "MGLYPH_OR_MALIGNMARK";
//            case TreeBuilder.MI_MO_MN_MS_MTEXT:
//                return "MI_MO_MN_MS_MTEXT";
//            case TreeBuilder.ANNOTATION_XML:
//                return "ANNOTATION_XML";
//            case TreeBuilder.FOREIGNOBJECT_OR_DESC:
//                return "FOREIGNOBJECT_OR_DESC";
//            case TreeBuilder.MENUITEM:
//                return "MENUITEM";
//        }
//        return null;
//    }
//
//    /**
//     * Regenerate self
//     * 
//     * @param args
//     */
//    public static void main(string[] args) {
//        Arrays.sort(ELEMENT_NAMES);
//        for (int i = 1; i < ELEMENT_NAMES.length; i++) {
//            if (ELEMENT_NAMES[i].hash() == ELEMENT_NAMES[i - 1].hash()) {
//                System.err.println("Hash collision: " + ELEMENT_NAMES[i].name
//                        + ", " + ELEMENT_NAMES[i - 1].name);
//                return;
//            }
//        }
//        for (int i = 0; i < ELEMENT_NAMES.length; i++) {
//            ElementName el = ELEMENT_NAMES[i];
//            System.out.println("public const ElementName "
//                    + el.constName() + " = new ElementName" + el.tostring()
//                    + ";");
//        }
//        System.out.println("private final static @NoLength ElementName[] ELEMENT_NAMES = {");
//        for (int i = 0; i < ELEMENT_NAMES.length; i++) {
//            ElementName el = ELEMENT_NAMES[i];
//            System.out.println(el.constName() + ",");
//        }
//        System.out.println("};");
//        System.out.println("private final static int[] ELEMENT_HASHES = {");
//        for (int i = 0; i < ELEMENT_NAMES.length; i++) {
//            ElementName el = ELEMENT_NAMES[i];
//            System.out.println(Integer.tostring(el.hash()) + ",");
//        }
//        System.out.println("};");
//    }

    // START GENERATED CODE
    public static readonly ElementName A = new ElementName("a", "a", TreeBuilderBase.A);
    public static readonly ElementName B = new ElementName("b", "b", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName G = new ElementName("g", "g", TreeBuilderBase.OTHER);
    public static readonly ElementName I = new ElementName("i", "i", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName P = new ElementName("p", "p", TreeBuilderBase.P | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName Q = new ElementName("q", "q", TreeBuilderBase.OTHER);
    public static readonly ElementName S = new ElementName("s", "s", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName U = new ElementName("u", "u", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName BR = new ElementName("br", "br", TreeBuilderBase.BR | SPECIAL);
    public static readonly ElementName CI = new ElementName("ci", "ci", TreeBuilderBase.OTHER);
    public static readonly ElementName CN = new ElementName("cn", "cn", TreeBuilderBase.OTHER);
    public static readonly ElementName DD = new ElementName("dd", "dd", TreeBuilderBase.DD_OR_DT | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName DL = new ElementName("dl", "dl", TreeBuilderBase.UL_OR_OL_OR_DL | SPECIAL);
    public static readonly ElementName DT = new ElementName("dt", "dt", TreeBuilderBase.DD_OR_DT | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName EM = new ElementName("em", "em", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName EQ = new ElementName("eq", "eq", TreeBuilderBase.OTHER);
    public static readonly ElementName FN = new ElementName("fn", "fn", TreeBuilderBase.OTHER);
    public static readonly ElementName H1 = new ElementName("h1", "h1", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName H2 = new ElementName("h2", "h2", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName H3 = new ElementName("h3", "h3", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName H4 = new ElementName("h4", "h4", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName H5 = new ElementName("h5", "h5", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName H6 = new ElementName("h6", "h6", TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
    public static readonly ElementName GT = new ElementName("gt", "gt", TreeBuilderBase.OTHER);
    public static readonly ElementName HR = new ElementName("hr", "hr", TreeBuilderBase.HR | SPECIAL);
    public static readonly ElementName IN = new ElementName("in", "in", TreeBuilderBase.OTHER);
    public static readonly ElementName LI = new ElementName("li", "li", TreeBuilderBase.LI | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName LN = new ElementName("ln", "ln", TreeBuilderBase.OTHER);
    public static readonly ElementName LT = new ElementName("lt", "lt", TreeBuilderBase.OTHER);
    public static readonly ElementName MI = new ElementName("mi", "mi", TreeBuilderBase.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
    public static readonly ElementName MN = new ElementName("mn", "mn", TreeBuilderBase.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
    public static readonly ElementName MO = new ElementName("mo", "mo", TreeBuilderBase.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
    public static readonly ElementName MS = new ElementName("ms", "ms", TreeBuilderBase.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
    public static readonly ElementName OL = new ElementName("ol", "ol", TreeBuilderBase.UL_OR_OL_OR_DL | SPECIAL);
    public static readonly ElementName OR = new ElementName("or", "or", TreeBuilderBase.OTHER);
    public static readonly ElementName PI = new ElementName("pi", "pi", TreeBuilderBase.OTHER);
    public static readonly ElementName RP = new ElementName("rp", "rp", TreeBuilderBase.RT_OR_RP | OPTIONAL_END_TAG);
    public static readonly ElementName RT = new ElementName("rt", "rt", TreeBuilderBase.RT_OR_RP | OPTIONAL_END_TAG);
    public static readonly ElementName TD = new ElementName("td", "td", TreeBuilderBase.TD_OR_TH | SPECIAL | SCOPING | OPTIONAL_END_TAG);
    public static readonly ElementName TH = new ElementName("th", "th", TreeBuilderBase.TD_OR_TH | SPECIAL | SCOPING | OPTIONAL_END_TAG);
    public static readonly ElementName TR = new ElementName("tr", "tr", TreeBuilderBase.TR | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
    public static readonly ElementName TT = new ElementName("tt", "tt", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName UL = new ElementName("ul", "ul", TreeBuilderBase.UL_OR_OL_OR_DL | SPECIAL);
    public static readonly ElementName AND = new ElementName("and", "and", TreeBuilderBase.OTHER);
    public static readonly ElementName ARG = new ElementName("arg", "arg", TreeBuilderBase.OTHER);
    public static readonly ElementName ABS = new ElementName("abs", "abs", TreeBuilderBase.OTHER);
    public static readonly ElementName BIG = new ElementName("big", "big", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName BDO = new ElementName("bdo", "bdo", TreeBuilderBase.OTHER);
    public static readonly ElementName CSC = new ElementName("csc", "csc", TreeBuilderBase.OTHER);
    public static readonly ElementName COL = new ElementName("col", "col", TreeBuilderBase.COL | SPECIAL);
    public static readonly ElementName COS = new ElementName("cos", "cos", TreeBuilderBase.OTHER);
    public static readonly ElementName COT = new ElementName("cot", "cot", TreeBuilderBase.OTHER);
    public static readonly ElementName DEL = new ElementName("del", "del", TreeBuilderBase.OTHER);
    public static readonly ElementName DFN = new ElementName("dfn", "dfn", TreeBuilderBase.OTHER);
    public static readonly ElementName DIR = new ElementName("dir", "dir", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName DIV = new ElementName("div", "div", TreeBuilderBase.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
    public static readonly ElementName EXP = new ElementName("exp", "exp", TreeBuilderBase.OTHER);
    public static readonly ElementName GCD = new ElementName("gcd", "gcd", TreeBuilderBase.OTHER);
    public static readonly ElementName GEQ = new ElementName("geq", "geq", TreeBuilderBase.OTHER);
    public static readonly ElementName IMG = new ElementName("img", "img", TreeBuilderBase.EMBED_OR_IMG | SPECIAL);
    public static readonly ElementName INS = new ElementName("ins", "ins", TreeBuilderBase.OTHER);
    public static readonly ElementName INT = new ElementName("int", "int", TreeBuilderBase.OTHER);
    public static readonly ElementName KBD = new ElementName("kbd", "kbd", TreeBuilderBase.OTHER);
    public static readonly ElementName LOG = new ElementName("log", "log", TreeBuilderBase.OTHER);
    public static readonly ElementName LCM = new ElementName("lcm", "lcm", TreeBuilderBase.OTHER);
    public static readonly ElementName LEQ = new ElementName("leq", "leq", TreeBuilderBase.OTHER);
    public static readonly ElementName MTD = new ElementName("mtd", "mtd", TreeBuilderBase.OTHER);
    public static readonly ElementName MIN = new ElementName("min", "min", TreeBuilderBase.OTHER);
    public static readonly ElementName MAP = new ElementName("map", "map", TreeBuilderBase.OTHER);
    public static readonly ElementName MTR = new ElementName("mtr", "mtr", TreeBuilderBase.OTHER);
    public static readonly ElementName MAX = new ElementName("max", "max", TreeBuilderBase.OTHER);
    public static readonly ElementName NEQ = new ElementName("neq", "neq", TreeBuilderBase.OTHER);
    public static readonly ElementName NOT = new ElementName("not", "not", TreeBuilderBase.OTHER);
    public static readonly ElementName NAV = new ElementName("nav", "nav", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName PRE = new ElementName("pre", "pre", TreeBuilderBase.PRE_OR_LISTING | SPECIAL);
    public static readonly ElementName REM = new ElementName("rem", "rem", TreeBuilderBase.OTHER);
    public static readonly ElementName SUB = new ElementName("sub", "sub", TreeBuilderBase.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
    public static readonly ElementName SEC = new ElementName("sec", "sec", TreeBuilderBase.OTHER);
    public static readonly ElementName SVG = new ElementName("svg", "svg", TreeBuilderBase.SVG);
    public static readonly ElementName SUM = new ElementName("sum", "sum", TreeBuilderBase.OTHER);
    public static readonly ElementName SIN = new ElementName("sin", "sin", TreeBuilderBase.OTHER);
    public static readonly ElementName SEP = new ElementName("sep", "sep", TreeBuilderBase.OTHER);
    public static readonly ElementName SUP = new ElementName("sup", "sup", TreeBuilderBase.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
    public static readonly ElementName SET = new ElementName("set", "set", TreeBuilderBase.OTHER);
    public static readonly ElementName TAN = new ElementName("tan", "tan", TreeBuilderBase.OTHER);
    public static readonly ElementName USE = new ElementName("use", "use", TreeBuilderBase.OTHER);
    public static readonly ElementName VAR = new ElementName("var", "var", TreeBuilderBase.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
    public static readonly ElementName WBR = new ElementName("wbr", "wbr", TreeBuilderBase.AREA_OR_WBR | SPECIAL);
    public static readonly ElementName XMP = new ElementName("xmp", "xmp", TreeBuilderBase.XMP);
    public static readonly ElementName XOR = new ElementName("xor", "xor", TreeBuilderBase.OTHER);
    public static readonly ElementName AREA = new ElementName("area", "area", TreeBuilderBase.AREA_OR_WBR | SPECIAL);
    public static readonly ElementName ABBR = new ElementName("abbr", "abbr", TreeBuilderBase.OTHER);
    public static readonly ElementName BASE = new ElementName("base", "base", TreeBuilderBase.BASE | SPECIAL);
    public static readonly ElementName BVAR = new ElementName("bvar", "bvar", TreeBuilderBase.OTHER);
    public static readonly ElementName BODY = new ElementName("body", "body", TreeBuilderBase.BODY | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName CARD = new ElementName("card", "card", TreeBuilderBase.OTHER);
    public static readonly ElementName CODE = new ElementName("code", "code", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName CITE = new ElementName("cite", "cite", TreeBuilderBase.OTHER);
    public static readonly ElementName CSCH = new ElementName("csch", "csch", TreeBuilderBase.OTHER);
    public static readonly ElementName COSH = new ElementName("cosh", "cosh", TreeBuilderBase.OTHER);
    public static readonly ElementName COTH = new ElementName("coth", "coth", TreeBuilderBase.OTHER);
    public static readonly ElementName CURL = new ElementName("curl", "curl", TreeBuilderBase.OTHER);
    public static readonly ElementName DESC = new ElementName("desc", "desc", TreeBuilderBase.FOREIGNOBJECT_OR_DESC | SCOPING_AS_SVG);
    public static readonly ElementName DIFF = new ElementName("diff", "diff", TreeBuilderBase.OTHER);
    public static readonly ElementName DEFS = new ElementName("defs", "defs", TreeBuilderBase.OTHER);
    public static readonly ElementName FORM = new ElementName("form", "form", TreeBuilderBase.FORM | SPECIAL);
    public static readonly ElementName FONT = new ElementName("font", "font", TreeBuilderBase.FONT);
    public static readonly ElementName GRAD = new ElementName("grad", "grad", TreeBuilderBase.OTHER);
    public static readonly ElementName HEAD = new ElementName("head", "head", TreeBuilderBase.HEAD | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName HTML = new ElementName("html", "html", TreeBuilderBase.HTML | SPECIAL | SCOPING | OPTIONAL_END_TAG);
    public static readonly ElementName LINE = new ElementName("line", "line", TreeBuilderBase.OTHER);
    public static readonly ElementName LINK = new ElementName("link", "link", TreeBuilderBase.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
    public static readonly ElementName LIST = new ElementName("list", "list", TreeBuilderBase.OTHER);
    public static readonly ElementName META = new ElementName("meta", "meta", TreeBuilderBase.META | SPECIAL);
    public static readonly ElementName MSUB = new ElementName("msub", "msub", TreeBuilderBase.OTHER);
    public static readonly ElementName MODE = new ElementName("mode", "mode", TreeBuilderBase.OTHER);
    public static readonly ElementName MATH = new ElementName("math", "math", TreeBuilderBase.MATH);
    public static readonly ElementName MARK = new ElementName("mark", "mark", TreeBuilderBase.OTHER);
    public static readonly ElementName MASK = new ElementName("mask", "mask", TreeBuilderBase.OTHER);
    public static readonly ElementName MEAN = new ElementName("mean", "mean", TreeBuilderBase.OTHER);
    public static readonly ElementName MSUP = new ElementName("msup", "msup", TreeBuilderBase.OTHER);
    public static readonly ElementName MENU = new ElementName("menu", "menu", TreeBuilderBase.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
    public static readonly ElementName MROW = new ElementName("mrow", "mrow", TreeBuilderBase.OTHER);
    public static readonly ElementName NONE = new ElementName("none", "none", TreeBuilderBase.OTHER);
    public static readonly ElementName NOBR = new ElementName("nobr", "nobr", TreeBuilderBase.NOBR);
    public static readonly ElementName NEST = new ElementName("nest", "nest", TreeBuilderBase.OTHER);
    public static readonly ElementName PATH = new ElementName("path", "path", TreeBuilderBase.OTHER);
    public static readonly ElementName PLUS = new ElementName("plus", "plus", TreeBuilderBase.OTHER);
    public static readonly ElementName RULE = new ElementName("rule", "rule", TreeBuilderBase.OTHER);
    public static readonly ElementName REAL = new ElementName("real", "real", TreeBuilderBase.OTHER);
    public static readonly ElementName RELN = new ElementName("reln", "reln", TreeBuilderBase.OTHER);
    public static readonly ElementName RECT = new ElementName("rect", "rect", TreeBuilderBase.OTHER);
    public static readonly ElementName ROOT = new ElementName("root", "root", TreeBuilderBase.OTHER);
    public static readonly ElementName RUBY = new ElementName("ruby", "ruby", TreeBuilderBase.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
    public static readonly ElementName SECH = new ElementName("sech", "sech", TreeBuilderBase.OTHER);
    public static readonly ElementName SINH = new ElementName("sinh", "sinh", TreeBuilderBase.OTHER);
    public static readonly ElementName SPAN = new ElementName("span", "span", TreeBuilderBase.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
    public static readonly ElementName SAMP = new ElementName("samp", "samp", TreeBuilderBase.OTHER);
    public static readonly ElementName STOP = new ElementName("stop", "stop", TreeBuilderBase.OTHER);
    public static readonly ElementName SDEV = new ElementName("sdev", "sdev", TreeBuilderBase.OTHER);
    public static readonly ElementName TIME = new ElementName("time", "time", TreeBuilderBase.OTHER);
    public static readonly ElementName TRUE = new ElementName("true", "true", TreeBuilderBase.OTHER);
    public static readonly ElementName TREF = new ElementName("tref", "tref", TreeBuilderBase.OTHER);
    public static readonly ElementName TANH = new ElementName("tanh", "tanh", TreeBuilderBase.OTHER);
    public static readonly ElementName TEXT = new ElementName("text", "text", TreeBuilderBase.OTHER);
    public static readonly ElementName VIEW = new ElementName("view", "view", TreeBuilderBase.OTHER);
    public static readonly ElementName ASIDE = new ElementName("aside", "aside", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName AUDIO = new ElementName("audio", "audio", TreeBuilderBase.OTHER);
    public static readonly ElementName APPLY = new ElementName("apply", "apply", TreeBuilderBase.OTHER);
    public static readonly ElementName EMBED = new ElementName("embed", "embed", TreeBuilderBase.EMBED_OR_IMG | SPECIAL);
    public static readonly ElementName FRAME = new ElementName("frame", "frame", TreeBuilderBase.FRAME | SPECIAL);
    public static readonly ElementName FALSE = new ElementName("false", "false", TreeBuilderBase.OTHER);
    public static readonly ElementName FLOOR = new ElementName("floor", "floor", TreeBuilderBase.OTHER);
    public static readonly ElementName GLYPH = new ElementName("glyph", "glyph", TreeBuilderBase.OTHER);
    public static readonly ElementName HKERN = new ElementName("hkern", "hkern", TreeBuilderBase.OTHER);
    public static readonly ElementName IMAGE = new ElementName("image", "image", TreeBuilderBase.IMAGE | SPECIAL);
    public static readonly ElementName IDENT = new ElementName("ident", "ident", TreeBuilderBase.OTHER);
    public static readonly ElementName INPUT = new ElementName("input", "input", TreeBuilderBase.INPUT | SPECIAL);
    public static readonly ElementName LABEL = new ElementName("label", "label", TreeBuilderBase.OUTPUT_OR_LABEL);
    public static readonly ElementName LIMIT = new ElementName("limit", "limit", TreeBuilderBase.OTHER);
    public static readonly ElementName MFRAC = new ElementName("mfrac", "mfrac", TreeBuilderBase.OTHER);
    public static readonly ElementName MPATH = new ElementName("mpath", "mpath", TreeBuilderBase.OTHER);
    public static readonly ElementName METER = new ElementName("meter", "meter", TreeBuilderBase.OTHER);
    public static readonly ElementName MOVER = new ElementName("mover", "mover", TreeBuilderBase.OTHER);
    public static readonly ElementName MINUS = new ElementName("minus", "minus", TreeBuilderBase.OTHER);
    public static readonly ElementName MROOT = new ElementName("mroot", "mroot", TreeBuilderBase.OTHER);
    public static readonly ElementName MSQRT = new ElementName("msqrt", "msqrt", TreeBuilderBase.OTHER);
    public static readonly ElementName MTEXT = new ElementName("mtext", "mtext", TreeBuilderBase.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
    public static readonly ElementName NOTIN = new ElementName("notin", "notin", TreeBuilderBase.OTHER);
    public static readonly ElementName PIECE = new ElementName("piece", "piece", TreeBuilderBase.OTHER);
    public static readonly ElementName PARAM = new ElementName("param", "param", TreeBuilderBase.PARAM_OR_SOURCE_OR_TRACK | SPECIAL);
    public static readonly ElementName POWER = new ElementName("power", "power", TreeBuilderBase.OTHER);
    public static readonly ElementName REALS = new ElementName("reals", "reals", TreeBuilderBase.OTHER);
    public static readonly ElementName STYLE = new ElementName("style", "style", TreeBuilderBase.STYLE | SPECIAL);
    public static readonly ElementName SMALL = new ElementName("small", "small", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName THEAD = new ElementName("thead", "thead", TreeBuilderBase.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
    public static readonly ElementName TABLE = new ElementName("table", "table", TreeBuilderBase.TABLE | SPECIAL | FOSTER_PARENTING | SCOPING);
    public static readonly ElementName TITLE = new ElementName("title", "title", TreeBuilderBase.TITLE | SPECIAL | SCOPING_AS_SVG);
    public static readonly ElementName TRACK = new ElementName("track", "track", TreeBuilderBase.PARAM_OR_SOURCE_OR_TRACK);
    public static readonly ElementName TSPAN = new ElementName("tspan", "tspan", TreeBuilderBase.OTHER);
    public static readonly ElementName TIMES = new ElementName("times", "times", TreeBuilderBase.OTHER);
    public static readonly ElementName TFOOT = new ElementName("tfoot", "tfoot", TreeBuilderBase.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
    public static readonly ElementName TBODY = new ElementName("tbody", "tbody", TreeBuilderBase.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
    public static readonly ElementName UNION = new ElementName("union", "union", TreeBuilderBase.OTHER);
    public static readonly ElementName VKERN = new ElementName("vkern", "vkern", TreeBuilderBase.OTHER);
    public static readonly ElementName VIDEO = new ElementName("video", "video", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCSEC = new ElementName("arcsec", "arcsec", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCCSC = new ElementName("arccsc", "arccsc", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCTAN = new ElementName("arctan", "arctan", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCSIN = new ElementName("arcsin", "arcsin", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCCOS = new ElementName("arccos", "arccos", TreeBuilderBase.OTHER);
    public static readonly ElementName APPLET = new ElementName("applet", "applet", TreeBuilderBase.MARQUEE_OR_APPLET | SPECIAL | SCOPING);
    public static readonly ElementName ARCCOT = new ElementName("arccot", "arccot", TreeBuilderBase.OTHER);
    public static readonly ElementName APPROX = new ElementName("approx", "approx", TreeBuilderBase.OTHER);
    public static readonly ElementName BUTTON = new ElementName("button", "button", TreeBuilderBase.BUTTON | SPECIAL);
    public static readonly ElementName CIRCLE = new ElementName("circle", "circle", TreeBuilderBase.OTHER);
    public static readonly ElementName CENTER = new ElementName("center", "center", TreeBuilderBase.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
    public static readonly ElementName CURSOR = new ElementName("cursor", "cursor", TreeBuilderBase.OTHER);
    public static readonly ElementName CANVAS = new ElementName("canvas", "canvas", TreeBuilderBase.OTHER);
    public static readonly ElementName DIVIDE = new ElementName("divide", "divide", TreeBuilderBase.OTHER);
    public static readonly ElementName DEGREE = new ElementName("degree", "degree", TreeBuilderBase.OTHER);
    public static readonly ElementName DOMAIN = new ElementName("domain", "domain", TreeBuilderBase.OTHER);
    public static readonly ElementName EXISTS = new ElementName("exists", "exists", TreeBuilderBase.OTHER);
    public static readonly ElementName FETILE = new ElementName("fetile", "feTile", TreeBuilderBase.OTHER);
    public static readonly ElementName FIGURE = new ElementName("figure", "figure", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName FORALL = new ElementName("forall", "forall", TreeBuilderBase.OTHER);
    public static readonly ElementName FILTER = new ElementName("filter", "filter", TreeBuilderBase.OTHER);
    public static readonly ElementName FOOTER = new ElementName("footer", "footer", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName HGROUP = new ElementName("hgroup", "hgroup", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName HEADER = new ElementName("header", "header", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName IFRAME = new ElementName("iframe", "iframe", TreeBuilderBase.IFRAME | SPECIAL);
    public static readonly ElementName KEYGEN = new ElementName("keygen", "keygen", TreeBuilderBase.KEYGEN | SPECIAL);
    public static readonly ElementName LAMBDA = new ElementName("lambda", "lambda", TreeBuilderBase.OTHER);
    public static readonly ElementName LEGEND = new ElementName("legend", "legend", TreeBuilderBase.OTHER);
    public static readonly ElementName MSPACE = new ElementName("mspace", "mspace", TreeBuilderBase.OTHER);
    public static readonly ElementName MTABLE = new ElementName("mtable", "mtable", TreeBuilderBase.OTHER);
    public static readonly ElementName MSTYLE = new ElementName("mstyle", "mstyle", TreeBuilderBase.OTHER);
    public static readonly ElementName MGLYPH = new ElementName("mglyph", "mglyph", TreeBuilderBase.MGLYPH_OR_MALIGNMARK);
    public static readonly ElementName MEDIAN = new ElementName("median", "median", TreeBuilderBase.OTHER);
    public static readonly ElementName MUNDER = new ElementName("munder", "munder", TreeBuilderBase.OTHER);
    public static readonly ElementName MARKER = new ElementName("marker", "marker", TreeBuilderBase.OTHER);
    public static readonly ElementName MERROR = new ElementName("merror", "merror", TreeBuilderBase.OTHER);
    public static readonly ElementName MOMENT = new ElementName("moment", "moment", TreeBuilderBase.OTHER);
    public static readonly ElementName MATRIX = new ElementName("matrix", "matrix", TreeBuilderBase.OTHER);
    public static readonly ElementName OPTION = new ElementName("option", "option", TreeBuilderBase.OPTION | OPTIONAL_END_TAG);
    public static readonly ElementName OBJECT = new ElementName("object", "object", TreeBuilderBase.OBJECT | SPECIAL | SCOPING);
    public static readonly ElementName OUTPUT = new ElementName("output", "output", TreeBuilderBase.OUTPUT_OR_LABEL);
    public static readonly ElementName PRIMES = new ElementName("primes", "primes", TreeBuilderBase.OTHER);
    public static readonly ElementName SOURCE = new ElementName("source", "source", TreeBuilderBase.PARAM_OR_SOURCE_OR_TRACK);
    public static readonly ElementName STRIKE = new ElementName("strike", "strike", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName STRONG = new ElementName("strong", "strong", TreeBuilderBase.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
    public static readonly ElementName SWITCH = new ElementName("switch", "switch", TreeBuilderBase.OTHER);
    public static readonly ElementName SYMBOL = new ElementName("symbol", "symbol", TreeBuilderBase.OTHER);
    public static readonly ElementName SELECT = new ElementName("select", "select", TreeBuilderBase.SELECT | SPECIAL);
    public static readonly ElementName SUBSET = new ElementName("subset", "subset", TreeBuilderBase.OTHER);
    public static readonly ElementName SCRIPT = new ElementName("script", "script", TreeBuilderBase.SCRIPT | SPECIAL);
    public static readonly ElementName TBREAK = new ElementName("tbreak", "tbreak", TreeBuilderBase.OTHER);
    public static readonly ElementName VECTOR = new ElementName("vector", "vector", TreeBuilderBase.OTHER);
    public static readonly ElementName ARTICLE = new ElementName("article", "article", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName ANIMATE = new ElementName("animate", "animate", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCSECH = new ElementName("arcsech", "arcsech", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCCSCH = new ElementName("arccsch", "arccsch", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCTANH = new ElementName("arctanh", "arctanh", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCSINH = new ElementName("arcsinh", "arcsinh", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCCOSH = new ElementName("arccosh", "arccosh", TreeBuilderBase.OTHER);
    public static readonly ElementName ARCCOTH = new ElementName("arccoth", "arccoth", TreeBuilderBase.OTHER);
    public static readonly ElementName ACRONYM = new ElementName("acronym", "acronym", TreeBuilderBase.OTHER);
    public static readonly ElementName ADDRESS = new ElementName("address", "address", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName BGSOUND = new ElementName("bgsound", "bgsound", TreeBuilderBase.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
    public static readonly ElementName COMMAND = new ElementName("command", "command", TreeBuilderBase.COMMAND | SPECIAL);
    public static readonly ElementName COMPOSE = new ElementName("compose", "compose", TreeBuilderBase.OTHER);
    public static readonly ElementName CEILING = new ElementName("ceiling", "ceiling", TreeBuilderBase.OTHER);
    public static readonly ElementName CSYMBOL = new ElementName("csymbol", "csymbol", TreeBuilderBase.OTHER);
    public static readonly ElementName CAPTION = new ElementName("caption", "caption", TreeBuilderBase.CAPTION | SPECIAL | SCOPING);
    public static readonly ElementName DISCARD = new ElementName("discard", "discard", TreeBuilderBase.OTHER);
    public static readonly ElementName DECLARE = new ElementName("declare", "declare", TreeBuilderBase.OTHER);
    public static readonly ElementName DETAILS = new ElementName("details", "details", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName ELLIPSE = new ElementName("ellipse", "ellipse", TreeBuilderBase.OTHER);
    public static readonly ElementName FEFUNCA = new ElementName("fefunca", "feFuncA", TreeBuilderBase.OTHER);
    public static readonly ElementName FEFUNCB = new ElementName("fefuncb", "feFuncB", TreeBuilderBase.OTHER);
    public static readonly ElementName FEBLEND = new ElementName("feblend", "feBlend", TreeBuilderBase.OTHER);
    public static readonly ElementName FEFLOOD = new ElementName("feflood", "feFlood", TreeBuilderBase.OTHER);
    public static readonly ElementName FEIMAGE = new ElementName("feimage", "feImage", TreeBuilderBase.OTHER);
    public static readonly ElementName FEMERGE = new ElementName("femerge", "feMerge", TreeBuilderBase.OTHER);
    public static readonly ElementName FEFUNCG = new ElementName("fefuncg", "feFuncG", TreeBuilderBase.OTHER);
    public static readonly ElementName FEFUNCR = new ElementName("fefuncr", "feFuncR", TreeBuilderBase.OTHER);
    public static readonly ElementName HANDLER = new ElementName("handler", "handler", TreeBuilderBase.OTHER);
    public static readonly ElementName INVERSE = new ElementName("inverse", "inverse", TreeBuilderBase.OTHER);
    public static readonly ElementName IMPLIES = new ElementName("implies", "implies", TreeBuilderBase.OTHER);
    public static readonly ElementName ISINDEX = new ElementName("isindex", "isindex", TreeBuilderBase.ISINDEX | SPECIAL);
    public static readonly ElementName LOGBASE = new ElementName("logbase", "logbase", TreeBuilderBase.OTHER);
    public static readonly ElementName LISTING = new ElementName("listing", "listing", TreeBuilderBase.PRE_OR_LISTING | SPECIAL);
    public static readonly ElementName MFENCED = new ElementName("mfenced", "mfenced", TreeBuilderBase.OTHER);
    public static readonly ElementName MPADDED = new ElementName("mpadded", "mpadded", TreeBuilderBase.OTHER);
    public static readonly ElementName MARQUEE = new ElementName("marquee", "marquee", TreeBuilderBase.MARQUEE_OR_APPLET | SPECIAL | SCOPING);
    public static readonly ElementName MACTION = new ElementName("maction", "maction", TreeBuilderBase.OTHER);
    public static readonly ElementName MSUBSUP = new ElementName("msubsup", "msubsup", TreeBuilderBase.OTHER);
    public static readonly ElementName NOEMBED = new ElementName("noembed", "noembed", TreeBuilderBase.NOEMBED | SPECIAL);
    public static readonly ElementName POLYGON = new ElementName("polygon", "polygon", TreeBuilderBase.OTHER);
    public static readonly ElementName PATTERN = new ElementName("pattern", "pattern", TreeBuilderBase.OTHER);
    public static readonly ElementName PRODUCT = new ElementName("product", "product", TreeBuilderBase.OTHER);
    public static readonly ElementName SETDIFF = new ElementName("setdiff", "setdiff", TreeBuilderBase.OTHER);
    public static readonly ElementName SECTION = new ElementName("section", "section", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName SUMMARY = new ElementName("summary", "summary", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName TENDSTO = new ElementName("tendsto", "tendsto", TreeBuilderBase.OTHER);
    public static readonly ElementName UPLIMIT = new ElementName("uplimit", "uplimit", TreeBuilderBase.OTHER);
    public static readonly ElementName ALTGLYPH = new ElementName("altglyph", "altGlyph", TreeBuilderBase.OTHER);
    public static readonly ElementName BASEFONT = new ElementName("basefont", "basefont", TreeBuilderBase.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
    public static readonly ElementName CLIPPATH = new ElementName("clippath", "clipPath", TreeBuilderBase.OTHER);
    public static readonly ElementName CODOMAIN = new ElementName("codomain", "codomain", TreeBuilderBase.OTHER);
    public static readonly ElementName COLGROUP = new ElementName("colgroup", "colgroup", TreeBuilderBase.COLGROUP | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName EMPTYSET = new ElementName("emptyset", "emptyset", TreeBuilderBase.OTHER);
    public static readonly ElementName FACTOROF = new ElementName("factorof", "factorof", TreeBuilderBase.OTHER);
    public static readonly ElementName FIELDSET = new ElementName("fieldset", "fieldset", TreeBuilderBase.FIELDSET | SPECIAL);
    public static readonly ElementName FRAMESET = new ElementName("frameset", "frameset", TreeBuilderBase.FRAMESET | SPECIAL);
    public static readonly ElementName FEOFFSET = new ElementName("feoffset", "feOffset", TreeBuilderBase.OTHER);
    public static readonly ElementName GLYPHREF = new ElementName("glyphref", "glyphRef", TreeBuilderBase.OTHER);
    public static readonly ElementName INTERVAL = new ElementName("interval", "interval", TreeBuilderBase.OTHER);
    public static readonly ElementName INTEGERS = new ElementName("integers", "integers", TreeBuilderBase.OTHER);
    public static readonly ElementName INFINITY = new ElementName("infinity", "infinity", TreeBuilderBase.OTHER);
    public static readonly ElementName LISTENER = new ElementName("listener", "listener", TreeBuilderBase.OTHER);
    public static readonly ElementName LOWLIMIT = new ElementName("lowlimit", "lowlimit", TreeBuilderBase.OTHER);
    public static readonly ElementName METADATA = new ElementName("metadata", "metadata", TreeBuilderBase.OTHER);
    public static readonly ElementName MENCLOSE = new ElementName("menclose", "menclose", TreeBuilderBase.OTHER);
    public static readonly ElementName MENUITEM = new ElementName("menuitem", "menuitem", TreeBuilderBase.MENUITEM | SPECIAL);
    public static readonly ElementName MPHANTOM = new ElementName("mphantom", "mphantom", TreeBuilderBase.OTHER);
    public static readonly ElementName NOFRAMES = new ElementName("noframes", "noframes", TreeBuilderBase.NOFRAMES | SPECIAL);
    public static readonly ElementName NOSCRIPT = new ElementName("noscript", "noscript", TreeBuilderBase.NOSCRIPT | SPECIAL);
    public static readonly ElementName OPTGROUP = new ElementName("optgroup", "optgroup", TreeBuilderBase.OPTGROUP | SPECIAL | OPTIONAL_END_TAG);
    public static readonly ElementName POLYLINE = new ElementName("polyline", "polyline", TreeBuilderBase.OTHER);
    public static readonly ElementName PREFETCH = new ElementName("prefetch", "prefetch", TreeBuilderBase.OTHER);
    public static readonly ElementName PROGRESS = new ElementName("progress", "progress", TreeBuilderBase.OTHER);
    public static readonly ElementName PRSUBSET = new ElementName("prsubset", "prsubset", TreeBuilderBase.OTHER);
    public static readonly ElementName QUOTIENT = new ElementName("quotient", "quotient", TreeBuilderBase.OTHER);
    public static readonly ElementName SELECTOR = new ElementName("selector", "selector", TreeBuilderBase.OTHER);
    public static readonly ElementName TEXTAREA = new ElementName("textarea", "textarea", TreeBuilderBase.TEXTAREA | SPECIAL);
    public static readonly ElementName TEXTPATH = new ElementName("textpath", "textPath", TreeBuilderBase.OTHER);
    public static readonly ElementName VARIANCE = new ElementName("variance", "variance", TreeBuilderBase.OTHER);
    public static readonly ElementName ANIMATION = new ElementName("animation", "animation", TreeBuilderBase.OTHER);
    public static readonly ElementName CONJUGATE = new ElementName("conjugate", "conjugate", TreeBuilderBase.OTHER);
    public static readonly ElementName CONDITION = new ElementName("condition", "condition", TreeBuilderBase.OTHER);
    public static readonly ElementName COMPLEXES = new ElementName("complexes", "complexes", TreeBuilderBase.OTHER);
    public static readonly ElementName FONT_FACE = new ElementName("font-face", "font-face", TreeBuilderBase.OTHER);
    public static readonly ElementName FACTORIAL = new ElementName("factorial", "factorial", TreeBuilderBase.OTHER);
    public static readonly ElementName INTERSECT = new ElementName("intersect", "intersect", TreeBuilderBase.OTHER);
    public static readonly ElementName IMAGINARY = new ElementName("imaginary", "imaginary", TreeBuilderBase.OTHER);
    public static readonly ElementName LAPLACIAN = new ElementName("laplacian", "laplacian", TreeBuilderBase.OTHER);
    public static readonly ElementName MATRIXROW = new ElementName("matrixrow", "matrixrow", TreeBuilderBase.OTHER);
    public static readonly ElementName NOTSUBSET = new ElementName("notsubset", "notsubset", TreeBuilderBase.OTHER);
    public static readonly ElementName OTHERWISE = new ElementName("otherwise", "otherwise", TreeBuilderBase.OTHER);
    public static readonly ElementName PIECEWISE = new ElementName("piecewise", "piecewise", TreeBuilderBase.OTHER);
    public static readonly ElementName PLAINTEXT = new ElementName("plaintext", "plaintext", TreeBuilderBase.PLAINTEXT | SPECIAL);
    public static readonly ElementName RATIONALS = new ElementName("rationals", "rationals", TreeBuilderBase.OTHER);
    public static readonly ElementName SEMANTICS = new ElementName("semantics", "semantics", TreeBuilderBase.OTHER);
    public static readonly ElementName TRANSPOSE = new ElementName("transpose", "transpose", TreeBuilderBase.OTHER);
    public static readonly ElementName ANNOTATION = new ElementName("annotation", "annotation", TreeBuilderBase.OTHER);
    public static readonly ElementName BLOCKQUOTE = new ElementName("blockquote", "blockquote", TreeBuilderBase.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
    public static readonly ElementName DIVERGENCE = new ElementName("divergence", "divergence", TreeBuilderBase.OTHER);
    public static readonly ElementName EULERGAMMA = new ElementName("eulergamma", "eulergamma", TreeBuilderBase.OTHER);
    public static readonly ElementName EQUIVALENT = new ElementName("equivalent", "equivalent", TreeBuilderBase.OTHER);
    public static readonly ElementName FIGCAPTION = new ElementName("figcaption", "figcaption", TreeBuilderBase.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
    public static readonly ElementName IMAGINARYI = new ElementName("imaginaryi", "imaginaryi", TreeBuilderBase.OTHER);
    public static readonly ElementName MALIGNMARK = new ElementName("malignmark", "malignmark", TreeBuilderBase.MGLYPH_OR_MALIGNMARK);
    public static readonly ElementName MUNDEROVER = new ElementName("munderover", "munderover", TreeBuilderBase.OTHER);
    public static readonly ElementName MLABELEDTR = new ElementName("mlabeledtr", "mlabeledtr", TreeBuilderBase.OTHER);
    public static readonly ElementName NOTANUMBER = new ElementName("notanumber", "notanumber", TreeBuilderBase.OTHER);
    public static readonly ElementName SOLIDCOLOR = new ElementName("solidcolor", "solidcolor", TreeBuilderBase.OTHER);
    public static readonly ElementName ALTGLYPHDEF = new ElementName("altglyphdef", "altGlyphDef", TreeBuilderBase.OTHER);
    public static readonly ElementName DETERMINANT = new ElementName("determinant", "determinant", TreeBuilderBase.OTHER);
    public static readonly ElementName FEMERGENODE = new ElementName("femergenode", "feMergeNode", TreeBuilderBase.OTHER);
    public static readonly ElementName FECOMPOSITE = new ElementName("fecomposite", "feComposite", TreeBuilderBase.OTHER);
    public static readonly ElementName FESPOTLIGHT = new ElementName("fespotlight", "feSpotLight", TreeBuilderBase.OTHER);
    public static readonly ElementName MALIGNGROUP = new ElementName("maligngroup", "maligngroup", TreeBuilderBase.OTHER);
    public static readonly ElementName MPRESCRIPTS = new ElementName("mprescripts", "mprescripts", TreeBuilderBase.OTHER);
    public static readonly ElementName MOMENTABOUT = new ElementName("momentabout", "momentabout", TreeBuilderBase.OTHER);
    public static readonly ElementName NOTPRSUBSET = new ElementName("notprsubset", "notprsubset", TreeBuilderBase.OTHER);
    public static readonly ElementName PARTIALDIFF = new ElementName("partialdiff", "partialdiff", TreeBuilderBase.OTHER);
    public static readonly ElementName ALTGLYPHITEM = new ElementName("altglyphitem", "altGlyphItem", TreeBuilderBase.OTHER);
    public static readonly ElementName ANIMATECOLOR = new ElementName("animatecolor", "animateColor", TreeBuilderBase.OTHER);
    public static readonly ElementName DATATEMPLATE = new ElementName("datatemplate", "datatemplate", TreeBuilderBase.OTHER);
    public static readonly ElementName EXPONENTIALE = new ElementName("exponentiale", "exponentiale", TreeBuilderBase.OTHER);
    public static readonly ElementName FETURBULENCE = new ElementName("feturbulence", "feTurbulence", TreeBuilderBase.OTHER);
    public static readonly ElementName FEPOINTLIGHT = new ElementName("fepointlight", "fePointLight", TreeBuilderBase.OTHER);
    public static readonly ElementName FEMORPHOLOGY = new ElementName("femorphology", "feMorphology", TreeBuilderBase.OTHER);
    public static readonly ElementName OUTERPRODUCT = new ElementName("outerproduct", "outerproduct", TreeBuilderBase.OTHER);
    public static readonly ElementName ANIMATEMOTION = new ElementName("animatemotion", "animateMotion", TreeBuilderBase.OTHER);
    public static readonly ElementName COLOR_PROFILE = new ElementName("color-profile", "color-profile", TreeBuilderBase.OTHER);
    public static readonly ElementName FONT_FACE_SRC = new ElementName("font-face-src", "font-face-src", TreeBuilderBase.OTHER);
    public static readonly ElementName FONT_FACE_URI = new ElementName("font-face-uri", "font-face-uri", TreeBuilderBase.OTHER);
    public static readonly ElementName FOREIGNOBJECT = new ElementName("foreignobject", "foreignObject", TreeBuilderBase.FOREIGNOBJECT_OR_DESC | SCOPING_AS_SVG);
    public static readonly ElementName FECOLORMATRIX = new ElementName("fecolormatrix", "feColorMatrix", TreeBuilderBase.OTHER);
    public static readonly ElementName MISSING_GLYPH = new ElementName("missing-glyph", "missing-glyph", TreeBuilderBase.OTHER);
    public static readonly ElementName MMULTISCRIPTS = new ElementName("mmultiscripts", "mmultiscripts", TreeBuilderBase.OTHER);
    public static readonly ElementName SCALARPRODUCT = new ElementName("scalarproduct", "scalarproduct", TreeBuilderBase.OTHER);
    public static readonly ElementName VECTORPRODUCT = new ElementName("vectorproduct", "vectorproduct", TreeBuilderBase.OTHER);
    public static readonly ElementName ANNOTATION_XML = new ElementName("annotation-xml", "annotation-xml", TreeBuilderBase.ANNOTATION_XML | SCOPING_AS_MATHML);
    public static readonly ElementName DEFINITION_SRC = new ElementName("definition-src", "definition-src", TreeBuilderBase.OTHER);
    public static readonly ElementName FONT_FACE_NAME = new ElementName("font-face-name", "font-face-name", TreeBuilderBase.OTHER);
    public static readonly ElementName FEGAUSSIANBLUR = new ElementName("fegaussianblur", "feGaussianBlur", TreeBuilderBase.OTHER);
    public static readonly ElementName FEDISTANTLIGHT = new ElementName("fedistantlight", "feDistantLight", TreeBuilderBase.OTHER);
    public static readonly ElementName LINEARGRADIENT = new ElementName("lineargradient", "linearGradient", TreeBuilderBase.OTHER);
    public static readonly ElementName NATURALNUMBERS = new ElementName("naturalnumbers", "naturalnumbers", TreeBuilderBase.OTHER);
    public static readonly ElementName RADIALGRADIENT = new ElementName("radialgradient", "radialGradient", TreeBuilderBase.OTHER);
    public static readonly ElementName ANIMATETRANSFORM = new ElementName("animatetransform", "animateTransform", TreeBuilderBase.OTHER);
    public static readonly ElementName CARTESIANPRODUCT = new ElementName("cartesianproduct", "cartesianproduct", TreeBuilderBase.OTHER);
    public static readonly ElementName FONT_FACE_FORMAT = new ElementName("font-face-format", "font-face-format", TreeBuilderBase.OTHER);
    public static readonly ElementName FECONVOLVEMATRIX = new ElementName("feconvolvematrix", "feConvolveMatrix", TreeBuilderBase.OTHER);
    public static readonly ElementName FEDIFFUSELIGHTING = new ElementName("fediffuselighting", "feDiffuseLighting", TreeBuilderBase.OTHER);
    public static readonly ElementName FEDISPLACEMENTMAP = new ElementName("fedisplacementmap", "feDisplacementMap", TreeBuilderBase.OTHER);
    public static readonly ElementName FESPECULARLIGHTING = new ElementName("fespecularlighting", "feSpecularLighting", TreeBuilderBase.OTHER);
    public static readonly ElementName DOMAINOFAPPLICATION = new ElementName("domainofapplication", "domainofapplication", TreeBuilderBase.OTHER);
    public static readonly ElementName FECOMPONENTTRANSFER = new ElementName("fecomponenttransfer", "feComponentTransfer", TreeBuilderBase.OTHER);
    private readonly ElementName[] ELEMENT_NAMES = {
    A,
    B,
    G,
    I,
    P,
    Q,
    S,
    U,
    BR,
    CI,
    CN,
    DD,
    DL,
    DT,
    EM,
    EQ,
    FN,
    H1,
    H2,
    H3,
    H4,
    H5,
    H6,
    GT,
    HR,
    IN,
    LI,
    LN,
    LT,
    MI,
    MN,
    MO,
    MS,
    OL,
    OR,
    PI,
    RP,
    RT,
    TD,
    TH,
    TR,
    TT,
    UL,
    AND,
    ARG,
    ABS,
    BIG,
    BDO,
    CSC,
    COL,
    COS,
    COT,
    DEL,
    DFN,
    DIR,
    DIV,
    EXP,
    GCD,
    GEQ,
    IMG,
    INS,
    INT,
    KBD,
    LOG,
    LCM,
    LEQ,
    MTD,
    MIN,
    MAP,
    MTR,
    MAX,
    NEQ,
    NOT,
    NAV,
    PRE,
    REM,
    SUB,
    SEC,
    SVG,
    SUM,
    SIN,
    SEP,
    SUP,
    SET,
    TAN,
    USE,
    VAR,
    WBR,
    XMP,
    XOR,
    AREA,
    ABBR,
    BASE,
    BVAR,
    BODY,
    CARD,
    CODE,
    CITE,
    CSCH,
    COSH,
    COTH,
    CURL,
    DESC,
    DIFF,
    DEFS,
    FORM,
    FONT,
    GRAD,
    HEAD,
    HTML,
    LINE,
    LINK,
    LIST,
    META,
    MSUB,
    MODE,
    MATH,
    MARK,
    MASK,
    MEAN,
    MSUP,
    MENU,
    MROW,
    NONE,
    NOBR,
    NEST,
    PATH,
    PLUS,
    RULE,
    REAL,
    RELN,
    RECT,
    ROOT,
    RUBY,
    SECH,
    SINH,
    SPAN,
    SAMP,
    STOP,
    SDEV,
    TIME,
    TRUE,
    TREF,
    TANH,
    TEXT,
    VIEW,
    ASIDE,
    AUDIO,
    APPLY,
    EMBED,
    FRAME,
    FALSE,
    FLOOR,
    GLYPH,
    HKERN,
    IMAGE,
    IDENT,
    INPUT,
    LABEL,
    LIMIT,
    MFRAC,
    MPATH,
    METER,
    MOVER,
    MINUS,
    MROOT,
    MSQRT,
    MTEXT,
    NOTIN,
    PIECE,
    PARAM,
    POWER,
    REALS,
    STYLE,
    SMALL,
    THEAD,
    TABLE,
    TITLE,
    TRACK,
    TSPAN,
    TIMES,
    TFOOT,
    TBODY,
    UNION,
    VKERN,
    VIDEO,
    ARCSEC,
    ARCCSC,
    ARCTAN,
    ARCSIN,
    ARCCOS,
    APPLET,
    ARCCOT,
    APPROX,
    BUTTON,
    CIRCLE,
    CENTER,
    CURSOR,
    CANVAS,
    DIVIDE,
    DEGREE,
    DOMAIN,
    EXISTS,
    FETILE,
    FIGURE,
    FORALL,
    FILTER,
    FOOTER,
    HGROUP,
    HEADER,
    IFRAME,
    KEYGEN,
    LAMBDA,
    LEGEND,
    MSPACE,
    MTABLE,
    MSTYLE,
    MGLYPH,
    MEDIAN,
    MUNDER,
    MARKER,
    MERROR,
    MOMENT,
    MATRIX,
    OPTION,
    OBJECT,
    OUTPUT,
    PRIMES,
    SOURCE,
    STRIKE,
    STRONG,
    SWITCH,
    SYMBOL,
    SELECT,
    SUBSET,
    SCRIPT,
    TBREAK,
    VECTOR,
    ARTICLE,
    ANIMATE,
    ARCSECH,
    ARCCSCH,
    ARCTANH,
    ARCSINH,
    ARCCOSH,
    ARCCOTH,
    ACRONYM,
    ADDRESS,
    BGSOUND,
    COMMAND,
    COMPOSE,
    CEILING,
    CSYMBOL,
    CAPTION,
    DISCARD,
    DECLARE,
    DETAILS,
    ELLIPSE,
    FEFUNCA,
    FEFUNCB,
    FEBLEND,
    FEFLOOD,
    FEIMAGE,
    FEMERGE,
    FEFUNCG,
    FEFUNCR,
    HANDLER,
    INVERSE,
    IMPLIES,
    ISINDEX,
    LOGBASE,
    LISTING,
    MFENCED,
    MPADDED,
    MARQUEE,
    MACTION,
    MSUBSUP,
    NOEMBED,
    POLYGON,
    PATTERN,
    PRODUCT,
    SETDIFF,
    SECTION,
    SUMMARY,
    TENDSTO,
    UPLIMIT,
    ALTGLYPH,
    BASEFONT,
    CLIPPATH,
    CODOMAIN,
    COLGROUP,
    EMPTYSET,
    FACTOROF,
    FIELDSET,
    FRAMESET,
    FEOFFSET,
    GLYPHREF,
    INTERVAL,
    INTEGERS,
    INFINITY,
    LISTENER,
    LOWLIMIT,
    METADATA,
    MENCLOSE,
    MENUITEM,
    MPHANTOM,
    NOFRAMES,
    NOSCRIPT,
    OPTGROUP,
    POLYLINE,
    PREFETCH,
    PROGRESS,
    PRSUBSET,
    QUOTIENT,
    SELECTOR,
    TEXTAREA,
    TEXTPATH,
    VARIANCE,
    ANIMATION,
    CONJUGATE,
    CONDITION,
    COMPLEXES,
    FONT_FACE,
    FACTORIAL,
    INTERSECT,
    IMAGINARY,
    LAPLACIAN,
    MATRIXROW,
    NOTSUBSET,
    OTHERWISE,
    PIECEWISE,
    PLAINTEXT,
    RATIONALS,
    SEMANTICS,
    TRANSPOSE,
    ANNOTATION,
    BLOCKQUOTE,
    DIVERGENCE,
    EULERGAMMA,
    EQUIVALENT,
    FIGCAPTION,
    IMAGINARYI,
    MALIGNMARK,
    MUNDEROVER,
    MLABELEDTR,
    NOTANUMBER,
    SOLIDCOLOR,
    ALTGLYPHDEF,
    DETERMINANT,
    FEMERGENODE,
    FECOMPOSITE,
    FESPOTLIGHT,
    MALIGNGROUP,
    MPRESCRIPTS,
    MOMENTABOUT,
    NOTPRSUBSET,
    PARTIALDIFF,
    ALTGLYPHITEM,
    ANIMATECOLOR,
    DATATEMPLATE,
    EXPONENTIALE,
    FETURBULENCE,
    FEPOINTLIGHT,
    FEMORPHOLOGY,
    OUTERPRODUCT,
    ANIMATEMOTION,
    COLOR_PROFILE,
    FONT_FACE_SRC,
    FONT_FACE_URI,
    FOREIGNOBJECT,
    FECOLORMATRIX,
    MISSING_GLYPH,
    MMULTISCRIPTS,
    SCALARPRODUCT,
    VECTORPRODUCT,
    ANNOTATION_XML,
    DEFINITION_SRC,
    FONT_FACE_NAME,
    FEGAUSSIANBLUR,
    FEDISTANTLIGHT,
    LINEARGRADIENT,
    NATURALNUMBERS,
    RADIALGRADIENT,
    ANIMATETRANSFORM,
    CARTESIANPRODUCT,
    FONT_FACE_FORMAT,
    FECONVOLVEMATRIX,
    FEDIFFUSELIGHTING,
    FEDISPLACEMENTMAP,
    FESPECULARLIGHTING,
    DOMAINOFAPPLICATION,
    FECOMPONENTTRANSFER,
    };
    private readonly int[] ELEMENT_HASHES = {
    1057,
    1090,
    1255,
    1321,
    1552,
    1585,
    1651,
    1717,
    68162,
    68899,
    69059,
    69764,
    70020,
    70276,
    71077,
    71205,
    72134,
    72232,
    72264,
    72296,
    72328,
    72360,
    72392,
    73351,
    74312,
    75209,
    78124,
    78284,
    78476,
    79149,
    79309,
    79341,
    79469,
    81295,
    81487,
    82224,
    84498,
    84626,
    86164,
    86292,
    86612,
    86676,
    87445,
    3183041,
    3186241,
    3198017,
    3218722,
    3226754,
    3247715,
    3256803,
    3263971,
    3264995,
    3289252,
    3291332,
    3295524,
    3299620,
    3326725,
    3379303,
    3392679,
    3448233,
    3460553,
    3461577,
    3510347,
    3546604,
    3552364,
    3556524,
    3576461,
    3586349,
    3588141,
    3590797,
    3596333,
    3622062,
    3625454,
    3627054,
    3675728,
    3749042,
    3771059,
    3771571,
    3776211,
    3782323,
    3782963,
    3784883,
    3785395,
    3788979,
    3815476,
    3839605,
    3885110,
    3917911,
    3948984,
    3951096,
    135304769,
    135858241,
    136498210,
    136906434,
    137138658,
    137512995,
    137531875,
    137548067,
    137629283,
    137645539,
    137646563,
    137775779,
    138529956,
    138615076,
    139040932,
    140954086,
    141179366,
    141690439,
    142738600,
    143013512,
    146979116,
    147175724,
    147475756,
    147902637,
    147936877,
    148017645,
    148131885,
    148228141,
    148229165,
    148309165,
    148395629,
    148551853,
    148618829,
    149076462,
    149490158,
    149572782,
    151277616,
    151639440,
    153268914,
    153486514,
    153563314,
    153750706,
    153763314,
    153914034,
    154406067,
    154417459,
    154600979,
    154678323,
    154680979,
    154866835,
    155366708,
    155375188,
    155391572,
    155465780,
    155869364,
    158045494,
    168988979,
    169321621,
    169652752,
    173151309,
    174240818,
    174247297,
    174669292,
    175391532,
    176638123,
    177380397,
    177879204,
    177886734,
    180753473,
    181020073,
    181503558,
    181686320,
    181999237,
    181999311,
    182048201,
    182074866,
    182078003,
    182083764,
    182920847,
    184716457,
    184976961,
    185145071,
    187281445,
    187872052,
    188100653,
    188875944,
    188919873,
    188920457,
    189107250,
    189203987,
    189371817,
    189414886,
    189567458,
    190266670,
    191318187,
    191337609,
    202479203,
    202493027,
    202835587,
    202843747,
    203013219,
    203036048,
    203045987,
    203177552,
    203898516,
    204648562,
    205067918,
    205078130,
    205096654,
    205689142,
    205690439,
    205988909,
    207213161,
    207794484,
    207800999,
    208023602,
    208213644,
    208213647,
    210261490,
    210310273,
    210940978,
    213325049,
    213946445,
    214055079,
    215125040,
    215134273,
    215135028,
    215237420,
    215418148,
    215553166,
    215553394,
    215563858,
    215627949,
    215754324,
    217529652,
    217713834,
    217732628,
    218731945,
    221417045,
    221424946,
    221493746,
    221515401,
    221658189,
    221908140,
    221910626,
    221921586,
    222659762,
    225001091,
    236105833,
    236113965,
    236194995,
    236195427,
    236206132,
    236206387,
    236211683,
    236212707,
    236381647,
    236571826,
    237124271,
    238172205,
    238210544,
    238270764,
    238435405,
    238501172,
    239224867,
    239257644,
    239710497,
    240307721,
    241208789,
    241241557,
    241318060,
    241319404,
    241343533,
    241344069,
    241405397,
    241765845,
    243864964,
    244502085,
    244946220,
    245109902,
    247647266,
    247707956,
    248648814,
    248648836,
    248682161,
    248986932,
    249058914,
    249697357,
    252132601,
    252135604,
    252317348,
    255007012,
    255278388,
    255641645,
    256365156,
    257566121,
    269763372,
    271202790,
    271863856,
    272049197,
    272127474,
    274339449,
    274939471,
    275388004,
    275388005,
    275388006,
    275977800,
    278267602,
    278513831,
    278712622,
    281613765,
    281683369,
    282120228,
    282250732,
    282498697,
    282508942,
    283743649,
    283787570,
    284710386,
    285391148,
    285478533,
    285854898,
    285873762,
    286931113,
    288964227,
    289445441,
    289689648,
    291671489,
    303512884,
    305319975,
    305610036,
    305764101,
    308448294,
    308675890,
    312085683,
    312264750,
    315032867,
    316391000,
    317331042,
    317902135,
    318950711,
    319447220,
    321499182,
    322538804,
    323145200,
    337067316,
    337826293,
    339905989,
    340833697,
    341457068,
    342310196,
    345302593,
    349554733,
    349771471,
    349786245,
    350819405,
    356072847,
    370349192,
    373962798,
    375558638,
    375574835,
    376053993,
    383276530,
    383373833,
    383407586,
    384439906,
    386079012,
    404133513,
    404307343,
    407031852,
    408072233,
    409112005,
    409608425,
    409771500,
    419040932,
    437730612,
    439529766,
    442616365,
    442813037,
    443157674,
    443295316,
    450118444,
    450482697,
    456789668,
    459935396,
    471217869,
    474073645,
    476230702,
    476665218,
    476717289,
    483014825,
    485083298,
    489306281,
    538364390,
    540675748,
    543819186,
    543958612,
    576960820,
    577242548,
    610515252,
    642202932,
    644420819,
    };
}
