

$(() => {

    LoadProdData();
    LoadScreenData();
    LoadScreenLowerData();
    LoadTotalData();
    LoadTotalMembersData();
    LoadTotalServedData();
    LoadTotalBeingServedData();
    LoadTotalServingStaffData();
    LoadTotalScreenservingData();
    LoadTotalScreenNextData();
    LoadTotalScreen2NextData();
    LoadTotalScreenCustomerData();
    LoadTotalScreenFosaData();
    LoadTotalScreenTellerData();
    LoadTotalScreenNextFosaData();
    LoadTotalScreenNextTellerData();
    LoadChatData();

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();
    connection.on("LoadTickets", function () {
        LoadProdData();
    })
    connection.on("LoadScreen", function () {
        LoadScreenData();
    })
    connection.on("LoadScreenLower", function () {
        LoadScreenLowerData();
    })
    connection.on("LoadTotal", function () {
        LoadTotalData();
    })
    connection.on("TotalMembers", function () {
        LoadTotalMembersData();
    })
    connection.on("TotalServedMembers", function () {
        LoadTotalServedData();
    })
    connection.on("TotalBeingMembers", function () {
        LoadTotalBeingServedData();
    })
    connection.on("TotalServingStaff", function () {
        LoadTotalServingStaffData();
    })
    connection.on("ScreenServing", function () {
        LoadTotalScreenservingData();
    })
    connection.on("ScreenNext", function () {
        LoadTotalScreenNextData();
    })
    connection.on("Screen2Next", function () {
        LoadTotalScreen2NextData();
    })
    connection.on("ScreenCustomer", function () {
        LoadTotalScreenCustomerData();
    })
    connection.on("ScreenFosa", function () {
        LoadTotalScreenFosaData();
    })
    connection.on("ScreenTeller", function () {
        LoadTotalScreenTellerData();
    })
    connection.on("ScreenNextFosa", function () {
        LoadTotalScreenNextFosaData();
    })
    connection.on("ScreenNextTeller", function () {
        LoadTotalScreenNextTellerData();
    })
    connection.on("chat", function () {
        LoadChatData();
    })


    LoadProdData();
    LoadScreenData();
    LoadScreenLowerData();
    LoadTotalData();
    LoadTotalMembersData();
    LoadTotalServedData();
    LoadTotalBeingServedData();
    LoadTotalServingStaffData();
    LoadTotalScreenservingData();
    LoadTotalScreenNextData();
    LoadTotalScreen2NextData();
    LoadTotalScreenCustomerData();
    LoadTotalScreenFosaData();
    LoadTotalScreenTellerData();
    LoadTotalScreenNextFosaData();
    LoadTotalScreenNextTellerData();
    LoadChatData();




    function LoadChatData() {
        var span = '';

        $.ajax({
            url: '/Home/GetChatCount',
            method: 'GET',
            success: (result) => {
                span = `${result}`
                $("#chat").html(span);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }



    function LoadTotalScreenNextFosaData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenNxtFosa',
            method: 'GET',
            success: (result) => {
                h1 = `<h1 class="text-center text-white"><strong style="font-size:80px">${result}</strong></h1>
                            <p class="float-right text-white px-2">NEXT - FOSA</p>`
                $("#ScrnNxtFosa").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadTotalScreenNextTellerData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenNxtTeller',
            method: 'GET',
            success: (result) => {
                h1 = `<h1 class="text-center text-white"><strong style="font-size:80px">${result}</strong></h1>
                            <p class="float-right text-white px-2">NEXT - TELLER</p>`
                $("#ScrnNxtTeller").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadTotalScreen2NextData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenNxtCS',
            method: 'GET',
            success: (result) => {
                h1 = `<h1 class="text-center text-white"><strong style="font-size:80px">${result}</strong></h1>
                            <p class="float-right text-white px-2">NEXT - CUSTOMER SERVICE</p>`
                $("#ScrnNxtCS").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalScreenNextData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenNxtReception',
            method: 'GET',
            success: (result) => {
                h1 = `<h1 class="text-center text-white"><strong style="font-size:80px">${result}</strong></h1>
                            <p class="float-right text-white px-2">NEXT - RECEPTION</p>`
                $("#ScrnNxtReception").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalScreenservingData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenReception',
            method: 'GET',
            success: (result) => {
                h1 += `
                    <h6 class="text-white">NOW SERVING - RECEPTION</h6>
                    <h1 style="font-size:45px; font:Maiandra GD;" class="text-center text-white"><strong>${result}</strong></h1>`
                $("#reception").html(h1);
            }, error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadTotalScreenCustomerData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenCustomer',
            method: 'GET',
            success: (result) => {
                h1 += `
                    <h6 class="text-white">NOW SERVING - CUSTOMER SERVICE</h6>
                    <h1 style="font-size:45px; font:Maiandra GD;" class="text-center text-white"><strong>${result}</strong></h1>`
                $("#SCS").html(h1);
            }, error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadTotalScreenFosaData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenFosa',
            method: 'GET',
            success: (result) => {
                h1 += `
                    <h6 class="text-white">NOW SERVING - FOSA</h6>
                    <h1 style="font-size:45px; font:Maiandra GD;" class="text-center text-white"><strong>${result}</strong></h1>`
                $("#Fosa").html(h1);
            }, error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadTotalScreenTellerData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/ScreenTeller',
            method: 'GET',
            success: (result) => {
                h1 += `
                    <h6 class="text-white">NOW SERVING - TELLER</h6>
                    <h1 style="font-size:45px; font:Maiandra GD;" class="text-center text-white"><strong>${result}</strong></h1>`
                $("#Teller").html(h1);
            }, error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalBeingServedData() {
        var h4 = '';

        $.ajax({
            url: '/Home/Getbeingservedmembers',
            method: 'GET',
            success: (result) => {
                h4 = `<h4 class="text-white">${result}</h4>`
                $("#totalbeingserved").html(h4);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalServingStaffData() {
        var h4 = '';

        $.ajax({
            url: '/Home/Getstaffservingmembers',
            method: 'GET',
            success: (result) => {
                h4 = `<h4 class="text-white">${result}</h4>`
                $("#totalstaffserving").html(h4);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalServedData() {
        var h4 = '';

        $.ajax({
            url: '/Home/Getservedmembers',
            method: 'GET',
            success: (result) => {
                h4 = `<h4 class="text-white">${result}</h4>`
                $("#totalmembersserved").html(h4);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalMembersData() {
        var h4 = '';
        
        $.ajax({
            url: '/Home/Getwaitingmembers',
            method: 'GET',
            success: (result) => {
                h4 = `<h4 class="text-white">${result}</h4>`
                $("#totalmembers").html(h4);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadTotalData() {
        var b = '';

        $.ajax({
            url: '/Home/GetCount',
            method: 'GET',
            success: (result) => {
                b = `<b>${result}</b>`
                $("#totalcount").html(b);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }


    function LoadProdData() {
        var tr = '';

        $.ajax({
            url: '/Home/GetTickets',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                            <td>${v.TicketNumber}</td>
                            <td>${v.Section.Name}</td>
                            <td>${v.Service.Name}</td>
                            <td>${v.CheckInTime}</td>
                           </tr>`
                })
                $("#tableBody").html(tr);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }


    function LoadScreenData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/GetLoadScreen',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    h1 += `<div class="card shadow-sm mb-2" style="background-color: #128315; height: 6.7rem;"><h1 class="text-center text-white float-left"><strong style="font-size:80px">${v.DeskName} - ${v.Status}</strong></h1>
                            </div>`
                })
                $("#myscreeen").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

    function LoadScreenLowerData() {
        var h1 = '';

        $.ajax({
            url: '/Tickets/GetLoadScreenLower',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    h1 += `<div class="col-lg-3">
                            <div class="row bg-dark">
                                <div class="col p-1 d-flex flex-column">
                                    <h1 class="text-center text-white"><strong style="font-size:80px">${v.TicketNumber}</strong></h1>
                                    <p class="float-right text-white px-2">COUNTER ${v.DeskName} - NOW SERVING</p>
                                </div>
                            </div>
                        </div>`
                })
                $("#myscreeenlower").html(h1);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

})