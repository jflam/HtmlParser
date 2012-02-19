/*
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
 * A common superclass for tree builders that coalesce their text nodes.
 * 
 * @version $Id$
 * @author hsivonen
 */
using System;

public abstract class CoalescingTreeBuilder<T> : TreeBuilder<T> where T:class {

    protected sealed override void accumulateCharacters(char[] buf, int start, int length) {
        int newLen = charBufferLen + length;
        if (newLen > charBuffer.Length) {
            char[] newBuf = new char[newLen];
            Array.Copy(charBuffer, 0, newBuf, 0, charBufferLen);
            charBuffer = null; // release the old buffer in C++
            charBuffer = newBuf;
        }
        Array.Copy(buf, start, charBuffer, charBufferLen, length);
        charBufferLen = newLen;
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilder#appendCharacters(java.lang.Object, char[], int, int)
     */
    protected sealed override void appendCharacters(T parent, char[] buf, int start, int length) {
        appendCharacters(parent, new String(buf, start, length));
    }

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilder#appendIsindexPrompt(java.lang.Object)
     */
    protected sealed override void appendIsindexPrompt(T parent) {
        appendCharacters(parent, "This is a searchable index. Enter search keywords: ");
    }

    protected abstract void appendCharacters(T parent, String text);

    /**
     * @see nu.validator.htmlparser.impl.TreeBuilder#appendComment(java.lang.Object, char[], int, int)
     */
    protected sealed override void appendComment(T parent, char[] buf, int start, int length) {
        appendComment(parent, new String(buf, start, length));
    }

    protected abstract void appendComment(T parent, String comment);
    
    /**
     * @see nu.validator.htmlparser.impl.TreeBuilder#appendCommentToDocument(char[], int, int)
     */
    protected sealed override void appendCommentToDocument(char[] buf, int start, int length) {
        // TODO Auto-generated method stub
        appendCommentToDocument(new String(buf, start, length));
    }

    protected abstract void appendCommentToDocument(String comment);
    
    /**
     * @see nu.validator.htmlparser.impl.TreeBuilder#insertFosterParentedCharacters(char[], int, int, java.lang.Object, java.lang.Object)
     */
    protected sealed override void insertFosterParentedCharacters(char[] buf, int start, int length, T table, T stackParent) {
        insertFosterParentedCharacters(new String(buf, start, length), table, stackParent);
    }
    
    protected abstract void insertFosterParentedCharacters(String text, T table, T stackParent);
}
