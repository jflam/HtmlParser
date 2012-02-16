using System;

public interface Locator {
      
      /**
       * Return the public identifier for the current document event.
       *
       * <p>The return value is the public identifier of the document
       * entity or of the external parsed entity in which the markup
       * triggering the event appears.</p>
       *
       * @return A string containing the public identifier, or
       *         null if none is available.
       * @see #getSystemId
       */
      String getPublicId();
      
      
      /**
       * Return the system identifier for the current document event.
       *
       * <p>The return value is the system identifier of the document
       * entity or of the external parsed entity in which the markup
       * triggering the event appears.</p>
       *
       * <p>If the system identifier is a URL, the parser must resolve it
       * fully before passing it to the application.  For example, a file
       * name must always be provided as a <em>file:...</em> URL, and other
       * kinds of relative URI are also resolved against their bases.</p>
       *
       * @return A string containing the system identifier, or null
       *         if none is available.
       * @see #getPublicId
       */
      String getSystemId();
      
      
      /**
       * Return the line number where the current document event ends.
       * Lines are delimited by line ends, which are defined in
       * the XML specification.
       *
       * <p><strong>Warning:</strong> The return value from the method
       * is intended only as an approximation for the sake of diagnostics;
       * it is not intended to provide sufficient information
       * to edit the character content of the original XML document.
       * In some cases, these "line" numbers match what would be displayed
       * as columns, and in others they may not match the source text
       * due to internal entity expansion.  </p>
       *
       * <p>The return value is an approximation of the line number
       * in the document entity or external parsed entity where the
       * markup triggering the event appears.</p>
       *
       * <p>If possible, the SAX driver should provide the line position 
       * of the first character after the text associated with the document 
      * event.  The first line is line 1.</p>
      *
      * @return The line number, or -1 if none is available.
      * @see #getColumnNumber
      */
      int getLineNumber();
     
     
     /**
      * Return the column number where the current document event ends.
      * This is one-based number of Java <code>char</code> values since
      * the last line end.
      *
      * <p><strong>Warning:</strong> The return value from the method
      * is intended only as an approximation for the sake of diagnostics;
      * it is not intended to provide sufficient information
      * to edit the character content of the original XML document.
      * For example, when lines contain combining character sequences, wide
      * characters, surrogate pairs, or bi-directional text, the value may
      * not correspond to the column in a text editor's display. </p>
      *
      * <p>The return value is an approximation of the column number
      * in the document entity or external parsed entity where the
      * markup triggering the event appears.</p>
      *
      * <p>If possible, the SAX driver should provide the line position 
      * of the first character after the text associated with the document 
      * event.  The first column in each line is column 1.</p>
      *
      * @return The column number, or -1 if none is available.
      * @see #getLineNumber
      */
      int getColumnNumber();
}