/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2007-2010 Mozilla Foundation
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

/**
 * This class implements an HTML5 parser that exposes data through the SAX2 
 * interface. 
 * 
 * <p>By default, when using the constructor without arguments, the 
 * this parser coerces XML 1.0-incompatible infosets into XML 1.0-compatible
 * infosets. This corresponds to <code>ALTER_INFOSET</code> as the general 
 * XML violation policy. To make the parser support non-conforming HTML fully 
 * per the HTML 5 spec while on the other hand potentially violating the SAX2 
 * API contract, set the general XML violation policy to <code>ALLOW</code>. 
 * It is possible to treat XML 1.0 infoset violations as fatal by setting 
 * the general XML violation policy to <code>FATAL</code>. 
 * 
 * <p>By default, this parser doesn't do true streaming but buffers everything 
 * first. The parser can be made truly streaming by calling 
 * <code>setStreamabilityViolationPolicy(XmlViolationPolicy.FATAL)</code>. This 
 * has the consequence that errors that require non-streamable recovery are 
 * treated as fatal.
 * 
 * <p>By default, in order to make the parse events emulate the parse events 
 * for a DTDless XML document, the parser does not report the doctype through 
 * <code>LexicalHandler</code>. Doctype reporting through 
 * <code>LexicalHandler</code> can be turned on by calling 
 * <code>setReportingDoctype(true)</code>.
 * 
 * @version $Id$
 * @author hsivonen
 */
using System;
using System.Collections.Generic;

public class HtmlParser<T> : XMLReader {

    private Driver driver = null;

    private TreeBuilder<T> treeBuilder = null;

    private SAXStreamer saxStreamer = null; // work around javac bug

    private SAXTreeBuilder saxTreeBuilder = null; // work around javac bug

    private ContentHandler contentHandler = null;

    private LexicalHandler lexicalHandler = null;

    private DTDHandler dtdHandler = null;

    private EntityResolver entityResolver = null;

    private ErrorHandler errorHandler = null;

    private DocumentModeHandler documentModeHandler = null;

    private DoctypeExpectation doctypeExpectation = DoctypeExpectation.HTML;

    private bool checkingNormalization = false;

    private bool scriptingEnabled = false;

    private readonly List<CharacterHandler> characterHandlers = new LinkedList<CharacterHandler>();
    
    private XmlViolationPolicy contentSpacePolicy = XmlViolationPolicy.FATAL;

    private XmlViolationPolicy contentNonXmlCharPolicy = XmlViolationPolicy.FATAL;

    private XmlViolationPolicy commentPolicy = XmlViolationPolicy.FATAL;

    private XmlViolationPolicy namePolicy = XmlViolationPolicy.FATAL;

    private XmlViolationPolicy streamabilityViolationPolicy = XmlViolationPolicy.ALLOW;
    
    private bool html4ModeCompatibleWithXhtml1Schemata = false;

    private bool mappingLangToXmlLang = false;

    private XmlViolationPolicy xmlnsPolicy = XmlViolationPolicy.FATAL;
    
    private bool reportingDoctype = true;

    private ErrorHandler treeBuilderErrorHandler = null;

    private Heuristics heuristics = Heuristics.NONE;

    private Dictionary<String, String> errorProfileMap = null;

    private TransitionHandler transitionHandler = null;
    
    /**
     * Instantiates the parser with a fatal XML violation policy.
     *
     */
    public HtmlParser() : base(XmlViolationPolicy.FATAL) {
    }
    
    /**
     * Instantiates the parser with a specific XML violation policy.
     * @param xmlPolicy the policy
     */
    public HtmlParser(XmlViolationPolicy xmlPolicy) {
        setXmlPolicy(xmlPolicy);
    }    

    private Tokenizer newTokenizer(TokenHandler handler, bool newAttributesEachTime) {
        if (errorHandler == null && transitionHandler == null &&
            contentNonXmlCharPolicy == XmlViolationPolicy.ALLOW) {
            return new Tokenizer(handler, newAttributesEachTime);
        }
        ErrorReportingTokenizer tokenizer = 
            new ErrorReportingTokenizer(handler, newAttributesEachTime);
        tokenizer.setErrorProfile(errorProfileMap);
        return tokenizer;
   }
    
