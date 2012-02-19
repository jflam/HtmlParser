/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2008-2010 Mozilla Foundation
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

public abstract class MetaScanner {

    /**
     * Constant for "charset".
     */
    private static readonly char[] CHARSET = "harset".ToCharArray();
    
    /**
     * Constant for "content".
     */
    private static readonly char[] CONTENT = "ontent".ToCharArray();

    /**
     * Constant for "http-equiv".
     */
    private static readonly char[] HTTP_EQUIV = "ttp-equiv".ToCharArray();

    /**
     * Constant for "content-type".
     */
    private static readonly char[] CONTENT_TYPE = "content-type".ToCharArray();

    private const int NO = 0;

    private const int M = 1;
    
    private const int E = 2;
    
    private const int T = 3;

    private const int A = 4;
    
    private const int DATA = 0;

    private const int TAG_OPEN = 1;

    private const int SCAN_UNTIL_GT = 2;

    private const int TAG_NAME = 3;

    private const int BEFORE_ATTRIBUTE_NAME = 4;

    private const int ATTRIBUTE_NAME = 5;

    private const int AFTER_ATTRIBUTE_NAME = 6;

    private const int BEFORE_ATTRIBUTE_VALUE = 7;

    private const int ATTRIBUTE_VALUE_DOUBLE_QUOTED = 8;

    private const int ATTRIBUTE_VALUE_SINGLE_QUOTED = 9;

    private const int ATTRIBUTE_VALUE_UNQUOTED = 10;

    private const int AFTER_ATTRIBUTE_VALUE_QUOTED = 11;

    private const int MARKUP_DECLARATION_OPEN = 13;
    
    private const int MARKUP_DECLARATION_HYPHEN = 14;

    private const int COMMENT_START = 15;

    private const int COMMENT_START_DASH = 16;

    private const int COMMENT = 17;

    private const int COMMENT_END_DASH = 18;

    private const int COMMENT_END = 19;
    
    private const int SELF_CLOSING_START_TAG = 20;
    
    private const int HTTP_EQUIV_NOT_SEEN = 0;
    
    private const int HTTP_EQUIV_CONTENT_TYPE = 1;

    private const int HTTP_EQUIV_OTHER = 2;

    /**
     * The data source.
     */
    protected ByteReadable readable;
    
    /**
     * The state of the state machine that recognizes the tag name "meta".
     */
    private int metaState = NO;

    /**
     * The current position in recognizing the attribute name "content".
     */
    private int contentIndex = Int32.MaxValue;
    
    /**
     * The current position in recognizing the attribute name "charset".
     */
    private int charsetIndex = Int32.MaxValue;

    /**
     * The current position in recognizing the attribute name "http-equive".
     */
    private int httpEquivIndex = Int32.MaxValue;

    /**
     * The current position in recognizing the attribute value "content-type".
     */
    private int contentTypeIndex = Int32.MaxValue;

    /**
     * The tokenizer state.
     */
    protected int stateSave = DATA;

    /**
     * The currently filled length of strBuf.
     */
    private int strBufLen;

    /**
     * Accumulation buffer for attribute values.
     */
    private char[] strBuf;
    
    private String content;
    
    private String charset;
    
    private int httpEquivState;
    
    public MetaScanner() {
        this.readable = null;
        this.metaState = NO;
        this.contentIndex = Int32.MaxValue;
        this.charsetIndex = Int32.MaxValue;
        this.httpEquivIndex = Int32.MaxValue;
        this.contentTypeIndex = Int32.MaxValue;
        this.stateSave = DATA;
        this.strBufLen = 0;
        this.strBuf = new char[36];
        this.content = null;
        this.charset = null;
        this.httpEquivState = HTTP_EQUIV_NOT_SEEN;
    }
    
    private void destructor() {
        Portability.releaseString(content);
        Portability.releaseString(charset);
    }

    // [NOCPP[
    
    /**
     * Reads a byte from the data source.
     * 
     * -1 means end.
     * @return
     * @throws IOException
     */
    protected int read() {
        return readable.readByte();
    }

