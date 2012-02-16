/*
 * Copyright (c) 2008-2009 Mozilla Foundation
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

public class Portability {

    // Allocating methods

    /**
     * Allocates a new local name object. In C++, the refcount must be set up in such a way that 
     * calling <code>releaseLocal</code> on the return value balances the refcount set by this method.
     */
    public static String newLocalNameFromBuffer(char[] buf, int offset, int length, Interner interner) {
        return new String(buf, offset, length).intern();
    }

    public static String newStringFromBuffer(char[] buf, int offset, int length) {
        return new String(buf, offset, length);
    }

    public static String newEmptyString() {
        return "";
    }

    public static String newStringFromLiteral(String literal) {
        return literal;
    }
    
    public static String newStringFromString(String str) {
        return str;
    }
    
    // XXX get rid of this
    public static char[] newCharArrayFromLocal(String local) {
        return local.ToCharArray();
    }

    public static char[] newCharArrayFromString(String str) {
        return str.ToCharArray();
    }
    
    public static String newLocalFromLocal(String local, Interner interner) {
        return local;
    }
    
    // Deallocation methods
    
    public static void releaseString(String str) {
        // No-op in Java
    }
    
    // Comparison methods
    
    public static bool localEqualsBuffer(String local, char[] buf, int offset, int length) {
        if (local.Length != length) {
            return false;
        }
        for (int i = 0; i < length; i++) {
            if (local[i] != buf[offset + i]) {
                return false;
            }
        }
        return true;
    }

    public static bool lowerCaseLiteralIsPrefixOfIgnoreAsciiCaseString(String lowerCaseLiteral, String str) {
        if (str == null) {
            return false;
        }
        if (lowerCaseLiteral.Length > str.Length) {
            return false;
        }
        for (int i = 0; i < lowerCaseLiteral.Length; i++) {
            char c0 = lowerCaseLiteral[i];
            char c1 = str[i];
            if (c1 >= 'A' && c1 <= 'Z') {
                c1 += (char)0x20;
            }
            if (c0 != c1) {
                return false;
            }
        }
        return true;
    }
    
    public static bool lowerCaseLiteralEqualsIgnoreAsciiCaseString(String lowerCaseLiteral, String str) {
        if (str == null) {
            return false;
        }
        if (lowerCaseLiteral.Length != str.Length) {
            return false;
        }
        for (int i = 0; i < lowerCaseLiteral.Length; i++) {
            char c0 = lowerCaseLiteral[i];
            char c1 = str[i];
            if (c1 >= 'A' && c1 <= 'Z') {
                c1 += (char)0x20;
            }
            if (c0 != c1) {
                return false;
            }
        }
        return true;
    }
    
    public static bool literalEqualsString(String literal, String str) {
        return literal.Equals(str);
    }

    public static bool stringEqualsString(String one, String other) {
        return one.Equals(other);
    }
    
    public static void delete(Object o) {
        
    }

    public static void deleteArray(Object o) {
        
    }
}
