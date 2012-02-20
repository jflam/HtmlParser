using System;

public class SAXException : Exception
{
    public SAXException(string message) : base(message) { }
}

public class SAXParseException : Exception
{
    public SAXParseException(string message) : base(message) { }
    public SAXParseException(string message, Locator locator) : base(message)
    {
        // TODO: something with the locator field ...
    }
    public SAXParseException(string message, Locator locator, Exception e) :base(message, e)
    {
        // TODO: something with the locator field ...
    }
}

public class SAXNotSupportedException : Exception
{
    public SAXNotSupportedException(string message) : base(message) { }
}

public class SAXNotRecognizedException : Exception
{
    public SAXNotRecognizedException() { }
    public SAXNotRecognizedException(string message) : base(message) { }
}