    // ]NOCPP]

    // WARNING When editing this, makes sure the bytecode length shown by javap
    // stays under 8000 bytes!
    /**
     * The runs the meta scanning algorithm.
     */
    protected void stateLoop(int state) {
        int c = -1;
        bool reconsume = false;
        for (;;) {
            switch (state) {
                case DATA:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '<':
                                state = MetaScanner.TAG_OPEN;
                                goto dataloop_break; // FALL THROUGH continue
                            // stateloop;
                            default:
                                continue;
                        }
                    }
            dataloop_break: goto case TAG_OPEN;
                    // WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
                case TAG_OPEN:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case 'm':
                            case 'M':
                                metaState = M;
                                state = MetaScanner.TAG_NAME;
                                goto tagopenloop_break;
                                // continue stateloop;                                
                            case '!':
                                state = MetaScanner.MARKUP_DECLARATION_OPEN;
                                goto stateloop_continue;
                            case '?':
                            case '/':
                                state = MetaScanner.SCAN_UNTIL_GT;
                                goto stateloop_continue;
                            case '>':
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
                                    metaState = NO;
                                    state = MetaScanner.TAG_NAME;
                                    goto tagopenloop_break;
                                    // continue stateloop;
                                }
                                state = MetaScanner.DATA;
                                reconsume = true;
                                goto stateloop_continue;
                        }
                    }
            tagopenloop_break: goto case TAG_NAME;
                    // FALL THROUGH DON'T REORDER
                case TAG_NAME:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                state = MetaScanner.BEFORE_ATTRIBUTE_NAME;
                                goto tagnameloop_break;
                            // continue stateloop;
                            case '/':
                                state = MetaScanner.SELF_CLOSING_START_TAG;
                                goto stateloop_continue;
                            case '>':
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            case 'e':
                            case 'E':
                                if (metaState == M) {
                                    metaState = E;
                                } else {
                                    metaState = NO;
                                }
                                continue;
                            case 't':
                            case 'T':
                                if (metaState == E) {
                                    metaState = T;
                                } else {
                                    metaState = NO;
                                }
                                continue;
                            case 'a':
                            case 'A':
                                if (metaState == T) {
                                    metaState = A;
                                } else {
                                    metaState = NO;
                                }
                                continue;
                            default:
                                metaState = NO;
                                continue;
                        }
                    }
            tagnameloop_break: goto case BEFORE_ATTRIBUTE_NAME;
                    // FALLTHRU DON'T REORDER
                case BEFORE_ATTRIBUTE_NAME:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        /*
                         * Consume the next input character:
                         */
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                continue;
                            case '/':
                                state = MetaScanner.SELF_CLOSING_START_TAG;
                                goto stateloop_continue;
                            case '>':
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = DATA;
                                goto stateloop_continue;
                            case 'c':
                            case 'C':
                                contentIndex = 0;
                                charsetIndex = 0;
                                httpEquivIndex = Int32.MaxValue;
                                contentTypeIndex = Int32.MaxValue;
                                state = MetaScanner.ATTRIBUTE_NAME;
                                goto beforeattributenameloop_break;                               
                            case 'h':
                            case 'H':
                                contentIndex = Int32.MaxValue;
                                charsetIndex = Int32.MaxValue;
                                httpEquivIndex = 0;
                                contentTypeIndex = Int32.MaxValue;
                                state = MetaScanner.ATTRIBUTE_NAME;
                                goto beforeattributenameloop_break;                               
                            default:
                                contentIndex = Int32.MaxValue;
                                charsetIndex = Int32.MaxValue;
                                httpEquivIndex = Int32.MaxValue;
                                contentTypeIndex = Int32.MaxValue;
                                state = MetaScanner.ATTRIBUTE_NAME;
                                goto beforeattributenameloop_break;                               
                            // continue stateloop;
                        }
                    }
            beforeattributenameloop_break: goto case ATTRIBUTE_NAME;
                    // FALLTHRU DON'T REORDER
                case ATTRIBUTE_NAME:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                state = MetaScanner.AFTER_ATTRIBUTE_NAME;
                                goto stateloop_continue;
                            case '/':
                                state = MetaScanner.SELF_CLOSING_START_TAG;
                                goto stateloop_continue;
                            case '=':
                                strBufLen = 0;
                                contentTypeIndex = 0;
                                state = MetaScanner.BEFORE_ATTRIBUTE_VALUE;
                                goto attributenameloop_break;
                            // continue stateloop;
                            case '>':
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                if (metaState == A) {
                                    if (c >= 'A' && c <= 'Z') {
                                        c += 0x20;
                                    }
                                    if (contentIndex < CONTENT.Length && c == CONTENT[contentIndex]) {
                                        ++contentIndex;
                                    } else {
                                        contentIndex = Int32.MaxValue;
                                    }
                                    if (charsetIndex < CHARSET.Length && c == CHARSET[charsetIndex]) {
                                        ++charsetIndex;
                                    } else {
                                        charsetIndex = Int32.MaxValue;
                                    }
                                    if (httpEquivIndex < HTTP_EQUIV.Length && c == HTTP_EQUIV[httpEquivIndex]) {
                                        ++httpEquivIndex;
                                    } else {
                                        httpEquivIndex = Int32.MaxValue;
                                    }                                    
                                }
                                continue;
                        }
                    }
            attributenameloop_break: goto case BEFORE_ATTRIBUTE_VALUE;
                    // FALLTHRU DON'T REORDER
                case BEFORE_ATTRIBUTE_VALUE:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                continue;
                            case '"':
                                state = MetaScanner.ATTRIBUTE_VALUE_DOUBLE_QUOTED;
                                goto beforeattributevalueloop_break;
                            // continue stateloop;
                            case '\'':
                                state = MetaScanner.ATTRIBUTE_VALUE_SINGLE_QUOTED;
                                goto stateloop_continue;
                            case '>':
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                handleCharInAttributeValue(c);
                                state = MetaScanner.ATTRIBUTE_VALUE_UNQUOTED;
                                goto stateloop_continue;
                        }
                    }
            beforeattributevalueloop_break: goto case ATTRIBUTE_VALUE_DOUBLE_QUOTED;
                    // FALLTHRU DON'T REORDER
                case ATTRIBUTE_VALUE_DOUBLE_QUOTED:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '"':
                                handleAttributeValue();
                                state = MetaScanner.AFTER_ATTRIBUTE_VALUE_QUOTED;
                                goto attributevaluedoublequotedloop_break;
                            // continue stateloop;
                            default:
                                handleCharInAttributeValue(c);
                                continue;
                        }
                    }
            attributevaluedoublequotedloop_break: goto case AFTER_ATTRIBUTE_VALUE_QUOTED;
                    // FALLTHRU DON'T REORDER
                case AFTER_ATTRIBUTE_VALUE_QUOTED:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                state = MetaScanner.BEFORE_ATTRIBUTE_NAME;
                                goto stateloop_continue;
                            case '/':
                                state = MetaScanner.SELF_CLOSING_START_TAG;
                                goto afterattributevaluequotedloop_break;
                            // continue stateloop;
                            case '>':
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                state = MetaScanner.BEFORE_ATTRIBUTE_NAME;
                                reconsume = true;
                                goto stateloop_continue;
                        }
                    }
            afterattributevaluequotedloop_break: goto case SELF_CLOSING_START_TAG;
                    // FALLTHRU DON'T REORDER
                case SELF_CLOSING_START_TAG:
                    c = read();
                    switch (c) {
                        case -1:
                            goto stateloop_break;
                        case '>':
                            if (handleTag()) {
                                goto stateloop_break;
                            }
                            state = MetaScanner.DATA;
                            goto stateloop_continue;
                        default:
                            state = MetaScanner.BEFORE_ATTRIBUTE_NAME;
                            reconsume = true;
                            goto stateloop_continue;
                    }
                    // XXX reorder point
                case ATTRIBUTE_VALUE_UNQUOTED:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':

                            case '\u000C':
                                handleAttributeValue();
                                state = MetaScanner.BEFORE_ATTRIBUTE_NAME;
                                goto stateloop_continue;
                            case '>':
                                handleAttributeValue();
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                handleCharInAttributeValue(c);
                                continue;
                        }
                    }
                    // XXX reorder point
                case AFTER_ATTRIBUTE_NAME:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\u000C':
                                continue;
                            case '/':
                                handleAttributeValue();
                                state = MetaScanner.SELF_CLOSING_START_TAG;
                                goto stateloop_continue;
                            case '=':
                                strBufLen = 0;
                                contentTypeIndex = 0;
                                state = MetaScanner.BEFORE_ATTRIBUTE_VALUE;
                                goto stateloop_continue;
                            case '>':
                                handleAttributeValue();
                                if (handleTag()) {
                                    goto stateloop_break;
                                }
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            case 'c':
                            case 'C':
                                contentIndex = 0;
                                charsetIndex = 0;
                                state = MetaScanner.ATTRIBUTE_NAME;
                                goto stateloop_continue;
                            default:
                                contentIndex = Int32.MaxValue;
                                charsetIndex = Int32.MaxValue;
                                state = MetaScanner.ATTRIBUTE_NAME;
                                goto stateloop_continue;
                        }
                    }
                    // XXX reorder point
                case MARKUP_DECLARATION_OPEN:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '-':
                                state = MetaScanner.MARKUP_DECLARATION_HYPHEN;
                                goto markupdeclarationopenloop_break;
                            // continue stateloop;
                            default:
                                state = MetaScanner.SCAN_UNTIL_GT;
                                reconsume = true;
                                goto stateloop_continue;
                        }
                    }
            markupdeclarationopenloop_break: goto case MARKUP_DECLARATION_HYPHEN;
                    // FALLTHRU DON'T REORDER
                case MARKUP_DECLARATION_HYPHEN:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '-':
                                state = MetaScanner.COMMENT_START;
                                goto markupdeclarationhyphenloop_break;
                            // continue stateloop;
                            default:
                                state = MetaScanner.SCAN_UNTIL_GT;
                                reconsume = true;
                                goto stateloop_continue;
                        }
                    }
            markupdeclarationhyphenloop_break: goto case COMMENT_START;
                    // FALLTHRU DON'T REORDER
                case COMMENT_START:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '-':
                                state = MetaScanner.COMMENT_START_DASH;
                                goto stateloop_continue;
                            case '>':
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                state = MetaScanner.COMMENT;
                                goto commentstartloop_break;
                            // continue stateloop;
                        }
                    }
            commentstartloop_break: goto case COMMENT;
                    // FALLTHRU DON'T REORDER
                case COMMENT:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '-':
                                state = MetaScanner.COMMENT_END_DASH;
                                goto commentloop_break;
                            // continue stateloop;
                            default:
                                continue;
                        }
                    }
            commentloop_break: goto case COMMENT_END_DASH;
                    // FALLTHRU DON'T REORDER
                case COMMENT_END_DASH:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '-':
                                state = MetaScanner.COMMENT_END;
                                goto commentenddashloop_break;
                            // continue stateloop;
                            default:
                                state = MetaScanner.COMMENT;
                                goto stateloop_continue;
                        }
                    }
            commentenddashloop_break: goto case COMMENT_END;
                    // FALLTHRU DON'T REORDER
                case COMMENT_END:
                    for (;;) {
                        c = read();
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '>':
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            case '-':
                                continue;
                            default:
                                state = MetaScanner.COMMENT;
                                goto stateloop_continue;
                        }
                    }
                    // XXX reorder point
                case COMMENT_START_DASH:
                    c = read();
                    switch (c) {
                        case -1:
                            goto stateloop_break;
                        case '-':
                            state = MetaScanner.COMMENT_END;
                            goto stateloop_continue;
                        case '>':
                            state = MetaScanner.DATA;
                            goto stateloop_continue;
                        default:
                            state = MetaScanner.COMMENT;
                            goto stateloop_continue;
                    }
                    // XXX reorder point
                case ATTRIBUTE_VALUE_SINGLE_QUOTED:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '\'':
                                handleAttributeValue();
                                state = MetaScanner.AFTER_ATTRIBUTE_VALUE_QUOTED;
                                goto stateloop_continue;
                            default:
                                handleCharInAttributeValue(c);
                                continue;
                        }
                    }
                    // XXX reorder point
                case SCAN_UNTIL_GT:
                    for (;;) {
                        if (reconsume) {
                            reconsume = false;
                        } else {
                            c = read();
                        }
                        switch (c) {
                            case -1:
                                goto stateloop_break;
                            case '>':
                                state = MetaScanner.DATA;
                                goto stateloop_continue;
                            default:
                                continue;
                        }
                    }
            }
        stateloop_continue: { }
        }
    stateloop_break: 
        stateSave  = state;
    }

    private void handleCharInAttributeValue(int c) {
        if (metaState == A) {
            if (contentIndex == CONTENT.Length || charsetIndex == CHARSET.Length) {
                addToBuffer(c);
            } else if (httpEquivIndex == HTTP_EQUIV.Length) {
                if (contentTypeIndex < CONTENT_TYPE.Length && toAsciiLowerCase(c) == CONTENT_TYPE[contentTypeIndex]) {
                    ++contentTypeIndex;
                } else {
                    contentTypeIndex = Int32.MaxValue;
                }
            }
        }
    }

    private int toAsciiLowerCase(int c) {
        if (c >= 'A' && c <= 'Z') {
            return c + 0x20;
        }
        return c;
    }

    /**
     * Adds a character to the accumulation buffer.
     * @param c the character to add
     */
    private void addToBuffer(int c) {
        if (strBufLen == strBuf.Length) {
            char[] newBuf = new char[strBuf.Length + (strBuf.Length << 1)];
            Array.Copy(strBuf, 0, newBuf, 0, strBuf.Length);
            strBuf = newBuf;
        }
        strBuf[strBufLen++] = (char)c;
    }

    /**
     * Attempts to extract a charset name from the accumulation buffer.
     * @return <code>true</code> if successful
     * @throws SAXException
     */
    private void handleAttributeValue() {
        if (metaState != A) {
            return;
        }
        if (contentIndex == CONTENT.Length && content == null) {
            content = Portability.newStringFromBuffer(strBuf, 0, strBufLen);
            return;
        }
        if (charsetIndex == CHARSET.Length && charset == null) {
            charset = Portability.newStringFromBuffer(strBuf, 0, strBufLen);            
            return;
        }
        if (httpEquivIndex == HTTP_EQUIV.Length
                && httpEquivState == HTTP_EQUIV_NOT_SEEN) {
            httpEquivState = (contentTypeIndex == CONTENT_TYPE.Length) ? HTTP_EQUIV_CONTENT_TYPE
                    : HTTP_EQUIV_OTHER;
            return;
        }
    }

    private bool handleTag() {
        bool stop = handleTagInner();
        Portability.releaseString(content);
        content = null;
        Portability.releaseString(charset);
        charset = null;
        httpEquivState = HTTP_EQUIV_NOT_SEEN;
        return stop;
    }
    
    private bool handleTagInner() {
        if (charset != null && tryCharset(charset)) {
                return true;
        }
        if (content != null && httpEquivState == HTTP_EQUIV_CONTENT_TYPE) {
            String extract = TreeBuilderBase.extractCharsetFromContent(content);
            if (extract == null) {
                return false;
            }
            bool success = tryCharset(extract);
            Portability.releaseString(extract);
            return success;
        }
        return false;
    }

    /**
     * Tries to switch to an encoding.
     * 
     * @param encoding
     * @return <code>true</code> if successful
     * @throws SAXException
     */
    protected abstract bool tryCharset(String encoding);
}
