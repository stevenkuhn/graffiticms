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
 * Plugin: automatically resizes the editor until a configurable maximun
 * height (TEConfig.AutoGrowMax), based on its contents.
 */

var TEAutoGrow_Min = window.frameElement.offsetHeight ;

function TEAutoGrow_Check()
{
	var oInnerDoc = TE.EditorDocument ;

	var iFrameHeight, iInnerHeight ;

	if ( TEBrowserInfo.IsIE )
	{
		iFrameHeight = TE.EditorWindow.frameElement.offsetHeight ;
		iInnerHeight = oInnerDoc.body.scrollHeight ;
	}
	else
	{
		iFrameHeight = TE.EditorWindow.innerHeight ;
		iInnerHeight = oInnerDoc.body.offsetHeight ;
	}

	var iDiff = iInnerHeight - iFrameHeight ;

	if ( iDiff != 0 )
	{
		var iMainFrameSize = window.frameElement.offsetHeight ;

		if ( iDiff > 0 && iMainFrameSize < TEConfig.AutoGrowMax )
		{
			iMainFrameSize += iDiff ;
			if ( iMainFrameSize > TEConfig.AutoGrowMax )
				iMainFrameSize = TEConfig.AutoGrowMax ;
		}
		else if ( iDiff < 0 && iMainFrameSize > TEAutoGrow_Min )
		{
			iMainFrameSize += iDiff ;
			if ( iMainFrameSize < TEAutoGrow_Min )
				iMainFrameSize = TEAutoGrow_Min ;
		}
		else
			return ;

		window.frameElement.height = iMainFrameSize ;
	}
}

TE.AttachToOnSelectionChange( TEAutoGrow_Check ) ;

function TEAutoGrow_SetListeners()
{
	if ( TE.EditMode != TE_EDITMODE_WYSIWYG )
		return ;

	TE.EditorWindow.attachEvent( 'onscroll', TEAutoGrow_Check ) ;
	TE.EditorDocument.attachEvent( 'onkeyup', TEAutoGrow_Check ) ;
}

if ( TEBrowserInfo.IsIE )
{
//	TEAutoGrow_SetListeners() ;
	TE.Events.AttachEvent( 'OnAfterSetHTML', TEAutoGrow_SetListeners ) ;
}

function TEAutoGrow_CheckEditorStatus( sender, status )
{
	if ( status == TE_STATUS_COMPLETE )
		TEAutoGrow_Check() ;
}

TE.Events.AttachEvent( 'OnStatusChange', TEAutoGrow_CheckEditorStatus ) ;