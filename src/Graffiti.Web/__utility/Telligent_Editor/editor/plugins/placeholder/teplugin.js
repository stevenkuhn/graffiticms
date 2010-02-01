/*

 * Copyright (C) 2003-2007 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Plugin to insert "Placeholders" in the editor.
 */

// Register the related command.
TECommands.RegisterCommand( 'Placeholder', new TEDialogCommand( 'Placeholder', TELang.PlaceholderDlgTitle, TEPlugins.Items['placeholder'].Path + 'te_placeholder.html', 340, 170 ) ) ;

// Create the "Plaholder" toolbar button.
var oPlaceholderItem = new TEToolbarButton( 'Placeholder', TELang.PlaceholderBtn ) ;
oPlaceholderItem.IconPath = TEPlugins.Items['placeholder'].Path + 'placeholder.gif' ;

TEToolbarItems.RegisterItem( 'Placeholder', oPlaceholderItem ) ;


// The object used for all Placeholder operations.
var TEPlaceholders = new Object() ;

// Add a new placeholder at the actual selection.
TEPlaceholders.Add = function( name )
{
	var oSpan = TE.InsertElement( 'span' ) ;
	this.SetupSpan( oSpan, name ) ;
}

TEPlaceholders.SetupSpan = function( span, name )
{
	span.innerHTML = '[[ ' + name + ' ]]' ;

	span.style.backgroundColor = '#ffff00' ;
	span.style.color = '#000000' ;

	if ( TEBrowserInfo.IsGecko )
		span.style.cursor = 'default' ;

	span._teplaceholder = name ;
	span.contentEditable = false ;

	// To avoid it to be resized.
	span.onresizestart = function()
	{
		TE.EditorWindow.event.returnValue = false ;
		return false ;
	}
}

// On Gecko we must do this trick so the user select all the SPAN when clicking on it.
TEPlaceholders._SetupClickListener = function()
{
	TEPlaceholders._ClickListener = function( e )
	{
		if ( e.target.tagName == 'SPAN' && e.target._teplaceholder )
			TESelection.SelectNode( e.target ) ;
	}

	TE.EditorDocument.addEventListener( 'click', TEPlaceholders._ClickListener, true ) ;
}

// Open the Placeholder dialog on double click.
TEPlaceholders.OnDoubleClick = function( span )
{
	if ( span.tagName == 'SPAN' && span._teplaceholder )
		TECommands.GetCommand( 'Placeholder' ).Execute() ;
}

TE.RegisterDoubleClickHandler( TEPlaceholders.OnDoubleClick, 'SPAN' ) ;

// Check if a Placholder name is already in use.
TEPlaceholders.Exist = function( name )
{
	var aSpans = TE.EditorDocument.getElementsByTagName( 'SPAN' ) ;

	for ( var i = 0 ; i < aSpans.length ; i++ )
	{
		if ( aSpans[i]._teplaceholder == name )
			return true ;
	}

	return false ;
}

if ( TEBrowserInfo.IsIE )
{
	TEPlaceholders.Redraw = function()
	{
		if ( TE.EditMode != TE_EDITMODE_WYSIWYG )
			return ;

		var aPlaholders = TE.EditorDocument.body.innerText.match( /\[\[[^\[\]]+\]\]/g ) ;
		if ( !aPlaholders )
			return ;

		var oRange = TE.EditorDocument.body.createTextRange() ;

		for ( var i = 0 ; i < aPlaholders.length ; i++ )
		{
			if ( oRange.findText( aPlaholders[i] ) )
			{
				var sName = aPlaholders[i].match( /\[\[\s*([^\]]*?)\s*\]\]/ )[1] ;
				oRange.pasteHTML( '<span style="color: #000000; background-color: #ffff00" contenteditable="false" _teplaceholder="' + sName + '">' + aPlaholders[i] + '</span>' ) ;
			}
		}
	}
}
else
{
	TEPlaceholders.Redraw = function()
	{
		if ( TE.EditMode != TE_EDITMODE_WYSIWYG )
			return ;

		var oInteractor = TE.EditorDocument.createTreeWalker( TE.EditorDocument.body, NodeFilter.SHOW_TEXT, TEPlaceholders._AcceptNode, true ) ;

		var	aNodes = new Array() ;

		while ( ( oNode = oInteractor.nextNode() ) )
		{
			aNodes[ aNodes.length ] = oNode ;
		}

		for ( var n = 0 ; n < aNodes.length ; n++ )
		{
			var aPieces = aNodes[n].nodeValue.split( /(\[\[[^\[\]]+\]\])/g ) ;

			for ( var i = 0 ; i < aPieces.length ; i++ )
			{
				if ( aPieces[i].length > 0 )
				{
					if ( aPieces[i].indexOf( '[[' ) == 0 )
					{
						var sName = aPieces[i].match( /\[\[\s*([^\]]*?)\s*\]\]/ )[1] ;

						var oSpan = TE.EditorDocument.createElement( 'span' ) ;
						TEPlaceholders.SetupSpan( oSpan, sName ) ;

						aNodes[n].parentNode.insertBefore( oSpan, aNodes[n] ) ;
					}
					else
						aNodes[n].parentNode.insertBefore( TE.EditorDocument.createTextNode( aPieces[i] ) , aNodes[n] ) ;
				}
			}

			aNodes[n].parentNode.removeChild( aNodes[n] ) ;
		}

		TEPlaceholders._SetupClickListener() ;
	}

	TEPlaceholders._AcceptNode = function( node )
	{
		if ( /\[\[[^\[\]]+\]\]/.test( node.nodeValue ) )
			return NodeFilter.FILTER_ACCEPT ;
		else
			return NodeFilter.FILTER_SKIP ;
	}
}

TE.Events.AttachEvent( 'OnAfterSetHTML', TEPlaceholders.Redraw ) ;

// We must process the SPAN tags to replace then with the real resulting value of the placeholder.
TEXHtml.TagProcessors['span'] = function( node, htmlNode )
{
	if ( htmlNode._teplaceholder )
		node = TEXHtml.XML.createTextNode( '[[' + htmlNode._teplaceholder + ']]' ) ;
	else
		TEXHtml._AppendChildNodes( node, htmlNode, false ) ;

	return node ;
}