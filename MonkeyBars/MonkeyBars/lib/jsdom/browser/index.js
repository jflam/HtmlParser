var http          = require('/lib/shred'),
    URL           = require('/lib/url'),
    HtmlToDom     = require('./htmltodom').HtmlToDom,
    domToHtml     = require('./domtohtml').domToHtml,
    htmlencoding  = require('./htmlencoding'),
    HTMLEncode    = htmlencoding.HTMLEncode,
    HTMLDecode    = htmlencoding.HTMLDecode,
    jsdom         = require('../../lib/jsdom'),
    Contextify    = null;

try {
  Contextify = require('contextify');
} catch (e) {
  // Shim for when the contextify compilation fails.
  // This is not quite as correct, but it gets the job done.
  Contextify = function(sandbox) {
    var vm = require('vm');
    var context = vm.createContext(sandbox);
    var global = null;

    sandbox.run = function(code, filename) {
      return vm.runInContext(code, context, filename);
    };

    sandbox.getGlobal = function() {
      if (!global) {
        global = vm.runInContext('this', context);
      }
      return global;
    };

    sandbox.dispose = function() {
      global = null;
      sandbox.run = function () {
        throw new Error("Called run() after dispose().");
      };
      sandbox.getGlobal = function () {
        throw new Error("Called getGlobal() after dispose().");
      };
      sandbox.dispose = function () {
        throw new Error("Called dispose() after dispose().");
      };
    };

    return sandbox;
  };
}

function NOT_IMPLEMENTED(target) {
  return function() {
    if (!jsdom.debugMode) {
      var raise = target ? target.raise : this.raise;
      raise.call(this, 'error', 'NOT IMPLEMENTED');
    }
  };
}

/**
 * Creates a window having a document. The document can be passed as option,
 * if omitted, a new document will be created.
 */
exports.windowAugmentation = function(dom, options) {
  options = options || {};
  var window = exports.createWindow(dom, options);

  if (!options.document) {
    var browser = browserAugmentation(dom, options);

    if (options.features && options.features.QuerySelector) {
      require(__dirname + "/../selectors/index").applyQuerySelectorPrototype(browser);
    }

    options.document = (browser.HTMLDocument)             ?
                        new browser.HTMLDocument(options) :
                        new browser.Document(options);



    options.document.write('<html><head></head><body></body></html>');
  }

  var doc = window.document = options.document;

  if (doc.addEventListener) {
    if (doc.readyState == 'complete') {
      var ev = doc.createEvent('HTMLEvents');
      ev.initEvent('load', false, false);
      window.dispatchEvent(ev);
    }
    else {
      doc.addEventListener('load', function(ev) {
        window.dispatchEvent(ev);
      });
    }
  }

  return window;
};

/**
 * Creates a document-less window.
 */
