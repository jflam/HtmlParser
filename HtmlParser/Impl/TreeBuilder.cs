/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2007-2011 Mozilla Foundation
 * Portions of comments Copyright 2004-2008 Apple Computer, Inc., Mozilla 
 * Foundation, and Opera Software ASA.
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

/*
 * The comments following this one that use the same comment syntax as this 
 * comment are quotes from the WHATWG HTML 5 spec as of 27 June 2007 
 * amended as of June 28 2007.
 * That document came with this statement:
 * "© Copyright 2004-2007 Apple Computer, Inc., Mozilla Foundation, and 
 * Opera Software ASA. You are granted a license to use, reproduce and 
 * create derivative works of this document."
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

public abstract class TreeBuilderBase
{
    // Start dispatch groups

    internal const int OTHER = 0;

    internal const int A = 1;

    internal const int BASE = 2;

    internal const int BODY = 3;

    internal const int BR = 4;

    internal const int BUTTON = 5;

    internal const int CAPTION = 6;

    internal const int COL = 7;

    internal const int COLGROUP = 8;

    internal const int FORM = 9;

    internal const int FRAME = 10;

    internal const int FRAMESET = 11;

    internal const int IMAGE = 12;

    internal const int INPUT = 13;

    internal const int ISINDEX = 14;

    internal const int LI = 15;

    internal const int LINK_OR_BASEFONT_OR_BGSOUND = 16;

    internal const int MATH = 17;

    internal const int META = 18;

    internal const int SVG = 19;

    internal const int HEAD = 20;

    internal const int HR = 22;

    internal const int HTML = 23;

    internal const int NOBR = 24;

    internal const int NOFRAMES = 25;

    internal const int NOSCRIPT = 26;

    internal const int OPTGROUP = 27;

    internal const int OPTION = 28;

    internal const int P = 29;

    internal const int PLAINTEXT = 30;

    internal const int SCRIPT = 31;

    internal const int SELECT = 32;

    internal const int STYLE = 33;

    internal const int TABLE = 34;

    internal const int TEXTAREA = 35;

    internal const int TITLE = 36;

    internal const int TR = 37;

    internal const int XMP = 38;

    internal const int TBODY_OR_THEAD_OR_TFOOT = 39;

    internal const int TD_OR_TH = 40;

    internal const int DD_OR_DT = 41;

    internal const int H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 = 42;

    internal const int MARQUEE_OR_APPLET = 43;

    internal const int PRE_OR_LISTING = 44;

    internal const int B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U = 45;

    internal const int UL_OR_OL_OR_DL = 46;

    internal const int IFRAME = 47;

    internal const int EMBED_OR_IMG = 48;

    internal const int AREA_OR_WBR = 49;

    internal const int DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU = 50;

    internal const int ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY = 51;

    internal const int RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR = 52;

    internal const int RT_OR_RP = 53;

    internal const int COMMAND = 54;

    internal const int PARAM_OR_SOURCE_OR_TRACK = 55;

    internal const int MGLYPH_OR_MALIGNMARK = 56;

    internal const int MI_MO_MN_MS_MTEXT = 57;

    internal const int ANNOTATION_XML = 58;

    internal const int FOREIGNOBJECT_OR_DESC = 59;

    internal const int NOEMBED = 60;

    internal const int FIELDSET = 61;

    internal const int OUTPUT_OR_LABEL = 62;

    internal const int OBJECT = 63;

    internal const int FONT = 64;

    internal const int KEYGEN = 65;

    internal const int MENUITEM = 66;

    /**
     * Array version of U+FFFD.
     */
    internal static readonly char[] REPLACEMENT_CHARACTER = { '\uFFFD' };
    
    // start insertion modes

    internal const int INITIAL = 0;

    internal const int BEFORE_HTML = 1;

    internal const int BEFORE_HEAD = 2;

    internal const int IN_HEAD = 3;

    internal const int IN_HEAD_NOSCRIPT = 4;

    internal const int AFTER_HEAD = 5;

    internal const int IN_BODY = 6;

    internal const int IN_TABLE = 7;

    internal const int IN_CAPTION = 8;

    internal const int IN_COLUMN_GROUP = 9;

    internal const int IN_TABLE_BODY = 10;

    internal const int IN_ROW = 11;

    internal const int IN_CELL = 12;

    internal const int IN_SELECT = 13;

    internal const int IN_SELECT_IN_TABLE = 14;

    internal const int AFTER_BODY = 15;

    internal const int IN_FRAMESET = 16;

    internal const int AFTER_FRAMESET = 17;

    internal const int AFTER_AFTER_BODY = 18;

    internal const int AFTER_AFTER_FRAMESET = 19;

    internal const int TEXT = 20;

    internal const int FRAMESET_OK = 21;

    // start charset states

    internal const int CHARSET_INITIAL = 0;

    internal const int CHARSET_C = 1;

    internal const int CHARSET_H = 2;

    internal const int CHARSET_A = 3;

    internal const int CHARSET_R = 4;

    internal const int CHARSET_S = 5;

    internal const int CHARSET_E = 6;

    internal const int CHARSET_T = 7;

    internal const int CHARSET_EQUALS = 8;

    internal const int CHARSET_SINGLE_QUOTED = 9;

    internal const int CHARSET_DOUBLE_QUOTED = 10;

    internal const int CHARSET_UNQUOTED = 11;

    // end pseudo enums

    // [NOCPP[

    internal static readonly String[] HTML4_PUBLIC_IDS = {
            "-//W3C//DTD HTML 4.0 Frameset//EN",
            "-//W3C//DTD HTML 4.0 Transitional//EN",
            "-//W3C//DTD HTML 4.0//EN", "-//W3C//DTD HTML 4.01 Frameset//EN",
            "-//W3C//DTD HTML 4.01 Transitional//EN",
            "-//W3C//DTD HTML 4.01//EN" };

    // ]NOCPP]

    internal static readonly String[] QUIRKY_PUBLIC_IDS = {
            "+//silmaril//dtd html pro v0r11 19970101//",
            "-//advasoft ltd//dtd html 3.0 aswedit + extensions//",
            "-//as//dtd html 3.0 aswedit + extensions//",
            "-//ietf//dtd html 2.0 level 1//",
            "-//ietf//dtd html 2.0 level 2//",
            "-//ietf//dtd html 2.0 strict level 1//",
            "-//ietf//dtd html 2.0 strict level 2//",
            "-//ietf//dtd html 2.0 strict//",
            "-//ietf//dtd html 2.0//",
            "-//ietf//dtd html 2.1e//",
            "-//ietf//dtd html 3.0//",
            "-//ietf//dtd html 3.2 final//",
            "-//ietf//dtd html 3.2//",
            "-//ietf//dtd html 3//",
            "-//ietf//dtd html level 0//",
            "-//ietf//dtd html level 1//",
            "-//ietf//dtd html level 2//",
            "-//ietf//dtd html level 3//",
            "-//ietf//dtd html strict level 0//",
            "-//ietf//dtd html strict level 1//",
            "-//ietf//dtd html strict level 2//",
            "-//ietf//dtd html strict level 3//",
            "-//ietf//dtd html strict//",
            "-//ietf//dtd html//",
            "-//metrius//dtd metrius presentational//",
            "-//microsoft//dtd internet explorer 2.0 html strict//",
            "-//microsoft//dtd internet explorer 2.0 html//",
            "-//microsoft//dtd internet explorer 2.0 tables//",
            "-//microsoft//dtd internet explorer 3.0 html strict//",
            "-//microsoft//dtd internet explorer 3.0 html//",
            "-//microsoft//dtd internet explorer 3.0 tables//",
            "-//netscape comm. corp.//dtd html//",
            "-//netscape comm. corp.//dtd strict html//",
            "-//o'reilly and associates//dtd html 2.0//",
            "-//o'reilly and associates//dtd html extended 1.0//",
            "-//o'reilly and associates//dtd html extended relaxed 1.0//",
            "-//softquad software//dtd hotmetal pro 6.0::19990601::extensions to html 4.0//",
            "-//softquad//dtd hotmetal pro 4.0::19971010::extensions to html 4.0//",
            "-//spyglass//dtd html 2.0 extended//",
            "-//sq//dtd html 2.0 hotmetal + extensions//",
            "-//sun microsystems corp.//dtd hotjava html//",
            "-//sun microsystems corp.//dtd hotjava strict html//",
            "-//w3c//dtd html 3 1995-03-24//", "-//w3c//dtd html 3.2 draft//",
            "-//w3c//dtd html 3.2 final//", "-//w3c//dtd html 3.2//",
            "-//w3c//dtd html 3.2s draft//", "-//w3c//dtd html 4.0 frameset//",
            "-//w3c//dtd html 4.0 transitional//",
            "-//w3c//dtd html experimental 19960712//",
            "-//w3c//dtd html experimental 970421//", "-//w3c//dtd w3 html//",
            "-//w3o//dtd w3 html 3.0//", "-//webtechs//dtd mozilla html 2.0//",
            "-//webtechs//dtd mozilla html//" };

    internal const int NOT_FOUND_ON_STACK = Int32.MaxValue;

    // [NOCPP[

    internal const String HTML_LOCAL = "html";
    
    // ]NOCPP]

    // TODO: see if we need to move all static methods to this base class as well
    /**
     * 
     * <p>
     * C++ memory note: The return value must be released.
     * 
     * @return
     * @throws SAXException
     * @throws StopSniffingException
     */
    public static String extractCharsetFromContent(String attributeValue) {
        // This is a bit ugly. Converting the string to char array in order to
        // make the portability layer smaller.
        int charsetState = CHARSET_INITIAL;
        int start = -1;
        int end = -1;
        char[] buffer = Portability.newCharArrayFromString(attributeValue);

        for (int i = 0; i < buffer.Length; i++) {
            char c = buffer[i];
            switch (charsetState) {
                case CHARSET_INITIAL:
                    switch (c) {
                        case 'c':
                        case 'C':
                            charsetState = CHARSET_C;
                            continue;
                        default:
                            continue;
                    }
                case CHARSET_C:
                    switch (c) {
                        case 'h':
                        case 'H':
                            charsetState = CHARSET_H;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_H:
                    switch (c) {
                        case 'a':
                        case 'A':
                            charsetState = CHARSET_A;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_A:
                    switch (c) {
                        case 'r':
                        case 'R':
                            charsetState = CHARSET_R;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_R:
                    switch (c) {
                        case 's':
                        case 'S':
                            charsetState = CHARSET_S;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_S:
                    switch (c) {
                        case 'e':
                        case 'E':
                            charsetState = CHARSET_E;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_E:
                    switch (c) {
                        case 't':
                        case 'T':
                            charsetState = CHARSET_T;
                            continue;
                        default:
                            charsetState = CHARSET_INITIAL;
                            continue;
                    }
                case CHARSET_T:
                    switch (c) {
                        case '\t':
                        case '\n':
                        case '\u000C':
                        case '\r':
                        case ' ':
                            continue;
                        case '=':
                            charsetState = CHARSET_EQUALS;
                            continue;
                        default:
                            return null;
                    }
                case CHARSET_EQUALS:
                    switch (c) {
                        case '\t':
                        case '\n':
                        case '\u000C':
                        case '\r':
                        case ' ':
                            continue;
                        case '\'':
                            start = i + 1;
                            charsetState = CHARSET_SINGLE_QUOTED;
                            continue;
                        case '\"':
                            start = i + 1;
                            charsetState = CHARSET_DOUBLE_QUOTED;
                            continue;
                        default:
                            start = i;
                            charsetState = CHARSET_UNQUOTED;
                            continue;
                    }
                case CHARSET_SINGLE_QUOTED:
                    switch (c) {
                        case '\'':
                            end = i;
                            goto charsetloop_break;
                        default:
                            continue;
                    }
                case CHARSET_DOUBLE_QUOTED:
                    switch (c) {
                        case '\"':
                            end = i;
                            goto charsetloop_break;
                        default:
                            continue;
                    }
                case CHARSET_UNQUOTED:
                    switch (c) {
                        case '\t':
                        case '\n':
                        case '\u000C':
                        case '\r':
                        case ' ':
                        case ';':
                            end = i;
                            goto charsetloop_break;
                        default:
                            continue;
                    }
            }
        }
charsetloop_break: String charset = null;
        if (start != -1) {
            if (end == -1) {
                end = buffer.Length;
            }
            charset = Portability.newStringFromBuffer(buffer, start, end
                    - start);
        }
        return charset;
    }
     
}

public abstract class TreeBuilder<T> : TreeBuilderBase, TreeBuilderState<T>, TokenHandler where T:class {
    

    private int mode = INITIAL;

    private int originalMode = INITIAL;
    
    /**
     * Used only when moving back to IN_BODY.
     */
    private bool framesetOk = true;

    protected Tokenizer tokenizer;

    // [NOCPP[

    protected ErrorHandler errorHandler;

    private DocumentModeHandler documentModeHandler;

    private DoctypeExpectation doctypeExpectation = DoctypeExpectation.HTML;

    private LocatorImpl firstCommentLocation;
    
    // ]NOCPP]

    private bool scriptingEnabled = false;

    private bool needToDropLF;

    // [NOCPP[

    private bool wantingComments;

    // ]NOCPP]

    private bool fragment;

    private String contextName;

    private String contextNamespace;

    private T contextNode;

    private StackNode<T>[] stack;

    private int currentPtr = -1;

    private StackNode<T>[] listOfActiveFormattingElements;

    private int listPtr = -1;

    private T formPointer;

    private T headPointer;

    /**
     * Used to work around Gecko limitations. Not used in Java.
     */
    private T deepTreeSurrogateParent;

    protected char[] charBuffer;

    protected int charBufferLen = 0;

    private bool quirks = false;

    // [NOCPP[

    private bool reportingDoctype = true;

    private XmlViolationPolicy namePolicy = XmlViolationPolicy.ALTER_INFOSET;

    private Dictionary<String, LocatorImpl> idLocations = new Dictionary<String, LocatorImpl>();

    private bool html4;

    // ]NOCPP]

    protected TreeBuilder() {
        fragment = false;
    }

    /**
     * Reports an condition that would make the infoset incompatible with XML
     * 1.0 as fatal.
     * 
     * @throws SAXException
     * @throws SAXParseException
     */
    protected void fatal() {
    }

    // [NOCPP[

    protected void fatal(Exception e) {
        SAXParseException spe = new SAXParseException(e.Message, tokenizer, e);
        if (errorHandler != null) {
            errorHandler.fatalError(spe);
        }
        throw spe;
    }

    public void fatal(String s) {
        SAXParseException spe = new SAXParseException(s, tokenizer);
        if (errorHandler != null) {
            errorHandler.fatalError(spe);
        }
        throw spe;
    }

    /**
     * Reports a Parse Error.
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    void err(String message) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck(message);
    }
    
    /**
     * Reports a Parse Error without checking if an error handler is present.
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    void errNoCheck(String message) {
        SAXParseException spe = new SAXParseException(message, tokenizer);
        errorHandler.error(spe);
    }

    private void errListUnclosedStartTags(int eltPos) {
        if (currentPtr != -1) {
            for (int i = currentPtr; i > eltPos; i--) {
                reportUnclosedElementNameAndLocation(i);
            }
        }
    }

    /**
     * Reports the name and location of an unclosed element.
     * 
     * @throws SAXException
     */
    private void reportUnclosedElementNameAndLocation(int pos) {
        StackNode<T> node = stack[pos];
        if (node.isOptionalEndTag()) {
            return;
        }
        TaintableLocatorImpl locator = node.getLocator();
        if (locator.isTainted()) {
            return;
        }
        locator.markTainted();
        SAXParseException spe = new SAXParseException(
                "Unclosed element \u201C" + node.popName + "\u201D.", locator);
        errorHandler.error(spe);
    }

    /**
     * Reports a warning
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    public void warn(String message) {
        if (errorHandler == null) {
            return;
        }
        SAXParseException spe = new SAXParseException(message, tokenizer);
        errorHandler.warning(spe);
    }

    /**
     * Reports a warning with an explicit locator
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    public void warn(String message, Locator locator) {
        if (errorHandler == null) {
            return;
        }
        SAXParseException spe = new SAXParseException(message, locator);
        errorHandler.warning(spe);
    }

    // ]NOCPP]
    
    public void startTokenization(Tokenizer self) {
        tokenizer = self;
        stack = new StackNode<T>[64];
        listOfActiveFormattingElements = new StackNode<T>[64];
        needToDropLF = false;
        originalMode = INITIAL;
        currentPtr = -1;
        listPtr = -1;
        formPointer = null;
        headPointer = null;
        deepTreeSurrogateParent = null;
        // [NOCPP[
        html4 = false;
        idLocations.Clear();
        wantingComments = wantsComments();
        firstCommentLocation = null;
        // ]NOCPP]
        start(fragment);
        charBufferLen = 0;
        charBuffer = new char[1024];
        framesetOk = true;
        if (fragment) {
            T elt;
            if (contextNode != null) {
                elt = contextNode;
            } else {
                elt = createHtmlElementSetAsRoot(tokenizer.emptyAttributes());
            }
            StackNode<T> node = new StackNode<T>(ElementName.HTML, elt
            // [NOCPP[
                    , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
            // ]NOCPP]
            );
            currentPtr++;
            stack[currentPtr] = node;
            resetTheInsertionMode();
            if ("title" == contextName || "textarea" == contextName) {
                tokenizer.setStateAndEndTagExpectation(Tokenizer.RCDATA, contextName);
            } else if ("style" == contextName || "xmp" == contextName
                    || "iframe" == contextName || "noembed" == contextName
                    || "noframes" == contextName
                    || (scriptingEnabled && "noscript" == contextName)) {
                tokenizer.setStateAndEndTagExpectation(Tokenizer.RAWTEXT, contextName);
            } else if ("plaintext" == contextName) {
                tokenizer.setStateAndEndTagExpectation(Tokenizer.PLAINTEXT, contextName);
            } else if ("script" == contextName) {
                tokenizer.setStateAndEndTagExpectation(Tokenizer.SCRIPT_DATA,
                        contextName);
            } else {
                tokenizer.setStateAndEndTagExpectation(Tokenizer.DATA, contextName);
            }
            contextName = null;
            contextNode = null;
        } else {
            mode = INITIAL;
            // If we are viewing XML source, put a foreign element permanently
            // on the stack so that cdataSectionAllowed() returns true.
            // CPPONLY: if (tokenizer.isViewingXmlSource()) {
            // CPPONLY: T elt = createElement("http://www.w3.org/2000/svg",
            // CPPONLY: "svg",
            // CPPONLY: tokenizer.emptyAttributes());
            // CPPONLY: StackNode<T> node = new StackNode<T>(ElementName.SVG,
            // CPPONLY: "svg",
            // CPPONLY: elt);
            // CPPONLY: currentPtr++;
            // CPPONLY: stack[currentPtr] = node;
            // CPPONLY: }
        }
    }

    public void doctype(String name, String publicIdentifier, String systemIdentifier, bool forceQuirks) {
        needToDropLF = false;
        if (!isInForeign()) {
            switch (mode) {
                case INITIAL:
                    // [NOCPP[
                    if (reportingDoctype) {
                        // ]NOCPP]
                        String emptyString = Portability.newEmptyString();
                        appendDoctypeToDocument(name == null ? "" : name,
                                publicIdentifier == null ? emptyString
                                        : publicIdentifier,
                                systemIdentifier == null ? emptyString
                                        : systemIdentifier);
                        Portability.releaseString(emptyString);
                        // [NOCPP[
                    }
                    switch (doctypeExpectation) {
                        case DoctypeExpectation.HTML:
                            // ]NOCPP]
                            if (isQuirky(name, publicIdentifier,
                                    systemIdentifier, forceQuirks)) {
                                errQuirkyDoctype();
                                documentModeInternal(DocumentMode.QUIRKS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            } else if (isAlmostStandards(publicIdentifier,
                                    systemIdentifier)) {
                                // [NOCPP[
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                // ]NOCPP]
                                errAlmostStandardsDoctype();
                                documentModeInternal(
                                        DocumentMode.ALMOST_STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            } else {
                                // [NOCPP[
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                if ((Portability.literalEqualsString(
                                        "-//W3C//DTD HTML 4.0//EN",
                                        publicIdentifier) && (systemIdentifier == null || Portability.literalEqualsString(
                                        "http://www.w3.org/TR/REC-html40/strict.dtd",
                                        systemIdentifier)))
                                        || (Portability.literalEqualsString(
                                                "-//W3C//DTD HTML 4.01//EN",
                                                publicIdentifier) && (systemIdentifier == null || Portability.literalEqualsString(
                                                "http://www.w3.org/TR/html4/strict.dtd",
                                                systemIdentifier)))
                                        || (Portability.literalEqualsString(
                                                "-//W3C//DTD XHTML 1.0 Strict//EN",
                                                publicIdentifier) && Portability.literalEqualsString(
                                                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd",
                                                systemIdentifier))
                                        || (Portability.literalEqualsString(
                                                "-//W3C//DTD XHTML 1.1//EN",
                                                publicIdentifier) && Portability.literalEqualsString(
                                                "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd",
                                                systemIdentifier))

                                ) {
                                    warn("Obsolete doctype. Expected \u201C<!DOCTYPE html>\u201D.");
                                } else if (!((systemIdentifier == null || Portability.literalEqualsString(
                                        "about:legacy-compat", systemIdentifier)) && publicIdentifier == null)) {
                                    err("Legacy doctype. Expected \u201C<!DOCTYPE html>\u201D.");
                                }
                                // ]NOCPP]
                                documentModeInternal(
                                        DocumentMode.STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            }
                            // [NOCPP[
                            break;
                        case DoctypeExpectation.HTML401_STRICT:
                            html4 = true;
                            tokenizer.turnOnAdditionalHtml4Errors();
                            if (isQuirky(name, publicIdentifier,
                                    systemIdentifier, forceQuirks)) {
                                err("Quirky doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                documentModeInternal(DocumentMode.QUIRKS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            } else if (isAlmostStandards(publicIdentifier,
                                    systemIdentifier)) {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                err("Almost standards mode doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                documentModeInternal(
                                        DocumentMode.ALMOST_STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            } else {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                if ("-//W3C//DTD HTML 4.01//EN".Equals(publicIdentifier)) {
                                    if (!"http://www.w3.org/TR/html4/strict.dtd".Equals(systemIdentifier)) {
                                        warn("The doctype did not contain the system identifier prescribed by the HTML 4.01 specification. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                    }
                                } else {
                                    err("The doctype was not the HTML 4.01 Strict doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                }
                                documentModeInternal(
                                        DocumentMode.STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            }
                            break;
                        case DoctypeExpectation.HTML401_TRANSITIONAL:
                            html4 = true;
                            tokenizer.turnOnAdditionalHtml4Errors();
                            if (isQuirky(name, publicIdentifier,
                                    systemIdentifier, forceQuirks)) {
                                err("Quirky doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                documentModeInternal(DocumentMode.QUIRKS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            } else if (isAlmostStandards(publicIdentifier,
                                    systemIdentifier)) {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                if ("-//W3C//DTD HTML 4.01 Transitional//EN".Equals(publicIdentifier)
                                        && systemIdentifier != null) {
                                    if (!"http://www.w3.org/TR/html4/loose.dtd".Equals(systemIdentifier)) {
                                        warn("The doctype did not contain the system identifier prescribed by the HTML 4.01 specification. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                    }
                                } else {
                                    err("The doctype was not a non-quirky HTML 4.01 Transitional doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                }
                                documentModeInternal(
                                        DocumentMode.ALMOST_STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            } else {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                err("The doctype was not the HTML 4.01 Transitional doctype. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                documentModeInternal(
                                        DocumentMode.STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        true);
                            }
                            break;
                        case DoctypeExpectation.AUTO:
                            html4 = isHtml4Doctype(publicIdentifier);
                            if (html4) {
                                tokenizer.turnOnAdditionalHtml4Errors();
                            }
                            if (isQuirky(name, publicIdentifier,
                                    systemIdentifier, forceQuirks)) {
                                err("Quirky doctype. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                                documentModeInternal(DocumentMode.QUIRKS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        html4);
                            } else if (isAlmostStandards(publicIdentifier,
                                    systemIdentifier)) {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                if ("-//W3C//DTD HTML 4.01 Transitional//EN".Equals(publicIdentifier)) {
                                    if (!"http://www.w3.org/TR/html4/loose.dtd".Equals(systemIdentifier)) {
                                        warn("The doctype did not contain the system identifier prescribed by the HTML 4.01 specification. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                    }
                                } else {
                                    err("Almost standards mode doctype. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                                }
                                documentModeInternal(
                                        DocumentMode.ALMOST_STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        html4);
                            } else {
                                if (firstCommentLocation != null) {
                                    warn("Comments seen before doctype. Internet Explorer will go into the quirks mode.", firstCommentLocation);
                                }
                                if ("-//W3C//DTD HTML 4.01//EN".Equals(publicIdentifier)) {
                                    if (!"http://www.w3.org/TR/html4/strict.dtd".Equals(systemIdentifier)) {
                                        warn("The doctype did not contain the system identifier prescribed by the HTML 4.01 specification. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                    }
                                } else {
                                    if (!(publicIdentifier == null && systemIdentifier == null)) {
                                        err("Legacy doctype. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                                    }
                                }
                                documentModeInternal(
                                        DocumentMode.STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        html4);
                            }
                            break;
                        case DoctypeExpectation.NO_DOCTYPE_ERRORS:
                            if (isQuirky(name, publicIdentifier,
                                    systemIdentifier, forceQuirks)) {
                                documentModeInternal(DocumentMode.QUIRKS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            } else if (isAlmostStandards(publicIdentifier,
                                    systemIdentifier)) {
                                documentModeInternal(
                                        DocumentMode.ALMOST_STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            } else {
                                documentModeInternal(
                                        DocumentMode.STANDARDS_MODE,
                                        publicIdentifier, systemIdentifier,
                                        false);
                            }
                            break;
                    }
                    // ]NOCPP]

                    /*
                     * 
                     * Then, switch to the root element mode of the tree
                     * construction stage.
                     */
                    mode = BEFORE_HTML;
                    return;
                default:
                    break;
            }
        }
        /*
         * A DOCTYPE token Parse error.
         */
        errStrayDoctype();
        /*
         * Ignore the token.
         */
        return;
    }

    // [NOCPP[

    private bool isHtml4Doctype(String publicIdentifier) {
        if (publicIdentifier != null
                && (Array.BinarySearch(TreeBuilderBase.HTML4_PUBLIC_IDS,
                        publicIdentifier) > -1)) {
            return true;
        }
        return false;
    }

    // ]NOCPP]

    public void comment(char[] buf, int start, int length) {
        needToDropLF = false;
        // [NOCPP[
        if (firstCommentLocation == null) {
            firstCommentLocation = new LocatorImpl(tokenizer);
        }
        if (!wantingComments) {
            return;
        }
        // ]NOCPP]
        if (!isInForeign()) {
            switch (mode) {
                case INITIAL:
                case BEFORE_HTML:
                case AFTER_AFTER_BODY:
                case AFTER_AFTER_FRAMESET:
                    /*
                     * A comment token Append a Comment node to the Document
                     * object with the data attribute set to the data given in
                     * the comment token.
                     */
                    appendCommentToDocument(buf, start, length);
                    return;
                case AFTER_BODY:
                    /*
                     * A comment token Append a Comment node to the first
                     * element in the stack of open elements (the html element),
                     * with the data attribute set to the data given in the
                     * comment token.
                     */
                    flushCharacters();
                    appendComment(stack[0].node, buf, start, length);
                    return;
                default:
                    break;
            }
        }
        /*
         * A comment token Append a Comment node to the current node with the
         * data attribute set to the data given in the comment token.
         */
        flushCharacters();
        appendComment(stack[currentPtr].node, buf, start, length);
        return;
    }

    /**
     * @see nu.validator.htmlparser.common.TokenHandler#characters(char[], int,
     *      int)
     */
    public void characters(char[] buf, int start, int length) {
        // Note: Can't attach error messages to EOF in C++ yet

        // CPPONLY: if (tokenizer.isViewingXmlSource()) {
        // CPPONLY: return;
        // CPPONLY: }
        if (needToDropLF) {
            needToDropLF = false;
            if (buf[start] == '\n') {
                start++;
                length--;
                if (length == 0) {
                    return;
                }
            }
        }

        // optimize the most common case
        switch (mode) {
            case IN_BODY:
            case IN_CELL:
            case IN_CAPTION:
                if (!isInForeignButNotHtmlIntegrationPoint()) {
                    reconstructTheActiveFormattingElements();
                }
                goto case TEXT;
                // fall through
            case TEXT:
                accumulateCharacters(buf, start, length);
                return;
            case IN_TABLE:
            case IN_TABLE_BODY:
            case IN_ROW:
                accumulateCharactersForced(buf, start, length);
                return;
            default:
                int end = start + length;
                for (int i = start; i < end; i++) {
                    switch (buf[i]) {
                        case ' ':
                        case '\t':
                        case '\n':
                        case '\r':
                        case '\u000C':
                            /*
                             * A character token that is one of one of U+0009
                             * CHARACTER TABULATION, U+000A LINE FEED (LF),
                             * U+000C FORM FEED (FF), or U+0020 SPACE
                             */
                            switch (mode) {
                                case INITIAL:
                                case BEFORE_HTML:
                                case BEFORE_HEAD:
                                    /*
                                     * Ignore the token.
                                     */
                                    start = i + 1;
                                    continue;
                                case IN_HEAD:
                                case IN_HEAD_NOSCRIPT:
                                case AFTER_HEAD:
                                case IN_COLUMN_GROUP:
                                case IN_FRAMESET:
                                case AFTER_FRAMESET:
                                    /*
                                     * Append the character to the current node.
                                     */
                                    continue;
                                case FRAMESET_OK:
                                case IN_BODY:
                                case IN_CELL:
                                case IN_CAPTION:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i - start);
                                        start = i;
                                    }

                                    /*
                                     * Reconstruct the active formatting
                                     * elements, if any.
                                     */
                                    if (!isInForeignButNotHtmlIntegrationPoint()) {
                                        flushCharacters();
                                        reconstructTheActiveFormattingElements();
                                    }
                                    /*
                                     * Append the token's character to the
                                     * current node.
                                     */
                                    goto charactersloop_break;
                                case IN_SELECT:
                                case IN_SELECT_IN_TABLE:
                                    goto charactersloop_break;
                                case IN_TABLE:
                                case IN_TABLE_BODY:
                                case IN_ROW:
                                    accumulateCharactersForced(buf, i, 1);
                                    start = i + 1;
                                    continue;
                                case AFTER_BODY:
                                case AFTER_AFTER_BODY:
                                case AFTER_AFTER_FRAMESET:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i - start);
                                        start = i;
                                    }
                                    /*
                                     * Reconstruct the active formatting
                                     * elements, if any. 
                                     */
                                    flushCharacters();
                                    reconstructTheActiveFormattingElements();
                                    /*
                                     * Append the token's character to the
                                     * current node.
                                     */
                                    continue;
                            }
                            // fall through 
                            // TODO: figure out how to solve the case of goto default in nested switch statements
                            goto default_hack;
                        default:
                        default_hack: { } 
                            /*
                             * A character token that is not one of one of
                             * U+0009 CHARACTER TABULATION, U+000A LINE FEED
                             * (LF), U+000C FORM FEED (FF), or U+0020 SPACE
                             */
                            switch (mode) {
                                case INITIAL:
                                    /*
                                     * Parse error.
                                     */
                                    // [NOCPP[
                                    switch (doctypeExpectation) {
                                        case DoctypeExpectation.AUTO:
                                            err("Non-space characters found without seeing a doctype first. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                                            break;
                                        case DoctypeExpectation.HTML:
                                            // XXX figure out a way to report this in the Gecko View Source case
                                            err("Non-space characters found without seeing a doctype first. Expected \u201C<!DOCTYPE html>\u201D.");
                                            break;
                                        case DoctypeExpectation.HTML401_STRICT:
                                            err("Non-space characters found without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                                            break;
                                        case DoctypeExpectation.HTML401_TRANSITIONAL:
                                            err("Non-space characters found without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                                            break;
                                        case DoctypeExpectation.NO_DOCTYPE_ERRORS:
                                            break;
                                    }
                                    // ]NOCPP]
                                    /*
                                     * 
                                     * Set the document to quirks mode.
                                     */
                                    documentModeInternal(DocumentMode.QUIRKS_MODE, null, null, false);
                                    /*
                                     * Then, switch to the root element mode of
                                     * the tree construction stage
                                     */
                                    mode = BEFORE_HTML;
                                    /*
                                     * and reprocess the current token.
                                     */
                                    i--;
                                    continue;
                                case BEFORE_HTML:
                                    /*
                                     * Create an HTMLElement node with the tag
                                     * name html, in the HTML namespace. Append
                                     * it to the Document object.
                                     */
                                    // No need to flush characters here,
                                    // because there's nothing to flush.
                                    appendHtmlElementToDocumentAndPush();
                                    /* Switch to the main mode */
                                    mode = BEFORE_HEAD;
                                    /*
                                     * reprocess the current token.
                                     */
                                    i--;
                                    continue;
                                case BEFORE_HEAD:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * /Act as if a start tag token with the tag
                                     * name "head" and no attributes had been
                                     * seen,
                                     */
                                    flushCharacters();
                                    appendToCurrentNodeAndPushHeadElement(HtmlAttributes.EMPTY_ATTRIBUTES);
                                    mode = IN_HEAD;
                                    /*
                                     * then reprocess the current token.
                                     * 
                                     * This will result in an empty head element
                                     * being generated, with the current token
                                     * being reprocessed in the "after head"
                                     * insertion mode.
                                     */
                                    i--;
                                    continue;
                                case IN_HEAD:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * Act as if an end tag token with the tag
                                     * name "head" had been seen,
                                     */
                                    flushCharacters();
                                    pop();
                                    mode = AFTER_HEAD;
                                    /*
                                     * and reprocess the current token.
                                     */
                                    i--;
                                    continue;
                                case IN_HEAD_NOSCRIPT:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * Parse error. Act as if an end tag with
                                     * the tag name "noscript" had been seen
                                     */
                                    errNonSpaceInNoscriptInHead();
                                    flushCharacters();
                                    pop();
                                    mode = IN_HEAD;
                                    /*
                                     * and reprocess the current token.
                                     */
                                    i--;
                                    continue;
                                case AFTER_HEAD:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i - start);
                                        start = i;
                                    }
                                    /*
                                     * Act as if a start tag token with the tag
                                     * name "body" and no attributes had been
                                     * seen,
                                     */
                                    flushCharacters();
                                    appendToCurrentNodeAndPushBodyElement();
                                    mode = FRAMESET_OK;
                                    /*
                                     * and then reprocess the current token.
                                     */
                                    i--;
                                    continue;
                                case FRAMESET_OK:
                                    framesetOk = false;
                                    mode = IN_BODY;
                                    i--;
                                    continue;
                                case IN_BODY:
                                case IN_CELL:
                                case IN_CAPTION:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i - start);
                                        start = i;
                                    }
                                    /*
                                     * Reconstruct the active formatting
                                     * elements, if any.
                                     */
                                    if (!isInForeignButNotHtmlIntegrationPoint()) {
                                        flushCharacters();
                                        reconstructTheActiveFormattingElements();
                                    }
                                    /*
                                     * Append the token's character to the
                                     * current node.
                                     */
                                    goto charactersloop_break;
                                case IN_TABLE:
                                case IN_TABLE_BODY:
                                case IN_ROW:
                                    accumulateCharactersForced(buf, i, 1);
                                    start = i + 1;
                                    continue;
                                case IN_COLUMN_GROUP:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * Act as if an end tag with the tag name
                                     * "colgroup" had been seen, and then, if
                                     * that token wasn't ignored, reprocess the
                                     * current token.
                                     */
                                    if (currentPtr == 0) {
                                        errNonSpaceInColgroupInFragment();
                                        start = i + 1;
                                        continue;
                                    }
                                    flushCharacters();
                                    pop();
                                    mode = IN_TABLE;
                                    i--;
                                    continue;
                                case IN_SELECT:
                                case IN_SELECT_IN_TABLE:
                                    goto charactersloop_break;
                                case AFTER_BODY:
                                    errNonSpaceAfterBody();
                                    fatal();
                                    mode = framesetOk ? FRAMESET_OK : IN_BODY;
                                    i--;
                                    continue;
                                case IN_FRAMESET:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * Parse error.
                                     */
                                    errNonSpaceInFrameset();
                                    /*
                                     * Ignore the token.
                                     */
                                    start = i + 1;
                                    continue;
                                case AFTER_FRAMESET:
                                    if (start < i) {
                                        accumulateCharacters(buf, start, i
                                                - start);
                                        start = i;
                                    }
                                    /*
                                     * Parse error.
                                     */
                                    errNonSpaceAfterFrameset();
                                    /*
                                     * Ignore the token.
                                     */
                                    start = i + 1;
                                    continue;
                                case AFTER_AFTER_BODY:
                                    /*
                                     * Parse error.
                                     */
                                    errNonSpaceInTrailer();
                                    /*
                                     * Switch back to the main mode and
                                     * reprocess the token.
                                     */
                                    mode = framesetOk ? FRAMESET_OK : IN_BODY;
                                    i--;
                                    continue;
                                case AFTER_AFTER_FRAMESET:
                                    errNonSpaceInTrailer();
                                    /*
                                     * Switch back to the main mode and
                                     * reprocess the token.
                                     */
                                    mode = IN_FRAMESET;
                                    i--;
                                    continue;
                            }
                    }
                }
            charactersloop_break: 
                if (start < end) {
                    accumulateCharacters(buf, start, end - start);
                }
        }
    }

    /**
     * @see nu.validator.htmlparser.common.TokenHandler#zeroOriginatingReplacementCharacter()
     */
    public void zeroOriginatingReplacementCharacter() {
        if (mode == TEXT) {
            accumulateCharacters(REPLACEMENT_CHARACTER, 0, 1);
            return;
        }
        if (currentPtr >= 0) {
            StackNode<T> stackNode = stack[currentPtr];
            if (stackNode.ns == "http://www.w3.org/1999/xhtml") {
                return;
            }
            if (stackNode.isHtmlIntegrationPoint()) {
                return;
            }
            accumulateCharacters(REPLACEMENT_CHARACTER, 0, 1);
        }
    }

    public void eof() {
        flushCharacters();
        // Note: Can't attach error messages to EOF in C++ yet
        for (;;) {
            if (isInForeign()) {
                // [NOCPP[
                err("End of file in a foreign namespace context.");
                // ]NOCPP]
                goto eofloop_exit;
            }
            switch (mode) {
                case INITIAL:
                    /*
                     * Parse error.
                     */
                    // [NOCPP[
                    switch (doctypeExpectation) {
                        case DoctypeExpectation.AUTO:
                            err("End of file seen without seeing a doctype first. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                            break;
                        case DoctypeExpectation.HTML:
                            err("End of file seen without seeing a doctype first. Expected \u201C<!DOCTYPE html>\u201D.");
                            break;
                        case DoctypeExpectation.HTML401_STRICT:
                            err("End of file seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.HTML401_TRANSITIONAL:
                            err("End of file seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.NO_DOCTYPE_ERRORS:
                            break;
                    }
                    // ]NOCPP]
                    /*
                     * 
                     * Set the document to quirks mode.
                     */
                    documentModeInternal(DocumentMode.QUIRKS_MODE, null, null,
                            false);
                    /*
                     * Then, switch to the root element mode of the tree
                     * construction stage
                     */
                    mode = BEFORE_HTML;
                    /*
                     * and reprocess the current token.
                     */
                    continue;
                case BEFORE_HTML:
                    /*
                     * Create an HTMLElement node with the tag name html, in the
                     * HTML namespace. Append it to the Document object.
                     */
                    appendHtmlElementToDocumentAndPush();
                    // XXX application cache manifest
                    /* Switch to the main mode */
                    mode = BEFORE_HEAD;
                    /*
                     * reprocess the current token.
                     */
                    continue;
                case BEFORE_HEAD:
                    appendToCurrentNodeAndPushHeadElement(HtmlAttributes.EMPTY_ATTRIBUTES);
                    mode = IN_HEAD;
                    continue;
                case IN_HEAD:
                    // [NOCPP[
                    if (errorHandler != null && currentPtr > 1) {
                        errEofWithUnclosedElements();
                    }
                    // ]NOCPP]
                    while (currentPtr > 0) {
                        popOnEof();
                    }
                    mode = AFTER_HEAD;
                    continue;
                case IN_HEAD_NOSCRIPT:
                    // [NOCPP[
                    errEofWithUnclosedElements();
                    // ]NOCPP]
                    while (currentPtr > 1) {
                        popOnEof();
                    }
                    mode = IN_HEAD;
                    continue;
                case AFTER_HEAD:
                    appendToCurrentNodeAndPushBodyElement();
                    mode = IN_BODY;
                    continue;
                case IN_COLUMN_GROUP:
                    if (currentPtr == 0) {
                        Debug.Assert(fragment);
                        goto eofloop_exit;
                    } else {
                        popOnEof();
                        mode = IN_TABLE;
                        continue;
                    }
                case FRAMESET_OK:
                case IN_CAPTION:
                case IN_CELL:
                case IN_BODY:
                    // [NOCPP[
                    for (int i = currentPtr; i >= 0; i--) {
                        int group = stack[i].getGroup();
                        switch (group) {
                            case DD_OR_DT:
                            case LI:
                            case P:
                            case TBODY_OR_THEAD_OR_TFOOT:
                            case TD_OR_TH:
                            case BODY:
                            case HTML:
                                break;
                            default:
                                errEofWithUnclosedElements();
                                goto openelementloop_exit;
                        }
                    }
            openelementloop_exit:
                    // ]NOCPP]
                    goto eofloop_exit;
                case TEXT:
                    // [NOCPP[
                    if (errorHandler != null) {
                        errNoCheck("End of file seen when expecting text or an end tag.");
                        errListUnclosedStartTags(0);
                    }
                    // ]NOCPP]
                    // XXX mark script as already executed
                    if (originalMode == AFTER_HEAD) {
                        popOnEof();
                    }
                    popOnEof();
                    mode = originalMode;
                    continue;
                case IN_TABLE_BODY:
                case IN_ROW:
                case IN_TABLE:
                case IN_SELECT:
                case IN_SELECT_IN_TABLE:
                case IN_FRAMESET:
                    // [NOCPP[
                    if (errorHandler != null && currentPtr > 0) {
                        errEofWithUnclosedElements();
                    }
                    // ]NOCPP]
                    goto eofloop_exit;
                case AFTER_BODY:
                case AFTER_FRAMESET:
                case AFTER_AFTER_BODY:
                case AFTER_AFTER_FRAMESET:
                default:
                    goto eofloop_exit;
            }
        }
    eofloop_exit: 
        while (currentPtr > 0) {
            popOnEof();
        }
        if (!fragment) {
            popOnEof();
        }
        /* Stop parsing. */
    }

    /**
     * @see nu.validator.htmlparser.common.TokenHandler#endTokenization()
     */
    public void endTokenization() {
        formPointer = null;
        headPointer = null;
        deepTreeSurrogateParent = null;
        if (stack != null) {
            while (currentPtr > -1) {
                stack[currentPtr].release();
                currentPtr--;
            }
            stack = null;
        }
        if (listOfActiveFormattingElements != null) {
            while (listPtr > -1) {
                if (listOfActiveFormattingElements[listPtr] != null) {
                    listOfActiveFormattingElements[listPtr].release();
                }
                listPtr--;
            }
            listOfActiveFormattingElements = null;
        }
        // [NOCPP[
        idLocations.Clear();
        // ]NOCPP]
        charBuffer = null;
        end();
    }

    public void startTag(ElementName elementName, HtmlAttributes attributes, bool selfClosing) {
        flushCharacters();

        // [NOCPP[
        if (errorHandler != null) {
            // ID uniqueness
            String id = attributes.getId();
            if (id != null) {
                LocatorImpl oldLoc = idLocations[id];
                if (oldLoc != null) {
                    err("Duplicate ID \u201C" + id + "\u201D.");
                    errorHandler.warning(new SAXParseException(
                            "The first occurrence of ID \u201C" + id
                            + "\u201D was here.", oldLoc));
                } else {
                    idLocations[id] = new LocatorImpl(tokenizer);
                }
            }
        }
        // ]NOCPP]

        int eltPos;
        needToDropLF = false;
        for (;;) {
            int group = elementName.getGroup();
            String name = elementName.name;
            if (isInForeign()) {
                StackNode<T> currentNode = stack[currentPtr];
                String currNs = currentNode.ns;
                if (!(currentNode.isHtmlIntegrationPoint() || (currNs == "http://www.w3.org/1998/Math/MathML" && ((currentNode.getGroup() == MI_MO_MN_MS_MTEXT && group != MGLYPH_OR_MALIGNMARK) || (currentNode.getGroup() == ANNOTATION_XML && group == SVG))))) {
                    switch (group) {
                        case B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U:
                        case DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU:
                        case BODY:
                        case BR:
                        case RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR:
                        case DD_OR_DT:
                        case UL_OR_OL_OR_DL:
                        case EMBED_OR_IMG:
                        case H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6:
                        case HEAD:
                        case HR:
                        case LI:
                        case META:
                        case NOBR:
                        case P:
                        case PRE_OR_LISTING:
                        case TABLE:
                            errHtmlStartTagInForeignContext(name);
                            while (!isSpecialParentInForeign(stack[currentPtr])) {
                                pop();
                            }
                            goto starttagloop_continue;
                        case FONT:
                            if (attributes.contains(AttributeName.COLOR)
                                    || attributes.contains(AttributeName.FACE)
                                    || attributes.contains(AttributeName.SIZE)) {
                                errHtmlStartTagInForeignContext(name);
                                while (!isSpecialParentInForeign(stack[currentPtr])) {
                                    pop();
                                }
                                goto starttagloop_continue;
                            }
                            // else fall thru
                            goto default;
                        default:
                            if ("http://www.w3.org/2000/svg" == currNs) {
                                attributes.adjustForSvg();
                                if (selfClosing) {
                                    appendVoidElementToCurrentMayFosterSVG(
                                            elementName, attributes);
                                    selfClosing = false;
                                } else {
                                    appendToCurrentNodeAndPushElementMayFosterSVG(
                                            elementName, attributes);
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            } else {
                                attributes.adjustForMath();
                                if (selfClosing) {
                                    appendVoidElementToCurrentMayFosterMathML(
                                            elementName, attributes);
                                    selfClosing = false;
                                } else {
                                    appendToCurrentNodeAndPushElementMayFosterMathML(
                                            elementName, attributes);
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            }
                    } // switch
                } // foreignObject / annotation-xml
            }
            switch (mode) {
                case IN_TABLE_BODY:
                    switch (group) {
                        case TR:
                            clearStackBackTo(findLastInTableScopeOrRootTbodyTheadTfoot());
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            mode = IN_ROW;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case TD_OR_TH:
                            errStartTagInTableBody(name);
                            clearStackBackTo(findLastInTableScopeOrRootTbodyTheadTfoot());
                            appendToCurrentNodeAndPushElement(
                                    ElementName.TR,
                                    HtmlAttributes.EMPTY_ATTRIBUTES);
                            mode = IN_ROW;
                            continue;
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case TBODY_OR_THEAD_OR_TFOOT:
                            eltPos = findLastInTableScopeOrRootTbodyTheadTfoot();
                            if (eltPos == 0) {
                                errStrayStartTag(name);
                                goto starttagloop_break;
                            } else {
                                clearStackBackTo(eltPos);
                                pop();
                                mode = IN_TABLE;
                                continue;
                            }
                        default:
                            // fall through to IN_ROW
                            goto in_row_fallthrough;
                    }
                case IN_ROW:
            in_row_fallthrough:
                    switch (group) {
                        case TD_OR_TH:
                            clearStackBackTo(findLastOrRoot(TreeBuilderBase.TR));
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            mode = IN_CELL;
                            insertMarker();
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                            eltPos = findLastOrRoot(TreeBuilderBase.TR);
                            if (eltPos == 0) {
                                Debug.Assert(fragment);
                                errNoTableRowToClose();
                                goto starttagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE_BODY;
                            continue;
                        default:
                            // fall through to IN_TABLE
                            break;
                    }
                    goto case IN_TABLE;
                case IN_TABLE:
                    for (;;) {
                        switch (group) {
                            case CAPTION:
                                clearStackBackTo(findLastOrRoot(TreeBuilderBase.TABLE));
                                insertMarker();
                                appendToCurrentNodeAndPushElement(
                                        elementName,
                                        attributes);
                                mode = IN_CAPTION;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case COLGROUP:
                                clearStackBackTo(findLastOrRoot(TreeBuilderBase.TABLE));
                                appendToCurrentNodeAndPushElement(
                                        elementName,
                                        attributes);
                                mode = IN_COLUMN_GROUP;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case COL:
                                clearStackBackTo(findLastOrRoot(TreeBuilderBase.TABLE));
                                appendToCurrentNodeAndPushElement(
                                        ElementName.COLGROUP,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                mode = IN_COLUMN_GROUP;
                                goto starttagloop_continue;
                            case TBODY_OR_THEAD_OR_TFOOT:
                                clearStackBackTo(findLastOrRoot(TreeBuilderBase.TABLE));
                                appendToCurrentNodeAndPushElement(
                                        elementName,
                                        attributes);
                                mode = IN_TABLE_BODY;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case TR:
                            case TD_OR_TH:
                                clearStackBackTo(findLastOrRoot(TreeBuilderBase.TABLE));
                                appendToCurrentNodeAndPushElement(
                                        ElementName.TBODY,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                mode = IN_TABLE_BODY;
                                goto starttagloop_continue;
                            case TABLE:
                                errTableSeenWhileTableOpen();
                                eltPos = findLastInTableScope(name);
                                if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                    Debug.Assert(fragment);
                                    goto starttagloop_break;
                                }
                                generateImpliedEndTags();
                                // XXX is the next if dead code?
                                if (errorHandler != null && !isCurrent("table")) {
                                    errNoCheckUnclosedElementsOnStack();
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                                resetTheInsertionMode();
                                goto starttagloop_continue;
                            case SCRIPT:
                                // XXX need to manage much more stuff
                                // here if
                                // supporting
                                // document.write()
                                appendToCurrentNodeAndPushElement(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.SCRIPT_DATA, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case STYLE:
                                appendToCurrentNodeAndPushElement(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RAWTEXT, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case INPUT:
                                if (!Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                                        "hidden",
                                        attributes.getValue(AttributeName.TYPE))) {
                                    goto intableloop_exit;
                                }
                                appendVoidElementToCurrent(
                                        name, attributes,
                                        formPointer);
                                selfClosing = false;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case FORM:
                                if (formPointer != null) {
                                    errFormWhenFormOpen();
                                    goto starttagloop_break;
                                } else {
                                    errStartTagInTable(name);
                                    appendVoidFormToCurrent(attributes);
                                    attributes = null; // CPP
                                    goto starttagloop_break;
                                }
                            default:
                                errStartTagInTable(name);
                                goto intableloop_exit;
                        }
                    }
                intableloop_exit: goto case IN_CAPTION;
                    // Fall though
                case IN_CAPTION:
                    switch (group) {
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                        case TD_OR_TH:
                            errStrayStartTag(name);
                            eltPos = findLastInTableScope("caption");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                goto starttagloop_break;
                            }
                            generateImpliedEndTags();
                            if (errorHandler != null && currentPtr != eltPos) {
                                errNoCheckUnclosedElementsOnStack();
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            clearTheListOfActiveFormattingElementsUpToTheLastMarker();
                            mode = IN_TABLE;
                            continue;
                        default:
                            // fall through to IN_CELL
                            goto in_cell_fallthrough;
                    }
                case IN_CELL:
            in_cell_fallthrough:
                    switch (group) {
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                        case TD_OR_TH:
                            eltPos = findLastInTableScopeTdTh();
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errNoCellToClose();
                                goto starttagloop_break;
                            } else {
                                closeTheCell(eltPos);
                                continue;
                            }
                        default:
                            // fall through to FRAMESET_OK
                            break;
                    }
                    goto case FRAMESET_OK;
                case FRAMESET_OK:
                    switch (group) {
                        case FRAMESET:
                            if (mode == FRAMESET_OK) {
                                if (currentPtr == 0 || stack[1].getGroup() != BODY) {
                                    Debug.Assert(fragment);
                                    errStrayStartTag(name);
                                    goto starttagloop_break;
                                } else {
                                    errFramesetStart();
                                    detachFromParent(stack[1].node);
                                    while (currentPtr > 0) {
                                        pop();
                                    }
                                    appendToCurrentNodeAndPushElement(
                                            elementName,
                                            attributes);
                                    mode = IN_FRAMESET;
                                    attributes = null; // CPP
                                    goto starttagloop_break;
                                }
                            } else {
                                errStrayStartTag(name);
                                goto starttagloop_break;
                            }
                            // NOT falling through!
                        case PRE_OR_LISTING:
                        case LI:
                        case DD_OR_DT:
                        case BUTTON:
                        case MARQUEE_OR_APPLET:
                        case OBJECT:
                        case TABLE:
                        case AREA_OR_WBR:
                        case BR:
                        case EMBED_OR_IMG:
                        case INPUT:
                        case KEYGEN:
                        case HR:
                        case TEXTAREA:
                        case XMP:
                        case IFRAME:
                        case SELECT:
                            if (mode == FRAMESET_OK
                                    && !(group == INPUT && Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                                            "hidden",
                                            attributes.getValue(AttributeName.TYPE)))) {
                                framesetOk = false;
                                mode = IN_BODY;
                            }
                            // fall through to IN_BODY
                            goto default;
                        default:
                            // fall through to IN_BODY
                            break;
                    }
                    goto case IN_BODY;
                case IN_BODY:
                    for (;;) {
                        switch (group) {
                            case HTML:
                                errStrayStartTag(name);
                                if (!fragment) {
                                    addAttributesToHtml(attributes);
                                    attributes = null; // CPP
                                }
                                goto starttagloop_break;
                            case BASE:
                            case LINK_OR_BASEFONT_OR_BGSOUND:
                            case META:
                            case STYLE:
                            case SCRIPT:
                            case TITLE:
                            case COMMAND:
                                // Fall through to IN_HEAD
                                goto inbodyloop_exit;
                            case BODY:
                                if (currentPtr == 0
                                        || stack[1].getGroup() != BODY) {
                                    Debug.Assert(fragment);
                                    errStrayStartTag(name);
                                    goto starttagloop_break;
                                }
                                errFooSeenWhenFooOpen(name);
                                framesetOk = false;
                                if (mode == FRAMESET_OK) {
                                    mode = IN_BODY;
                                }
                                if (addAttributesToBody(attributes)) {
                                    attributes = null; // CPP
                                }
                                goto starttagloop_break;
                            case P:
                            case DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU:
                            case UL_OR_OL_OR_DL:
                            case ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY:
                                implicitlyCloseP();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6:
                                implicitlyCloseP();
                                if (stack[currentPtr].getGroup() == H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6) {
                                    errHeadingWhenHeadingOpen();
                                    pop();
                                }
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case FIELDSET:
                                implicitlyCloseP();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes, formPointer);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case PRE_OR_LISTING:
                                implicitlyCloseP();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                needToDropLF = true;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case FORM:
                                if (formPointer != null) {
                                    errFormWhenFormOpen();
                                    goto starttagloop_break;
                                } else {
                                    implicitlyCloseP();
                                    appendToCurrentNodeAndPushFormElementMayFoster(attributes);
                                    attributes = null; // CPP
                                    goto starttagloop_break;
                                }
                            case LI:
                            case DD_OR_DT:
                                eltPos = currentPtr;
                                for (;;) {
                                    StackNode<T> node = stack[eltPos]; // weak
                                    // ref
                                    if (node.getGroup() == group) { // LI or
                                        // DD_OR_DT
                                        generateImpliedEndTagsExceptFor(node.name);
                                        if (errorHandler != null
                                                && eltPos != currentPtr) {
                                            errUnclosedElementsImplied(eltPos, name);
                                        }
                                        while (currentPtr >= eltPos) {
                                            pop();
                                        }
                                        break;
                                    } else if (node.isScoping()
                                            || (node.isSpecial()
                                                    && node.name != "p"
                                                    && node.name != "address" && node.name != "div")) {
                                        break;
                                    }
                                    eltPos--;
                                }
                                implicitlyCloseP();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case PLAINTEXT:
                                implicitlyCloseP();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.PLAINTEXT, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case A:
                                int activeAPos = findInListOfActiveFormattingElementsContainsBetweenEndAndLastMarker("a");
                                if (activeAPos != -1) {
                                    errFooSeenWhenFooOpen(name);
                                    StackNode<T> activeA = listOfActiveFormattingElements[activeAPos];
                                    activeA.retain();
                                    adoptionAgencyEndTag("a");
                                    removeFromStack(activeA);
                                    activeAPos = findInListOfActiveFormattingElements(activeA);
                                    if (activeAPos != -1) {
                                        removeFromListOfActiveFormattingElements(activeAPos);
                                    }
                                    activeA.release();
                                }
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushFormattingElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U:
                            case FONT:
                                reconstructTheActiveFormattingElements();
                                maybeForgetEarlierDuplicateFormattingElement(elementName.name, attributes);
                                appendToCurrentNodeAndPushFormattingElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case NOBR:
                                reconstructTheActiveFormattingElements();
                                if (TreeBuilderBase.NOT_FOUND_ON_STACK != findLastInScope("nobr")) {
                                    errFooSeenWhenFooOpen(name);
                                    adoptionAgencyEndTag("nobr");
                                    reconstructTheActiveFormattingElements();
                                }
                                appendToCurrentNodeAndPushFormattingElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case BUTTON:
                                eltPos = findLastInScope(name);
                                if (eltPos != TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                    errFooSeenWhenFooOpen(name);
                                    generateImpliedEndTags();
                                    if (errorHandler != null
                                            && !isCurrent(name)) {
                                        errUnclosedElementsImplied(eltPos, name);
                                    }
                                    while (currentPtr >= eltPos) {
                                        pop();
                                    }
                                    goto starttagloop_continue;
                                } else {
                                    reconstructTheActiveFormattingElements();
                                    appendToCurrentNodeAndPushElementMayFoster(
                                            elementName,
                                            attributes, formPointer);
                                    attributes = null; // CPP
                                    goto starttagloop_break;
                                }
                            case OBJECT:
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes, formPointer);
                                insertMarker();
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case MARQUEE_OR_APPLET:
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                insertMarker();
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case TABLE:
                                // The only quirk. Blame Hixie and
                                // Acid2.
                                if (!quirks) {
                                    implicitlyCloseP();
                                }
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                mode = IN_TABLE;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case BR:
                            case EMBED_OR_IMG:
                            case AREA_OR_WBR:
                                reconstructTheActiveFormattingElements();
                                // FALL THROUGH to PARAM_OR_SOURCE_OR_TRACK
                                goto param_or_source_or_track_fallthrough;
                            // CPPONLY: case MENUITEM:
                            case PARAM_OR_SOURCE_OR_TRACK:
                        param_or_source_or_track_fallthrough:
                                appendVoidElementToCurrentMayFoster(
                                        elementName,
                                        attributes);
                                selfClosing = false;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case HR:
                                implicitlyCloseP();
                                appendVoidElementToCurrentMayFoster(
                                        elementName,
                                        attributes);
                                selfClosing = false;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case IMAGE:
                                errImage();
                                elementName = ElementName.IMG;
                                goto starttagloop_continue;
                            case KEYGEN:
                            case INPUT:
                                reconstructTheActiveFormattingElements();
                                appendVoidElementToCurrentMayFoster(
                                        name, attributes,
                                        formPointer);
                                selfClosing = false;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case ISINDEX:
                                errIsindex();
                                if (formPointer != null) {
                                    goto starttagloop_break;
                                }
                                implicitlyCloseP();
                                HtmlAttributes formAttrs = new HtmlAttributes(0);
                                int actionIndex = attributes.getIndex(AttributeName.ACTION);
                                if (actionIndex > -1) {
                                    formAttrs.addAttribute(
                                            AttributeName.ACTION,
                                            attributes.getValue(actionIndex) 
                                            // [NOCPP[
                                            , XmlViolationPolicy.ALLOW
                                    // ]NOCPP]
                                    );
                                }
                                appendToCurrentNodeAndPushFormElementMayFoster(formAttrs);
                                appendVoidElementToCurrentMayFoster(
                                        ElementName.HR,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                appendToCurrentNodeAndPushElementMayFoster(
                                        ElementName.LABEL,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                int promptIndex = attributes.getIndex(AttributeName.PROMPT);
                                if (promptIndex > -1) {
                                    char[] prompt = Portability.newCharArrayFromString(attributes.getValue(promptIndex));
                                    appendCharacters(stack[currentPtr].node,
                                            prompt, 0, prompt.Length);
                                } else {
                                    appendIsindexPrompt(stack[currentPtr].node);
                                }
                                HtmlAttributes inputAttributes = new HtmlAttributes(
                                        0);
                                inputAttributes.addAttribute(
                                        AttributeName.NAME,
                                        Portability.newStringFromLiteral("isindex")
                                        // [NOCPP[
                                        , XmlViolationPolicy.ALLOW
                                // ]NOCPP]
                                );
                                for (int i = 0; i < attributes.getLength(); i++) {
                                    AttributeName attributeQName = attributes.getAttributeName(i);
                                    if (AttributeName.NAME == attributeQName
                                            || AttributeName.PROMPT == attributeQName) {
                                        attributes.releaseValue(i);
                                    } else if (AttributeName.ACTION != attributeQName) {
                                        inputAttributes.addAttribute(
                                                attributeQName,
                                                attributes.getValue(i)
                                                // [NOCPP[
                                                , XmlViolationPolicy.ALLOW
                                        // ]NOCPP]

                                        );
                                    }
                                }
                                attributes.clearWithoutReleasingContents();
                                appendVoidElementToCurrentMayFoster(
                                        "input",
                                        inputAttributes, formPointer);
                                pop(); // label
                                appendVoidElementToCurrentMayFoster(
                                        ElementName.HR,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                pop(); // form
                                selfClosing = false;
                                // Portability.delete(formAttrs);
                                // Portability.delete(inputAttributes);
                                // Don't delete attributes, they are deleted
                                // later
                                goto starttagloop_break;
                            case TEXTAREA:
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes, formPointer);
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RCDATA, elementName);
                                originalMode = mode;
                                mode = TEXT;
                                needToDropLF = true;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case XMP:
                                implicitlyCloseP();
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RAWTEXT, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case NOSCRIPT:
                                if (!scriptingEnabled) {
                                    reconstructTheActiveFormattingElements();
                                    appendToCurrentNodeAndPushElementMayFoster(
                                            elementName,
                                            attributes);
                                    attributes = null; // CPP
                                    goto starttagloop_break;
                                } else {
                                    // fall through
                                    goto case NOFRAMES;
                                }
                            case NOFRAMES:
                            case IFRAME:
                            case NOEMBED:
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RAWTEXT, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case SELECT:
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes, formPointer);
                                switch (mode) {
                                    case IN_TABLE:
                                    case IN_CAPTION:
                                    case IN_COLUMN_GROUP:
                                    case IN_TABLE_BODY:
                                    case IN_ROW:
                                    case IN_CELL:
                                        mode = IN_SELECT_IN_TABLE;
                                        break;
                                    default:
                                        mode = IN_SELECT;
                                        break;
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case OPTGROUP:
                            case OPTION:
                                if (isCurrent("option")) {
                                    pop();
                                }
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case RT_OR_RP:
                                /*
                                 * If the stack of open elements has a ruby
                                 * element in scope, then generate implied end
                                 * tags. If the current node is not then a ruby
                                 * element, this is a parse error; pop all the
                                 * nodes from the current node up to the node
                                 * immediately before the bottommost ruby
                                 * element on the stack of open elements.
                                 * 
                                 * Insert an HTML element for the token.
                                 */
                                eltPos = findLastInScope("ruby");
                                if (eltPos != NOT_FOUND_ON_STACK) {
                                    generateImpliedEndTags();
                                }
                                if (eltPos != currentPtr) {
                                    if (eltPos != NOT_FOUND_ON_STACK) {
                                        errStartTagSeenWithoutRuby(name);
                                    } else {
                                        errUnclosedChildrenInRuby();
                                    }
                                    while (currentPtr > eltPos) {
                                        pop();
                                    }
                                }
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case MATH:
                                reconstructTheActiveFormattingElements();
                                attributes.adjustForMath();
                                if (selfClosing) {
                                    appendVoidElementToCurrentMayFosterMathML(
                                            elementName, attributes);
                                    selfClosing = false;
                                } else {
                                    appendToCurrentNodeAndPushElementMayFosterMathML(
                                            elementName, attributes);
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case SVG:
                                reconstructTheActiveFormattingElements();
                                attributes.adjustForSvg();
                                if (selfClosing) {
                                    appendVoidElementToCurrentMayFosterSVG(
                                            elementName,
                                            attributes);
                                    selfClosing = false;
                                } else {
                                    appendToCurrentNodeAndPushElementMayFosterSVG(
                                            elementName, attributes);
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case CAPTION:
                            case COL:
                            case COLGROUP:
                            case TBODY_OR_THEAD_OR_TFOOT:
                            case TR:
                            case TD_OR_TH:
                            case FRAME:
                            case FRAMESET:
                            case HEAD:
                                errStrayStartTag(name);
                                goto starttagloop_break;
                            case OUTPUT_OR_LABEL:
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes, formPointer);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            default:
                                reconstructTheActiveFormattingElements();
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                attributes = null; // CPP
                                goto starttagloop_break;
                        }
                    }
                inbodyloop_exit: goto case IN_HEAD;
                case IN_HEAD:
                    for (;;) {
                        switch (group) {
                            case HTML:
                                errStrayStartTag(name);
                                if (!fragment) {
                                    addAttributesToHtml(attributes);
                                    attributes = null; // CPP
                                }
                                goto starttagloop_break;
                            case BASE:
                            case COMMAND:
                                appendVoidElementToCurrentMayFoster(
                                        elementName,
                                        attributes);
                                selfClosing = false;
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case META:
                            case LINK_OR_BASEFONT_OR_BGSOUND:
                                // Fall through to IN_HEAD_NOSCRIPT
                                goto inheadloop_exit;
                            case TITLE:
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RCDATA, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case NOSCRIPT:
                                if (scriptingEnabled) {
                                    appendToCurrentNodeAndPushElement(
                                            elementName,
                                            attributes);
                                    originalMode = mode;
                                    mode = TEXT;
                                    tokenizer.setStateAndEndTagExpectation(
                                            Tokenizer.RAWTEXT, elementName);
                                } else {
                                    appendToCurrentNodeAndPushElementMayFoster(
                                            elementName,
                                            attributes);
                                    mode = IN_HEAD_NOSCRIPT;
                                }
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case SCRIPT:
                                // XXX need to manage much more stuff
                                // here if
                                // supporting
                                // document.write()
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.SCRIPT_DATA, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case STYLE:
                            case NOFRAMES:
                                appendToCurrentNodeAndPushElementMayFoster(
                                        elementName,
                                        attributes);
                                originalMode = mode;
                                mode = TEXT;
                                tokenizer.setStateAndEndTagExpectation(
                                        Tokenizer.RAWTEXT, elementName);
                                attributes = null; // CPP
                                goto starttagloop_break;
                            case HEAD:
                                /* Parse error. */
                                errFooSeenWhenFooOpen(name);
                                /* Ignore the token. */
                                goto starttagloop_break;
                            default:
                                pop();
                                mode = AFTER_HEAD;
                                goto starttagloop_continue;
                        }
                    }
                inheadloop_exit: goto case IN_HEAD_NOSCRIPT;
                case IN_HEAD_NOSCRIPT:
                    switch (group) {
                        case HTML:
                            // XXX did Hixie really mean to omit "base"
                            // here?
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case LINK_OR_BASEFONT_OR_BGSOUND:
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case META:
                            checkMetaCharset(attributes);
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case STYLE:
                        case NOFRAMES:
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.RAWTEXT, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case HEAD:
                            errFooSeenWhenFooOpen(name);
                            goto starttagloop_break;
                        case NOSCRIPT:
                            errFooSeenWhenFooOpen(name);
                            goto starttagloop_break;
                        default:
                            errBadStartTagInHead(name);
                            pop();
                            mode = IN_HEAD;
                            continue;
                    }
                case IN_COLUMN_GROUP:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case COL:
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            if (currentPtr == 0) {
                                Debug.Assert(fragment);
                                errGarbageInColgroup();
                                goto starttagloop_break;
                            }
                            pop();
                            mode = IN_TABLE;
                            continue;
                    }
                case IN_SELECT_IN_TABLE:
                    switch (group) {
                        case CAPTION:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                        case TD_OR_TH:
                        case TABLE:
                            errStartTagWithSelectOpen(name);
                            eltPos = findLastInTableScope("select");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                Debug.Assert(fragment);
                                goto starttagloop_break; // http://www.w3.org/Bugs/Public/show_bug.cgi?id=8375
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            resetTheInsertionMode();
                            continue;
                        default:
                            // fall through to IN_SELECT
                            break;
                    }
                    goto case IN_SELECT;
                case IN_SELECT:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case OPTION:
                            if (isCurrent("option")) {
                                pop();
                            }
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case OPTGROUP:
                            if (isCurrent("option")) {
                                pop();
                            }
                            if (isCurrent("optgroup")) {
                                pop();
                            }
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case SELECT:
                            errStartSelectWhereEndSelectExpected();
                            eltPos = findLastInTableScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                Debug.Assert(fragment);
                                errNoSelectInTableScope();
                                goto starttagloop_break;
                            } else {
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                                resetTheInsertionMode();
                                goto starttagloop_break;
                            }
                        case INPUT:
                        case TEXTAREA:
                        case KEYGEN:
                            errStartTagWithSelectOpen(name);
                            eltPos = findLastInTableScope("select");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                Debug.Assert(fragment);
                                goto starttagloop_break;
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            resetTheInsertionMode();
                            continue;
                        case SCRIPT:
                            // XXX need to manage much more stuff
                            // here if
                            // supporting
                            // document.write()
                            appendToCurrentNodeAndPushElementMayFoster(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.SCRIPT_DATA, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            errStrayStartTag(name);
                            goto starttagloop_break;
                    }
                case AFTER_BODY:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        default:
                            errStrayStartTag(name);
                            mode = framesetOk ? FRAMESET_OK : IN_BODY;
                            continue;
                    }
                case IN_FRAMESET:
                    switch (group) {
                        case FRAMESET:
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case FRAME:
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            // fall through to AFTER_FRAMESET
                            break;
                    }
                    goto case AFTER_FRAMESET;
                case AFTER_FRAMESET:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case NOFRAMES:
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.RAWTEXT, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            errStrayStartTag(name);
                            goto starttagloop_break;
                    }
                case INITIAL:
                    /*
                     * Parse error.
                     */
                    // [NOCPP[
                    switch (doctypeExpectation) {
                        case DoctypeExpectation.AUTO:
                            err("Start tag seen without seeing a doctype first. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                            break;
                        case DoctypeExpectation.HTML:
                            // ]NOCPP]
                            errStartTagWithoutDoctype();
                            // [NOCPP[
                            break;
                        case DoctypeExpectation.HTML401_STRICT:
                            err("Start tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.HTML401_TRANSITIONAL:
                            err("Start tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.NO_DOCTYPE_ERRORS:
                            break;
                    }
                    // ]NOCPP]
                    /*
                     * 
                     * Set the document to quirks mode.
                     */
                    documentModeInternal(DocumentMode.QUIRKS_MODE, null, null,
                            false);
                    /*
                     * Then, switch to the root element mode of the tree
                     * construction stage
                     */
                    mode = BEFORE_HTML;
                    /*
                     * and reprocess the current token.
                     */
                    continue;
                case BEFORE_HTML:
                    switch (group) {
                        case HTML:
                            // optimize error check and streaming SAX by
                            // hoisting
                            // "html" handling here.
                            if (attributes == HtmlAttributes.EMPTY_ATTRIBUTES) {
                                // This has the right magic side effect
                                // that
                                // it
                                // makes attributes in SAX Tree mutable.
                                appendHtmlElementToDocumentAndPush();
                            } else {
                                appendHtmlElementToDocumentAndPush(attributes);
                            }
                            // XXX application cache should fire here
                            mode = BEFORE_HEAD;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            /*
                             * Create an HTMLElement node with the tag name
                             * html, in the HTML namespace. Append it to the
                             * Document object.
                             */
                            appendHtmlElementToDocumentAndPush();
                            /* Switch to the main mode */
                            mode = BEFORE_HEAD;
                            /*
                             * reprocess the current token.
                             */
                            continue;
                    }
                case BEFORE_HEAD:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case HEAD:
                            /*
                             * A start tag whose tag name is "head"
                             * 
                             * Create an element for the token.
                             * 
                             * Set the head element pointer to this new element
                             * node.
                             * 
                             * Append the new element to the current node and
                             * push it onto the stack of open elements.
                             */
                            appendToCurrentNodeAndPushHeadElement(attributes);
                            /*
                             * Change the insertion mode to "in head".
                             */
                            mode = IN_HEAD;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            /*
                             * Any other start tag token
                             * 
                             * Act as if a start tag token with the tag name
                             * "head" and no attributes had been seen,
                             */
                            appendToCurrentNodeAndPushHeadElement(HtmlAttributes.EMPTY_ATTRIBUTES);
                            mode = IN_HEAD;
                            /*
                             * then reprocess the current token.
                             * 
                             * This will result in an empty head element being
                             * generated, with the current token being
                             * reprocessed in the "after head" insertion mode.
                             */
                            continue;
                    }
                case AFTER_HEAD:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case BODY:
                            if (attributes.getLength() == 0) {
                                // This has the right magic side effect
                                // that
                                // it
                                // makes attributes in SAX Tree mutable.
                                appendToCurrentNodeAndPushBodyElement();
                            } else {
                                appendToCurrentNodeAndPushBodyElement(attributes);
                            }
                            framesetOk = false;
                            mode = IN_BODY;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case FRAMESET:
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            mode = IN_FRAMESET;
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case BASE:
                            errFooBetweenHeadAndBody(name);
                            pushHeadPointerOntoStack();
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            pop(); // head
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case LINK_OR_BASEFONT_OR_BGSOUND:
                            errFooBetweenHeadAndBody(name);
                            pushHeadPointerOntoStack();
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            pop(); // head
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case META:
                            errFooBetweenHeadAndBody(name);
                            checkMetaCharset(attributes);
                            pushHeadPointerOntoStack();
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    attributes);
                            selfClosing = false;
                            pop(); // head
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case SCRIPT:
                            errFooBetweenHeadAndBody(name);
                            pushHeadPointerOntoStack();
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.SCRIPT_DATA, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case STYLE:
                        case NOFRAMES:
                            errFooBetweenHeadAndBody(name);
                            pushHeadPointerOntoStack();
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.RAWTEXT, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case TITLE:
                            errFooBetweenHeadAndBody(name);
                            pushHeadPointerOntoStack();
                            appendToCurrentNodeAndPushElement(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.RCDATA, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        case HEAD:
                            errStrayStartTag(name);
                            goto starttagloop_break;
                        default:
                            appendToCurrentNodeAndPushBodyElement();
                            mode = FRAMESET_OK;
                            continue;
                    }
                case AFTER_AFTER_BODY:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        default:
                            errStrayStartTag(name);
                            fatal();
                            mode = framesetOk ? FRAMESET_OK : IN_BODY;
                            continue;
                    }
                case AFTER_AFTER_FRAMESET:
                    switch (group) {
                        case HTML:
                            errStrayStartTag(name);
                            if (!fragment) {
                                addAttributesToHtml(attributes);
                                attributes = null; // CPP
                            }
                            goto starttagloop_break;
                        case NOFRAMES:
                            appendToCurrentNodeAndPushElementMayFoster(
                                    elementName,
                                    attributes);
                            originalMode = mode;
                            mode = TEXT;
                            tokenizer.setStateAndEndTagExpectation(
                                    Tokenizer.SCRIPT_DATA, elementName);
                            attributes = null; // CPP
                            goto starttagloop_break;
                        default:
                            errStrayStartTag(name);
                            goto starttagloop_break;
                    }
                case TEXT:
                    Debug.Assert(false);
                    goto starttagloop_break; // Avoid infinite loop if the assertion fails
            }
starttagloop_continue:
            { } // continue the loop
        }
starttagloop_break:
        if (selfClosing) {
            errSelfClosing();
        }
        if (attributes != HtmlAttributes.EMPTY_ATTRIBUTES) {
            Portability.delete(attributes);
        }
    }

    private bool isSpecialParentInForeign(StackNode<T> stackNode) {
        String ns = stackNode.ns;
        return ("http://www.w3.org/1999/xhtml" == ns)
                || (stackNode.isHtmlIntegrationPoint())
                || (("http://www.w3.org/1998/Math/MathML" == ns) && (stackNode.getGroup() == MI_MO_MN_MS_MTEXT));
    }

    private void checkMetaCharset(HtmlAttributes attributes) {
        String charset = attributes.getValue(AttributeName.CHARSET);
        if (charset != null) {
            if (tokenizer.internalEncodingDeclaration(charset)) {
                requestSuspension();
                return;
            }            
            return;
        }
        if (!Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                "content-type",
                attributes.getValue(AttributeName.HTTP_EQUIV))) {
            return;
        }
        String content = attributes.getValue(AttributeName.CONTENT);
        if (content != null) {
            String extract = TreeBuilderBase.extractCharsetFromContent(content);
            // remember not to return early without releasing the string
            if (extract != null) {
                if (tokenizer.internalEncodingDeclaration(extract)) {
                    requestSuspension();
                }                
            }
            Portability.releaseString(extract);
        }
    }

    public void endTag(ElementName elementName) {
        flushCharacters();
        needToDropLF = false;
        int eltPos;
        int group = elementName.getGroup();
        String name = elementName.name;
        for (;;) {
            if (isInForeign()) {
                if (stack[currentPtr].name != name) {
                    errEndTagDidNotMatchCurrentOpenElement(name, stack[currentPtr].popName);
                }
                eltPos = currentPtr;
                for (;;) {
                    if (stack[eltPos].name == name) {
                        while (currentPtr >= eltPos) {
                            pop();
                        }
                        goto endtagloop_break;
                    }
                    if (stack[--eltPos].ns == "http://www.w3.org/1999/xhtml") {
                        break;
                    }
                }
            }
            switch (mode) {
                case IN_ROW:
                    switch (group) {
                        case TR:
                            eltPos = findLastOrRoot(TreeBuilderBase.TR);
                            if (eltPos == 0) {
                                Debug.Assert(fragment);
                                errNoTableRowToClose();
                                goto endtagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE_BODY;
                            goto endtagloop_break;
                        case TABLE:
                            eltPos = findLastOrRoot(TreeBuilderBase.TR);
                            if (eltPos == 0) {
                                Debug.Assert(fragment);
                                errNoTableRowToClose();
                                goto endtagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE_BODY;
                            continue;
                        case TBODY_OR_THEAD_OR_TFOOT:
                            if (findLastInTableScope(name) == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            eltPos = findLastOrRoot(TreeBuilderBase.TR);
                            if (eltPos == 0) {
                                Debug.Assert(fragment);
                                errNoTableRowToClose();
                                goto endtagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE_BODY;
                            continue;
                        case BODY:
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case HTML:
                        case TD_OR_TH:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            // fall through to IN_TABLE
                            break;
                    }
                    goto case IN_TABLE_BODY;
                case IN_TABLE_BODY:
                    switch (group) {
                        case TBODY_OR_THEAD_OR_TFOOT:
                            eltPos = findLastOrRoot(name);
                            if (eltPos == 0) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE;
                            goto endtagloop_break;
                        case TABLE:
                            eltPos = findLastInTableScopeOrRootTbodyTheadTfoot();
                            if (eltPos == 0) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            clearStackBackTo(eltPos);
                            pop();
                            mode = IN_TABLE;
                            continue;
                        case BODY:
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case HTML:
                        case TD_OR_TH:
                        case TR:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            // fall through to IN_TABLE
                            break;
                    }
                    goto case IN_TABLE;
                case IN_TABLE:
                    switch (group) {
                        case TABLE:
                            eltPos = findLast("table");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            resetTheInsertionMode();
                            goto endtagloop_break;
                        case BODY:
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case HTML:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TD_OR_TH:
                        case TR:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            errStrayEndTag(name);
                            // fall through to IN_BODY
                            break;
                    }
                    goto case IN_CAPTION;
                case IN_CAPTION:
                    switch (group) {
                        case CAPTION:
                            eltPos = findLastInTableScope("caption");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                goto endtagloop_break;
                            }
                            generateImpliedEndTags();
                            if (errorHandler != null && currentPtr != eltPos) {
                                errUnclosedElements(eltPos, name);
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            clearTheListOfActiveFormattingElementsUpToTheLastMarker();
                            mode = IN_TABLE;
                            goto endtagloop_break;
                        case TABLE:
                            errTableClosedWhileCaptionOpen();
                            eltPos = findLastInTableScope("caption");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                goto endtagloop_break;
                            }
                            generateImpliedEndTags();
                            if (errorHandler != null && currentPtr != eltPos) {
                                errUnclosedElements(eltPos, name);
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            clearTheListOfActiveFormattingElementsUpToTheLastMarker();
                            mode = IN_TABLE;
                            continue;
                        case BODY:
                        case COL:
                        case COLGROUP:
                        case HTML:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TD_OR_TH:
                        case TR:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            // fall through to IN_BODY
                            break;
                    }
                    goto case IN_CELL;
                case IN_CELL:
                    switch (group) {
                        case TD_OR_TH:
                            eltPos = findLastInTableScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            generateImpliedEndTags();
                            if (errorHandler != null && !isCurrent(name)) {
                                errUnclosedElements(eltPos, name);
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            clearTheListOfActiveFormattingElementsUpToTheLastMarker();
                            mode = IN_ROW;
                            goto endtagloop_break;
                        case TABLE:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                            if (findLastInTableScope(name) == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            closeTheCell(findLastInTableScopeTdTh());
                            continue;
                        case BODY:
                        case CAPTION:
                        case COL:
                        case COLGROUP:
                        case HTML:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            // fall through to IN_BODY
                            break;
                    }
                    goto case FRAMESET_OK;
                case FRAMESET_OK:
                case IN_BODY:
                    switch (group) {
                        case BODY:
                            if (!isSecondOnStackBody()) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            Debug.Assert(currentPtr >= 1);
                            if (errorHandler != null) {
                                uncloseloop1: for (int i = 2; i <= currentPtr; i++) {
                                    switch (stack[i].getGroup()) {
                                        case DD_OR_DT:
                                        case LI:
                                        case OPTGROUP:
                                        case OPTION: // is this possible?
                                        case P:
                                        case RT_OR_RP:
                                        case TD_OR_TH:
                                        case TBODY_OR_THEAD_OR_TFOOT:
                                            break;
                                        default:
                                            errEndWithUnclosedElements(name);
                                            goto uncloseloop1;
                                    }
                                }
                            }
                            mode = AFTER_BODY;
                            goto endtagloop_break;
                        case HTML:
                            if (!isSecondOnStackBody()) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            if (errorHandler != null) {
                                uncloseloop2: for (int i = 0; i <= currentPtr; i++) {
                                    switch (stack[i].getGroup()) {
                                        case DD_OR_DT:
                                        case LI:
                                        case P:
                                        case TBODY_OR_THEAD_OR_TFOOT:
                                        case TD_OR_TH:
                                        case BODY:
                                        case HTML:
                                            break;
                                        default:
                                            errEndWithUnclosedElements(name);
                                            goto uncloseloop2;
                                    }
                                }
                            }
                            mode = AFTER_BODY;
                            continue;
                        case DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU:
                        case UL_OR_OL_OR_DL:
                        case PRE_OR_LISTING:
                        case FIELDSET:
                        case BUTTON:
                        case ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY:
                            eltPos = findLastInScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                            } else {
                                generateImpliedEndTags();
                                if (errorHandler != null && !isCurrent(name)) {
                                    errUnclosedElements(eltPos, name);
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                            }
                            goto endtagloop_break;
                        case FORM:
                            if (formPointer == null) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            formPointer = null;
                            eltPos = findLastInScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            generateImpliedEndTags();
                            if (errorHandler != null && !isCurrent(name)) {
                                errUnclosedElements(eltPos, name);
                            }
                            removeFromStack(eltPos);
                            goto endtagloop_break;
                        case P:
                            eltPos = findLastInButtonScope("p");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errNoElementToCloseButEndTagSeen("p");
                                // XXX Can the 'in foreign' case happen anymore?
                                if (isInForeign()) {
                                    errHtmlStartTagInForeignContext(name);
                                    while (stack[currentPtr].ns != "http://www.w3.org/1999/xhtml") {
                                        pop();
                                    }
                                }
                                appendVoidElementToCurrentMayFoster(
                                        elementName,
                                        HtmlAttributes.EMPTY_ATTRIBUTES);
                                goto endtagloop_break;
                            }
                            generateImpliedEndTagsExceptFor("p");
                            Debug.Assert(eltPos != TreeBuilderBase.NOT_FOUND_ON_STACK);
                            if (errorHandler != null && eltPos != currentPtr) {
                                errUnclosedElements(eltPos, name);
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            goto endtagloop_break;
                        case LI:
                            eltPos = findLastInListScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errNoElementToCloseButEndTagSeen(name);
                            } else {
                                generateImpliedEndTagsExceptFor(name);
                                if (errorHandler != null
                                        && eltPos != currentPtr) {
                                    errUnclosedElements(eltPos, name);
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                            }
                            goto endtagloop_break;
                        case DD_OR_DT:
                            eltPos = findLastInScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errNoElementToCloseButEndTagSeen(name);
                            } else {
                                generateImpliedEndTagsExceptFor(name);
                                if (errorHandler != null
                                        && eltPos != currentPtr) {
                                    errUnclosedElements(eltPos, name);
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                            }
                            goto endtagloop_break;
                        case H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6:
                            eltPos = findLastInScopeHn();
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                            } else {
                                generateImpliedEndTags();
                                if (errorHandler != null && !isCurrent(name)) {
                                    errUnclosedElements(eltPos, name);
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                            }
                            goto endtagloop_break;
                        case OBJECT:
                        case MARQUEE_OR_APPLET:
                            eltPos = findLastInScope(name);
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                errStrayEndTag(name);
                            } else {
                                generateImpliedEndTags();
                                if (errorHandler != null && !isCurrent(name)) {
                                    errUnclosedElements(eltPos, name);
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                                clearTheListOfActiveFormattingElementsUpToTheLastMarker();
                            }
                            goto endtagloop_break;
                        case BR:
                            errEndTagBr();
                            if (isInForeign()) {
                                errHtmlStartTagInForeignContext(name);
                                while (stack[currentPtr].ns != "http://www.w3.org/1999/xhtml") {
                                    pop();
                                }
                            }
                            reconstructTheActiveFormattingElements();
                            appendVoidElementToCurrentMayFoster(
                                    elementName,
                                    HtmlAttributes.EMPTY_ATTRIBUTES);
                            goto endtagloop_break;
                        case AREA_OR_WBR:
                        // CPPONLY: case MENUITEM:
                        case PARAM_OR_SOURCE_OR_TRACK:
                        case EMBED_OR_IMG:
                        case IMAGE:
                        case INPUT:
                        case KEYGEN: // XXX??
                        case HR:
                        case ISINDEX:
                        case IFRAME:
                        case NOEMBED: // XXX???
                        case NOFRAMES: // XXX??
                        case SELECT:
                        case TABLE:
                        case TEXTAREA: // XXX??
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        case NOSCRIPT:
                            if (scriptingEnabled) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            } else {
                                // fall through
                                goto case A;
                            }
                        case A:
                        case B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U:
                        case FONT:
                        case NOBR:
                            if (adoptionAgencyEndTag(name)) {
                                goto endtagloop_break;
                            }
                            goto default;
                            // else handle like any other tag
                        default:
                            if (isCurrent(name)) {
                                pop();
                                goto endtagloop_break;
                            }

                            eltPos = currentPtr;
                            for (;;) {
                                StackNode<T> node = stack[eltPos];
                                if (node.name == name) {
                                    generateImpliedEndTags();
                                    if (errorHandler != null
                                            && !isCurrent(name)) {
                                        errUnclosedElements(eltPos, name);
                                    }
                                    while (currentPtr >= eltPos) {
                                        pop();
                                    }
                                    goto endtagloop_break;
                                } else if (node.isSpecial()) {
                                    errStrayEndTag(name);
                                    goto endtagloop_break;
                                }
                                eltPos--;
                            }
                    }
                case IN_COLUMN_GROUP:
                    switch (group) {
                        case COLGROUP:
                            if (currentPtr == 0) {
                                Debug.Assert(fragment);
                                errGarbageInColgroup();
                                goto endtagloop_break;
                            }
                            pop();
                            mode = IN_TABLE;
                            goto endtagloop_break;
                        case COL:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                        default:
                            if (currentPtr == 0) {
                                Debug.Assert(fragment);
                                errGarbageInColgroup();
                                goto endtagloop_break;
                            }
                            pop();
                            mode = IN_TABLE;
                            continue;
                    }
                case IN_SELECT_IN_TABLE:
                    switch (group) {
                        case CAPTION:
                        case TABLE:
                        case TBODY_OR_THEAD_OR_TFOOT:
                        case TR:
                        case TD_OR_TH:
                            errEndTagSeenWithSelectOpen(name);
                            if (findLastInTableScope(name) != TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                eltPos = findLastInTableScope("select");
                                if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                    Debug.Assert(fragment);
                                    goto endtagloop_break; // http://www.w3.org/Bugs/Public/show_bug.cgi?id=8375
                                }
                                while (currentPtr >= eltPos) {
                                    pop();
                                }
                                resetTheInsertionMode();
                                continue;
                            } else {
                                goto endtagloop_break;
                            }
                        default:
                            // fall through to IN_SELECT
                            break;
                    }
                    goto case IN_SELECT;
                case IN_SELECT:
                    switch (group) {
                        case OPTION:
                            if (isCurrent("option")) {
                                pop();
                                goto endtagloop_break;
                            } else {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                        case OPTGROUP:
                            if (isCurrent("option")
                                    && "optgroup" == stack[currentPtr - 1].name) {
                                pop();
                            }
                            if (isCurrent("optgroup")) {
                                pop();
                            } else {
                                errStrayEndTag(name);
                            }
                            goto endtagloop_break;
                        case SELECT:
                            eltPos = findLastInTableScope("select");
                            if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            while (currentPtr >= eltPos) {
                                pop();
                            }
                            resetTheInsertionMode();
                            goto endtagloop_break;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case AFTER_BODY:
                    switch (group) {
                        case HTML:
                            if (fragment) {
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            } else {
                                mode = AFTER_AFTER_BODY;
                                goto endtagloop_break;
                            }
                        default:
                            errEndTagAfterBody();
                            mode = framesetOk ? FRAMESET_OK : IN_BODY;
                            continue;
                    }
                case IN_FRAMESET:
                    switch (group) {
                        case FRAMESET:
                            if (currentPtr == 0) {
                                Debug.Assert(fragment);
                                errStrayEndTag(name);
                                goto endtagloop_break;
                            }
                            pop();
                            if ((!fragment) && !isCurrent("frameset")) {
                                mode = AFTER_FRAMESET;
                            }
                            goto endtagloop_break;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case AFTER_FRAMESET:
                    switch (group) {
                        case HTML:
                            mode = AFTER_AFTER_FRAMESET;
                            goto endtagloop_break;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case INITIAL:
                    /*
                     * Parse error.
                     */
                    // [NOCPP[
                    switch (doctypeExpectation) {
                        case DoctypeExpectation.AUTO:
                            err("End tag seen without seeing a doctype first. Expected e.g. \u201C<!DOCTYPE html>\u201D.");
                            break;
                        case DoctypeExpectation.HTML:
                            // ]NOCPP]
                            errEndTagSeenWithoutDoctype();
                            // [NOCPP[
                            break;
                        case DoctypeExpectation.HTML401_STRICT:
                            err("End tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.HTML401_TRANSITIONAL:
                            err("End tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\u201D.");
                            break;
                        case DoctypeExpectation.NO_DOCTYPE_ERRORS:
                            break;
                    }
                    // ]NOCPP]
                    /*
                     * 
                     * Set the document to quirks mode.
                     */
                    documentModeInternal(DocumentMode.QUIRKS_MODE, null, null,
                            false);
                    /*
                     * Then, switch to the root element mode of the tree
                     * construction stage
                     */
                    mode = BEFORE_HTML;
                    /*
                     * and reprocess the current token.
                     */
                    continue;
                case BEFORE_HTML:
                    switch (group) {
                        case HEAD:
                        case BR:
                        case HTML:
                        case BODY:
                            /*
                             * Create an HTMLElement node with the tag name
                             * html, in the HTML namespace. Append it to the
                             * Document object.
                             */
                            appendHtmlElementToDocumentAndPush();
                            /* Switch to the main mode */
                            mode = BEFORE_HEAD;
                            /*
                             * reprocess the current token.
                             */
                            continue;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case BEFORE_HEAD:
                    switch (group) {
                        case HEAD:
                        case BR:
                        case HTML:
                        case BODY:
                            appendToCurrentNodeAndPushHeadElement(HtmlAttributes.EMPTY_ATTRIBUTES);
                            mode = IN_HEAD;
                            continue;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case IN_HEAD:
                    switch (group) {
                        case HEAD:
                            pop();
                            mode = AFTER_HEAD;
                            goto endtagloop_break;
                        case BR:
                        case HTML:
                        case BODY:
                            pop();
                            mode = AFTER_HEAD;
                            continue;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case IN_HEAD_NOSCRIPT:
                    switch (group) {
                        case NOSCRIPT:
                            pop();
                            mode = IN_HEAD;
                            goto endtagloop_break;
                        case BR:
                            errStrayEndTag(name);
                            pop();
                            mode = IN_HEAD;
                            continue;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case AFTER_HEAD:
                    switch (group) {
                        case HTML:
                        case BODY:
                        case BR:
                            appendToCurrentNodeAndPushBodyElement();
                            mode = FRAMESET_OK;
                            continue;
                        default:
                            errStrayEndTag(name);
                            goto endtagloop_break;
                    }
                case AFTER_AFTER_BODY:
                    errStrayEndTag(name);
                    mode = framesetOk ? FRAMESET_OK : IN_BODY;
                    continue;
                case AFTER_AFTER_FRAMESET:
                    errStrayEndTag(name);
                    mode = IN_FRAMESET;
                    continue;
                case TEXT:
                    // XXX need to manage insertion point here
                    pop();
                    if (originalMode == AFTER_HEAD) {
                        silentPop();
                    }
                    mode = originalMode;
                    goto endtagloop_break;
            }
        } // endtagloop
    endtagloop_break: { }
    }

    private int findLastInTableScopeOrRootTbodyTheadTfoot() {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].getGroup() == TreeBuilderBase.TBODY_OR_THEAD_OR_TFOOT) {
                return i;
            }
        }
        return 0;
    }

    private int findLast(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private int findLastInTableScope(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            } else if (stack[i].name == "table") {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private int findLastInButtonScope(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            } else if (stack[i].isScoping() || stack[i].name == "button") {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private int findLastInScope(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            } else if (stack[i].isScoping()) {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private int findLastInListScope(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            } else if (stack[i].isScoping() || stack[i].name == "ul" || stack[i].name == "ol") {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }
    
    private int findLastInScopeHn() {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].getGroup() == TreeBuilderBase.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6) {
                return i;
            } else if (stack[i].isScoping()) {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private void generateImpliedEndTagsExceptFor(String name) {
        for (;;) {
            StackNode<T> node = stack[currentPtr];
            switch (node.getGroup()) {
                case P:
                case LI:
                case DD_OR_DT:
                case OPTION:
                case OPTGROUP:
                case RT_OR_RP:
                    if (node.name == name) {
                        return;
                    }
                    pop();
                    continue;
                default:
                    return;
            }
        }
    }

    private void generateImpliedEndTags() {
        for (;;) {
            switch (stack[currentPtr].getGroup()) {
                case P:
                case LI:
                case DD_OR_DT:
                case OPTION:
                case OPTGROUP:
                case RT_OR_RP:
                    pop();
                    continue;
                default:
                    return;
            }
        }
    }

    private bool isSecondOnStackBody() {
        return currentPtr >= 1 && stack[1].getGroup() == TreeBuilderBase.BODY;
    }

    private void documentModeInternal(DocumentMode m, String publicIdentifier, String systemIdentifier, bool html4SpecificAdditionalErrorChecks) {
        quirks = (m == DocumentMode.QUIRKS_MODE);
        if (documentModeHandler != null) {
            documentModeHandler.documentMode(
                    m
                    // [NOCPP[
                    , publicIdentifier, systemIdentifier,
                    html4SpecificAdditionalErrorChecks
            // ]NOCPP]
            );
        }
        // [NOCPP[
        documentMode(m, publicIdentifier, systemIdentifier,
                html4SpecificAdditionalErrorChecks);
        // ]NOCPP]
    }

    private bool isAlmostStandards(String publicIdentifier, String systemIdentifier) {
        if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                "-//w3c//dtd xhtml 1.0 transitional//en", publicIdentifier)) {
            return true;
        }
        if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                "-//w3c//dtd xhtml 1.0 frameset//en", publicIdentifier)) {
            return true;
        }
        if (systemIdentifier != null) {
            if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                    "-//w3c//dtd html 4.01 transitional//en", publicIdentifier)) {
                return true;
            }
            if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                    "-//w3c//dtd html 4.01 frameset//en", publicIdentifier)) {
                return true;
            }
        }
        return false;
    }

    private bool isQuirky( String name, String publicIdentifier, String systemIdentifier, bool forceQuirks) {
        if (forceQuirks) {
            return true;
        }
        if (name != HTML_LOCAL) {
            return true;
        }
        if (publicIdentifier != null) {
            for (int i = 0; i < TreeBuilderBase.QUIRKY_PUBLIC_IDS.Length; i++) {
                if (Portability.lowerCaseLiteralIsPrefixOfIgnoreAsciiCaseString(
                        TreeBuilderBase.QUIRKY_PUBLIC_IDS[i], publicIdentifier)) {
                    return true;
                }
            }
            if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                    "-//w3o//dtd w3 html strict 3.0//en//", publicIdentifier)
                    || Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                            "-/w3c/dtd html 4.0 transitional/en",
                            publicIdentifier)
                    || Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                            "html", publicIdentifier)) {
                return true;
            }
        }
        if (systemIdentifier == null) {
            if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                    "-//w3c//dtd html 4.01 transitional//en", publicIdentifier)) {
                return true;
            } else if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                    "-//w3c//dtd html 4.01 frameset//en", publicIdentifier)) {
                return true;
            }
        } else if (Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd",
                systemIdentifier)) {
            return true;
        }
        return false;
    }

    private void closeTheCell(int eltPos) {
        generateImpliedEndTags();
        if (errorHandler != null && eltPos != currentPtr) {
            errUnclosedElementsCell(eltPos);
        }
        while (currentPtr >= eltPos) {
            pop();
        }
        clearTheListOfActiveFormattingElementsUpToTheLastMarker();
        mode = IN_ROW;
        return;
    }

    private int findLastInTableScopeTdTh() {
        for (int i = currentPtr; i > 0; i--) {
             String name = stack[i].name;
            if ("td" == name || "th" == name) {
                return i;
            } else if (name == "table") {
                return TreeBuilderBase.NOT_FOUND_ON_STACK;
            }
        }
        return TreeBuilderBase.NOT_FOUND_ON_STACK;
    }

    private void clearStackBackTo(int eltPos) {
        while (currentPtr > eltPos) { // > not >= intentional
            pop();
        }
    }

    private void resetTheInsertionMode() {
        StackNode<T> node;
        String name;
        String ns;
        for (int i = currentPtr; i >= 0; i--) {
            node = stack[i];
            name = node.name;
            ns = node.ns;
            if (i == 0) {
                if (!(contextNamespace == "http://www.w3.org/1999/xhtml" && (contextName == "td" || contextName == "th"))) {
                    name = contextName;
                    ns = contextNamespace;
                } else {
                    mode = framesetOk ? FRAMESET_OK : IN_BODY; // XXX from Hixie's email
                    return;
                }
            }
            if ("select" == name) {
                mode = IN_SELECT;
                return;
            } else if ("td" == name || "th" == name) {
                mode = IN_CELL;
                return;
            } else if ("tr" == name) {
                mode = IN_ROW;
                return;
            } else if ("tbody" == name || "thead" == name || "tfoot" == name) {
                mode = IN_TABLE_BODY;
                return;
            } else if ("caption" == name) {
                mode = IN_CAPTION;
                return;
            } else if ("colgroup" == name) {
                mode = IN_COLUMN_GROUP;
                return;
            } else if ("table" == name) {
                mode = IN_TABLE;
                return;
            } else if ("http://www.w3.org/1999/xhtml" != ns) {
                mode = framesetOk ? FRAMESET_OK : IN_BODY;
                return;
            } else if ("head" == name) {
                mode = framesetOk ? FRAMESET_OK : IN_BODY; // really
                return;
            } else if ("body" == name) {
                mode = framesetOk ? FRAMESET_OK : IN_BODY;
                return;
            } else if ("frameset" == name) {
                mode = IN_FRAMESET;
                return;
            } else if ("html" == name) {
                if (headPointer == null) {
                    mode = BEFORE_HEAD;
                } else {
                    mode = AFTER_HEAD;
                }
                return;
            } else if (i == 0) {
                mode = framesetOk ? FRAMESET_OK : IN_BODY;
                return;
            }
        }
    }

    /**
     * @throws SAXException
     * 
     */
    private void implicitlyCloseP() {
        int eltPos = findLastInButtonScope("p");
        if (eltPos == TreeBuilderBase.NOT_FOUND_ON_STACK) {
            return;
        }
        generateImpliedEndTagsExceptFor("p");
        if (errorHandler != null && eltPos != currentPtr) {
            errUnclosedElementsImplied(eltPos, "p");
        }
        while (currentPtr >= eltPos) {
            pop();
        }
    }

    private bool clearLastStackSlot() {
        stack[currentPtr] = null;
        return true;
    }

    private bool clearLastListSlot() {
        listOfActiveFormattingElements[listPtr] = null;
        return true;
    }

    private void push(StackNode<T> node) {
        currentPtr++;
        if (currentPtr == stack.Length) {
            StackNode<T>[] newStack = new StackNode<T>[stack.Length + 64];
            Array.Copy(stack, 0, newStack, 0, stack.Length);
            stack = newStack;
        }
        stack[currentPtr] = node;
        elementPushed(node.ns, node.popName, node.node);
    }

    private void silentPush(StackNode<T> node) {
        currentPtr++;
        if (currentPtr == stack.Length) {
            StackNode<T>[] newStack = new StackNode<T>[stack.Length + 64];
            Array.Copy(stack, 0, newStack, 0, stack.Length);
            stack = newStack;
        }
        stack[currentPtr] = node;
    }

    private void append(StackNode<T> node) {
        listPtr++;
        if (listPtr == listOfActiveFormattingElements.Length) {
            StackNode<T>[] newList = new StackNode<T>[listOfActiveFormattingElements.Length + 64];
            Array.Copy(listOfActiveFormattingElements, 0, newList, 0, listOfActiveFormattingElements.Length);
            listOfActiveFormattingElements = newList;
        }
        listOfActiveFormattingElements[listPtr] = node;
    }

    private void insertMarker() {
        append(null);
    }

    private void clearTheListOfActiveFormattingElementsUpToTheLastMarker() {
        while (listPtr > -1) {
            if (listOfActiveFormattingElements[listPtr] == null) {
                --listPtr;
                return;
            }
            listOfActiveFormattingElements[listPtr].release();
            --listPtr;
        }
    }

    private bool isCurrent( String name) {
        return name == stack[currentPtr].name;
    }

    private void removeFromStack(int pos) {
        if (currentPtr == pos) {
            pop();
        } else {
            fatal();
            stack[pos].release();
            Array.Copy(stack, pos + 1, stack, pos, currentPtr - pos);
            Debug.Assert(clearLastStackSlot());
            currentPtr--;
        }
    }

    private void removeFromStack(StackNode<T> node) {
        if (stack[currentPtr] == node) {
            pop();
        } else {
            int pos = currentPtr - 1;
            while (pos >= 0 && stack[pos] != node) {
                pos--;
            }
            if (pos == -1) {
                // dead code?
                return;
            }
            fatal();
            node.release();
            Array.Copy(stack, pos + 1, stack, pos, currentPtr - pos);
            currentPtr--;
        }
    }

    private void removeFromListOfActiveFormattingElements(int pos) {
        Debug.Assert(listOfActiveFormattingElements[pos] != null);
        listOfActiveFormattingElements[pos].release();
        if (pos == listPtr) {
            Debug.Assert(clearLastListSlot());
            listPtr--;
            return;
        }
        Debug.Assert(pos < listPtr);
        Array.Copy(listOfActiveFormattingElements, pos + 1, listOfActiveFormattingElements, pos, listPtr - pos);
        Debug.Assert(clearLastListSlot());
        listPtr--;
    }

    private bool adoptionAgencyEndTag(String name) {
        // If you crash around here, perhaps some stack node variable claimed to
        // be a weak ref isn't.
        for (int i = 0; i < 8; ++i) {
            int formattingEltListPos = listPtr;
            while (formattingEltListPos > -1) {
                StackNode<T> listNode = listOfActiveFormattingElements[formattingEltListPos]; // weak
                                                                                              // ref
                if (listNode == null) {
                    formattingEltListPos = -1;
                    break;
                } else if (listNode.name == name) {
                    break;
                }
                formattingEltListPos--;
            }
            if (formattingEltListPos == -1) {
                return false;
            }
            StackNode<T> formattingElt = listOfActiveFormattingElements[formattingEltListPos]; // this
            // *looks*
            // like
            // a
            // weak
            // ref
            // to
            // the
            // list
            // of
            // formatting
            // elements
            int formattingEltStackPos = currentPtr;
            bool inScope = true;
            while (formattingEltStackPos > -1) {
                StackNode<T> node = stack[formattingEltStackPos]; // weak ref
                if (node == formattingElt) {
                    break;
                } else if (node.isScoping()) {
                    inScope = false;
                }
                formattingEltStackPos--;
            }
            if (formattingEltStackPos == -1) {
                errNoElementToCloseButEndTagSeen(name);
                removeFromListOfActiveFormattingElements(formattingEltListPos);
                return true;
            }
            if (!inScope) {
                errNoElementToCloseButEndTagSeen(name);
                return true;
            }
            // stackPos now points to the formatting element and it is in scope
            if (formattingEltStackPos != currentPtr) {
                errEndTagViolatesNestingRules(name);
            }
            int furthestBlockPos = formattingEltStackPos + 1;
            while (furthestBlockPos <= currentPtr) {
                StackNode<T> node = stack[furthestBlockPos]; // weak ref
                if (node.isSpecial()) {
                    break;
                }
                furthestBlockPos++;
            }
            if (furthestBlockPos > currentPtr) {
                // no furthest block
                while (currentPtr >= formattingEltStackPos) {
                    pop();
                }
                removeFromListOfActiveFormattingElements(formattingEltListPos);
                return true;
            }
            StackNode<T> commonAncestor = stack[formattingEltStackPos - 1]; // weak
            // ref
            StackNode<T> furthestBlock = stack[furthestBlockPos]; // weak ref
            // detachFromParent(furthestBlock.node); XXX AAA CHANGE
            int bookmark = formattingEltListPos;
            int nodePos = furthestBlockPos;
            StackNode<T> lastNode = furthestBlock; // weak ref
            for (int j = 0; j < 3; ++j) {
                nodePos--;
                StackNode<T> node = stack[nodePos]; // weak ref
                int nodeListPos = findInListOfActiveFormattingElements(node);
                if (nodeListPos == -1) {
                    Debug.Assert(formattingEltStackPos < nodePos);
                    Debug.Assert(bookmark < nodePos);
                    Debug.Assert(furthestBlockPos > nodePos);
                    removeFromStack(nodePos); // node is now a bad pointer in
                    // C++
                    furthestBlockPos--;
                    continue;
                }
                // now node is both on stack and in the list
                if (nodePos == formattingEltStackPos) {
                    break;
                }
                if (nodePos == furthestBlockPos) {
                    bookmark = nodeListPos + 1;
                }
                // if (hasChildren(node.node)) { XXX AAA CHANGE
                Debug.Assert(node == listOfActiveFormattingElements[nodeListPos]);
                Debug.Assert(node == stack[nodePos]);
                T clone = createElement("http://www.w3.org/1999/xhtml",
                        node.name, node.attributes.cloneAttributes(null));
                StackNode<T> newNode = new StackNode<T>(node.getFlags(), node.ns,
                        node.name, clone, node.popName, node.attributes
                        // [NOCPP[
                        , node.getLocator()
                // ]NOCPP]       
                ); // creation
                // ownership
                // goes
                // to
                // stack
                node.dropAttributes(); // adopt ownership to newNode
                stack[nodePos] = newNode;
                newNode.retain(); // retain for list
                listOfActiveFormattingElements[nodeListPos] = newNode;
                node.release(); // release from stack
                node.release(); // release from list
                node = newNode;
                // } XXX AAA CHANGE
                detachFromParent(lastNode.node);
                appendElement(lastNode.node, node.node);
                lastNode = node;
            }
            if (commonAncestor.isFosterParenting()) {
                fatal();
                detachFromParent(lastNode.node);
                insertIntoFosterParent(lastNode.node);
            } else {
                detachFromParent(lastNode.node);
                appendElement(lastNode.node, commonAncestor.node);
            }
            T clone = createElement("http://www.w3.org/1999/xhtml",
                    formattingElt.name,
                    formattingElt.attributes.cloneAttributes(null));
            StackNode<T> formattingClone = new StackNode<T>(
                    formattingElt.getFlags(), formattingElt.ns,
                    formattingElt.name, clone, formattingElt.popName,
                    formattingElt.attributes
                    // [NOCPP[
                    , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
            // ]NOCPP]
            ); // Ownership
            // transfers
            // to
            // stack
            // below
            formattingElt.dropAttributes(); // transfer ownership to
                                            // formattingClone
            appendChildrenToNewParent(furthestBlock.node, clone);
            appendElement(clone, furthestBlock.node);
            removeFromListOfActiveFormattingElements(formattingEltListPos);
            insertIntoListOfActiveFormattingElements(formattingClone, bookmark);
            Debug.Assert(formattingEltStackPos < furthestBlockPos);
            removeFromStack(formattingEltStackPos);
            // furthestBlockPos is now off by one and points to the slot after
            // it
            insertIntoStack(formattingClone, furthestBlockPos);
        }
        return true;
    }

    private void insertIntoStack(StackNode<T> node, int position) {
        Debug.Assert(currentPtr + 1 < stack.Length);
        Debug.Assert(position <= currentPtr + 1);
        if (position == currentPtr + 1) {
            push(node);
        } else {
            Array.Copy(stack, position, stack, position + 1, (currentPtr - position) + 1);
            currentPtr++;
            stack[position] = node;
        }
    }

    private void insertIntoListOfActiveFormattingElements(StackNode<T> formattingClone, int bookmark) {
        formattingClone.retain();
        Debug.Assert(listPtr + 1 < listOfActiveFormattingElements.Length);
        if (bookmark <= listPtr) {
            Array.Copy(listOfActiveFormattingElements, bookmark, listOfActiveFormattingElements, bookmark + 1, (listPtr - bookmark) + 1);
        }
        listPtr++;
        listOfActiveFormattingElements[bookmark] = formattingClone;
    }

    private int findInListOfActiveFormattingElements(StackNode<T> node) {
        for (int i = listPtr; i >= 0; i--) {
            if (node == listOfActiveFormattingElements[i]) {
                return i;
            }
        }
        return -1;
    }

    private int findInListOfActiveFormattingElementsContainsBetweenEndAndLastMarker(String name) {
        for (int i = listPtr; i >= 0; i--) {
            StackNode<T> node = listOfActiveFormattingElements[i];
            if (node == null) {
                return -1;
            } else if (node.name == name) {
                return i;
            }
        }
        return -1;
    }


    private void maybeForgetEarlierDuplicateFormattingElement(String name, HtmlAttributes attributes) {
        int candidate = -1;
        int count = 0;
        for (int i = listPtr; i >= 0; i--) {
            StackNode<T> node = listOfActiveFormattingElements[i];
            if (node == null) {
                break;
            }
            if (node.name == name && node.attributes.equalsAnother(attributes)) {
                candidate = i;
                ++count;
            }
        }
        if (count >= 3) {
            removeFromListOfActiveFormattingElements(candidate);
        }
    }
    
    private int findLastOrRoot(String name) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].name == name) {
                return i;
            }
        }
        return 0;
    }

    private int findLastOrRoot(int group) {
        for (int i = currentPtr; i > 0; i--) {
            if (stack[i].getGroup() == group) {
                return i;
            }
        }
        return 0;
    }

    /**
     * Attempt to add attribute to the body element.
     * @param attributes the attributes
     * @return <code>true</code> iff the attributes were added
     * @throws SAXException
     */
    private bool addAttributesToBody(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        if (currentPtr >= 1) {
            StackNode<T> body = stack[1];
            if (body.getGroup() == TreeBuilderBase.BODY) {
                addAttributesToElement(body.node, attributes);
                return true;
            }
        }
        return false;
    }

    private void addAttributesToHtml(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        addAttributesToElement(stack[0].node, attributes);
    }

    private void pushHeadPointerOntoStack() {
        Debug.Assert(headPointer != null);
        Debug.Assert(!fragment);
        Debug.Assert(mode == AFTER_HEAD);
        fatal();
        silentPush(new StackNode<T>(ElementName.HEAD, headPointer
        // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        ));
    }

    /**
     * @throws SAXException
     * 
     */
    private void reconstructTheActiveFormattingElements() {
        if (listPtr == -1) {
            return;
        }
        StackNode<T> mostRecent = listOfActiveFormattingElements[listPtr];
        if (mostRecent == null || isInStack(mostRecent)) {
            return;
        }
        int entryPos = listPtr;
        for (;;) {
            entryPos--;
            if (entryPos == -1) {
                break;
            }
            if (listOfActiveFormattingElements[entryPos] == null) {
                break;
            }
            if (isInStack(listOfActiveFormattingElements[entryPos])) {
                break;
            }
        }
        while (entryPos < listPtr) {
            entryPos++;
            StackNode<T> entry = listOfActiveFormattingElements[entryPos];
            T clone = createElement("http://www.w3.org/1999/xhtml", entry.name,
                    entry.attributes.cloneAttributes(null));
            StackNode<T> entryClone = new StackNode<T>(entry.getFlags(),
                    entry.ns, entry.name, clone, entry.popName,
                    entry.attributes
                    // [NOCPP[
                    , entry.getLocator()
            // ]NOCPP]
            );
            entry.dropAttributes(); // transfer ownership to entryClone
            StackNode<T> currentNode = stack[currentPtr];
            if (currentNode.isFosterParenting()) {
                insertIntoFosterParent(clone);
            } else {
                appendElement(clone, currentNode.node);
            }
            push(entryClone);
            // stack takes ownership of the local variable
            listOfActiveFormattingElements[entryPos] = entryClone;
            // overwriting the old entry on the list, so release & retain
            entry.release();
            entryClone.retain();
        }
    }

    private void insertIntoFosterParent(T child) {
        int eltPos = findLastOrRoot(TreeBuilderBase.TABLE);
        StackNode<T> node = stack[eltPos];
        T elt = node.node;
        if (eltPos == 0) {
            appendElement(child, elt);
            return;
        }
        insertFosterParentedChild(child, elt, stack[eltPos - 1].node);
    }

    private bool isInStack(StackNode<T> node) {
        for (int i = currentPtr; i >= 0; i--) {
            if (stack[i] == node) {
                return true;
            }
        }
        return false;
    }

    private void pop() {
        StackNode<T> node = stack[currentPtr];
        Debug.Assert(clearLastStackSlot());
        currentPtr--;
        elementPopped(node.ns, node.popName, node.node);
        node.release();
    }

    private void silentPop() {
        StackNode<T> node = stack[currentPtr];
        Debug.Assert(clearLastStackSlot());
        currentPtr--;
        node.release();
    }

    private void popOnEof() {
        StackNode<T> node = stack[currentPtr];
        Debug.Assert(clearLastStackSlot());
        currentPtr--;
        markMalformedIfScript(node.node);
        elementPopped(node.ns, node.popName, node.node);
        node.release();
    }

    // [NOCPP[
    private void checkAttributes(HtmlAttributes attributes, String ns) {
        if (errorHandler != null) {
            int len = attributes.getXmlnsLength();
            for (int i = 0; i < len; i++) {
                AttributeName name = attributes.getXmlnsAttributeName(i);
                if (name == AttributeName.XMLNS) {
                    if (html4) {
                        err("Attribute \u201Cxmlns\u201D not allowed here. (HTML4-only error.)");
                    } else {
                        String xmlns = attributes.getXmlnsValue(i);
                        if (!ns.Equals(xmlns)) {
                            err("Bad value \u201C"
                                    + xmlns
                                    + "\u201D for the attribute \u201Cxmlns\u201D (only \u201C"
                                    + ns + "\u201D permitted here).");
                            switch (namePolicy) {
                                case XmlViolationPolicy.ALTER_INFOSET:
                                    // fall through
                                case XmlViolationPolicy.ALLOW:
                                    warn("Attribute \u201Cxmlns\u201D is not serializable as XML 1.0.");
                                    break;
                                case XmlViolationPolicy.FATAL:
                                    fatal("Attribute \u201Cxmlns\u201D is not serializable as XML 1.0.");
                                    break;
                            }
                        }
                    }
                } else if (ns != "http://www.w3.org/1999/xhtml"
                        && name == AttributeName.XMLNS_XLINK) {
                    String xmlns = attributes.getXmlnsValue(i);
                    if (!"http://www.w3.org/1999/xlink".Equals(xmlns)) {
                        err("Bad value \u201C"
                                + xmlns
                                + "\u201D for the attribute \u201Cxmlns:link\u201D (only \u201Chttp://www.w3.org/1999/xlink\u201D permitted here).");
                        switch (namePolicy) {
                            case XmlViolationPolicy.ALTER_INFOSET:
                                // fall through
                            case XmlViolationPolicy.ALLOW:
                                warn("Attribute \u201Cxmlns:xlink\u201D with a value other than \u201Chttp://www.w3.org/1999/xlink\u201D is not serializable as XML 1.0 without changing document semantics.");
                                break;
                            case XmlViolationPolicy.FATAL:
                                fatal("Attribute \u201Cxmlns:xlink\u201D with a value other than \u201Chttp://www.w3.org/1999/xlink\u201D is not serializable as XML 1.0 without changing document semantics.");
                                break;
                        }
                    }
                } else {
                    err("Attribute \u201C" + attributes.getXmlnsLocalName(i)
                            + "\u201D not allowed here.");
                    switch (namePolicy) {
                        case XmlViolationPolicy.ALTER_INFOSET:
                            // fall through
                        case XmlViolationPolicy.ALLOW:
                            warn("Attribute with the local name \u201C"
                                    + attributes.getXmlnsLocalName(i)
                                    + "\u201D is not serializable as XML 1.0.");
                            break;
                        case XmlViolationPolicy.FATAL:
                            fatal("Attribute with the local name \u201C"
                                    + attributes.getXmlnsLocalName(i)
                                    + "\u201D is not serializable as XML 1.0.");
                            break;
                    }
                }
            }
        }
        attributes.processNonNcNames(this, namePolicy);
    }

    private String checkPopName(String name) {
        if (NCName.isNCName(name)) {
            return name;
        } else {
            switch (namePolicy) {
                case XmlViolationPolicy.ALLOW:
                    warn("Element name \u201C" + name
                            + "\u201D cannot be represented as XML 1.0.");
                    return name;
                case XmlViolationPolicy.ALTER_INFOSET:
                    warn("Element name \u201C" + name
                            + "\u201D cannot be represented as XML 1.0.");
                    return NCName.escapeName(name);
                case XmlViolationPolicy.FATAL:
                    fatal("Element name \u201C" + name
                            + "\u201D cannot be represented as XML 1.0.");
                    break;
            }
        }
        return null; // keep compiler happy
    }

    // ]NOCPP]

    private void appendHtmlElementToDocumentAndPush(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        T elt = createHtmlElementSetAsRoot(attributes);
        StackNode<T> node = new StackNode<T>(ElementName.HTML,
                elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendHtmlElementToDocumentAndPush() {
        appendHtmlElementToDocumentAndPush(tokenizer.emptyAttributes());
    }

    private void appendToCurrentNodeAndPushHeadElement(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1999/xhtml", "head",
                attributes);
        appendElement(elt, stack[currentPtr].node);
        headPointer = elt;
        StackNode<T> node = new StackNode<T>(ElementName.HEAD,
                elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendToCurrentNodeAndPushBodyElement(HtmlAttributes attributes) {
        appendToCurrentNodeAndPushElement(ElementName.BODY, attributes);
    }

    private void appendToCurrentNodeAndPushBodyElement() {
        appendToCurrentNodeAndPushBodyElement(tokenizer.emptyAttributes());
    }

    private void appendToCurrentNodeAndPushFormElementMayFoster(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1999/xhtml", "form",
                attributes);
        formPointer = elt;
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        StackNode<T> node = new StackNode<T>(ElementName.FORM,
                elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendToCurrentNodeAndPushFormattingElementMayFoster(ElementName elementName, HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        // This method can't be called for custom elements
        T elt = createElement("http://www.w3.org/1999/xhtml", elementName.name, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        StackNode<T> node = new StackNode<T>(elementName, elt, attributes.cloneAttributes(null)
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
        append(node);
        node.retain(); // append doesn't retain itself
    }

    private void appendToCurrentNodeAndPushElement(ElementName elementName, HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        // This method can't be called for custom elements
        T elt = createElement("http://www.w3.org/1999/xhtml", elementName.name, attributes);
        appendElement(elt, stack[currentPtr].node);
        StackNode<T> node = new StackNode<T>(elementName, elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendToCurrentNodeAndPushElementMayFoster(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.name;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1999/xhtml", popName, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        StackNode<T> node = new StackNode<T>(elementName, elt, popName
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendToCurrentNodeAndPushElementMayFosterMathML(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.name;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1998/Math/MathML");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1998/Math/MathML", popName,
                attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        bool markAsHtmlIntegrationPoint = false;
        if (ElementName.ANNOTATION_XML == elementName
                && annotationXmlEncodingPermitsHtml(attributes)) {
            markAsHtmlIntegrationPoint = true;
        }
        StackNode<T> node = new StackNode<T>(elementName, elt, popName,
                markAsHtmlIntegrationPoint
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private bool annotationXmlEncodingPermitsHtml(HtmlAttributes attributes) {
        String encoding = attributes.getValue(AttributeName.ENCODING);
        if (encoding == null) {
            return false;
        }
        return Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                "application/xhtml+xml", encoding)
                || Portability.lowerCaseLiteralEqualsIgnoreAsciiCaseString(
                        "text/html", encoding);
    }

    private void appendToCurrentNodeAndPushElementMayFosterSVG(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.camelCaseName;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/2000/svg");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/2000/svg", popName, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        StackNode<T> node = new StackNode<T>(elementName, popName, elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendToCurrentNodeAndPushElementMayFoster(ElementName elementName, HtmlAttributes attributes, T form) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        // Can't be called for custom elements
        T elt = createElement("http://www.w3.org/1999/xhtml", elementName.name, attributes, fragment ? null
                : form);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        StackNode<T> node = new StackNode<T>(elementName, elt
                // [NOCPP[
                , errorHandler == null ? null : new TaintableLocatorImpl(tokenizer)
        // ]NOCPP]
        );
        push(node);
    }

    private void appendVoidElementToCurrentMayFoster(String name, HtmlAttributes attributes, T form) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        // Can't be called for custom elements
        T elt = createElement("http://www.w3.org/1999/xhtml", name, attributes, fragment ? null : form);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        elementPushed("http://www.w3.org/1999/xhtml", name, elt);
        elementPopped("http://www.w3.org/1999/xhtml", name, elt);
    }

    private void appendVoidElementToCurrentMayFoster(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.name;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1999/xhtml", popName, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        elementPushed("http://www.w3.org/1999/xhtml", popName, elt);
        elementPopped("http://www.w3.org/1999/xhtml", popName, elt);
    }

    private void appendVoidElementToCurrentMayFosterSVG(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.camelCaseName;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/2000/svg");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/2000/svg", popName, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        elementPushed("http://www.w3.org/2000/svg", popName, elt);
        elementPopped("http://www.w3.org/2000/svg", popName, elt);
    }

    private void appendVoidElementToCurrentMayFosterMathML(ElementName elementName, HtmlAttributes attributes) {
        String popName = elementName.name;
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1998/Math/MathML");
        if (elementName.isCustom()) {
            popName = checkPopName(popName);
        }
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1998/Math/MathML", popName, attributes);
        StackNode<T> current = stack[currentPtr];
        if (current.isFosterParenting()) {
            fatal();
            insertIntoFosterParent(elt);
        } else {
            appendElement(elt, current.node);
        }
        elementPushed("http://www.w3.org/1998/Math/MathML", popName, elt);
        elementPopped("http://www.w3.org/1998/Math/MathML", popName, elt);
    }

    private void appendVoidElementToCurrent(String name, HtmlAttributes attributes, T form) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        // Can't be called for custom elements
        T elt = createElement("http://www.w3.org/1999/xhtml", name, attributes, fragment ? null : form);
        StackNode<T> current = stack[currentPtr];
        appendElement(elt, current.node);
        elementPushed("http://www.w3.org/1999/xhtml", name, elt);
        elementPopped("http://www.w3.org/1999/xhtml", name, elt);
    }

    private void appendVoidFormToCurrent(HtmlAttributes attributes) {
        // [NOCPP[
        checkAttributes(attributes, "http://www.w3.org/1999/xhtml");
        // ]NOCPP]
        T elt = createElement("http://www.w3.org/1999/xhtml", "form",
                attributes);
        formPointer = elt;
        // ownership transferred to form pointer
        StackNode<T> current = stack[currentPtr];
        appendElement(elt, current.node);
        elementPushed("http://www.w3.org/1999/xhtml", "form", elt);
        elementPopped("http://www.w3.org/1999/xhtml", "form", elt);
    }

    // [NOCPP[
    
    private void accumulateCharactersForced(char[] buf, int start, int length) {
        int newLen = charBufferLen + length;
        if (newLen > charBuffer.Length) {
            char[] newBuf = new char[newLen];
            Array.Copy(charBuffer, 0, newBuf, 0, charBufferLen);
            charBuffer = newBuf;
        }
        Array.Copy(buf, start, charBuffer, charBufferLen, length);
        charBufferLen = newLen;
    }
    
    // ]NOCPP]
    
    protected virtual void accumulateCharacters(char[] buf, int start, int length) {
        appendCharacters(stack[currentPtr].node, buf, start, length);
    }

    // ------------------------------- //

    protected void requestSuspension() {
        tokenizer.requestSuspension();
    }

    protected abstract T createElement(String ns,  String name, HtmlAttributes attributes);

    protected T createElement(String ns, String name, HtmlAttributes attributes, T form) {
        return createElement("http://www.w3.org/1999/xhtml", name, attributes);
    }

    protected abstract T createHtmlElementSetAsRoot(HtmlAttributes attributes);

    protected abstract void detachFromParent(T element);

    protected abstract bool hasChildren(T element);

    protected abstract void appendElement(T child, T newParent);

    protected abstract void appendChildrenToNewParent(T oldParent, T newParent);

    protected abstract void insertFosterParentedChild(T child, T table, T stackParent);

    protected abstract void insertFosterParentedCharacters(char[] buf, int start, int length, T table, T stackParent);

    protected abstract void appendCharacters(T parent, char[] buf, int start, int length);

    protected abstract void appendIsindexPrompt(T parent);
    
    protected abstract void appendComment(T parent, char[] buf, int start, int length);

    protected abstract void appendCommentToDocument(char[] buf, int start, int length);

    protected abstract void addAttributesToElement(T element, HtmlAttributes attributes);

    protected void markMalformedIfScript(T elt) {

    }

    protected void start(bool fragmentMode) {

    }

    protected void end() {

    }

    protected void appendDoctypeToDocument(String name, String publicIdentifier, String systemIdentifier) {

    }

    protected void elementPushed(String ns, String name, T node) {

    }

    protected void elementPopped(String ns, String name, T node) {

    }

    // [NOCPP[

    protected void documentMode(DocumentMode m, String publicIdentifier, String systemIdentifier, bool html4SpecificAdditionalErrorChecks) {

    }

    /**
     * @see nu.validator.htmlparser.common.TokenHandler#wantsComments()
     */
    public bool wantsComments() {
        return wantingComments;
    }

    public void setIgnoringComments(bool ignoreComments) {
        wantingComments = !ignoreComments;
    }

    /**
     * Sets the errorHandler.
     * 
     * @param errorHandler
     *            the errorHandler to set
     */
    public void setErrorHandler(ErrorHandler errorHandler) {
        this.errorHandler = errorHandler;
    }

    /**
     * Returns the errorHandler.
     * 
     * @return the errorHandler
     */
    public ErrorHandler getErrorHandler() {
        return errorHandler;
    }

    /**
     * The argument MUST be an interned string or <code>null</code>.
     * 
     * @param context
     */
    public void setFragmentContext(String context) {
        this.contextName = context;
        this.contextNamespace = "http://www.w3.org/1999/xhtml";
        this.contextNode = null;
        this.fragment = (contextName != null);
        this.quirks = false;
    }

    // ]NOCPP]

    /**
     * @see nu.validator.htmlparser.common.TokenHandler#cdataSectionAllowed()
     */
    public bool cdataSectionAllowed() {
        return isInForeign();
    }
    
    private bool isInForeign() {
        return currentPtr >= 0
                && stack[currentPtr].ns != "http://www.w3.org/1999/xhtml";
    }

    private bool isInForeignButNotHtmlIntegrationPoint() {
        return currentPtr >= 0
                && stack[currentPtr].ns != "http://www.w3.org/1999/xhtml"
                && !stack[currentPtr].isHtmlIntegrationPoint();
    }

    /**
     * The argument MUST be an interned string or <code>null</code>.
     * 
     * @param context
     */
    public void setFragmentContext( String context, String ns, T node, bool quirks) {
        this.contextName = context;
        this.contextNamespace = ns;
        this.contextNode = node;
        this.fragment = (contextName != null);
        this.quirks = quirks;
    }

    protected T currentNode() {
        return stack[currentPtr].node;
    }

    /**
     * Returns the scriptingEnabled.
     * 
     * @return the scriptingEnabled
     */
    public bool isScriptingEnabled() {
        return scriptingEnabled;
    }

    /**
     * Sets the scriptingEnabled.
     * 
     * @param scriptingEnabled
     *            the scriptingEnabled to set
     */
    public void setScriptingEnabled(bool scriptingEnabled) {
        this.scriptingEnabled = scriptingEnabled;
    }

    // [NOCPP[

    /**
     * Sets the doctypeExpectation.
     * 
     * @param doctypeExpectation
     *            the doctypeExpectation to set
     */
    public void setDoctypeExpectation(DoctypeExpectation doctypeExpectation) {
        this.doctypeExpectation = doctypeExpectation;
    }

    public void setNamePolicy(XmlViolationPolicy namePolicy) {
        this.namePolicy = namePolicy;
    }

    /**
     * Sets the documentModeHandler.
     * 
     * @param documentModeHandler
     *            the documentModeHandler to set
     */
    public void setDocumentModeHandler(DocumentModeHandler documentModeHandler) {
        this.documentModeHandler = documentModeHandler;
    }

    /**
     * Sets the reportingDoctype.
     * 
     * @param reportingDoctype
     *            the reportingDoctype to set
     */
    public void setReportingDoctype(bool reportingDoctype) {
        this.reportingDoctype = reportingDoctype;
    }

    // ]NOCPP]

    /**
     * Flushes the pending characters. Public for document.write use cases only.
     * @throws SAXException
     */
    public void flushCharacters() {
        if (charBufferLen > 0) {
            if ((mode == IN_TABLE || mode == IN_TABLE_BODY || mode == IN_ROW)
                    && charBufferContainsNonWhitespace()) {
                errNonSpaceInTable();
                reconstructTheActiveFormattingElements();
                if (!stack[currentPtr].isFosterParenting()) {
                    // reconstructing gave us a new current node
                    appendCharacters(currentNode(), charBuffer, 0,
                            charBufferLen);
                    charBufferLen = 0;
                    return;
                }
                int eltPos = findLastOrRoot(TreeBuilderBase.TABLE);
                StackNode<T> node = stack[eltPos];
                T elt = node.node;
                if (eltPos == 0) {
                    appendCharacters(elt, charBuffer, 0, charBufferLen);
                    charBufferLen = 0;
                    return;
                }
                insertFosterParentedCharacters(charBuffer, 0, charBufferLen,
                        elt, stack[eltPos - 1].node);
                charBufferLen = 0;
                return;
            }
            appendCharacters(currentNode(), charBuffer, 0, charBufferLen);
            charBufferLen = 0;
        }
    }

    private bool charBufferContainsNonWhitespace() {
        for (int i = 0; i < charBufferLen; i++) {
            switch (charBuffer[i]) {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                case '\u000C':
                    continue;
                default:
                    return true;
            }
        }
        return false;
    }

    /**
     * Creates a comparable snapshot of the tree builder state. Snapshot
     * creation is only supported immediately after a script end tag has been
     * processed. In C++ the caller is responsible for calling
     * <code>delete</code> on the returned object.
     * 
     * @return a snapshot.
     * @throws SAXException
     */
    public TreeBuilderState<T> newSnapshot() {
        StackNode<T>[] listCopy = new StackNode<T>[listPtr + 1];
        for (int i = 0; i < listCopy.Length; i++) {
            StackNode<T> node = listOfActiveFormattingElements[i];
            if (node != null) {
                StackNode<T> newNode = new StackNode<T>(node.getFlags(), node.ns,
                        node.name, node.node, node.popName,
                        node.attributes.cloneAttributes(null)
                        // [NOCPP[
                        , node.getLocator()
                // ]NOCPP]
                );
                listCopy[i] = newNode;
            } else {
                listCopy[i] = null;
            }
        }
        StackNode<T>[] stackCopy = new StackNode<T>[currentPtr + 1];
        for (int i = 0; i < stackCopy.Length; i++) {
            StackNode<T> node = stack[i];
            int listIndex = findInListOfActiveFormattingElements(node);
            if (listIndex == -1) {
                StackNode<T> newNode = new StackNode<T>(node.getFlags(), node.ns,
                        node.name, node.node, node.popName,
                        null
                        // [NOCPP[
                        , node.getLocator()
                // ]NOCPP]
                );
                stackCopy[i] = newNode;
            } else {
                stackCopy[i] = listCopy[listIndex];
                stackCopy[i].retain();
            }
        }
        return new StateSnapshot<T>(stackCopy, listCopy, formPointer, headPointer, deepTreeSurrogateParent, mode, originalMode, framesetOk, needToDropLF, quirks);
    }

    public bool snapshotMatches(TreeBuilderState<T> snapshot) {
        StackNode<T>[] stackCopy = snapshot.getStack();
        int stackLen = snapshot.getStackLength();
        StackNode<T>[] listCopy = snapshot.getListOfActiveFormattingElements();
        int listLen = snapshot.getListOfActiveFormattingElementsLength();

        if (stackLen != currentPtr + 1
                || listLen != listPtr + 1
                || formPointer != snapshot.getFormPointer()
                || headPointer != snapshot.getHeadPointer()
                || deepTreeSurrogateParent != snapshot.getDeepTreeSurrogateParent()
                || mode != snapshot.getMode()
                || originalMode != snapshot.getOriginalMode()
                || framesetOk != snapshot.isFramesetOk()
                || needToDropLF != snapshot.isNeedToDropLF()
                || quirks != snapshot.isQuirks()) { // maybe just assert quirks
            return false;
        }
        for (int i = listLen - 1; i >= 0; i--) {
            if (listCopy[i] == null
                    && listOfActiveFormattingElements[i] == null) {
                continue;
            } else if (listCopy[i] == null
                    || listOfActiveFormattingElements[i] == null) {
                return false;
            }
            if (listCopy[i].node != listOfActiveFormattingElements[i].node) {
                return false; // it's possible that this condition is overly
                              // strict
            }
        }
        for (int i = stackLen - 1; i >= 0; i--) {
            if (stackCopy[i].node != stack[i].node) {
                return false;
            }
        }
        return true;
    }

    public void loadState(TreeBuilderState<T> snapshot, Interner interner) {
        StackNode<T>[] stackCopy = snapshot.getStack();
        int stackLen = snapshot.getStackLength();
        StackNode<T>[] listCopy = snapshot.getListOfActiveFormattingElements();
        int listLen = snapshot.getListOfActiveFormattingElementsLength();
        
        for (int i = 0; i <= listPtr; i++) {
            if (listOfActiveFormattingElements[i] != null) {
                listOfActiveFormattingElements[i].release();
            }
        }
        if (listOfActiveFormattingElements.Length < listLen) {
            listOfActiveFormattingElements = new StackNode<T>[listLen];
        }
        listPtr = listLen - 1;

        for (int i = 0; i <= currentPtr; i++) {
            stack[i].release();
        }
        if (stack.Length < stackLen) {
            stack = new StackNode<T>[stackLen];
        }
        currentPtr = stackLen - 1;

        for (int i = 0; i < listLen; i++) {
            StackNode<T> node = listCopy[i];
            if (node != null) {
                StackNode<T> newNode = new StackNode<T>(node.getFlags(), node.ns,
                        Portability.newLocalFromLocal(node.name, interner), node.node,
                        Portability.newLocalFromLocal(node.popName, interner),
                        node.attributes.cloneAttributes(null)
                        // [NOCPP[
                        , node.getLocator()
                // ]NOCPP]
                );
                listOfActiveFormattingElements[i] = newNode;
            } else {
                listOfActiveFormattingElements[i] = null;
            }
        }
        for (int i = 0; i < stackLen; i++) {
            StackNode<T> node = stackCopy[i];
            int listIndex = findInArray(node, listCopy);
            if (listIndex == -1) {
                StackNode<T> newNode = new StackNode<T>(node.getFlags(), node.ns,
                        Portability.newLocalFromLocal(node.name, interner), node.node,
                        Portability.newLocalFromLocal(node.popName, interner),
                        null
                        // [NOCPP[
                        , node.getLocator()
                // ]NOCPP]       
                );
                stack[i] = newNode;
            } else {
                stack[i] = listOfActiveFormattingElements[listIndex];
                stack[i].retain();
            }
        }
        formPointer = snapshot.getFormPointer();
        headPointer = snapshot.getHeadPointer();
        deepTreeSurrogateParent = snapshot.getDeepTreeSurrogateParent();
        mode = snapshot.getMode();
        originalMode = snapshot.getOriginalMode();
        framesetOk = snapshot.isFramesetOk();
        needToDropLF = snapshot.isNeedToDropLF();
        quirks = snapshot.isQuirks();
    }

    private int findInArray(StackNode<T> node, StackNode<T>[] arr) {
        for (int i = listPtr; i >= 0; i--) {
            if (node == arr[i]) {
                return i;
            }
        }
        return -1;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilderState#getFormPointer()
     */
    public T getFormPointer() {
        return formPointer;
    }

    /**
     * Returns the headPointer.
     * 
     * @return the headPointer
     */
    public T getHeadPointer() {
        return headPointer;
    }
    
    /**
     * Returns the deepTreeSurrogateParent.
     * 
     * @return the deepTreeSurrogateParent
     */
    public T getDeepTreeSurrogateParent() {
        return deepTreeSurrogateParent;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilderState#getListOfActiveFormattingElements()
     */
    public StackNode<T>[] getListOfActiveFormattingElements() {
        return listOfActiveFormattingElements;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilderState#getStack()
     */
    public StackNode<T>[] getStack() {
        return stack;
    }

    /**
     * Returns the mode.
     * 
     * @return the mode
     */
    public int getMode() {
        return mode;
    }

    /**
     * Returns the originalMode.
     * 
     * @return the originalMode
     */
    public int getOriginalMode() {
        return originalMode;
    }

    /**
     * Returns the framesetOk.
     * 
     * @return the framesetOk
     */
    public bool isFramesetOk() {
        return framesetOk;
    }
    
    /**
     * Returns the needToDropLF.
     * 
     * @return the needToDropLF
     */
    public bool isNeedToDropLF() {
        return needToDropLF;
    }

    /**
     * Returns the quirks.
     * 
     * @return the quirks
     */
    public bool isQuirks() {
        return quirks;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilderState#getListOfActiveFormattingElementsLength()
     */
    public int getListOfActiveFormattingElementsLength() {
        return listPtr + 1;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilderState#getStackLength()
     */
    public int getStackLength() {
        return currentPtr + 1;
    }

    /**
     * Reports a stray start tag.
     * @param name the name of the stray tag
     * 
     * @throws SAXException
     */
    private void errStrayStartTag( String name) {
        err("Stray end tag \u201C" + name + "\u201D.");
    }

    /**
     * Reports a stray end tag.
     * @param name the name of the stray tag
     * 
     * @throws SAXException
     */
    private void errStrayEndTag( String name) {
        err("Stray end tag \u201C" + name + "\u201D.");
    }
    
    /**
     * Reports a state when elements expected to be closed were not.
     * 
     * @param eltPos the position of the start tag on the stack of the element
     * being closed.
     * @param name the name of the end tag
     * 
     * @throws SAXException
     */
    private void errUnclosedElements(int eltPos,  String name) {
        errNoCheck("End tag \u201C" + name + "\u201D seen, but there were open elements.");
        errListUnclosedStartTags(eltPos);
    }

    /**
     * Reports a state when elements expected to be closed ahead of an implied 
     * end tag but were not.
     * 
     * @param eltPos the position of the start tag on the stack of the element
     * being closed.
     * @param name the name of the end tag
     * 
     * @throws SAXException
     */
    private void errUnclosedElementsImplied(int eltPos, String name) {
        errNoCheck("End tag \u201C" + name + "\u201D implied, but there were open elements.");
        errListUnclosedStartTags(eltPos);
    }

    /**
     * Reports a state when elements expected to be closed ahead of an implied 
     * table cell close.
     * 
     * @param eltPos the position of the start tag on the stack of the element
     * being closed.
     * @throws SAXException
     */
    private void errUnclosedElementsCell(int eltPos) {
        errNoCheck("A table cell was implicitly closed, but there were open elements.");
        errListUnclosedStartTags(eltPos);
    }
    
    private void errStrayDoctype() {
        err("Stray doctype.");
    }

    private void errAlmostStandardsDoctype() {
        err("Almost standards mode doctype. Expected \u201C<!DOCTYPE html>\u201D.");
    }

    private void errQuirkyDoctype() {
        err("Quirky doctype. Expected \u201C<!DOCTYPE html>\u201D.");
    }

    private void errNonSpaceInTrailer() {
        err("Non-space character in page trailer.");
    }

    private void errNonSpaceAfterFrameset() {
        err("Non-space after \u201Cframeset\u201D.");
    }

    private void errNonSpaceInFrameset() {
        err("Non-space in \u201Cframeset\u201D.");
    }

    private void errNonSpaceAfterBody() {
        err("Non-space character after body.");
    }

    private void errNonSpaceInColgroupInFragment() {
        err("Non-space in \u201Ccolgroup\u201D when parsing fragment.");
    }

    private void errNonSpaceInNoscriptInHead() {
        err("Non-space character inside \u201Cnoscript\u201D inside \u201Chead\u201D.");
    }

    private void errFooBetweenHeadAndBody(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("\u201C" + name + "\u201D element between \u201Chead\u201D and \u201Cbody\u201D.");
    }

    private void errStartTagWithoutDoctype() {
        err("Start tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE html>\u201D.");
    }

    private void errNoSelectInTableScope() {
        err("No \u201Cselect\u201D in table scope.");
    }

    private void errStartSelectWhereEndSelectExpected() {
        err("\u201Cselect\u201D start tag where end tag expected.");
    }

    private void errStartTagWithSelectOpen(String name)
    {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("\u201C" + name
                + "\u201D start tag with \u201Cselect\u201D open.");
    }

    private void errBadStartTagInHead(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("Bad start tag in \u201C" + name
                + "\u201D in \u201Chead\u201D.");
    }

    private void errImage() {
        err("Saw a start tag \u201Cimage\u201D.");
    }

    private void errIsindex() {
        err("\u201Cisindex\u201D seen.");
    }

    private void errFooSeenWhenFooOpen(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("An \u201C" + name + "\u201D start tag seen but an element of the same type was already open.");
    }

    private void errHeadingWhenHeadingOpen() {
        err("Heading cannot be a child of another heading.");
    }

    private void errFramesetStart() {
        err("\u201Cframeset\u201D start tag seen.");
    }

    private void errNoCellToClose() {
        err("No cell to close.");
    }

    private void errStartTagInTable( String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("Start tag \u201C" + name
                + "\u201D seen in \u201Ctable\u201D.");
    }

    private void errFormWhenFormOpen() {
        err("Saw a \u201Cform\u201D start tag, but there was already an active \u201Cform\u201D element. Nested forms are not allowed. Ignoring the tag.");
    }

    private void errTableSeenWhileTableOpen() {
        err("Start tag for \u201Ctable\u201D seen but the previous \u201Ctable\u201D is still open.");
    }

    private void errStartTagInTableBody(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("\u201C" + name + "\u201D start tag in table body.");
    }

    private void errEndTagSeenWithoutDoctype() {
        err("End tag seen without seeing a doctype first. Expected \u201C<!DOCTYPE html>\u201D.");
    }

    private void errEndTagAfterBody() {
        err("Saw an end tag after \u201Cbody\u201D had been closed.");
    }

    private void errEndTagSeenWithSelectOpen(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("\u201C" + name
                + "\u201D end tag with \u201Cselect\u201D open.");
    }

    private void errGarbageInColgroup() {
        err("Garbage in \u201Ccolgroup\u201D fragment.");
    }

    private void errEndTagBr() {
        err("End tag \u201Cbr\u201D.");
    }

    private void errNoElementToCloseButEndTagSeen(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("No \u201C" + name + "\u201D element in scope but a \u201C"
                + name + "\u201D end tag seen.");
    }

    private void errHtmlStartTagInForeignContext( String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("HTML start tag \u201C" + name
                + "\u201D in a foreign namespace context.");
    }

    private void errTableClosedWhileCaptionOpen() {
        err("\u201Ctable\u201D closed but \u201Ccaption\u201D was still open.");
    }

    private void errNoTableRowToClose() {
        err("No table row to close.");
    }

    private void errNonSpaceInTable() {
        err("Misplaced non-space characters insided a table.");
    }

    private void errUnclosedChildrenInRuby() {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("Unclosed children in \u201Cruby\u201D.");
    }

    private void errStartTagSeenWithoutRuby( String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("Start tag \u201C"
                + name
                + "\u201D seen without a \u201Cruby\u201D element being open.");
    }

    private void errSelfClosing() {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("Self-closing syntax (\u201C/>\u201D) used on a non-void HTML element. Ignoring the slash and treating as a start tag.");
    }

    private void errNoCheckUnclosedElementsOnStack() {
        errNoCheck("Unclosed elements on stack.");
    }

    private void errEndTagDidNotMatchCurrentOpenElement( String name,
             String currOpenName) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("End tag \u201C"
                + name
                + "\u201D did not match the name of the current open element (\u201C"
                + currOpenName + "\u201D).");
    }

    private void errEndTagViolatesNestingRules( String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("End tag \u201C" + name + "\u201D violates nesting rules.");
    }

    private void errEofWithUnclosedElements() {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("End of file seen and there were open elements.");
        // just report all remaining unclosed elements
        errListUnclosedStartTags(0);
    }

    /**
     * Reports arriving at/near end of document with unclosed elements remaining.
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    private void errEndWithUnclosedElements(String name) {
        if (errorHandler == null) {
            return;
        }
        errNoCheck("End tag for  \u201C"
                + name
                + "\u201D seen, but there were unclosed elements.");
        // just report all remaining unclosed elements
        errListUnclosedStartTags(0);
    }
}