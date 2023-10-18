// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


    function GetGlobalIp(){
        $.ajax({
            type: "POST",
            url: "/Home/GetGlobalIpAdress",
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    alert("ip : " + response.dnsString.ToString());
                } else {
                    alert("Something went wrong");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

