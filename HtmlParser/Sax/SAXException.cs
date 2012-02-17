using System;

public class SAXException : Exception
{
    public SAXException(string message) : base(message) { }
}

public class SAXParseException : Exception
{
    public SAXParseException(string message) : base(message) { }
    public SAXParseException(string message, Locator locator)
        : base(message)
    {
        // TODO: something with the locator field ...
    }
}