
    // utility function to set a global variable if it is not already set
    function setDefault(name, val) {
        if (typeof (window[name]) == "undefined" || window[name] == null) {
            window[name] = val;
        }
    }

    // Full expands a tree with a given ID
    function expandTree(treeId) {
        var ul = document.getElementById(treeId);
        if (ul == null) { return false; }
        expandCollapseList(ul, nodeOpenClass);
    }

    // Fully collapses a tree with a given ID
    function collapseTree(treeId) {
        var ul = document.getElementById(treeId);
        if (ul == null) { return false; }
        expandCollapseList(ul, nodeClosedClass);
    }

    // Performs 3 functions:
    // a) Expand all nodes
    // b) Collapse all nodes
    // c) Expand all nodes to reach a certain ID
    function expandCollapseList(ul, cName, itemId) {
        if (!ul.childNodes || ul.childNodes.length == 0) { return false; }
        // Iterate LIs
        for (var itemi = 0; itemi < ul.childNodes.length; itemi++) {
            var item = ul.childNodes[itemi];
            if (itemId != null && item.id == itemId) { return true; }
            if (item.nodeName == "LI") {
                // Iterate things in this LI
                var subLists = false;
                for (var sitemi = 0; sitemi < item.childNodes.length; sitemi++) {
                    var sitem = item.childNodes[sitemi];
                    if (sitem.nodeName == "UL") {
                        subLists = true;
                        var ret = expandCollapseList(sitem, cName, itemId);
                        if (itemId != null && ret) {
                            item.className = cName;
                            return true;
                        }
                    }
                }
                if (subLists && itemId == null) {
                    item.className = cName;
                }
            }
        }
    }

    // Search the document for UL elements with the correct CLASS name, then process them
    function convertTrees() {
        setDefault("treeClass", "mktree");
        setDefault("nodeClosedClass", "liClosed");
        setDefault("nodeOpenClass", "liOpen");
        setDefault("nodeBulletClass", "liBullet");
        setDefault("nodeLinkClass", "bullet");
        setDefault("preProcessTrees", true);
        if (preProcessTrees) {
            if (!document.createElement) { return; } // Without createElement, we can't do anything
            var uls = document.getElementsByTagName("ul");
            if (uls == null) { return; }
            var uls_length = uls.length;
            for (var uli = 0; uli < uls_length; uli++) {
                var ul = uls[uli];
                if (ul.nodeName == "UL" && ul.className == treeClass) {
                    processList(ul);
                }
            }
        }
    }

    function treeNodeOnclick() {
        this.parentNode.className = (this.parentNode.className == nodeOpenClass) ? nodeClosedClass : nodeOpenClass;
        return false;
    }
    function retFalse() {
        return false;
    }
    // Process a UL tag and all its children, to convert to a tree
    function processList(ul) {
        if (!ul.childNodes || ul.childNodes.length == 0) { return; }
        // Iterate LIs
        var childNodesLength = ul.childNodes.length;
        for (var itemi = 0; itemi < childNodesLength; itemi++) {
            var item = ul.childNodes[itemi];
            if (item.nodeName == "LI") {
                // Iterate things in this LI
                var subLists = false;
                var itemChildNodesLength = item.childNodes.length;
                for (var sitemi = 0; sitemi < itemChildNodesLength; sitemi++) {
                    var sitem = item.childNodes[sitemi];
                    if (sitem.nodeName == "UL") {
                        subLists = true;
                        processList(sitem);
                    }
                }
                var s = document.createElement("SPAN");
                var t = '\u00A0'; // &nbsp;
                s.className = nodeLinkClass;
                if (subLists) {
                    // This LI has UL's in it, so it's a +/- node
                    if (item.className == null || item.className == "") {
                        item.className = nodeClosedClass;
                    }
                    // If it's just text, make the text work as the link also
                    if (item.firstChild.nodeName == "#text") {
                        t = t + item.firstChild.nodeValue;
                        item.removeChild(item.firstChild);
                    }
                    s.onclick = treeNodeOnclick;
                }
                else {
                    // No sublists, so it's just a bullet node
                    item.className = nodeBulletClass;
                    s.onclick = retFalse;
                }
                s.appendChild(document.createTextNode(t));
                item.insertBefore(s, item.firstChild);
            }
        }
    }

