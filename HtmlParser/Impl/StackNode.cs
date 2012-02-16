/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2007-2011 Mozilla Foundation
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

public class StackNode<T> {
    public readonly int flags;

    public readonly String name;

    public readonly String popName;

    public readonly String ns;

    public readonly T node;

    // Only used on the list of formatting elements
    public HtmlAttributes attributes;

    private int refcount = 1;

    // [NOCPP[

    private readonly TaintableLocatorImpl locator;
    
    public TaintableLocatorImpl getLocator() {
        return locator;
    }

    // ]NOCPP]

    public int getFlags() {
        return flags;
    }

    public int getGroup() {
        return flags & ElementName.GROUP_MASK;
    }

    public bool isScoping() {
        return (flags & ElementName.SCOPING) != 0;
    }

    public bool isSpecial() {
        return (flags & ElementName.SPECIAL) != 0;
    }

    public bool isFosterParenting() {
        return (flags & ElementName.FOSTER_PARENTING) != 0;
    }

    public bool isHtmlIntegrationPoint() {
        return (flags & ElementName.HTML_INTEGRATION_POINT) != 0;
    }

    // [NOCPP[
    
    public bool isOptionalEndTag() {
        return (flags & ElementName.OPTIONAL_END_TAG) != 0;
    }
    
    // ]NOCPP]

    /**
     * Constructor for copying. This doesn't take another <code>StackNode</code>
     * because in C++ the caller is reponsible for reobtaining the local names
     * from another interner.
     * 
     * @param flags
     * @param ns
     * @param name
     * @param node
     * @param popName
     * @param attributes
     */
    public StackNode(int flags, String ns, String name, T node, String popName, HtmlAttributes attributes, TaintableLocatorImpl locator) {
        this.flags = flags;
        this.name = name;
        this.popName = popName;
        this.ns = ns;
        this.node = node;
        this.attributes = attributes;
        this.refcount = 1;
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    /**
     * Short hand for well-known HTML elements.
     * 
     * @param elementName
     * @param node
     */
    public StackNode(ElementName elementName, T node, TaintableLocatorImpl locator) {
        this.flags = elementName.getFlags();
        this.name = elementName.name;
        this.popName = elementName.name;
        this.ns = "http://www.w3.org/1999/xhtml";
        this.node = node;
        this.attributes = null;
        this.refcount = 1;
        Debug.Assert(!elementName.isCustom(), "Don't use this constructor for custom elements.");
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    /**
     * Constructor for HTML formatting elements.
     * 
     * @param elementName
     * @param node
     * @param attributes
     */
    public StackNode(ElementName elementName, T node, HtmlAttributes attributes, TaintableLocatorImpl locator) {
        this.flags = elementName.getFlags();
        this.name = elementName.name;
        this.popName = elementName.name;
        this.ns = "http://www.w3.org/1999/xhtml";
        this.node = node;
        this.attributes = attributes;
        this.refcount = 1;
        Debug.Assert(!elementName.isCustom(), "Don't use this constructor for custom elements.");
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    /**
     * The common-case HTML constructor.
     * 
     * @param elementName
     * @param node
     * @param popName
     */
    public StackNode(ElementName elementName, T node, String popName, TaintableLocatorImpl locator) {
        this.flags = elementName.getFlags();
        this.name = elementName.name;
        this.popName = popName;
        this.ns = "http://www.w3.org/1999/xhtml";
        this.node = node;
        this.attributes = null;
        this.refcount = 1;
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    /**
     * Constructor for SVG elements. Note that the order of the arguments is
     * what distinguishes this from the HTML constructor. This is ugly, but
     * AFAICT the least disruptive way to make this work with Java's generics
     * and without unnecessary branches. :-(
     * 
     * @param elementName
     * @param popName
     * @param node
     */
    public StackNode(ElementName elementName, String popName, T node, TaintableLocatorImpl locator) {
        this.flags = prepareSvgFlags(elementName.getFlags());
        this.name = elementName.name;
        this.popName = popName;
        this.ns = "http://www.w3.org/2000/svg";
        this.node = node;
        this.attributes = null;
        this.refcount = 1;
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    /**
     * Constructor for MathML.
     * 
     * @param elementName
     * @param node
     * @param popName
     * @param markAsIntegrationPoint
     */
    public StackNode(ElementName elementName, T node, String popName, bool markAsIntegrationPoint, TaintableLocatorImpl locator) {
        this.flags = prepareMathFlags(elementName.getFlags(), markAsIntegrationPoint);
        this.name = elementName.name;
        this.popName = popName;
        this.ns = "http://www.w3.org/1998/Math/MathML";
        this.node = node;
        this.attributes = null;
        this.refcount = 1;
        // [NOCPP[
        this.locator = locator;
        // ]NOCPP]
    }

    private static int prepareSvgFlags(int flags) {
        flags &= ~(ElementName.FOSTER_PARENTING | ElementName.SCOPING
                | ElementName.SPECIAL | ElementName.OPTIONAL_END_TAG);
        if ((flags & ElementName.SCOPING_AS_SVG) != 0) {
            flags |= (ElementName.SCOPING | ElementName.SPECIAL | ElementName.HTML_INTEGRATION_POINT);
        }
        return flags;
    }

    private static int prepareMathFlags(int flags, bool markAsIntegrationPoint) {
        flags &= ~(ElementName.FOSTER_PARENTING | ElementName.SCOPING
                | ElementName.SPECIAL | ElementName.OPTIONAL_END_TAG);
        if ((flags & ElementName.SCOPING_AS_MATHML) != 0) {
            flags |= (ElementName.SCOPING | ElementName.SPECIAL);
        }
        if (markAsIntegrationPoint) {
            flags |= ElementName.HTML_INTEGRATION_POINT;
        }
        return flags;
    }

    private void destructor() {
        Portability.delete(attributes);
    }

    public void dropAttributes() {
        attributes = null;
    }

    // [NOCPP[
    /**
     * @see java.lang.Object#toString()
     */
    public String toString() {
        return name;
    }

    // ]NOCPP]

    public void retain() {
        refcount++;
    }

    public void release() {
        refcount--;
        if (refcount == 0) {
            Portability.delete(this);
        }
    }
}
