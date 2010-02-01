function TagTextBox (varName, allTagsId, selectedTagsId) 
{
	this.VariableName = varName;
	this.AllTagsHandle = document.getElementById(allTagsId);
	this.SelectedTagsHandle = document.getElementById(selectedTagsId);
	this.PreviousTags = new Array();
	this.PreviousValue = "";
	this.CurrentTag = "";
	this.CurrentTagPosition = 0;
	
	this.GetAllTags = function ()
	{
		var allTags = this.AllTagsHandle.value.split('&');
		var newTags = new Array();
		for (var i = 0; i < allTags.length; i+=2)
		{
			if (allTags[i] != '')
				newTags[newTags.length] = decodeURIComponent(allTags[i].replace(/\+/g, ' '));
		}
		
		return newTags;
	}
	
	this.GetAllTagsWithEncoding = function()
	{
		var allTags = this.AllTagsHandle.value.split('&');
		var newTags = new Array();
		for (var i = 0; i < allTags.length; i+=2)
		{
			if (allTags[i] != '')
				newTags[newTags.length] = new Array(decodeURIComponent(allTags[i].replace(/\+/g, ' ')), decodeURIComponent(allTags[i + 1].replace(/\+/g, ' ')));
		}
		
		return newTags;
	}
	
	this.SetSelectedTags = function (tags)
	{
		var newTags = new Array();
		var selTags = this.GetSelectedTags();
		var allTags = this.GetAllTags();
		
		// add tags that were new (not in GetAllTags())
		var found;
		for (var i = 0; i < selTags.length; i++)
		{
			found = false;
			for (var j = 0; j < allTags.length; j++)
			{
				if (selTags[i].toUpperCase() == allTags[j].toUpperCase())
				{
					found = true;
					break;
				}
			}
			
			if (!found)
				newTags[newTags.length] = selTags[i];
		}
		
		// add tags that were selected from GetAllTags())
		for (var i = 0; i < tags.length; i++)
		{
			newTags[newTags.length] = tags[i];
		}
		
		this.SelectedTagsHandle.value = newTags.join('; ');
	}
	
	this.GetSelectedTags = function ()
	{
		var tTags = this.SelectedTagsHandle.value.split(/;|,/);
		var tags = new Array();
		
		// filter out blank items
		for (var i = 0; i < tTags.length; i++)
		{
			tTags[i] = this.Trim(tTags[i]);
			if (tTags[i] != "")
				tags[tags.length] = tTags[i];
		}
		
		return tags;
	}
	
	this.Trim = function (text)
	{
		return text.replace(/^\s+|\s+$/g, '');
	}
	
	this.EditorKeyDown = function (event)
	{
		if (!event)
			event = window.event;
			
		if (!event)
			return;
	
		if (event.keyCode == 13)
		{
			this.SelectedTagsHandle.focus();
			
			if (document.selection)
			{
				var sel = window.document.selection.createRange();
				if (sel.text && sel.text.length > 0)
				{
					sel.moveStart('character', sel.text.length);
					sel.select();
				}
			}
			else if (this.SelectedTagsHandle.selectionStart || this.SelectedTagsHandle.selectionStart == '0')
			{
				this.SelectedTagsHandle.selectionStart = this.SelectedTagsHandle.selectionEnd;
			}
			
			event.cancelBubble = true;
			event.returnValue = false;
			return false;
		}
		else if (event.keyCode == 8)
		{
			this.SelectedTagsHandle.focus();
			
			if (document.selection)
			{
				var sel = window.document.selection.createRange();
				if (sel)
				{
					sel.moveStart('character', -1);
					sel.text = "";
					
					event.cancelBubble = true;
					event.returnValue = false;
					return false;
				}
			}
			else if (this.SelectedTagsHandle.selectionStart || this.SelectedTagsHandle.selectionStart == '0')
			{
				var start = this.SelectedTagsHandle.selectionStart;
				var end = this.SelectedTagsHandle.selectionEnd;
			
				if (start == end)
					return true;
				
				if (start > 0)
					start--;
				
				this.SelectedTagsHandle.value = this.SelectedTagsHandle.value.substring(0, start) + this.SelectedTagsHandle.value.substring(end);
				this.SelectedTagsHandle.selectionStart = start;
				this.SelectedTagsHandle.selectionEnd = start;
				
				event.cancelBubble = true;
				event.returnValue = false;
				return false;
			}
		}
		
		return true;
	}	
		
	this.EditorKeyUp = function(event)
	{
		if (!document.selection && !this.SelectedTagsHandle.selectionStart && this.SelectedTagsHandle.selectionStart != '0')
			return true;
	
		if (this.SelectedTagsHandle.value == this.PreviousValue)
			return true;
			
		this.PreviousValue = this.SelectedTagsHandle.value;
	
		this.GetCurrentTag();
		if (this.CurrentTag == "")
			return true;
			
		var suggestion = this.GetTagSuggestion(this.CurrentTag);
		if (!suggestion || suggestion.length == this.CurrentTag.length)
			return true;

		suggestion = suggestion.substr(this.CurrentTag.length);

		this.SelectedTagsHandle.value = this.SelectedTagsHandle.value.substr(0, this.CurrentTagPosition + this.CurrentTag.length) + suggestion + this.SelectedTagsHandle.value.substr(this.CurrentTagPosition + this.CurrentTag.length);
		this.SelectedTagsHandle.focus();
		
		if (document.selection)
		{
			var textrange = this.SelectedTagsHandle.createTextRange();
			textrange.select();

			textrange.moveStart('character', this.CurrentTagPosition + this.CurrentTag.length);
			textrange.moveEnd('character', -(this.SelectedTagsHandle.value.length - (this.CurrentTagPosition + this.CurrentTag.length + suggestion.length)));
			textrange.select();
		}
		else if (this.SelectedTagsHandle.selectionStart || this.SelectedTagsHandle.selectionStart == '0')
		{
			this.SelectedTagsHandle.selectionStart = this.CurrentTagPosition + this.CurrentTag.length;
			this.SelectedTagsHandle.selectionEnd = this.CurrentTagPosition + this.CurrentTag.length + suggestion.length;
		}
		
		return true;
	}
	
	this.GetCurrentTag = function()
	{
		var newTags = this.SelectedTagsHandle.value.toLowerCase().split(/;|,/);
		var i, j, matched;
		this.CurrentTag = "";
		var position = 0;
		var tempTag;
		var hasCurrentTag = true;
		for (i = 0; i < newTags.length; i++)
		{
			tempTag = newTags[i].replace(/^\s+/g, '');
			position += newTags[i].length - tempTag.length;
			newTags[i] = tempTag;
		
			matched = false;
			for (j = 0; j < this.PreviousTags.length && !matched; j++)
			{
				if (newTags[i] == this.PreviousTags[j])
				{
					this.PreviousTags.splice(j, 1);
					matched = true;					
				}
			}
			
			if (!matched)
			{
				if (this.CurrentTag != "")
					hasCurrentTag = false;
				else
				{
					this.CurrentTag = newTags[i];
					this.CurrentTagPosition = position;
				}
			}
			
			position += newTags[i].length + 1;
		}
		
		if (!hasCurrentTag)
			this.CurrentTag = "";
		
		this.PreviousTags = newTags;
	}
	
	this.GetTagSuggestion = function(tag)
	{
		var i, j, match;
		var tags = this.GetAllTags();
		
		for (i = 0; i < tags.length; i++)
		{
			if (tags[i].toLowerCase().indexOf(tag) == 0)
			{
				match = false;
				
				for (j = 0; j < this.PreviousTags.length && !match; j++)
				{
					if (this.PreviousTags[j] == tags[i].toLowerCase())
						match = true;
				}
			
				if (!match)
					return tags[i];
			}
		}
		
		return null;
	}
	
	if (this.SelectedTagsHandle)
	{
		this.GetCurrentTag();
		this.SelectedTagsHandle.onkeyup = new Function('event', 'return ' + this.VariableName + '.EditorKeyUp(event);');
		this.SelectedTagsHandle.onkeydown = new Function('event', 'return ' + this.VariableName + '.EditorKeyDown(event);');
	}
}
