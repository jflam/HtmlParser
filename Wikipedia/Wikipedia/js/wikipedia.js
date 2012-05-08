// Contains all of the code for the wikipedia class

(function () {

    function inner_html(node) {
        var html = "";

        // This is an internal function that walks all of the descendents
        // of node and generates a string. This function is currently
        // hard-coded to fix up wikipedia img tags to include http:
        // In a future revision of this function, it will be generalized
        // to accept a list of lambdas that will be iteratively applied
        // across each node in the tree. The lambda has the option
        // to mutate the underlying node's attributes.
        function walk(node) {
            // fix up image tags to pre-pend http:
            if (node.type == "tag" && node.name.toLowerCase() == "img") {
                var src = node.attribs.src;
                if (src.indexOf("//") == 0) {
                    node.attribs.src = "http:" + node.attribs.src;
                }
            }

            var attributes = "";
            for (var key in node.attribs) {
                attributes += key + "=\"" + node.attribs[key] + "\" ";
            }

            if (node.type == "text") {
                html += node.data;
            } else if (node.type == "tag") {
                html += "<" + node.name + " " + attributes + ">";
            }


            if (node.children != null) {
                for (var i = 0; i < node.children.length; i++) {
                    var child = node.children[i];
                    if (child.type == "text") {
                        html += child.data;
                    } else {
                        walk(child);
                    }
                }
            }

            if (node.type == "tag") {
                html += "</" + node.name + ">";
            }
        };

        walk(node);
        return html;
    };

    // Publish functions as part of the wikipedia namespace
    WinJS.Namespace.define("wikipedia", {
    });
})();