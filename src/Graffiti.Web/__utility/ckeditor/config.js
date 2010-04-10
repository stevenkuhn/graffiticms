/*
Copyright (c) 2003-2010, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/
CKEDITOR.editorConfig = function(config) {
	// Define changes to default configuration here. For example:
	// config.uiColor = '#AADC6E';

	config.toolbar = 'Normal';
	config.toolbar_Normal =
	[
		['Font', 'FontSize', 'TextColor', 'BGColor'],
		['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
		['Undo', 'Redo', '-', 'SpellChecker', 'Scayt', '-', 'Maximize', 'Source'],
		'/',
		['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
		['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
		['Link', 'Unlink', 'Anchor'],
		['Image', 'Table', 'SpecialChar', 'PasteFromWord', '-', 'Templates']
	];

	config.toolbar = 'Simple';
	config.toolbar_Simple =
	[
		['Bold', 'Italic', 'Underline', 'Strike'],
		['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
		['Link', 'Unlink', 'Anchor'],
		['Image', 'Table', 'SpecialChar', 'PasteFromWord'],
		['SpellChecker', 'Maximize', 'Source']
	];
};
