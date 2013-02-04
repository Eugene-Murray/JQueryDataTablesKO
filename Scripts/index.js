
$(document).ready(function () {
    
    var WebApp = window.WebApp = window.WebApp || {};
    
    WebApp.AjaxHelper = function (ajaxConfig) {
        return $.ajax({
            url: ajaxConfig.Url,
            type: ajaxConfig.VerbType,
            contentType: ajaxConfig.ContentType
        });
    };

    
    ko.bindingHandlers.dataTable = {
        init: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            // If the binding is an object with an options field,
            // initialise the dataTable with those options. 
            if (binding.options) {
                $(element).dataTable(binding.options);
            }
        },
        update: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            // If the binding isn't an object, turn it into one. 
            if (!binding.data) {
                binding = { data: valueAccessor() };
            }

            // Clear table
            $(element).dataTable().fnClearTable();

            // Rebuild table from data source specified in binding
            $(element).dataTable().fnAddData(binding.data());
        }
    };

    //var oTable = $('#myDataTable').dataTable({
    //	"bServerSide": true,
    //	"sAjaxSource": "Home/AjaxHandler", 
    //	"bProcessing": true,
    //	"aoColumns": [
    //                    {   "sName": "ID",
    //                        "bSearchable": false,
    //                        "bSortable": false,
    //                        "fnRender": function (oObj) {
    //                            return '<a href=\"Company/Details/' + oObj.aData[0] + '\">View</a>';
    //                        }
    //                    },
	//		            { "sName": "COMPANY_NAME" },
	//		            { "sName": "ADDRESS" },
	//		            { "sName": "TOWN" }
	//	            ]
    //});
    
    var dataTableExampleViewModel = {
        tableData: ko.observableArray([["Existing", "Data"], ["In An", "observableArray"]]),
        add: function () {
            this.tableData.push([(new Date()).getTime(), "Added"]);
        },
        remove: function () {
            this.tableData.pop();
        },
        
        getParents: function () {

            var ajaxConfig = { Url: "/Home/GetDataJson", VerbType: "GET", ContentType: "application/json;charset=utf-8" };

            var response = $.ajax({
                url: ajaxConfig.Url,
                type: ajaxConfig.VerbType,
                contentType: ajaxConfig.ContentType
            });


            response.success($.proxy(function(data) {
                //dataTableExampleViewModel.tableData(data);
                console.log(data);

                $.each(data, function(key, value) {
                    console.log(value.Address);
                    console.log(value.Town);
                    dataTableExampleViewModel.tableData.push([value.Address, value.Town]);
                });

            }, this));

            response.fail(function (jqXhr, textStatus) {
                // TODO: notify user
                console.log(jqXhr);
                console.log(textStatus);
                alert("Error!");
            });

        }
    };
    dataTableExampleViewModel.getParents();
    ko.applyBindings(dataTableExampleViewModel);
    
   
});
