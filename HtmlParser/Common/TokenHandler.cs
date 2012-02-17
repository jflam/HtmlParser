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

/**
 * <code>Tokenizer</code> reports tokens through this interface.
 * 
 * @version $Id$
 * @author hsivonen
 */
public interface TokenHandler {

    /**
     * This method is called at the start of tokenization before any other
     * methods on this interface are called. Implementations should hold the
     * reference to the <code>Tokenizer</code> in order to set the content
     * model flag and in order to be able to query for <code>Locator</code>
     * data.
     * 
     * @param self
     *            the <code>Tokenizer</code>.
     * @throws SAXException
     *             if something went wrong
     */
    void startTokenization(Tokenizer self);  
    
    /**
     * If this handler implementation cares about comments, return
     * <code>true</code>. If not, return <code>false</code>.
     * 
     * @return whether this handler wants comments
     * @throws SAXException
     *             if something went wrong
     */
    bool wantsComments(); 

    /**
     * Receive a doctype token.
     * 
     * @param name
     *            the name
     * @param publicIdentifier
     *            the public id
     * @param systemIdentifier
     *            the system id
     * @param forceQuirks
     *            whether the token is correct
     * @throws SAXException
     *             if something went wrong
     */
    void doctype(string name, string publicIdentifier, string systemIdentifier, bool forceQuirks);

    /**
     * Receive a start tag token.
     * 
     * @param eltName
     *            the tag name
     * @param attributes
     *            the attributes
     * @param selfClosing
     *            TODO
     * @throws SAXException
     *             if something went wrong
     */
    void startTag(ElementName eltName, HtmlAttributes attributes, bool selfClosing);

    /**
     * Receive an end tag token.
     * 
     * @param eltName
     *            the tag name
     * @throws SAXException
     *             if something went wrong
     */
    void endTag(ElementName eltName);

    /**
     * Receive a comment token. The data is junk if the
     * <code>wantsComments()</code> returned <code>false</code>.
     * 
     * @param buf
     *            a buffer holding the data
     * @param start the offset into the buffer
     * @param length
     *            the number of code units to read
     * @throws SAXException
     *             if something went wrong
     */
    void comment(char[] buf, int start, int length);

    /**
     * Receive character tokens. This method has the same semantics as the SAX
     * method of the same name.
     * 
     * @param buf
     *            a buffer holding the data
     * @param start
     *            offset into the buffer
     * @param length
     *            the number of code units to read
     * @throws SAXException
     *             if something went wrong
     * @see org.xml.sax.ContentHandler#characters(char[], int, int)
     */
    void characters(char[] buf, int start, int length);

    /**
     * Reports a U+0000 that's being turned into a U+FFFD.
     * 
     * @throws SAXException
     *             if something went wrong
     */
    void zeroOriginatingReplacementCharacter();
    
    /**
     * The end-of-file token.
     * 
     * @throws SAXException
     *             if something went wrong
     */
    void eof();

    /**
     * The perform final cleanup.
     * 
     * @throws SAXException
     *             if something went wrong
     */
    void endTokenization();

    /**
     * Checks if the CDATA sections are allowed.
     * 
     * @return <code>true</code> if CDATA sections are allowed
     * @throws SAXException
     *             if something went wrong
     */
    bool cdataSectionAllowed();
}
