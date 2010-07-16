/*** graffit.js starts here ***/

function $$(id) {
    if (id.substring(1, 0) != "#")
        id = "#" + id; 
        
    return $(id)[0];
}

String.prototype.endsWith = function(suffix) {
	return (this.substr(this.length - suffix.length) === suffix);
}

String.prototype.startsWith = function(prefix) {
	return (this.substr(0, prefix.length) === prefix);
}

var _tmplCache = {}
this.parseTemplate = function(str, data) {
    /// <summary>
    /// Client side template parser that uses &lt;#= #&gt; and &lt;# code #&gt; expressions.
    /// and # # code blocks for template expansion.
    /// NOTE: chokes on single quotes in the document in some situations
    ///       use &amp;rsquo; for literals in text and avoid any single quote
    ///       attribute delimiters.
    /// </summary>    
    /// <param name="str" type="string">The text of the template to expand</param>    
    /// <param name="data" type="var">
    /// Any data that is to be merged. Pass an object and
    /// that object's properties are visible as variables.
    /// </param>    
    /// <returns type="string" />  
    var err = "";
    try {
        var func = _tmplCache[str];
        if (!func) {
            var strFunc =
            "var p=[],print=function(){p.push.apply(p,arguments);};" +
                        "with(obj){p.push('" +

            str.replace(/[\r\t\n]/g, " ")
               .replace(/'(?=[^#]*#>)/g, "\t")
               .split("'").join("\\'")
               .split("\t").join("'")
               .replace(/<#=(.+?)#>/g, "',$1,'")
               .split("<#").join("');")
               .split("#>").join("p.push('")
               + "');}return p.join('');";

            //alert(strFunc);
            func = new Function("obj", strFunc);
            _tmplCache[str] = func;
        }
        return func(data);
    } catch (e) { err = e.message; }
    return "< # ERROR: " + err + " # >";
}

var GraffitiHelpers = new Object();

GraffitiHelpers.statusMessage = function(name, text, alertOnNull) {

    var result = $('#' + name);

    if (result != null) {
        result.show();
        result.html(text);
    }
    else if (alertOnNull) {
        alert(text);
    }
}


/********************* COMMENTS ************************/

var Comments = new Object();

Comments.statusMessage = function(text, alertOnNull) {
    var result = $('#comment_status');

    if (result != null) {
        result.show();
        result.html(text);
    }
    else if (alertOnNull) {
        alert(text);
    }

    if (typeof decrementComments == 'function')
        decrementComments();
}

Comments.deleteCommentWithStatus = function(url, id, tempparam) {
    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=deleteCommentWithStatus',
        data: { commentid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });

            if ($('#commentsPending')) {
                $('#commentsPending').html(parseInt($('#commentsPending').html()) - 1);
            }

            var response = transport || "no response text";
            Comments.statusMessage(response, false);
        },

        error: function() {
            alert('Something went wrong...');
        }
    });
}

Comments.approve = function(url, id) {
    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=approve',
        data: { commentid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });

            var response = transport || "no response text";
            Comments.statusMessage(response, false);
        },

        error: function() {
            alert('Something went wrong...');
        }
    });
}

Comments.unDelete = function(url, id) {

    var dontrefresh = false;
    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            dontrefresh = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=unDelete',
        data: { commentid: id },
        success: function(transport) {
            var response = transport || "no response text";
            Comments.statusMessage(response, false);

            if (!dontrefresh)
                window.location = window.location.pathname;
        },

        error: function() {
            alert('Something went wrong...');
        }
    });
}

Comments.deleteComment = function(url, id) {
    //if(!confirm('Are you sure you want to delete this comment ' + id + '? This action can not be undone!'))
    //    return false;

    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=deleteComment',
        data: { commentid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });
        },

        error: function() {
            alert('Something went wrong...');
        }
    });
}

//Add New Comment

Comments.submitComment = function(url) {
    Comments.statusMessage('Sending... please wait', true);

    $.ajax({
        type: "POST",
        url: url + '?command=newComment',
        data: $("#comment_form").serialize(),
        success: function(transport) {
            var response = transport || "no response text";
            Comments.statusMessage(response, true);
            $('#comment').val('');
        },

        error: function() {
            Comments.statusMessage('Something went wrong. The comment was likely not saved.', true);
        }
    });
}

/********************* POSTS ************************/
var Posts = new Object();

Posts.deletePost = function(url, id) {
    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=deletePost',
        data: { postid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });

            var response = transport || "no response text";
            Comments.statusMessage(response, false);
        },
        error: function() {
            alert('Something went wrong...');
        }
    });
}

Posts.permanentDeletePost = function(url, id) {

    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=permanentDeletePost',
        data: { postid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });

            var response = transport || "unknown";
            Comments.statusMessage("Post " + response + " has been permenantly deleted!", false);
        },

        error: function() {
            alert('Something went wrong...');
        }
    });
}

Posts.unDeletePost = function(url, id) {
    var itemToRemove = new Array();

    if (arguments.length > 2) {
        for (var i = 2; i < arguments.length; i++) {
            itemToRemove[i - 2] = arguments[i];
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?command=unDeletePost',
        data: { postid: id },
        success: function(transport) {
            $.each(itemToRemove, function() { $("#" + this).hide(); });
            var response = transport || "no response text";
            window.location = window.location.pathname;
        },
        error: function() {
            alert('Something went wrong...');
        }
    });
}

/**************** CONTACT ***************************/

var Contact = new Object();

Contact.submitMessage = function(url) {
    GraffitiHelpers.statusMessage('contact_status', 'sending', true);

    $.ajax({
        type: "POST",
        url: url + '?command=newContactMessage',
        data: $("#contact_form").serialize(),
        success: function(transport) {
            var response = transport || "no response text";
            GraffitiHelpers.statusMessage('contact_status', response, true);
            $('#message').val('');
        },
        error: function() {
            GraffitiHelpers.statusMessage('contact_status', 'Something went wrong. The contact request was likely not sent.', true);
        }
    });
}

/**************** PERMISSIONS ***************************/

function togglePermissions(readbox, editbox, publishbox, command) {
    readbox = document.getElementById(readbox);
    editbox = document.getElementById(editbox);
    publishbox = document.getElementById(publishbox);
    if (command == 'read') {
        if (readbox.checked == false) {

            editbox.checked = false;
            publishbox.checked = false;
        }
    }

    if (command == 'edit') {

        if (editbox.checked == true) {
            readbox.checked = true;
        }
        else {
            publishbox.checked = false;
        }
    }

    if (command == 'publish') {
        if (publishbox.checked == true) {
            readbox.checked = true;
            editbox.checked = true;
        }
    }

    if (editbox.checked)
        readbox.checked = true;

}