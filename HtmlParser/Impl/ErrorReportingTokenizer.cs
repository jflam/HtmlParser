/*
 * Copyright (c) 2009-2010 Mozilla Foundation
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

public class ErrorReportingTokenizer : Tokenizer {

    /**
     * Magic value for UTF-16 operations.
     */
    private const int SURROGATE_OFFSET = (0x10000 - (0xD800 << 10) - 0xDC00);

    /**
     * The policy for non-space non-XML characters.
     */
    private XmlViolationPolicy contentNonXmlCharPolicy = XmlViolationPolicy.ALTER_INFOSET;

    /**
     * Used together with <code>nonAsciiProhibited</code>.
     */
    private bool alreadyComplainedAboutNonAscii;

    /**
     * Keeps track of PUA warnings.
     */
    private bool alreadyWarnedAboutPrivateUseCharacters;

    /**
     * The current line number in the current resource being parsed. (First line
     * is 1.) Passed on as locator data.
     */
    private int line;

    private int linePrev;

    /**
     * The current column number in the current resource being tokenized. (First
     * column is 1, counted by UTF-16 code units.) Passed on as locator data.
     */
    private int col;

    private int colPrev;

    private bool nextCharOnNewLine;

    private char prev;

    private HashMap<String, String> errorProfileMap = null;

    private TransitionHandler transitionHandler = null;

    private int transitionBaseOffset = 0;

    /**
     * @param tokenHandler
     * @param newAttributesEachTime
     */
    public ErrorReportingTokenizer(TokenHandler tokenHandler, bool newAttributesEachTime) : base(tokenHandler, newAttributesEachTime) {
    }

    /**
     * @param tokenHandler
     */
    public ErrorReportingTokenizer(TokenHandler tokenHandler) : base(tokenHandler) {
    }

    /**
     * @see org.xml.sax.Locator#getLineNumber()
     */
    public int getLineNumber() {
        if (line > 0) {
            return line;
        } else {
            return -1;
        }
    }

    /**
     * @see org.xml.sax.Locator#getColumnNumber()
     */
    public int getColumnNumber() {
        if (col > 0) {
            return col;
        } else {
            return -1;
        }
    }

    /**
     * Sets the contentNonXmlCharPolicy.
     * 
     * @param contentNonXmlCharPolicy
     *            the contentNonXmlCharPolicy to set
     */
    public void setContentNonXmlCharPolicy(
            XmlViolationPolicy contentNonXmlCharPolicy) {
        this.contentNonXmlCharPolicy = contentNonXmlCharPolicy;
    }

    /**
     * Sets the errorProfile.
     * 
     * @param errorProfile
     */
    public void setErrorProfile(HashMap<String, String> errorProfileMap) {
        this.errorProfileMap = errorProfileMap;
    }

    /**
     * Reports on an event based on profile selected.
     * 
     * @param profile
     *            the profile this message belongs to
     * @param message
     *            the message itself
     * @throws SAXException
     */
    public void note(String profile, String message) {
        if (errorProfileMap == null)
            return;
        String level = errorProfileMap.get(profile);
        if ("warn".Equals(level)) {
            warn(message);
        } else if ("err".Equals(level)) {
            err(message);
            // } else if ("info".equals(level)) {
            // info(message);
        }
    }

    protected void startErrorReporting() {
        alreadyComplainedAboutNonAscii = false;
        line = linePrev = 0;
        col = colPrev = 1;
        nextCharOnNewLine = true;
        prev = '\u0000';
        alreadyWarnedAboutPrivateUseCharacters = false;
        transitionBaseOffset = 0;
    }

    protected void silentCarriageReturn() {
        nextCharOnNewLine = true;
        lastCR = true;
    }

    protected void silentLineFeed() {
        nextCharOnNewLine = true;
    }

    /**
     * Returns the line.
     * 
     * @return the line
     */
    public int getLine() {
        return line;
    }

    /**
     * Returns the col.
     * 
     * @return the col
     */
    public int getCol() {
        return col;
    }

    /**
     * Returns the nextCharOnNewLine.
     * 
     * @return the nextCharOnNewLine
     */
    public bool isNextCharOnNewLine() {
        return nextCharOnNewLine;
    }

    private void complainAboutNonAscii() {
        String encoding = null;
        if (encodingDeclarationHandler != null) {
            encoding = encodingDeclarationHandler.getCharacterEncoding();
        }
        if (encoding == null) {
            err("The character encoding of the document was not explicit but the document contains non-ASCII.");
        } else {
            err("No explicit character encoding declaration has been seen yet (assumed \u201C"
                    + encoding + "\u201D) but the document contains non-ASCII.");
        }
    }

    /**
     * Returns the alreadyComplainedAboutNonAscii.
     * 
     * @return the alreadyComplainedAboutNonAscii
     */
    public bool isAlreadyComplainedAboutNonAscii() {
        return alreadyComplainedAboutNonAscii;
    }

    /**
     * Flushes coalesced character tokens.
     * 
     * @param buf
     *            TODO
     * @param pos
     *            TODO
     * 
     * @throws SAXException
     */
    protected void flushChars(char[] buf, int pos) {
        if (pos > cstart) {
            int currLine = line;
            int currCol = col;
            line = linePrev;
            col = colPrev;
            tokenHandler.characters(buf, cstart, pos - cstart);
            line = currLine;
            col = currCol;
        }
        cstart = 0x7fffffff;
    }

    protected char checkChar(char[] buf, int pos) {
        linePrev = line;
        colPrev = col;
        if (nextCharOnNewLine) {
            line++;
            col = 1;
            nextCharOnNewLine = false;
        } else {
            col++;
        }

        char c = buf[pos];
        if (!confident && !alreadyComplainedAboutNonAscii && c > '\u007F') {
            complainAboutNonAscii();
            alreadyComplainedAboutNonAscii = true;
        }
        switch (c) {
            case '\u0000':
                err("Saw U+0000 in stream.");
            case '\t':
            case '\r':
            case '\n':
                break;
            case '\u000C':
                if (contentNonXmlCharPolicy == XmlViolationPolicy.FATAL) {
                    fatal("This document is not mappable to XML 1.0 without data loss due to "
                            + toUPlusString(c)
                            + " which is not a legal XML 1.0 character.");
                } else {
                    if (contentNonXmlCharPolicy == XmlViolationPolicy.ALTER_INFOSET) {
                        c = buf[pos] = ' ';
                    }
                    warn("This document is not mappable to XML 1.0 without data loss due to "
                            + toUPlusString(c)
                            + " which is not a legal XML 1.0 character.");
                }
                break;
            default:
                if ((c & 0xFC00) == 0xDC00) {
                    // Got a low surrogate. See if prev was high
                    // surrogate
                    if ((prev & 0xFC00) == 0xD800) {
                        int intVal = (prev << 10) + c + SURROGATE_OFFSET;
                        if ((intVal & 0xFFFE) == 0xFFFE) {
                            err("Astral non-character.");
                        }
                        if (isAstralPrivateUse(intVal)) {
                            warnAboutPrivateUseChar();
                        }
                    }
                } else if ((c < ' ' || ((c & 0xFFFE) == 0xFFFE))) {
                    switch (contentNonXmlCharPolicy) {
                        case XmlViolationPolicy.FATAL:
                            fatal("Forbidden code point " + toUPlusString(c)
                                    + ".");
                            break;
                        case XmlViolationPolicy.ALTER_INFOSET:
                            c = buf[pos] = '\uFFFD';
                            // fall through
                            goto case XmlViolationPolicy.ALLOW;
                        case XmlViolationPolicy.ALLOW:
                            err("Forbidden code point " + toUPlusString(c) + ".");
                            break;
                    }
                } else if ((c >= '\u007F') && (c <= '\u009F')
                        || (c >= '\uFDD0') && (c <= '\uFDEF')) {
                    err("Forbidden code point " + toUPlusString(c) + ".");
                } else if (isPrivateUse(c)) {
                    warnAboutPrivateUseChar();
                }
                break;
        }
        prev = c;
        return c;
    }

    /**
     * @throws SAXException
     * @see nu.validator.htmlparser.impl.Tokenizer#transition(int, int, bool,
     *      int)
     */
    protected int transition(int from, int to, bool reconsume, int pos) {
        if (transitionHandler != null) {
            transitionHandler.transition(from, to, reconsume, transitionBaseOffset + pos);
        }
        return to;
    }

    private String toUPlusString(int c) {
        String hexString = c.ToString("X");
        switch (hexString.Length) {
            case 1:
                return "U+000" + hexString;
            case 2:
                return "U+00" + hexString;
            case 3:
                return "U+0" + hexString;
            default:
                return "U+" + hexString;
        }
    }

    /**
     * Emits a warning about private use characters if the warning has not been
     * emitted yet.
     * 
     * @throws SAXException
     */
    private void warnAboutPrivateUseChar() {
        if (!alreadyWarnedAboutPrivateUseCharacters) {
            warn("Document uses the Unicode Private Use Area(s), which should not be used in publicly exchanged documents. (Charmod C073)");
            alreadyWarnedAboutPrivateUseCharacters = true;
        }
    }

    /**
     * Tells if the argument is a BMP PUA character.
     * 
     * @param c
     *            the UTF-16 code unit to check
     * @return <code>true</code> if PUA character
     */
    private bool isPrivateUse(char c) {
        return c >= '\uE000' && c <= '\uF8FF';
    }

    /**
     * Tells if the argument is an astral PUA character.
     * 
     * @param c
     *            the code point to check
     * @return <code>true</code> if astral private use
     */
    private bool isAstralPrivateUse(int c) {
        return (c >= 0xF0000 && c <= 0xFFFFD)
                || (c >= 0x100000 && c <= 0x10FFFD);
    }

    protected void errGarbageAfterLtSlash() {
        err("Garbage after \u201C</\u201D.");
    }

    protected void errLtSlashGt() {
        err("Saw \u201C</>\u201D. Probable causes: Unescaped \u201C<\u201D (escape as \u201C&lt;\u201D) or mistyped end tag.");
    }

    protected void errWarnLtSlashInRcdata() {
        if (html4) {
            err((stateSave == Tokenizer.DATA ? "CDATA" : "RCDATA")
                    + " element \u201C"
                    + endTagExpectation
                    + "\u201D contained the string \u201C</\u201D, but it was not the start of the end tag. (HTML4-only error)");
        } else {
            warn((stateSave == Tokenizer.DATA ? "CDATA" : "RCDATA")
                    + " element \u201C"
                    + endTagExpectation
                    + "\u201D contained the string \u201C</\u201D, but this did not close the element.");
        }
    }

    protected void errHtml4LtSlashInRcdata(char folded) {
        if (html4 && (index > 0 || (folded >= 'a' && folded <= 'z'))
                && ElementName.IFRAME != endTagExpectation) {
            err((stateSave == Tokenizer.DATA ? "CDATA" : "RCDATA")
                    + " element \u201C"
                    + endTagExpectation.name
                    + "\u201D contained the string \u201C</\u201D, but it was not the start of the end tag. (HTML4-only error)");
        }
    }

    protected void errCharRefLacksSemicolon() {
        err("Character reference was not terminated by a semicolon.");
    }

    protected void errNoDigitsInNCR() {
        err("No digits after \u201C" + strBufToString() + "\u201D.");
    }

    protected void errGtInSystemId() {
        err("\u201C>\u201D in system identifier.");
    }

    protected void errGtInPublicId() {
        err("\u201C>\u201D in public identifier.");
    }

    protected void errNamelessDoctype() {
        err("Nameless doctype.");
    }

    protected void errConsecutiveHyphens() {
        err("Consecutive hyphens did not terminate a comment. \u201C--\u201D is not permitted inside a comment, but e.g. \u201C- -\u201D is.");
    }

    protected void errPrematureEndOfComment() {
        err("Premature end of comment. Use \u201C-->\u201D to end a comment properly.");
    }

    protected void errBogusComment() {
        err("Bogus comment.");
    }

    protected void errUnquotedAttributeValOrNull(char c) {
        switch (c) {
            case '<':
                err("\u201C<\u201D in an unquoted attribute value. Probable cause: Missing \u201C>\u201D immediately before.");
                return;
            case '`':
                err("\u201C`\u201D in an unquoted attribute value. Probable cause: Using the wrong character as a quote.");
                return;
            case '\uFFFD':
                return;
            default:
                err("\u201C"
                        + c
                        + "\u201D in an unquoted attribute value. Probable causes: Attributes running together or a URL query string in an unquoted attribute value.");
                return;
        }
    }

    protected void errSlashNotFollowedByGt() {
        err("A slash was not immediately followed by \u201C>\u201D.");
    }

    protected void errHtml4XmlVoidSyntax() {
        if (html4) {
            err("The \u201C/>\u201D syntax on void elements is not allowed.  (This is an HTML4-only error.)");
        }
    }

    protected void errNoSpaceBetweenAttributes() {
        err("No space between attributes.");
    }

    protected void errHtml4NonNameInUnquotedAttribute(char c) {
        if (html4
                && !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                        || (c >= '0' && c <= '9') || c == '.' || c == '-'
                        || c == '_' || c == ':')) {
            err("Non-name character in an unquoted attribute value. (This is an HTML4-only error.)");
        }
    }

    protected void errLtOrEqualsOrGraveInUnquotedAttributeOrNull(char c) {
        switch (c) {
            case '=':
                err("\u201C=\u201D at the start of an unquoted attribute value. Probable cause: Stray duplicate equals sign.");
                return;
            case '<':
                err("\u201C<\u201D at the start of an unquoted attribute value. Probable cause: Missing \u201C>\u201D immediately before.");
                return;
            case '`':
                err("\u201C`\u201D at the start of an unquoted attribute value. Probable cause: Using the wrong character as a quote.");
                return;
        }
    }

    protected void errAttributeValueMissing() {
        err("Attribute value missing.");
    }

    protected void errBadCharBeforeAttributeNameOrNull(char c) {
        if (c == '<') {
            err("Saw \u201C<\u201D when expecting an attribute name. Probable cause: Missing \u201C>\u201D immediately before.");
        } else if (c == '=') {
            errEqualsSignBeforeAttributeName();
        } else if (c != '\uFFFD') {
            errQuoteBeforeAttributeName(c);
        }
    }

    protected void errEqualsSignBeforeAttributeName() {
        err("Saw \u201C=\u201D when expecting an attribute name. Probable cause: Attribute name missing.");
    }

    protected void errBadCharAfterLt(char c) {
        err("Bad character \u201C"
                + c
                + "\u201D after \u201C<\u201D. Probable cause: Unescaped \u201C<\u201D. Try escaping it as \u201C&lt;\u201D.");
    }

    protected void errLtGt() {
        err("Saw \u201C<>\u201D. Probable causes: Unescaped \u201C<\u201D (escape as \u201C&lt;\u201D) or mistyped start tag.");
    }

    protected void errProcessingInstruction() {
        err("Saw \u201C<?\u201D. Probable cause: Attempt to use an XML processing instruction in HTML. (XML processing instructions are not supported in HTML.)");
    }

    protected void errUnescapedAmpersandInterpretedAsCharacterReference() {
        if (errorHandler == null) {
            return;
        }
        SAXParseException spe = new SAXParseException(
                "The string following \u201C&\u201D was interpreted as a character reference. (\u201C&\u201D probably should have been escaped as \u201C&amp;\u201D.)",
                ampersandLocation);
        errorHandler.error(spe);
    }

    protected void errNotSemicolonTerminated() {
        err("Named character reference was not terminated by a semicolon. (Or \u201C&\u201D should have been escaped as \u201C&amp;\u201D.)");
    }

    protected void errNoNamedCharacterMatch() {
        if (errorHandler == null) {
            return;
        }
        SAXParseException spe = new SAXParseException(
                "\u201C&\u201D did not start a character reference. (\u201C&\u201D probably should have been escaped as \u201C&amp;\u201D.)",
                ampersandLocation);
        errorHandler.error(spe);
    }

    protected void errQuoteBeforeAttributeName(char c) {
        err("Saw \u201C"
                + c
                + "\u201D when expecting an attribute name. Probable cause: \u201C=\u201D missing immediately before.");
    }

    protected void errQuoteOrLtInAttributeNameOrNull(char c) {
        if (c == '<') {
            err("\u201C<\u201D in attribute name. Probable cause: \u201C>\u201D missing immediately before.");
        } else if (c != '\uFFFD') {
            err("Quote \u201C"
                    + c
                    + "\u201D in attribute name. Probable cause: Matching quote missing somewhere earlier.");
        }
    }

    protected void errExpectedPublicId() {
        err("Expected a public identifier but the doctype ended.");
    }

    protected void errBogusDoctype() {
        err("Bogus doctype.");
    }

    protected void maybeWarnPrivateUseAstral() {
        if (errorHandler != null && isAstralPrivateUse(value)) {
            warnAboutPrivateUseChar();
        }
    }

    protected void maybeWarnPrivateUse(char ch) {
        if (errorHandler != null && isPrivateUse(ch)) {
            warnAboutPrivateUseChar();
        }
    }

    protected void maybeErrAttributesOnEndTag(HtmlAttributes attrs) {
        if (attrs.getLength() != 0) {
            /*
             * When an end tag token is emitted with attributes, that is a parse
             * error.
             */
            err("End tag had attributes.");
        }
    }

    protected void maybeErrSlashInEndTag(bool selfClosing) {
        if (selfClosing && endTag) {
            err("Stray \u201C/\u201D at the end of an end tag.");
        }
    }

    protected char errNcrNonCharacter(char ch) {
        switch (contentNonXmlCharPolicy) {
            case XmlViolationPolicy.FATAL:
                fatal("Character reference expands to a non-character ("
                        + toUPlusString((char) value) + ").");
                break;
            case XmlViolationPolicy.ALTER_INFOSET:
                ch = '\uFFFD';
                goto case XmlViolationPolicy.ALLOW;
                // fall through
            case XmlViolationPolicy.ALLOW:
                err("Character reference expands to a non-character ("
                        + toUPlusString((char) value) + ").");
                break;
        }
        return ch;
    }

    /**
     * @see nu.validator.htmlparser.impl.Tokenizer#errAstralNonCharacter(int)
     */
    protected void errAstralNonCharacter(int ch) {
        err("Character reference expands to an astral non-character ("
                + toUPlusString(value) + ").");
    }

    protected void errNcrSurrogate() {
        err("Character reference expands to a surrogate.");
    }

    protected char errNcrControlChar(char ch) {
        switch (contentNonXmlCharPolicy) {
            case XmlViolationPolicy.FATAL:
                fatal("Character reference expands to a control character ("
                        + toUPlusString((char) value) + ").");
                break;
            case XmlViolationPolicy.ALTER_INFOSET:
                ch = '\uFFFD';
                goto case XmlViolationPolicy.ALLOW;
                // fall through
            case XmlViolationPolicy.ALLOW:
                err("Character reference expands to a control character ("
                        + toUPlusString((char) value) + ").");
                break;
        }
        return ch;
    }

    protected void errNcrCr() {
        err("A numeric character reference expanded to carriage return.");
    }

    protected void errNcrInC1Range() {
        err("A numeric character reference expanded to the C1 controls range.");
    }

    protected void errEofInPublicId() {
        err("End of file inside public identifier.");
    }

    protected void errEofInComment() {
        err("End of file inside comment.");
    }

    protected void errEofInDoctype() {
        err("End of file inside doctype.");
    }

    protected void errEofInAttributeValue() {
        err("End of file reached when inside an attribute value. Ignoring tag.");
    }

    protected void errEofInAttributeName() {
        err("End of file occurred in an attribute name. Ignoring tag.");
    }

    protected void errEofWithoutGt() {
        err("Saw end of file without the previous tag ending with \u201C>\u201D. Ignoring tag.");
    }

    protected void errEofInTagName() {
        err("End of file seen when looking for tag name. Ignoring tag.");
    }

    protected void errEofInEndTag() {
        err("End of file inside end tag. Ignoring tag.");
    }

    protected void errEofAfterLt() {
        err("End of file after \u201C<\u201D.");
    }

    protected void errNcrOutOfRange() {
        err("Character reference outside the permissible Unicode range.");
    }

    protected void errNcrUnassigned() {
        err("Character reference expands to a permanently unassigned code point.");
    }

    protected void errDuplicateAttribute() {
        err("Duplicate attribute \u201C"
                + attributeName.getLocal(AttributeName.HTML) + "\u201D.");
    }

    protected void errEofInSystemId() {
        err("End of file inside system identifier.");
    }

    protected void errExpectedSystemId() {
        err("Expected a system identifier but the doctype ended.");
    }

    protected void errMissingSpaceBeforeDoctypeName() {
        err("Missing space before doctype name.");
    }

    protected void errHyphenHyphenBang() {
        err("\u201C--!\u201D found in comment.");
    }

    protected void errNcrControlChar() {
        err("Character reference expands to a control character ("
                + toUPlusString((char) value) + ").");
    }

    protected void errNcrZero() {
        err("Character reference expands to zero.");
    }

    protected void errNoSpaceBetweenDoctypeSystemKeywordAndQuote() {
        err("No space between the doctype \u201CSYSTEM\u201D keyword and the quote.");
    }

    protected void errNoSpaceBetweenPublicAndSystemIds() {
        err("No space between the doctype public and system identifiers.");
    }

    protected void errNoSpaceBetweenDoctypePublicKeywordAndQuote() {
        err("No space between the doctype \u201CPUBLIC\u201D keyword and the quote.");
    }

    protected void noteAttributeWithoutValue() {
        note("xhtml2", "Attribute without value");
    }

    protected void noteUnquotedAttributeValue() {
        note("xhtml1", "Unquoted attribute value.");
    }

    /**
     * Sets the transitionHandler.
     * 
     * @param transitionHandler
     *            the transitionHandler to set
     */
    public void setTransitionHandler(TransitionHandler transitionHandler) {
        this.transitionHandler = transitionHandler;
    }

    /**
     * Sets an offset to be added to the position reported to
     * <code>TransitionHandler</code>.
     * 
     * @param offset
     *            the offset
     */
    public void setTransitionBaseOffset(int offset) {
        this.transitionBaseOffset = offset;
    }
}
