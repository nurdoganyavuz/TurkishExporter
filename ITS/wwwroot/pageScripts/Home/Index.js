; (function ($, window, document, undefined) {
    $('.date-own').datepicker({
        minViewMode: 2,
        format: 'yyyy',
        autoclose: true
    });

    $(document).on('change', '#requestStatusFilterYear', function () {
        window.location.replace("/Home?year=" + $(this).val());
    });

    ////////////////////////////////////////
    var plotObj = $.plot($("#sendTheMostRequestByDepartment"), sendTheMostRequestByDepartment, {
        series: {
            pie: {
                show: true
            }
        },
        grid: {
            hoverable: true
        },
        tooltip: true,
        tooltipOpts: {
            content: "%p.0%, %s", // show percentages, rounding to 2 decimal places
            shifts: {
                x: 20,
                y: 0
            },
            defaultTheme: false
        }
    });
    ////////////////////////////////////////

    var barOptions = {
        responsive: true
    };


    var ctx2 = document.getElementById("requestStatus").getContext("2d");
    new Chart(ctx2, { type: 'bar', data: requestsData, options: barOptions });
})(jQuery, window, document);