    /**
     * This class wraps different tree builders depending on configuration. This 
     * method does the work of hiding this from the user of the class.
     */
    private void lazyInit() {
        if (driver == null) {
            if (streamabilityViolationPolicy == XmlViolationPolicy.ALLOW) {
                this.saxTreeBuilder = new SAXTreeBuilder();
                this.treeBuilder = this.saxTreeBuilder;
                this.saxStreamer = null;
                this.driver = new Driver(newTokenizer(treeBuilder, true));
            } else {
                this.saxStreamer = new SAXStreamer();
                this.treeBuilder = this.saxStreamer;
                this.saxTreeBuilder = null;
                this.driver = new Driver(newTokenizer(treeBuilder, false));
            }
            this.driver.setErrorHandler(errorHandler);
            this.driver.setTransitionHandler(transitionHandler);
            this.treeBuilder.setErrorHandler(treeBuilderErrorHandler);
            this.driver.setCheckingNormalization(checkingNormalization);
            this.driver.setCommentPolicy(commentPolicy);
            this.driver.setContentNonXmlCharPolicy(contentNonXmlCharPolicy);
            this.driver.setContentSpacePolicy(contentSpacePolicy);
            this.driver.setHtml4ModeCompatibleWithXhtml1Schemata(html4ModeCompatibleWithXhtml1Schemata);
            this.driver.setMappingLangToXmlLang(mappingLangToXmlLang);
            this.driver.setXmlnsPolicy(xmlnsPolicy);
            this.driver.setHeuristics(heuristics);
            foreach (CharacterHandler characterHandler in characterHandlers) {
                this.driver.addCharacterHandler(characterHandler);
            }
            this.treeBuilder.setDoctypeExpectation(doctypeExpectation);
            this.treeBuilder.setDocumentModeHandler(documentModeHandler);
            this.treeBuilder.setIgnoringComments(lexicalHandler == null);
            this.treeBuilder.setScriptingEnabled(scriptingEnabled);
            this.treeBuilder.setReportingDoctype(reportingDoctype);
            this.treeBuilder.setNamePolicy(namePolicy);
            if (saxStreamer != null) {
                saxStreamer.setContentHandler(contentHandler == null ? new DefaultHandler()
                        : contentHandler);
                saxStreamer.setLexicalHandler(lexicalHandler);
                driver.setAllowRewinding(false);
            }
        }
    }

    /**
     * @see org.xml.sax.XMLReader#getContentHandler()
     */
    public ContentHandler getContentHandler() {
        return contentHandler;
    }

    /**
     * @see org.xml.sax.XMLReader#getDTDHandler()
     */
    public DTDHandler getDTDHandler() {
        return dtdHandler;
    }

    /**
     * @see org.xml.sax.XMLReader#getEntityResolver()
     */
    public EntityResolver getEntityResolver() {
        return entityResolver;
    }

    /**
     * @see org.xml.sax.XMLReader#getErrorHandler()
     */
    public ErrorHandler getErrorHandler() {
        return errorHandler;
    }

    /**
     * Exposes the configuration of the emulated XML parser as well as
     * bool-valued configuration without using non-<code>XMLReader</code>
     * getters directly.
     * 
     * <dl>
     * <dt><code>http://xml.org/sax/features/external-general-entities</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/external-parameter-entities</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/is-standalone</code></dt>
     * <dd><code>true</code></dd>
     * <dt><code>http://xml.org/sax/features/lexical-handler/parameter-entities</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/namespaces</code></dt>
     * <dd><code>true</code></dd>
     * <dt><code>http://xml.org/sax/features/namespace-prefixes</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/resolve-dtd-uris</code></dt>
     * <dd><code>true</code></dd>
     * <dt><code>http://xml.org/sax/features/string-interning</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/unicode-normalization-checking</code></dt>
     * <dd><code>isCheckingNormalization</code></dd>
     * <dt><code>http://xml.org/sax/features/use-attributes2</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/use-locator2</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/use-entity-resolver2</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/validation</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/xmlns-uris</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://xml.org/sax/features/xml-1.1</code></dt>
     * <dd><code>false</code></dd>
     * <dt><code>http://validator.nu/features/html4-mode-compatible-with-xhtml1-schemata</code></dt>
     * <dd><code>isHtml4ModeCompatibleWithXhtml1Schemata</code></dd>
     * <dt><code>http://validator.nu/features/mapping-lang-to-xml-lang</code></dt>
     * <dd><code>isMappingLangToXmlLang</code></dd>
     * <dt><code>http://validator.nu/features/scripting-enabled</code></dt>
     * <dd><code>isScriptingEnabled</code></dd>
     * </dl>
     * 
     * @param name
     *            feature URI string
     * @return a value per the list above
     * @see org.xml.sax.XMLReader#getFeature(java.lang.String)
     */
    public bool getFeature(String name) {
        if ("http://xml.org/sax/features/external-general-entities".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/external-parameter-entities".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/is-standalone".Equals(name)) {
            return true;
        } else if ("http://xml.org/sax/features/lexical-handler/parameter-entities".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/namespaces".Equals(name)) {
            return true;
        } else if ("http://xml.org/sax/features/namespace-prefixes".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/resolve-dtd-uris".Equals(name)) {
            return true; // default value--applicable scenario never happens
        } else if ("http://xml.org/sax/features/string-interning".Equals(name)) {
            return true;
        } else if ("http://xml.org/sax/features/unicode-normalization-checking".Equals(name)) {
            return isCheckingNormalization(); // the checks aren't really per
            // XML 1.1
        } else if ("http://xml.org/sax/features/use-attributes2".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/use-locator2".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/use-entity-resolver2".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/validation".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/xmlns-uris".Equals(name)) {
            return false;
        } else if ("http://xml.org/sax/features/xml-1.1".Equals(name)) {
            return false;
        } else if ("http://validator.nu/features/html4-mode-compatible-with-xhtml1-schemata".Equals(name)) {
            return isHtml4ModeCompatibleWithXhtml1Schemata();
        } else if ("http://validator.nu/features/mapping-lang-to-xml-lang".Equals(name)) {
            return isMappingLangToXmlLang();
        } else if ("http://validator.nu/features/scripting-enabled".Equals(name)) {
            return isScriptingEnabled();
        } else {
            throw new SAXNotRecognizedException();
        }
    }

