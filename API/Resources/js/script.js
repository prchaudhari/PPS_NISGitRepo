var apiUrl = 'http://localhost:{{APIPORTNO}}/AnalyticsData/Save';
//var apiUrl = 'http://nisqa-api.azurewebsites.net/AnalyticsData/Save';

$(document).ready(function () {
    $(".mainNav").on('click', function (e) {
        $('.tabDivClass').hide();
        var tag = e.currentTarget;
        $('.mainNav').removeClass('active');
        let newClasses = 'active ' + $(tag).attr('class');
        $(tag).attr('class', newClasses);
        let classlist = $(tag).attr('class').split(' ');
        let className = classlist[classlist.length - 1];
        if ($('.' + className).hasClass('d-none')) {
            $('.' + className).removeClass('d-none');
        }
        $('#' + className).show();
        var x = document.getElementsByClassName(newClasses);
        x[0].id;
        //console.log(x);
        var newItems = newClasses.split(' ');
        var length = newItems.length;
        var newItemsPart = newItems[length - 1].split('-');
        pageId = newItemsPart[newItemsPart.length - 1];
        //pageId = x[0].id.split('_')[x[0].id.split('_').length - 1];
        var object = {
            "StatementId": statementId,
            "CustomerId": customer,
            "PageId": pageId,
            "WidgetId": 0,
            "EventDate": new Date(),
            "EventType": "PageLoad",
            "AccountId": "",
            "TenantCode": TenantCode
        };
        //console.log(object);
        var data = [];
        data.push(object);
        //console.log(object);
        $.ajax({
            url: apiUrl,
            type: 'POST',
            dataType: 'json',
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, textStatus, xhr) {
                console.log(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error in Operation');
            }
        });
    });
});

var customer = 10;
var statementId = 15;
var pageId = 0;
var widgetId = 0;
var accountId = 0;
var TenantCode = '';
function click_event(event) {
    // console.log(event);
    //console.log(customer);
    var value = checkElement(event.srcElement);
    if (value) {

    }
}

function checkElement(element) {
    if (element != null) {
        if (element.id != undefined) {
            var elementId = element.id;

            if (elementId.includes("PageWidgetId_")) {
                var id = '';
                var elements = elementId.split("_");
                pageId = elements[1];
                //widgetId = elements[3];
                var accId = element.parentElement.parentElement.parentElement.id;
                if (accId.includes("AccountNumber-"));
                {
                    var idParts = accId.split("-");
                    accountId = idParts[3];
                }
                var object = {
                    "StatementId": parseInt(statementId),
                    "CustomerId": parseInt(customer),
                    "AccountId": parseInt(accountId),
                    "PageId": 0,
                    "PageWidgetId": parseInt(pageId),
                    "WidgetId": 0,
                    "EventDate": new Date(),
                    "EventType": "Click",
                    "TenantCode": TenantCode
                };
                //console.log(object);
                var data = [];
                data.push(object);

                $.ajax({
                    url: apiUrl,
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    success: function (data, textStatus, xhr) {
                        console.log(data);
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        console.log('Error in Operation');
                    }
                });
                return true;
            }
            else {
                checkElement(element.parentElement);
            }
        }
        else {
            if (element.parentElement != null) {
                checkElement(element.parentElement);
            }
            else {
                return null;
            }
        }
    }
    else {
        return null;
    }
}

function onPageLoad() {
    //console.log("on onPageLoad");
    pageId = document.getElementById("FirstPageId").value;
    customer = document.getElementById("CustomerId").value;
    statementId = document.getElementById("StatementId").value;
    TenantCode = document.getElementById("TenantCode").value;
    var object = {
        "StatementId": statementId,
        "CustomerId": customer,
        "PageId": 0,
        "WidgetId": 0,
        "EventDate": new Date(),
        "EventType": "StatementOpen",
        "AccountId": 0,
        "TenantCode": TenantCode
    };
    //console.log(object);
    var data = [];
    data.push(object);
    object = {
        "StatementId": statementId,
        "CustomerId": customer,
        "PageId": pageId,
        "WidgetId": 0,
        "EventDate": new Date(),
        "EventType": "PageLoad",
        "AccountId": 0,
        "TenantCode": TenantCode
    };
    data.push(object);
    //console.log(object);
    $.ajax({
        url: apiUrl,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, textStatus, xhr) {
            console.log(data);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error in Operation');
        }
    });
}

document.addEventListener('click', click_event, true);

function setSubTab(event) {
    console.log(event);
}