exports.createWindow = function(dom, options) {
  var timers = [];

  function startTimer(startFn, stopFn, callback, ms) {
	  var res = startFn(callback, ms);
	  timers.push( [ res, stopFn ] );
	  return res;
  }

  function stopTimer(id) {
	  if (typeof id === 'undefined') {
		  return;
	  }
	  for (var i in timers) {
		  if (timers[i][0] === id) {
			  timers[i][1].call(this, id);
			  timers.splice(i, 1);
			  break;
		  }
	  }
  }

  function stopAllTimers() {
	  timers.forEach(function (t) {
		  t[1].call(this, t[0]);
	  });
	  timers = [];
  }

  function DOMWindow(options) {
    var href = (options || {}).url || 'file://' + __filename;
    this.location = URL.parse(href);
    this.location.reload = NOT_IMPLEMENTED(this);
    this.location.replace = NOT_IMPLEMENTED(this);
    this.location.toString = function() {
      return href;
    };

    var window = this.console._window = this;

    /* Location hash support */
    this.location.__defineGetter__("hash", function() {
      return (window.location.href.split("#").length > 1)
        ? "#"+window.location.href.split("#")[1]
        : "";
    });

    this.location.__defineSetter__("hash", function(val) {
      /* TODO: Should fire a hashchange event, but tests aren't working */
      window.location.href = window.location.href.split("#")[0] + val;
    });

    /* Location search support */
    this.location.__defineGetter__("search", function() {
      return (window.location.href.split("?").length > 1)
        ? "?"+window.location.href.match(/\?([^#]+)/)[1]
        : "";
    });

    this.location.__defineSetter__("search", function(val) {
      window.location.href = (window.location.href.indexOf("?") > 0)
        ? window.location.href.replace(/\?([^#]+)/, val)
        : window.location.href.match(/^([^#?]+)/)[0] + val + window.location.hash;
    });

    if (options && options.document) {
      options.document.location = this.location;
    }
    this.addEventListener = function() {
      dom.Node.prototype.addEventListener.apply(window, arguments);
    };
    this.removeEventListener = function() {
      dom.Node.prototype.removeEventListener.apply(window, arguments);
    };
    this.dispatchEvent = function() {
      dom.Node.prototype.dispatchEvent.apply(window, arguments);
    };
    this.raise = function(){
      dom.Node.prototype.raise.apply(window.document, arguments);
    };

    this.setTimeout = function (fn, ms) { return startTimer(setTimeout, clearTimeout, fn, ms); };
    this.setInterval = function (fn, ms) { return startTimer(setInterval, clearInterval, fn, ms); };
    this.clearInterval = stopTimer;
    this.clearTimeout = stopTimer;
    this.__stopAllTimers = stopAllTimers;
  }

  DOMWindow.prototype = {
    __proto__: dom,
    // This implements window.frames.length, since window.frames returns a
    // self reference to the window object.  This value is incremented in the
    // HTMLFrameElement init function (see: level2/html.js).
    _length : 0,
    get length () {
      return this._length;
    },
    close : function() {
      // Recursively close child frame windows, then ourselves.
      var currentWindow = this;
      (function windowCleaner (window) {
        var i;
        // We could call window.frames.length etc, but window.frames just points
        // back to window.
        if (window.length > 0) {
          for (i = 0; i < window.length; i++) {
            windowCleaner(window[i]);
          }
        }
        // We're already in our own window.close().
        if (window !== currentWindow) {
          window.close();
        }
      })(this);

      if (this.document) {
        if (this.document.body) {
          this.document.body.innerHTML = "";
        }

        if (this.document.close) {
          // We need to empty out the event listener array because
          // document.close() causes 'load' event to re-fire.
          this.document._listeners = [];
          this.document.close();
        }
        delete this.document;
      }

      stopAllTimers();
      // Clean up the window's execution context.
      // dispose() is added by Contextify.
      this.dispose();
    },
    getComputedStyle: function(node) {
      var s = node.style,
          cs = {};

      for (var n in s) {
        cs[n] = s[n];
      }
      cs.__proto__ = {
        getPropertyValue: function(name) {
          return node.style[name];
        }
      };
      return cs;
    },
    console: {
      log:   function(message) { this._window.raise('log',   message) },
      info:  function(message) { this._window.raise('info',  message) },
      warn:  function(message) { this._window.raise('warn',  message) },
      error: function(message) { this._window.raise('error', message) }
    },
    navigator: {
      userAgent: 'Node.js (' + process.platform + '; U; rv:' + process.version + ')',
      appName: 'Node.js jsDom',
      platform: process.platform,
      appVersion: process.version
    },
    XMLHttpRequest: function XMLHttpRequest() {},

    name: 'nodejs',
    innerWidth: 1024,
    innerHeight: 768,
    outerWidth: 1024,
    outerHeight: 768,
    pageXOffset: 0,
    pageYOffset: 0,
    screenX: 0,
    screenY: 0,
    screenLeft: 0,
    screenTop: 0,
    scrollX: 0,
    scrollY: 0,
    scrollTop: 0,
    scrollLeft: 0,
    alert: NOT_IMPLEMENTED(),
    blur: NOT_IMPLEMENTED(),
    confirm: NOT_IMPLEMENTED(),
    createPopup: NOT_IMPLEMENTED(),
    focus: NOT_IMPLEMENTED(),
    moveBy: NOT_IMPLEMENTED(),
    moveTo: NOT_IMPLEMENTED(),
    open: NOT_IMPLEMENTED(),
    print: NOT_IMPLEMENTED(),
    prompt: NOT_IMPLEMENTED(),
    resizeBy: NOT_IMPLEMENTED(),
    resizeTo: NOT_IMPLEMENTED(),
    scroll: NOT_IMPLEMENTED(),
    scrollBy: NOT_IMPLEMENTED(),
    scrollTo: NOT_IMPLEMENTED(),
    screen : {
      width : 0,
      height : 0
    },
    Image : NOT_IMPLEMENTED()
  };

  var window = new DOMWindow(options);

  Contextify(window);

  // We need to set up self references using Contextify's getGlobal() so that
  // the global object identity is correct (window === this).
  // See Contextify README for more info.
  var global = window.getGlobal();

  // Set up the window as if it's a top level window.
  // If it's not, then references will be corrected by frame/iframe code.
  // Note: window.frames is maintained in the HTMLFrameElement init function.
  window.window = window.frames
                = window.self
                = window.parent
                = window.top = global;

  return window;
};

//Caching for HTMLParser require. HUGE performace boost.
/**
* 5000 iterations
* Without cache: ~1800+ms
* With cache: ~80ms
*/
var defaultParser = null;
function getDefaultParser() {
  if (defaultParser === null) {
    try {
      defaultParser = require('htmlparser');
    }
    catch (e) {
      try {
        defaultParser = require('node-htmlparser/lib/node-htmlparser');
      }
      catch (e2) {
        defaultParser = undefined;
      }
    }
  }
  return defaultParser;
}

/**
 * Augments the given DOM by adding browser-specific properties and methods (BOM).
 * Returns the augmented DOM.
 */
var browserAugmentation = exports.browserAugmentation = function(dom, options) {

  if (dom._augmented) {
    return dom;
  }

  if(!options) {
    options = {};
  }

  // set up html parser - use a provided one or try and load from library
  var htmltodom = new HtmlToDom(options.parser || getDefaultParser());

  if (!dom.HTMLDocument) {
    dom.HTMLDocument = dom.Document;
  }
  if (!dom.HTMLDocument.prototype.write) {
    dom.HTMLDocument.prototype.write = function(html) {
      this.innerHTML = html;
    };
  }

  dom.Element.prototype.getElementsByClassName = function(className) {

    function filterByClassName(child) {
      if (!child) {
        return false;
      }

      if (child.nodeType &&
          child.nodeType === dom.Node.ENTITY_REFERENCE_NODE)
      {
        child = child._entity;
      }

      var classString = child.className;
      if (classString) {
        var s = classString.split(" ");
        for (var i=0; i<s.length; i++) {
          if (s[i] === className) {
            return true;
          }
        }
      }
      return false;
    }

    return new dom.NodeList(this.ownerDocument || this, dom.mapper(this, filterByClassName));
  };

  dom.Element.prototype.__defineGetter__('sourceIndex', function() {
    /*
    * According to QuirksMode:
    * Get the sourceIndex of element x. This is also the index number for
    * the element in the document.getElementsByTagName('*') array.
    * http://www.quirksmode.org/dom/w3c_core.html#t77
    */
    var items = this.ownerDocument.getElementsByTagName('*'),
        len = items.length;

    for (var i = 0; i < len; i++) {
      if (items[i] === this) {
        return i;
      }
    }
  });

  dom.Document.prototype.__defineGetter__('outerHTML', function() {
    return domToHtml(this);
  });

  dom.Element.prototype.__defineGetter__('outerHTML', function() {
    return domToHtml(this);
  });

  dom.Element.prototype.__defineGetter__('innerHTML', function() {

    if (this._tagName === 'script' &&
        this._attributes.length > 0 &&
        typeof(this._attributes._nodes.type) !== "undefined" &&
        this._attributes._nodes.type._nodeValue.indexOf("text") === 0) {
        return domToHtml(this._childNodes, true, true);
    }

    return domToHtml(this._childNodes, true);
  });

  dom.Element.prototype.__defineSetter__('doctype', function() {
    throw new core.DOMException(NO_MODIFICATION_ALLOWED_ERR);
  });
  dom.Element.prototype.__defineGetter__('doctype', function() {
    var r = null;
    if (this.nodeName == '#document') {
        if (this._doctype) {
         r = this._doctype;
        }
    }
    return r;
  });

  dom.Element.prototype.__defineSetter__('innerHTML', function(html) {
    //Clear the children first:
    var child;
    while ((child = this._childNodes[0])) {
      this.removeChild(child);
    }

    if (this.nodeName === '#document') {
      parseDocType(this, html);
    }
    if (html !== "" && html != null) {
      htmltodom.appendHtmlToElement(html, this);
    }
    return html;
  });


  dom.Document.prototype.__defineGetter__('innerHTML', function() {
    return domToHtml(this._childNodes, true);
  });

  dom.Document.prototype.__defineSetter__('innerHTML', function(html) {
    //Clear the children first:
    var child;
    while ((child = this._childNodes[0])) {
      this.removeChild(child);
    }

    if (this.nodeName === '#document') {
      parseDocType(this, html);
    }
    if (html !== "" && html != null) {
      htmltodom.appendHtmlToElement(html, this);
    }
    return html;
  });

  var DOC_HTML5      = /<!doctype html>/i,
      DOC_TYPE       = /<!DOCTYPE (\w(.|\n)*)">/i,
      DOC_TYPE_START = '<!DOCTYPE ',
      DOC_TYPE_END   = '">';

  function parseDocType(doc, html) {
    var publicID = '',
        systemID = '',
        fullDT = '',
        name = 'HTML',
        set = true,
        doctype = html.match(DOC_HTML5);

    //Default, No doctype === null
    doc._doctype = null;

    if (doctype && doctype[0]) { //Handle the HTML shorty doctype
      fullDT = doctype[0];
    } else { //Parse the doctype
      // find the start
      var start     = html.indexOf(DOC_TYPE_START),
          end       = html.indexOf(DOC_TYPE_END),
          docString;

      if (start < 0 || end < 0) {
        return;
      }

      docString = html.substr(start, (end-start)+DOC_TYPE_END.length);
      doctype = docString.replace(/[\n\r]/g,'').match(DOC_TYPE);

      if (!doctype) {
        return;
      }

      fullDT = doctype[0];
      doctype = doctype[1].split(' "');
      var _id1 = doctype.length ? doctype.pop().replace(/"/g, '') : '',
          _id2 = doctype.length ? doctype.pop().replace(/"/g, '') : '';

      if (_id1.indexOf('-//') !== -1) {
        publicID = _id1;
      }
      if (_id2.indexOf('-//') !== -1) {
        publicID = _id2;
      }
      if (_id1.indexOf('://') !== -1) {
        systemID = _id1;
      }
      if (_id2.indexOf('://') !== -1) {
        systemID = _id2;
      }
      if (doctype.length) {
        doctype = doctype[0].split(' ');
        name = doctype[0].toUpperCase();
      }
    }
    doc._doctype = new dom.DOMImplementation().createDocumentType(name, publicID, systemID);
    doc._doctype._ownerDocument = doc;
    doc._doctype._fullDT = fullDT;
    doc._doctype.toString = function() {
      return this._fullDT;
    };
  }

  dom.Document.prototype.getElementsByClassName = function(className) {

    function filterByClassName(child) {
      if (!child) {
        return false;
      }

      if (child.nodeType &&
          child.nodeType === dom.Node.ENTITY_REFERENCE_NODE)
      {
        child = child._entity;
      }

      var classString = child.className;
      if (classString) {
        var s = classString.split(" ");
        for (var i=0; i<s.length; i++) {
          if (s[i] === className) {
            return true;
          }
        }
      }
      return false;
    }

    return new dom.NodeList(this.ownerDocument || this, dom.mapper(this, filterByClassName));
  };

  dom.Element.prototype.__defineGetter__('nodeName', function(val) {
    return this._nodeName.toUpperCase();
  });

  dom.Element.prototype.__defineGetter__('tagName', function(val) {
    var t = this._tagName.toUpperCase();
    //Document should not return a tagName
    if (this.nodeName === '#document') {
      t = null;
    }
    return t;
  });

  dom.Element.prototype.scrollTop = 0;
  dom.Element.prototype.scrollLeft = 0;

  dom.Document.prototype.__defineGetter__('parentWindow', function() {
    if (!this._parentWindow) {
      var window = exports.windowAugmentation(dom, {document: this, url: this.URL});
      this._parentWindow = window.getGlobal();
    }
    return this._parentWindow;
  });

  dom.Document.prototype.__defineSetter__('parentWindow', function(window) {
    this._parentWindow = window.getGlobal();
  });

  dom.Document.prototype.__defineGetter__('defaultView', function() {
    return this.parentWindow;
  });

  dom._augmented = true;
  return dom;
};
