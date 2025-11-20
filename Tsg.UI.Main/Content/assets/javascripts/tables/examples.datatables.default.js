///*
//Name: 			Tables / Advanced - Examples
//Written by: 	Okler Themes - (http://www.okler.net)
//Theme Version: 	1.5.2
//*/

//(function($) {

//	'use strict';

//	var datatableInit = function() {

//	    //$('#datatable-default').dataTable();

//	    $.fn.dataTableExt.afnFiltering.push(function (oSettings, aData, iDataIndex) {
//	        var checked = $('#checkbox').is(':checked');
	        
//	        if (checked && aData[2] != 0.0) {
//	            return true;
//	        }
//	        if (!checked) {
//	            return true;
//	        }
//	        return false;
//	    });

//	    var oTable = $('#datatable-default').dataTable({
//	        "order": [[0, "desc"]]
//	    });

//	    $('#checkbox').on("click", function (e) {	        
//	        var labelText = 'Uncheck to show all accounts';
//	        if ($(this).is(':checked')) {

//	            labelText = 'Uncheck to show all accounts';
//	        } else {
//	            labelText = '';
//	        }
//	        $(this).closest('div').find('label').html(labelText);

//	        oTable.fnDraw();
//	    });

//	};

//	$(function() {
//		datatableInit();
//	});

//}).apply(this, [jQuery]);