
/* Script on scroll
------------------------------------------------------------------------------*/
$(window).scroll(function () {

});

/* Script on resiz
------------------------------------------------------------------------------*/
$(window).resize(function () {

});

/* Script on load
------------------------------------------------------------------------------*/
$(window).load(function () {

});

/* Script on ready
------------------------------------------------------------------------------*/
$(document).ready(function () {
    //---- mobile nav script ----- //
    $(".btn-m-nav").click(function () {
        if ($(this).hasClass("isopen")) {
            $(this).removeClass("isopen");
            $("#wrapper").animate({ "left": "0" }, 500);
            $("footer.main-footer").animate({ "left": "0" }, 500);
            $(".nav-content ul li ul.first-sub").slideUp();
            $(".nav-content ul li ul.second-sub").slideUp();
            $('.nav-content ul li.isopen').removeClass("isopen");

        } else {
            $(this).addClass("isopen");
            $("#wrapper").animate({ "left": "-80%" }, 500);
            $("footer.main-footer").animate({ "left": "-80%" }, 500);
        }
    });
    $(".nav-content ul li").find("ul").parents("li").append("<em>")
    $(".nav-content ul li ul").addClass("first-sub");
    $(".nav-content ul li ul").next("em").addClass("first-em");
    $(".nav-content ul li ul ul").removeClass("first-sub");
    $(".nav-content ul li ul ul").addClass("second-sub");
    $(".nav-content ul li ul ul").next("em").addClass("second-em");
    $(".nav-content ul li ul ul").next("em").removeClass("first-em");
    $(".nav-content ul li em.first-em").click(function (e) {
        if ($(this).parent("li").hasClass("isopen")) {
            $(this).parent("li").removeClass("isopen");
            $(this).prev("ul.first-sub").slideUp();
            $(".nav-content ul li ul.second-sub li").removeClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();
        } else {
            $(".nav-content ul li").removeClass("isopen");
            $(this).parent("li").addClass("isopen");
            $(".nav-content ul li ul.first-sub").slideUp();
            $(this).prev("ul.first-sub").slideDown();
            $(".nav-content ul li ul.second-sub li").removeClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();
        }
    });

    $(".nav-content ul li ul.first-sub li em.second-em").click(function (e) {
        if ($(this).parent("li").hasClass("isopen")) {
            $(this).parent("li").removeClass("isopen");
            $(this).prev("ul.second-sub").slideUp();
        } else {
            $(".nav-content ul li ul li").removeClass("isopen");
            $(this).parent("li").addClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();
            $(this).prev("ul.second-sub").slideDown();
        }
    });

    $(".nav-content ul li a").click(function () {
        $(this).removeClass("active");
        $("#wrapper").animate({ "left": "0" }, 500);
        $("footer.main-footer").animate({ "left": "0" }, 500);
        $(".nav-content ul li ul.first-sub").slideUp();
        $(".nav-content ul li ul.second-sub").slideUp();
        $('.nav-content ul li.isopen').removeClass("isopen");
    });

    /* star-rating script */
    $('.starbox1').each(function () {
        var starbox = jQuery(this);
        starbox.starbox({
            average: parseFloat(starbox.attr("title")),
            stars: 5,
            buttons: 5, //false will allow any value between 0 and 1 to be set
            ghosting: true,
            changeable: false, // true, false, or "once"
            autoUpdateAverage: false
        });
    });

    $('.starbox2').each(function () {
        var starbox = jQuery(this);
        starbox.starbox({
            average: parseFloat(starbox.attr("title")),
            stars: 5,
            buttons: 5, //false will allow any value between 0 and 1 to be set
            ghosting: true,
            changeable: false, // true, false, or "once"
            autoUpdateAverage: false
        });
    });
    $('.starbox3').each(function () {
        var starbox = jQuery(this);
        starbox.starbox({
            average: parseFloat(starbox.attr("title")),
            stars: 5,
            buttons: 10, //false will allow any value between 0 and 1 to be set
            ghosting: true,
            changeable: true, // true, false, or "once"
            autoUpdateAverage: true
        }).bind('starbox-value-changed', function (event, value) {
            $("#hdnRate").val(value);
        });
    });

    /* jquery ui script */
    $(".checkbox-outer, .radio-outer, .radio-outer-dot").buttonset();
    //$(".select-outer select").selectmenu("value", value);
    $(".scroll-select select").selectmenu().selectmenu("menuWidget").addClass("overflow");
    $(".select-outer select").selectmenu({
        select: function (event, ui) {
            $(this).trigger("change");
        }
    }
    );
    $(".drpMonth select").selectmenu().selectmenu("menuWidget").addClass("overflow");
    $(".drpYear select").selectmenu().selectmenu("menuWidget").addClass("overflow");

    //$(".datepicker").datepicker();
    $(".datepicker").datepicker({ minDate: 1 });

   /* dashboard slider script */
    if ($('.all-unit-blocks').length) {
        $('.all-unit-blocks').owlCarousel({
            loop: false,
            margin: 12,
            nav: false,
            dots: false,
            mouseDrag: true,
            responsive: {
                0: {
                    items: 2
                },
                640: {
                    items: 4
                },
                768: {
                    items: 4
                },
                1024: {
                    items: 5
                }
            }
        })
    }


    //---- mobile nav script ----- //
    $("a.add-new-card").click(function () {
        if ($(this).hasClass('open')) {
            $(this).removeClass('open')
            $('.new-card-detail-inner').slideUp();
        }
        else {
            $(this).addClass('open')
            $('.new-card-detail-inner').slideDown();
        }
    });



});

/* Script all functions
------------------------------------------------------------------------------*/
//---- sticky footer script ----- //
function StickyFooter() {
    var Stickyfooter = $('footer.main-footer').outerHeight()
    $('#wrapper').css('margin-bottom', -Stickyfooter)
    $('#wrapper .footer-push').css('height', Stickyfooter)
}
$(window).on("load resize scroll ready", function () {
    var width = $(window).width();
    if (width <= 767) {
        $('html,body').css("height", "auto");
    }
    else {
        StickyFooter()
        $('html,body').css("height", "100%");
    }
});
