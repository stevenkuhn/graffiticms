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
 * This plugin register Toolbar items for the combos modifying the style to
 * not show the box.
 */

TEToolbarItems.RegisterItem( 'SourceSimple'	, new TEToolbarButton( 'Source', TELang.Source, null, TE_TOOLBARITEM_ONLYICON, true, true, 1 ) ) ;
TEToolbarItems.RegisterItem( 'StyleSimple'		, new TEToolbarStyleCombo( null, TE_TOOLBARITEM_ONLYTEXT ) ) ;
TEToolbarItems.RegisterItem( 'FontNameSimple'	, new TEToolbarFontsCombo( null, TE_TOOLBARITEM_ONLYTEXT ) ) ;
TEToolbarItems.RegisterItem( 'FontSizeSimple'	, new TEToolbarFontSizeCombo( null, TE_TOOLBARITEM_ONLYTEXT ) ) ;
TEToolbarItems.RegisterItem( 'FontFormatSimple', new TEToolbarFontFormatCombo( null, TE_TOOLBARITEM_ONLYTEXT ) ) ;
