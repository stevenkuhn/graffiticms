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
 * This plugin register the required Toolbar items to be able to insert the
 * table commands in the toolbar.
 */

TEToolbarItems.RegisterItem( 'TableInsertRowAfter'		, new TEToolbarButton( 'TableInsertRowAfter'	, TELang.InsertRowAfter, null, null, null, true, 62 ) ) ;
TEToolbarItems.RegisterItem( 'TableDeleteRows'		, new TEToolbarButton( 'TableDeleteRows'	, TELang.DeleteRows, null, null, null, true, 63 ) ) ;
TEToolbarItems.RegisterItem( 'TableInsertColumnAfter'	, new TEToolbarButton( 'TableInsertColumnAfter'	, TELang.InsertColumnAfter, null, null, null, true, 64 ) ) ;
TEToolbarItems.RegisterItem( 'TableDeleteColumns'	, new TEToolbarButton( 'TableDeleteColumns', TELang.DeleteColumns, null, null, null, true, 65 ) ) ;
TEToolbarItems.RegisterItem( 'TableInsertCellAfter'		, new TEToolbarButton( 'TableInsertCellAfter'	, TELang.InsertCellAfter, null, null, null, true, 58 ) ) ;
TEToolbarItems.RegisterItem( 'TableDeleteCells'	, new TEToolbarButton( 'TableDeleteCells'	, TELang.DeleteCells, null, null, null, true, 59 ) ) ;
TEToolbarItems.RegisterItem( 'TableMergeCells'		, new TEToolbarButton( 'TableMergeCells'	, TELang.MergeCells, null, null, null, true, 60 ) ) ;
TEToolbarItems.RegisterItem( 'TableHorizontalSplitCell'		, new TEToolbarButton( 'TableHorizontalSplitCell'	, TELang.SplitCell, null, null, null, true, 61 ) ) ;
TEToolbarItems.RegisterItem( 'TableCellProp'		, new TEToolbarButton( 'TableCellProp'	, TELang.CellProperties, null, null, null, true, 57 ) ) ;
