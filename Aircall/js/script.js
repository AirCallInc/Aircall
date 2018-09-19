
/* Script on scroll
------------------------------------------------------------------------------*/
$(window).scroll(function() {
	//---- main banner script ----- //
	
	//----- news banner script----- //
});

/* Script on resiz
------------------------------------------------------------------------------*/
$(window).resize(function() {
    
});

/* Script on load
------------------------------------------------------------------------------*/
$(window).load(function() {
	
	
});

/* Script on ready
------------------------------------------------------------------------------*/	
$(document).ready(function(){
	
    //---- mobile nav script ----- //
    $(".btn-m-nav").click(function(){
        if($(this).hasClass("isopen")){
            $(this).removeClass("isopen");
            $("#wrapper").animate({"left":"0"},500);
            $("footer").animate({"left":"0"},500);
            $(".nav-content ul li ul.first-sub").slideUp();	
            $(".nav-content ul li ul.second-sub").slideUp();
            $('.nav-content ul li.isopen').removeClass("isopen");

        }else{
            $(this).addClass("isopen");
            $("#wrapper").animate({"left":"-80%"},500);		
            $("footer").animate({"left":"-80%"},500);
        }	
    });
	 
	$(".nav-content ul li").find("ul").parents("li").append("<em>")
    $(".nav-content ul li ul").addClass("first-sub");
    $(".nav-content ul li ul").next("em").addClass("first-em");
    $(".nav-content ul li ul ul").removeClass("first-sub");
    $(".nav-content ul li ul ul").addClass("second-sub");
    $(".nav-content ul li ul ul").next("em").addClass("second-em");
    $(".nav-content ul li ul ul").next("em").removeClass("first-em");
    $(".nav-content ul li em.first-em").click(function(e) {
        if($(this).parent("li").hasClass("isopen")){
            $(this).parent("li").removeClass("isopen");
            $(this).prev("ul.first-sub").slideUp();
            $(".nav-content ul li ul.second-sub li").removeClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();
        }else{
            $(".nav-content ul li").removeClass("isopen");
            $(this).parent("li").addClass("isopen");
            $(".nav-content ul li ul.first-sub").slideUp();	
            $(this).prev("ul.first-sub").slideDown();
            $(".nav-content ul li ul.second-sub li").removeClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();
        }
    });
	
    $(".nav-content ul li ul.first-sub li em.second-em").click(function(e) {
        if($(this).parent("li").hasClass("isopen")){
            $(this).parent("li").removeClass("isopen");
            $(this).prev("ul.second-sub").slideUp();
        }else{
            $(".nav-content ul li ul li").removeClass("isopen");
            $(this).parent("li").addClass("isopen");
            $(".nav-content ul li ul.second-sub").slideUp();	
            $(this).prev("ul.second-sub").slideDown();
        }
     });
	 
	 $(".nav-content ul li a").click(function(){
		$(this).removeClass("isopen");
		$("#wrapper").animate({"left":"0"},500);
		$("footer").animate({"left":"0"},500);
		$(".nav-content ul li ul.first-sub").slideUp();	
		$(".nav-content ul li ul.second-sub").slideUp();
		$('.nav-content ul li.isopen').removeClass("isopen");
	 });
    
    
    //----- Choosing plan tabing ----- //
	$('.tab-links a.main-btn').on('click', function(e)  {
		var currentAttrValue = jQuery(this).attr('href');
		jQuery('.tab-content ' + currentAttrValue).fadeIn(400).addClass('active').siblings().removeClass('active').hide();
		jQuery(this).parent('li').addClass('active').siblings().removeClass('active');
		e.preventDefault();
	});
    
    /* star-rating script */
    $('.starbox').each(function() {
        var starbox = jQuery(this);
        starbox.starbox({
            average: 0.8,
            stars: 5,
            buttons: 5, //false will allow any value between 0 and 1 to be set
            ghosting: true,
            changeable: true, // true, false, or "once"
            autoUpdateAverage: true
        });
    });
    
    /* jquery ui script */
    $( ".checkbox-outer, .radio-outer" ).buttonset();
    $( ".select-outer select" ).selectmenu();
    $( ".datepicker" ).datepicker();
    
    /* schedule datepicker script */
    $( ".schedule-calender #datepicker" ).datepicker({
        autoSize: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        dayNamesMin: [ "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" ],
        beforeShowDay : function(date) {
            var day = date.getDate();
            return [true, (day < 10 ? "zero" : "")];
        }
    });
    
    /* Homepage Banner slider script */
    if($('.banner-home').length){
        //$('.banner-home').owlCarousel({
        //    loop:false,
        //    margin:0,
        //    nav:false,
        //    dots: true,
        //    mouseDrag: false,
        //    responsive:{
        //        0:{
        //            items:1
        //        },
        //        640:{
        //            items:1
        //        },
        //        768:{
        //            items:1
        //        },
        //        1024:{
        //            items:1
        //        }
        //    }
        //})
    }
    
    
    
    /* Homepage Banner slider script */
    //if($('.testimonial-slider-inner').length){
    //    $('.testimonial-slider-inner').owlCarousel({
    //        loop:false,
    //        margin:0,
    //        nav:false,
    //        dots: true,
    //        mouseDrag: true,
    //        responsive:{
    //            0:{
    //                items:1
    //            },
    //            640:{
    //                items:1
    //            },
    //            768:{
    //                items:1
    //            },
    //            1024:{
    //                items:1
    //            }
    //        }
    //    })
    //}
    
    
});


/* Script all functions
------------------------------------------------------------------------------*/	
	//---- sticky footer script ----- //
	function StickyFooter(){
		var Stickyfooter =    $('footer').outerHeight()
		$('#wrapper').css('margin-bottom',-Stickyfooter)
		$('#wrapper .footer-push').css('height',Stickyfooter)
	}
	$(window).on("load resize scroll ready",function(){
		var width = $(window).width();
		if( width <= 767){
			$('html,body').css("height","auto");
		}
		else{
			StickyFooter()
			$('html,body').css("height","100%");
		}
	});


    //----- testimonial equal height script ----- //
    function equal(){
        $(".testimonial, .benefits").each(function(){
            var max = 1; 
            $(this).find(".equal").css("height","auto"); 
            $(this).find(".equal").each(function() {
                var height1 = $(this).innerHeight(); 
                max = (height1 > max) ? height1 : max;
            });

            $(this).find(".equal").css("height",max); 
        }) 
    }
    $(window).on("load resize scroll ready",function(){
        setTimeout(function(){
            equal();    
        },1000)
    });