    /**
     * Allows <code>XMLReader</code>-level access to non-bool valued
     * getters.
     * 
     * <p>
     * The properties are mapped as follows:
     * 
     * <dl>
     * <dt><code>http://xml.org/sax/properties/document-xml-version</code></dt>
     * <dd><code>"1.0"</code></dd>
     * <dt><code>http://xml.org/sax/properties/lexical-handler</code></dt>
     * <dd><code>getLexicalHandler</code></dd>
     * <dt><code>http://validator.nu/properties/content-space-policy</code></dt>
     * <dd><code>getContentSpacePolicy</code></dd>
     * <dt><code>http://validator.nu/properties/content-non-xml-char-policy</code></dt>
     * <dd><code>getContentNonXmlCharPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/comment-policy</code></dt>
     * <dd><code>getCommentPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/xmlns-policy</code></dt>
     * <dd><code>getXmlnsPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/name-policy</code></dt>
     * <dd><code>getNamePolicy</code></dd>
     * <dt><code>http://validator.nu/properties/streamability-violation-policy</code></dt>
     * <dd><code>getStreamabilityViolationPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/document-mode-handler</code></dt>
     * <dd><code>getDocumentModeHandler</code></dd>
     * <dt><code>http://validator.nu/properties/doctype-expectation</code></dt>
     * <dd><code>getDoctypeExpectation</code></dd>
     * <dt><code>http://xml.org/sax/features/unicode-normalization-checking</code></dt>
     * </dl>
     * 
     * @param name
     *            property URI string
     * @return a value per the list above
     * @see org.xml.sax.XMLReader#getProperty(java.lang.String)
     */
    public Object getProperty(String name) {
        if ("http://xml.org/sax/properties/declaration-handler".Equals(name)) {
            throw new SAXNotSupportedException(
                    "This parser does not suppert DeclHandler.");
        } else if ("http://xml.org/sax/properties/document-xml-version".Equals(name)) {
            return "1.0"; // Emulating an XML 1.1 parser is not supported.
        } else if ("http://xml.org/sax/properties/dom-node".Equals(name)) {
            throw new SAXNotSupportedException(
                    "This parser does not walk the DOM.");
        } else if ("http://xml.org/sax/properties/lexical-handler".Equals(name)) {
            return getLexicalHandler();
        } else if ("http://xml.org/sax/properties/xml-string".Equals(name)) {
            throw new SAXNotSupportedException(
                    "This parser does not expose the source as a string.");
        } else if ("http://validator.nu/properties/content-space-policy".Equals(name)) {
            return getContentSpacePolicy();
        } else if ("http://validator.nu/properties/content-non-xml-char-policy".Equals(name)) {
            return getContentNonXmlCharPolicy();
        } else if ("http://validator.nu/properties/comment-policy".Equals(name)) {
            return getCommentPolicy();
        } else if ("http://validator.nu/properties/xmlns-policy".Equals(name)) {
            return getXmlnsPolicy();
        } else if ("http://validator.nu/properties/name-policy".Equals(name)) {
            return getNamePolicy();
        } else if ("http://validator.nu/properties/streamability-violation-policy".Equals(name)) {
            return getStreamabilityViolationPolicy();
        } else if ("http://validator.nu/properties/document-mode-handler".Equals(name)) {
            return getDocumentModeHandler();
        } else if ("http://validator.nu/properties/doctype-expectation".Equals(name)) {
            return getDoctypeExpectation();
        } else if ("http://validator.nu/properties/xml-policy".Equals(name)) {
            throw new SAXNotSupportedException(
                    "Cannot get a convenience setter.");
        } else if ("http://validator.nu/properties/heuristics".Equals(name)) {
            return getHeuristics();
        } else {
            throw new SAXNotRecognizedException();
        }
    }

