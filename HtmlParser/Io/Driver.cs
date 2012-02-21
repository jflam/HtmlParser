/*
 * Copyright (c) 2005, 2006, 2007 Henri Sivonen
 * Copyright (c) 2007-2008 Mozilla Foundation
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
using System.Diagnostics;

public class Driver : EncodingDeclarationHandler {

    /**
     * The input UTF-16 code unit stream. If a byte stream was given, this
     * object is an instance of <code>HtmlInputStreamReader</code>.
     */
    private Reader reader;

    /**
     * The reference to the rewindable byte stream. <code>null</code> if p
     * rohibited or no longer needed.
     */
    private RewindableInputStream rewindableInputStream;

    private bool swallowBom;

    private Encoding characterEncoding;

    private bool allowRewinding = true;

    private Heuristics heuristics = Heuristics.NONE;
    
    private readonly Tokenizer tokenizer;
    
    private Confidence confidence;

    /**
     * Used for NFC checking if non-<code>null</code>, source code capture,
     * etc.
     */
    private CharacterHandler[] characterHandlers = new CharacterHandler[0];

    public Driver(Tokenizer tokenizer) {
        this.tokenizer = tokenizer;
        tokenizer.setEncodingDeclarationHandler(this);
    }
    
    /**
     * Returns the allowRewinding.
     * 
     * @return the allowRewinding
     */
    public bool isAllowRewinding() {
        return allowRewinding;
    }

    /**
     * Sets the allowRewinding.
     * 
     * @param allowRewinding
     *            the allowRewinding to set
     */
    public void setAllowRewinding(bool allowRewinding) {
        this.allowRewinding = allowRewinding;
    }

    /**
     * Turns NFC checking on or off.
     * 
     * @param enable
     *            <code>true</code> if checking on
     */
    public void setCheckingNormalization(bool enable) {
        if (enable) {
            if (isCheckingNormalization()) {
                return;
            } else {
                NormalizationChecker normalizationChecker = new NormalizationChecker(tokenizer);
                normalizationChecker.setErrorHandler(tokenizer.getErrorHandler());

            }
        } else {
            if (isCheckingNormalization()) {
                CharacterHandler[] newHandlers = new CharacterHandler[characterHandlers.Length - 1];
                bool skipped = false;
                int j = 0;
                for (int i = 0; i < characterHandlers.Length; i++) {
                    CharacterHandler ch = characterHandlers[i];
                    if (!(!skipped && (ch is NormalizationChecker))) {
                        newHandlers[j] = ch;
                        j++;
                    }
                }
                characterHandlers = newHandlers;
            } else {
                return;
            }
        }
    }

    public void addCharacterHandler(CharacterHandler characterHandler) {
        if (characterHandler == null) {
            throw new ArgumentNullException("characterHandler");
        }
        CharacterHandler[] newHandlers = new CharacterHandler[characterHandlers.Length + 1];
        Array.Copy(characterHandlers, 0, newHandlers, 0, characterHandlers.Length);
        newHandlers[characterHandlers.Length] = characterHandler;
        characterHandlers = newHandlers;
    }

    /**
     * Query if checking normalization.
     * 
     * @return <code>true</code> if checking on
     */
    public bool isCheckingNormalization() {
        for (int i = 0; i < characterHandlers.Length; i++) {
            CharacterHandler ch = characterHandlers[i];
            if (ch is NormalizationChecker) {
                return true;
            }
        }
        return false;
    }

    /**
     * Runs the tokenization. This is the main entry point.
     * 
     * @param is
     *            the input source
     * @throws SAXException
     *             on fatal error (if configured to treat XML violations as
     *             fatal) or if the token handler threw
     * @throws IOException
     *             if the stream threw
     */
    public void tokenize(InputSource inputSource) {
        if (inputSource == null) {
            throw new IllegalArgumentException("InputSource was null.");
        }
        tokenizer.start();
        confidence = Confidence.TENTATIVE;
        swallowBom = true;
        rewindableInputStream = null;
        tokenizer.initLocation(inputSource.getPublicId(), inputSource.getSystemId());
        this.reader = inputSource.getCharacterStream();
        this.characterEncoding = encodingFromExternalDeclaration(inputSource.getEncoding());
        if (this.reader == null) {
            InputStream inputStream = inputSource.getByteStream();
            if (inputStream == null) {
                throw new SAXException("Both streams in InputSource were null.");
            }
            if (this.characterEncoding == null) {
                if (allowRewinding) {
                    inputStream = rewindableInputStream = new RewindableInputStream(
                            inputStream);
                }
                this.reader = new HtmlInputStreamReader(inputStream,
                        tokenizer.getErrorHandler(), tokenizer, this, heuristics);
            } else {
                becomeConfident();
                this.reader = new HtmlInputStreamReader(inputStream,
                        tokenizer.getErrorHandler(), tokenizer, this, this.characterEncoding);
            }
        } else {
            becomeConfident();
        }
        Throwable t = null;
        try {
            for (;;) {
                try {
                    for (int i = 0; i < characterHandlers.length; i++) {
                        CharacterHandler ch = characterHandlers[i];
                        ch.start();
                    }
                    runStates();
                    if (confidence == Confidence.TENTATIVE
                            && !tokenizer.isAlreadyComplainedAboutNonAscii()) {
                        warnWithoutLocation("The character encoding of the document was not declared.");
                    }
                    break;
                } catch (ReparseException e) {
                    if (rewindableInputStream == null) {
                        tokenizer.fatal("Changing encoding at this point would need non-streamable behavior.");
                    } else {
                        rewindableInputStream.rewind();
                        becomeConfident();
                        this.reader = new HtmlInputStreamReader(
                                rewindableInputStream, tokenizer.getErrorHandler(), tokenizer,
                                this, this.characterEncoding);
                    }
                    continue;
                }
            }
        } catch (Throwable tr) {
            t = tr;
        } finally {
            try {
                tokenizer.end();
                characterEncoding = null;
                for (int i = 0; i < characterHandlers.Length; i++) {
                    CharacterHandler ch = characterHandlers[i];
                    ch.end();
                }
                reader.close();
                reader = null;
                rewindableInputStream = null;
            } catch (Throwable tr) {
                if (t == null) {
                    t = tr;
                } // else drop the later throwable
            }
            if (t != null) {
                if (t is IOException) {
                    throw (IOException) t;
                } else if (t is SAXException) {
                    throw (SAXException) t;
                } else if (t is RuntimeException) {
                    throw (RuntimeException) t;
                } else if (t is Error) {
                    throw (Error) t;
                } else {
                    // impossible
                    throw new RuntimeException(t);
                }
            }
        }
    }

    void dontSwallowBom() {
        swallowBom = false;
    }

    private void runStates() {
        char[] buffer = new char[2048];
        UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
        bool lastWasCR = false;
        int len = -1;
        if ((len = reader.read(buffer)) != -1) {
            Debug.Assert(len > 0);
            int streamOffset = 0;
            int offset = 0;
            int length = len;
            if (swallowBom) {
                if (buffer[0] == '\uFEFF') {
                    streamOffset = -1;
                    offset = 1;
                    length--;
                }
            }
            if (length > 0) {
                for (int i = 0; i < characterHandlers.Length; i++) {
                    CharacterHandler ch = characterHandlers[i];
                    ch.characters(buffer, offset, length);
                }
                tokenizer.setTransitionBaseOffset(streamOffset);
                bufr.setStart(offset);
                bufr.setEnd(offset + length);
                while (bufr.hasMore()) {
                    bufr.adjust(lastWasCR);
                    lastWasCR = false;
                    if (bufr.hasMore()) {
                        lastWasCR = tokenizer.tokenizeBuffer(bufr);                    
                    }
                }
            }
            streamOffset = length;
            while ((len = reader.read(buffer)) != -1) {
                Debug.Assert(len > 0);
                for (int i = 0; i < characterHandlers.Length; i++) {
                    CharacterHandler ch = characterHandlers[i];
                    ch.characters(buffer, 0, len);
                }
                tokenizer.setTransitionBaseOffset(streamOffset);
                bufr.setStart(0);
                bufr.setEnd(len);
                while (bufr.hasMore()) {
                    bufr.adjust(lastWasCR);
                    lastWasCR = false;
                    if (bufr.hasMore()) {
                        lastWasCR = tokenizer.tokenizeBuffer(bufr);                    
                    }
                }
                streamOffset += len;
            }
        }
        tokenizer.eof();
    }

    public void setEncoding(Encoding encoding, Confidence confidence) {
        this.characterEncoding = encoding;
        if (confidence == Confidence.CERTAIN) {
            becomeConfident();
        }
    }

    public bool internalEncodingDeclaration(String internalCharset) {
        try {
            internalCharset = Encoding.toAsciiLowerCase(internalCharset);
            Encoding cs;
            if ("utf-16".Equals(internalCharset)
                    || "utf-16be".Equals(internalCharset)
                    || "utf-16le".Equals(internalCharset)) {
                tokenizer.errTreeBuilder("Internal encoding declaration specified \u201C"
                        + internalCharset
                        + "\u201D which is not an ASCII superset. Continuing as if the encoding had been \u201Cutf-8\u201D.");
                cs = Encoding.UTF8;
                internalCharset = "utf-8";
            } else {
                cs = Encoding.forName(internalCharset);
            }
            Encoding actual = cs.getActualHtmlEncoding();
            if (actual == null) {
                actual = cs;
            }
            if (!actual.isAsciiSuperset()) {
                tokenizer.errTreeBuilder("Internal encoding declaration specified \u201C"
                        + internalCharset
                        + "\u201D which is not an ASCII superset. Not changing the encoding.");
                return false;
            }
            if (characterEncoding == null) {
                // Reader case
                return true;
            }
            if (characterEncoding == actual) {
                becomeConfident();
                return true;
            }
            if (confidence == Confidence.CERTAIN && actual != characterEncoding) {
                tokenizer.errTreeBuilder("Internal encoding declaration \u201C"
                        + internalCharset
                        + "\u201D disagrees with the actual encoding of the document (\u201C"
                        + characterEncoding.getCanonName() + "\u201D).");
            } else {
                Encoding newEnc = whineAboutEncodingAndReturnActual(
                        internalCharset, cs);
                tokenizer.errTreeBuilder("Changing character encoding \u201C"
                        + internalCharset + "\u201D and reparsing.");
                characterEncoding = newEnc;
                throw new ReparseException();
            }
            return true;
        } catch (UnsupportedCharsetException e) {
            tokenizer.errTreeBuilder("Internal encoding declaration named an unsupported chararacter encoding \u201C"
                    + internalCharset + "\u201D.");
            return false;
        }
    }

    /**
     * 
     */
    private void becomeConfident() {
        if (rewindableInputStream != null) {
            rewindableInputStream.willNotRewind();
        }
        confidence = Confidence.CERTAIN;
        tokenizer.becomeConfident();
    }

    /**
     * Sets the encoding sniffing heuristics.
     * 
     * @param heuristics
     *            the heuristics to set
     */
    public void setHeuristics(Heuristics heuristics) {
        this.heuristics = heuristics;
    }

    /**
     * Reports a warning without line/col
     * 
     * @param message
     *            the message
     * @throws SAXException
     */
    protected void warnWithoutLocation(String message) {
        ErrorHandler errorHandler = tokenizer.getErrorHandler();
        if (errorHandler == null) {
            return;
        }
        SAXParseException spe = new SAXParseException(message, null, tokenizer.getSystemId(), -1, -1);
        errorHandler.warning(spe);
    }

    /**
     * Initializes a decoder from external decl.
     */
    protected Encoding encodingFromExternalDeclaration(String encoding) {
        if (encoding == null) {
            return null;
        }
        encoding = Encoding.toAsciiLowerCase(encoding);
        try {
            Encoding cs = Encoding.forName(encoding);
            if ("utf-16".Equals(cs.getCanonName())
                    || "utf-32".Equals(cs.getCanonName())) {
                swallowBom = false;
            }
            return whineAboutEncodingAndReturnActual(encoding, cs);
        } catch (UnsupportedCharsetException e) {
            tokenizer.err("Unsupported character encoding name: \u201C" + encoding
                    + "\u201D. Will sniff.");
            swallowBom = true;
        }
        return null; // keep the compiler happy
    }

    /**
     * @param encoding
     * @param cs
     * @return
     * @throws SAXException
     */
    protected Encoding whineAboutEncodingAndReturnActual(String encoding, Encoding cs) {
        String canonName = cs.getCanonName();
        if (!cs.isRegistered()) {
            if (encoding.StartsWith("x-")) {
                tokenizer.err("The encoding \u201C"
                        + encoding
                        + "\u201D is not an IANA-registered encoding. (Charmod C022)");
            } else {
                tokenizer.err("The encoding \u201C"
                        + encoding
                        + "\u201D is not an IANA-registered encoding and did not use the \u201Cx-\u201D prefix. (Charmod C023)");
            }
        } else if (!canonName.Equals(encoding)) {
            tokenizer.err("The encoding \u201C"
                    + encoding
                    + "\u201D is not the preferred name of the character encoding in use. The preferred name is \u201C"
                    + canonName + "\u201D. (Charmod C024)");
        }
        if (cs.isShouldNot()) {
            tokenizer.warn("Authors should not use the character encoding \u201C"
                    + encoding
                    + "\u201D. It is recommended to use \u201CUTF-8\u201D.");
        } else if (cs.isLikelyEbcdic()) {
            tokenizer.warn("Authors should not use EBCDIC-based encodings. It is recommended to use \u201CUTF-8\u201D.");
        } else if (cs.isObscure()) {
            tokenizer.warn("The character encoding \u201C"
                    + encoding
                    + "\u201D is not widely supported. Better interoperability may be achieved by using \u201CUTF-8\u201D.");
        }
        Encoding actual = cs.getActualHtmlEncoding();
        if (actual == null) {
            return cs;
        } else {
            tokenizer.warn("Using \u201C" + actual.getCanonName()
                    + "\u201D instead of the declared encoding \u201C"
                    + encoding + "\u201D.");
            return actual;
        }
    }

    private class ReparseException : SAXException {

    }

    public void notifyAboutMetaBoundary() {
        tokenizer.notifyAboutMetaBoundary();
    }

    /**
     * @param commentPolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setCommentPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setCommentPolicy(XmlViolationPolicy commentPolicy) {
        tokenizer.setCommentPolicy(commentPolicy);
    }

    /**
     * @param contentNonXmlCharPolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setContentNonXmlCharPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setContentNonXmlCharPolicy(
            XmlViolationPolicy contentNonXmlCharPolicy) {
        tokenizer.setContentNonXmlCharPolicy(contentNonXmlCharPolicy);
    }

    /**
     * @param contentSpacePolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setContentSpacePolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setContentSpacePolicy(XmlViolationPolicy contentSpacePolicy) {
        tokenizer.setContentSpacePolicy(contentSpacePolicy);
    }

    /**
     * @param eh
     * @see nu.validator.htmlparser.impl.Tokenizer#setErrorHandler(org.xml.sax.ErrorHandler)
     */
    public void setErrorHandler(ErrorHandler eh) {
        tokenizer.setErrorHandler(eh);
        for (int i = 0; i < characterHandlers.Length; i++) {
            CharacterHandler ch = characterHandlers[i];
            if (ch is NormalizationChecker) {
                NormalizationChecker nc = (NormalizationChecker) ch;
                nc.setErrorHandler(eh);
            }
        }
    }
    
    public void setTransitionHandler(TransitionHandler transitionHandler) {
        if (tokenizer is ErrorReportingTokenizer) {
            ErrorReportingTokenizer ert = (ErrorReportingTokenizer) tokenizer;
            ert.setTransitionHandler(transitionHandler);
        } else if (transitionHandler != null) {
            throw new IllegalStateException("Attempt to set a transition handler on a plain tokenizer.");
        }
    }

    /**
     * @param html4ModeCompatibleWithXhtml1Schemata
     * @see nu.validator.htmlparser.impl.Tokenizer#setHtml4ModeCompatibleWithXhtml1Schemata(bool)
     */
    public void setHtml4ModeCompatibleWithXhtml1Schemata(
            bool html4ModeCompatibleWithXhtml1Schemata) {
        tokenizer.setHtml4ModeCompatibleWithXhtml1Schemata(html4ModeCompatibleWithXhtml1Schemata);
    }

    /**
     * @param mappingLangToXmlLang
     * @see nu.validator.htmlparser.impl.Tokenizer#setMappingLangToXmlLang(bool)
     */
    public void setMappingLangToXmlLang(bool mappingLangToXmlLang) {
        tokenizer.setMappingLangToXmlLang(mappingLangToXmlLang);
    }

    /**
     * @param namePolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setNamePolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setNamePolicy(XmlViolationPolicy namePolicy) {
        tokenizer.setNamePolicy(namePolicy);
    }

    /**
     * @param xmlnsPolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setXmlnsPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setXmlnsPolicy(XmlViolationPolicy xmlnsPolicy) {
        tokenizer.setXmlnsPolicy(xmlnsPolicy);
    }
 
    public String getCharacterEncoding() {
        return characterEncoding.getCanonName();
    }

    public Locator getDocumentLocator() {
        return tokenizer;
    }
}