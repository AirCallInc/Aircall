/**
* @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
* For licensing, see LICENSE.md or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.format_tags = 'p;h1;h2;h3;pre';
    config.allowedContent = true;
    // Make dialogs simpler.
    config.removeDialogTabs = 'image:advanced;link:advanced';
    config.filebrowserImageBrowseUrl = CKEDITOR.basePath + "filemanager/index.html";
    config.filebrowserBrowseUrl = CKEDITOR.basePath + "filemanager/index.html";
    config.filebrowserUploadUrl = CKEDITOR.basePath + "filemanager/index.html";
    config.filebrowserWindowWidth = 500;
    config.filebrowserWindowHeight = 650;
    config.filebrowserImageWindowWidth = 780;
    config.filebrowserImageWindowHeight = 720;
    config.width = "auto";
    config.height = "auto";
	config.removePlugins = 'elementspath,save';
};