    /**
     * @see org.xml.sax.XMLReader#parse(org.xml.sax.InputSource)
     */
    public void parse(InputSource input) {
        lazyInit();
        try {
            treeBuilder.setFragmentContext(null);
            tokenize(input);
        } finally {
            if (saxTreeBuilder != null) {
                Document document = saxTreeBuilder.getDocument();
                if (document != null) {
                    new TreeParser(contentHandler, lexicalHandler).parse(document);
                }
            }
        }
    }

    /**
     * Parses a fragment.
     * 
     * @param input the input to parse
     * @param context the name of the context element
     * @throws IOException
     * @throws SAXException
     */
    public void parseFragment(InputSource input, String context) {
        lazyInit();
        try {
            treeBuilder.setFragmentContext(context);
            tokenize(input);
        } finally {
            if (saxTreeBuilder != null) {
                DocumentFragment fragment = saxTreeBuilder.getDocumentFragment();
                new TreeParser(contentHandler, lexicalHandler).parse(fragment);
            }
        }
    }
    
    /**
     * @param is
     * @throws SAXException
     * @throws IOException
     * @throws MalformedURLException
     */
    private void tokenize(InputSource inputSource) {
        if (inputSource == null) {
            throw new ArgumentNullException("inputSource");            
        }
        if (inputSource.getByteStream() == null && inputSource.getCharacterStream() == null) {
            String systemId = inputSource.getSystemId();
            if (systemId == null) {
                throw new ArgumentException("No byte stream, no character stream nor URI.");
            }
            if (entityResolver != null) {
                inputSource = entityResolver.resolveEntity(inputSource.getPublicId(), systemId);
            }
            if (inputSource.getByteStream() == null || inputSource.getCharacterStream() == null) {
                inputSource = new InputSource();
                inputSource.setSystemId(systemId);
                inputSource.setByteStream(new URL(systemId).openStream());
            }
        }
        driver.tokenize(inputSource);
    }

    /**
     * @see org.xml.sax.XMLReader#parse(java.lang.String)
     */
    public void parse(String systemId) {
        parse(new InputSource(systemId));
    }

    /**
     * @see org.xml.sax.XMLReader#setContentHandler(org.xml.sax.ContentHandler)
     */
    public void setContentHandler(ContentHandler handler) {
        contentHandler = handler;
        if (saxStreamer != null) {
            saxStreamer.setContentHandler(contentHandler == null ? new DefaultHandler()
                    : contentHandler);
        }
    }

    /**
     * Sets the lexical handler.
     * @param handler the hander.
     */
    public void setLexicalHandler(LexicalHandler handler) {
        lexicalHandler = handler;
        if (treeBuilder != null) {
            treeBuilder.setIgnoringComments(handler == null);
            if (saxStreamer != null) {
                saxStreamer.setLexicalHandler(handler);
            }
        }
    }

    /**
     * @see org.xml.sax.XMLReader#setDTDHandler(org.xml.sax.DTDHandler)
     */
    public void setDTDHandler(DTDHandler handler) {
        dtdHandler = handler;
    }

    /**
     * @see org.xml.sax.XMLReader#setEntityResolver(org.xml.sax.EntityResolver)
     */
    public void setEntityResolver(EntityResolver resolver) {
        entityResolver = resolver;
    }

    /**
     * @see org.xml.sax.XMLReader#setErrorHandler(org.xml.sax.ErrorHandler)
     */
    public void setErrorHandler(ErrorHandler handler) {
        errorHandler = handler;
        treeBuilderErrorHandler = handler;
        driver = null;
    }

    public void setTransitionHandler(TransitionHandler handler) {
        transitionHandler = handler;
        driver = null;
    }
    
    /**
     * @see org.xml.sax.XMLReader#setErrorHandler(org.xml.sax.ErrorHandler)
     * @deprecated For Validator.nu internal use
     */
    public void setTreeBuilderErrorHandlerOverride(ErrorHandler handler) {
        treeBuilderErrorHandler = handler;
        if (driver != null) {
            treeBuilder.setErrorHandler(handler);
        }
    }
    
    /**
     * Sets a bool feature without having to use non-<code>XMLReader</code>
     * setters directly.
     * 
     * <p>
     * The supported features are:
     * 
     * <dl>
     * <dt><code>http://xml.org/sax/features/unicode-normalization-checking</code></dt>
     * <dd><code>setCheckingNormalization</code></dd>
     * <dt><code>http://validator.nu/features/html4-mode-compatible-with-xhtml1-schemata</code></dt>
     * <dd><code>setHtml4ModeCompatibleWithXhtml1Schemata</code></dd>
     * <dt><code>http://validator.nu/features/mapping-lang-to-xml-lang</code></dt>
     * <dd><code>setMappingLangToXmlLang</code></dd>
     * <dt><code>http://validator.nu/features/scripting-enabled</code></dt>
     * <dd><code>setScriptingEnabled</code></dd>
     * </dl>
     * 
     * @see org.xml.sax.XMLReader#setFeature(java.lang.String, bool)
     */
    public void setFeature(String name, bool value) {
        if ("http://xml.org/sax/features/external-general-entities".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/external-parameter-entities".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/is-standalone".Equals(name)) {
            if (!value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/lexical-handler/parameter-entities".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/namespaces".Equals(name)) {
            if (!value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/namespace-prefixes".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/resolve-dtd-uris".Equals(name)) {
            if (!value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/string-interning".Equals(name)) {
            if (!value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/unicode-normalization-checking".Equals(name)) {
            setCheckingNormalization(value);
        } else if ("http://xml.org/sax/features/use-attributes2".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/use-locator2".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/use-entity-resolver2".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/validation".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/xmlns-uris".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://xml.org/sax/features/xml-1.1".Equals(name)) {
            if (value) {
                throw new SAXNotSupportedException("Cannot set " + name + ".");
            }
        } else if ("http://validator.nu/features/html4-mode-compatible-with-xhtml1-schemata".Equals(name)) {
            setHtml4ModeCompatibleWithXhtml1Schemata(value);
        } else if ("http://validator.nu/features/mapping-lang-to-xml-lang".Equals(name)) {
            setMappingLangToXmlLang(value);
        } else if ("http://validator.nu/features/scripting-enabled".Equals(name)) {
            setScriptingEnabled(value);
        } else {
            throw new SAXNotRecognizedException();
        }
    }

    /**
     * Sets a non-bool property without having to use non-<code>XMLReader</code>
     * setters directly.
     * 
     * <dl>
     * <dt><code>http://xml.org/sax/properties/lexical-handler</code></dt>
     * <dd><code>setLexicalHandler</code></dd>
     * <dt><code>http://validator.nu/properties/content-space-policy</code></dt>
     * <dd><code>setContentSpacePolicy</code></dd>
     * <dt><code>http://validator.nu/properties/content-non-xml-char-policy</code></dt>
     * <dd><code>setContentNonXmlCharPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/comment-policy</code></dt>
     * <dd><code>setCommentPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/xmlns-policy</code></dt>
     * <dd><code>setXmlnsPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/name-policy</code></dt>
     * <dd><code>setNamePolicy</code></dd>
     * <dt><code>http://validator.nu/properties/streamability-violation-policy</code></dt>
     * <dd><code>setStreamabilityViolationPolicy</code></dd>
     * <dt><code>http://validator.nu/properties/document-mode-handler</code></dt>
     * <dd><code>setDocumentModeHandler</code></dd>
     * <dt><code>http://validator.nu/properties/doctype-expectation</code></dt>
     * <dd><code>setDoctypeExpectation</code></dd>
     * <dt><code>http://validator.nu/properties/xml-policy</code></dt>
     * <dd><code>setXmlPolicy</code></dd>
     * </dl>
     * 
     * @see org.xml.sax.XMLReader#setProperty(java.lang.String,
     *      java.lang.Object)
     */
    public void setProperty(String name, Object value) {
        if ("http://xml.org/sax/properties/declaration-handler".Equals(name)) {
            throw new SAXNotSupportedException(
                    "This parser does not suppert DeclHandler.");
        } else if ("http://xml.org/sax/properties/document-xml-version".Equals(name)) {
            throw new SAXNotSupportedException(
                    "Can't set document-xml-version.");
        } else if ("http://xml.org/sax/properties/dom-node".Equals(name)) {
            throw new SAXNotSupportedException("Can't set dom-node.");
        } else if ("http://xml.org/sax/properties/lexical-handler".Equals(name)) {
            setLexicalHandler((LexicalHandler) value);
        } else if ("http://xml.org/sax/properties/xml-string".Equals(name)) {
            throw new SAXNotSupportedException("Can't set xml-string.");
        } else if ("http://validator.nu/properties/content-space-policy".Equals(name)) {
            setContentSpacePolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/content-non-xml-char-policy".Equals(name)) {
            setContentNonXmlCharPolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/comment-policy".Equals(name)) {
            setCommentPolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/xmlns-policy".Equals(name)) {
            setXmlnsPolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/name-policy".Equals(name)) {
            setNamePolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/streamability-violation-policy".Equals(name)) {
            setStreamabilityViolationPolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/document-mode-handler".Equals(name)) {
            setDocumentModeHandler((DocumentModeHandler) value);
        } else if ("http://validator.nu/properties/doctype-expectation".Equals(name)) {
            setDoctypeExpectation((DoctypeExpectation) value);
        } else if ("http://validator.nu/properties/xml-policy".Equals(name)) {
            setXmlPolicy((XmlViolationPolicy) value);
        } else if ("http://validator.nu/properties/heuristics".Equals(name)) {
            setHeuristics((Heuristics) value);
        } else {
            throw new SAXNotRecognizedException();
        }
    }

    /**
     * Indicates whether NFC normalization of source is being checked.
     * @return <code>true</code> if NFC normalization of source is being checked.
     * @see nu.validator.htmlparser.impl.Tokenizer#isCheckingNormalization()
     */
    public bool isCheckingNormalization() {
        return checkingNormalization;
    }

    /**
     * Toggles the checking of the NFC normalization of source.
     * @param enable <code>true</code> to check normalization
     * @see nu.validator.htmlparser.impl.Tokenizer#setCheckingNormalization(bool)
     */
    public void setCheckingNormalization(bool enable) {
        this.checkingNormalization = enable;
        if (driver != null) {
            driver.setCheckingNormalization(checkingNormalization);
        }
    }

    /**
     * Sets the policy for consecutive hyphens in comments.
     * @param commentPolicy the policy
     * @see nu.validator.htmlparser.impl.Tokenizer#setCommentPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setCommentPolicy(XmlViolationPolicy commentPolicy) {
        this.commentPolicy = commentPolicy;
        if (driver != null) {
            driver.setCommentPolicy(commentPolicy);
        }
    }

    /**
     * Sets the policy for non-XML characters except white space.
     * @param contentNonXmlCharPolicy the policy
     * @see nu.validator.htmlparser.impl.Tokenizer#setContentNonXmlCharPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setContentNonXmlCharPolicy(
            XmlViolationPolicy contentNonXmlCharPolicy) {
        this.contentNonXmlCharPolicy = contentNonXmlCharPolicy;
        driver = null;
    }

    /**
     * Sets the policy for non-XML white space.
     * @param contentSpacePolicy the policy
     * @see nu.validator.htmlparser.impl.Tokenizer#setContentSpacePolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setContentSpacePolicy(XmlViolationPolicy contentSpacePolicy) {
        this.contentSpacePolicy = contentSpacePolicy;
        if (driver != null) {
            driver.setContentSpacePolicy(contentSpacePolicy);
        }
    }

    /**
     * Whether the parser considers scripting to be enabled for noscript treatment.
     * 
     * @return <code>true</code> if enabled
     * @see nu.validator.htmlparser.impl.TreeBuilder#isScriptingEnabled()
     */
    public bool isScriptingEnabled() {
        return scriptingEnabled;
    }

    /**
     * Sets whether the parser considers scripting to be enabled for noscript treatment.
     * @param scriptingEnabled <code>true</code> to enable
     * @see nu.validator.htmlparser.impl.TreeBuilder#setScriptingEnabled(bool)
     */
    public void setScriptingEnabled(bool scriptingEnabled) {
        this.scriptingEnabled = scriptingEnabled;
        if (treeBuilder != null) {
            treeBuilder.setScriptingEnabled(scriptingEnabled);
        }
    }

    /**
     * Returns the doctype expectation.
     * 
     * @return the doctypeExpectation
     */
    public DoctypeExpectation getDoctypeExpectation() {
        return doctypeExpectation;
    }

    /**
     * Sets the doctype expectation.
     * 
     * @param doctypeExpectation
     *            the doctypeExpectation to set
     * @see nu.validator.htmlparser.impl.TreeBuilder#setDoctypeExpectation(nu.validator.htmlparser.common.DoctypeExpectation)
     */
    public void setDoctypeExpectation(DoctypeExpectation doctypeExpectation) {
        this.doctypeExpectation = doctypeExpectation;
        if (treeBuilder != null) {
            treeBuilder.setDoctypeExpectation(doctypeExpectation);
        }
    }

    /**
     * Returns the document mode handler.
     * 
     * @return the documentModeHandler
     */
    public DocumentModeHandler getDocumentModeHandler() {
        return documentModeHandler;
    }

    /**
     * Sets the document mode handler.
     * 
     * @param documentModeHandler
     *            the documentModeHandler to set
     * @see nu.validator.htmlparser.impl.TreeBuilder#setDocumentModeHandler(nu.validator.htmlparser.common.DocumentModeHandler)
     */
    public void setDocumentModeHandler(DocumentModeHandler documentModeHandler) {
        this.documentModeHandler = documentModeHandler;
    }

    /**
     * Returns the streamabilityViolationPolicy.
     * 
     * @return the streamabilityViolationPolicy
     */
    public XmlViolationPolicy getStreamabilityViolationPolicy() {
        return streamabilityViolationPolicy;
    }

    /**
     * Sets the streamabilityViolationPolicy.
     * 
     * @param streamabilityViolationPolicy
     *            the streamabilityViolationPolicy to set
     */
    public void setStreamabilityViolationPolicy(
            XmlViolationPolicy streamabilityViolationPolicy) {
        this.streamabilityViolationPolicy = streamabilityViolationPolicy;
        driver = null;
    }

    /**
     * Whether the HTML 4 mode reports bool attributes in a way that repeats
     * the name in the value.
     * @param html4ModeCompatibleWithXhtml1Schemata
     */
    public void setHtml4ModeCompatibleWithXhtml1Schemata(
            bool html4ModeCompatibleWithXhtml1Schemata) {
        this.html4ModeCompatibleWithXhtml1Schemata = html4ModeCompatibleWithXhtml1Schemata;
        if (driver != null) {
            driver.setHtml4ModeCompatibleWithXhtml1Schemata(html4ModeCompatibleWithXhtml1Schemata);
        }
    }

    /**
     * Returns the <code>Locator</code> during parse.
     * @return the <code>Locator</code>
     */
    public Locator getDocumentLocator() {
        return driver.getDocumentLocator();
    }

    /**
     * Whether the HTML 4 mode reports bool attributes in a way that repeats
     * the name in the value.
     * 
     * @return the html4ModeCompatibleWithXhtml1Schemata
     */
    public bool isHtml4ModeCompatibleWithXhtml1Schemata() {
        return html4ModeCompatibleWithXhtml1Schemata;
    }

    /**
     * Whether <code>lang</code> is mapped to <code>xml:lang</code>.
     * @param mappingLangToXmlLang
     * @see nu.validator.htmlparser.impl.Tokenizer#setMappingLangToXmlLang(bool)
     */
    public void setMappingLangToXmlLang(bool mappingLangToXmlLang) {
        this.mappingLangToXmlLang = mappingLangToXmlLang;
        if (driver != null) {
            driver.setMappingLangToXmlLang(mappingLangToXmlLang);
        }
    }

    /**
     * Whether <code>lang</code> is mapped to <code>xml:lang</code>.
     * 
     * @return the mappingLangToXmlLang
     */
    public bool isMappingLangToXmlLang() {
        return mappingLangToXmlLang;
    }

    /**
     * Whether the <code>xmlns</code> attribute on the root element is 
     * passed to through. (FATAL not allowed.)
     * @param xmlnsPolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setXmlnsPolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setXmlnsPolicy(XmlViolationPolicy xmlnsPolicy) {
        if (xmlnsPolicy == XmlViolationPolicy.FATAL) {
            throw new ArgumentOutOfRangeException("Can't use FATAL here.");
        }
        this.xmlnsPolicy = xmlnsPolicy;
        if (driver != null) {
            driver.setXmlnsPolicy(xmlnsPolicy);
        }
    }

    /**
     * Returns the xmlnsPolicy.
     * 
     * @return the xmlnsPolicy
     */
    public XmlViolationPolicy getXmlnsPolicy() {
        return xmlnsPolicy;
    }

    /**
     * Returns the lexicalHandler.
     * 
     * @return the lexicalHandler
     */
    public LexicalHandler getLexicalHandler() {
        return lexicalHandler;
    }

    /**
     * Returns the commentPolicy.
     * 
     * @return the commentPolicy
     */
    public XmlViolationPolicy getCommentPolicy() {
        return commentPolicy;
    }

    /**
     * Returns the contentNonXmlCharPolicy.
     * 
     * @return the contentNonXmlCharPolicy
     */
    public XmlViolationPolicy getContentNonXmlCharPolicy() {
        return contentNonXmlCharPolicy;
    }

    /**
     * Returns the contentSpacePolicy.
     * 
     * @return the contentSpacePolicy
     */
    public XmlViolationPolicy getContentSpacePolicy() {
        return contentSpacePolicy;
    }

    /**
     * @param reportingDoctype
     * @see nu.validator.htmlparser.impl.TreeBuilder#setReportingDoctype(bool)
     */
    public void setReportingDoctype(bool reportingDoctype) {
        this.reportingDoctype = reportingDoctype;
        if (treeBuilder != null) {
            treeBuilder.setReportingDoctype(reportingDoctype);
        }
    }

    /**
     * Returns the reportingDoctype.
     * 
     * @return the reportingDoctype
     */
    public bool isReportingDoctype() {
        return reportingDoctype;
    }

    /**
     * @param errorProfile
     * @see nu.validator.htmlparser.impl.errorReportingTokenizer#setErrorProfile(set)
     */
    public void setErrorProfile(Dictionary<String, String> errorProfileMap) {
        this.errorProfileMap = errorProfileMap;
    }

    /**
     * The policy for non-NCName element and attribute names.
     * @param namePolicy
     * @see nu.validator.htmlparser.impl.Tokenizer#setNamePolicy(nu.validator.htmlparser.common.XmlViolationPolicy)
     */
    public void setNamePolicy(XmlViolationPolicy namePolicy) {
        this.namePolicy = namePolicy;
        if (driver != null) {
            driver.setNamePolicy(namePolicy);
            treeBuilder.setNamePolicy(namePolicy);
        }
    }
    
    /**
     * Sets the encoding sniffing heuristics.
     * 
     * @param heuristics the heuristics to set
     * @see nu.validator.htmlparser.impl.Tokenizer#setHeuristics(nu.validator.htmlparser.common.Heuristics)
     */
    public void setHeuristics(Heuristics heuristics) {
        this.heuristics = heuristics;
        if (driver != null) {
            driver.setHeuristics(heuristics);
        }
    }
    
    public Heuristics getHeuristics() {
        return this.heuristics;
    }

    /**
     * This is a catch-all convenience method for setting name, xmlns, content space, 
     * content non-XML char and comment policies in one go. This does not affect the 
     * streamability policy or doctype reporting.
     * 
     * @param xmlPolicy
     */
    public void setXmlPolicy(XmlViolationPolicy xmlPolicy) {
        setNamePolicy(xmlPolicy);
        setXmlnsPolicy(xmlPolicy == XmlViolationPolicy.FATAL ? XmlViolationPolicy.ALTER_INFOSET : xmlPolicy);
        setContentSpacePolicy(xmlPolicy);
        setContentNonXmlCharPolicy(xmlPolicy);
        setCommentPolicy(xmlPolicy);
    }

    /**
     * The policy for non-NCName element and attribute names.
     * 
     * @return the namePolicy
     */
    public XmlViolationPolicy getNamePolicy() {
        return namePolicy;
    }

    /**
     * Does nothing.
     * @deprecated
     */
    public void setBogusXmlnsPolicy(
            XmlViolationPolicy bogusXmlnsPolicy) {
    }

    /**
     * Returns <code>XmlViolationPolicy.ALTER_INFOSET</code>.
     * @deprecated
     * @return <code>XmlViolationPolicy.ALTER_INFOSET</code>
     */
    public XmlViolationPolicy getBogusXmlnsPolicy() {
        return XmlViolationPolicy.ALTER_INFOSET;
    }
    
    public void addCharacterHandler(CharacterHandler characterHandler) {
        this.characterHandlers.add(characterHandler);
        if (driver != null) {
            driver.addCharacterHandler(characterHandler);
        }
    }
}
