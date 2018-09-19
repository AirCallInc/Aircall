$(document).ready(function () {

    if (document.URL.indexOf("dashboard.aspx") != -1) {
        $("#liDashboard").addClass("active");
    }
    else if (document.URL.indexOf("Commission_Report.aspx") != -1) {
        $("#liCommission").addClass("active");
    }
    else if (document.URL.indexOf("Ticket_List.aspx") != -1 || document.URL.indexOf("Ticket_Add.aspx") != -1 || document.URL.indexOf("Ticket_Update.aspx") != -1) {
        $("#liTickets").addClass("active");
    }
    else if (document.URL.indexOf("Client_List.aspx") != -1) {
        $("#liClients").addClass("active");
    }
});

function validateLogin() {
    var error = 0;
    //ckeck if inputs aren't empty        

    var value = jQuery('#txtUsername').val();
    var password = jQuery('#txtPassword').val();
    if (value.length < 1 || value == 'Username') {
        jQuery('#dvUserName').addClass('error');
        jQuery('#dvUserName').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvUserName').removeClass('error');
        jQuery('#dvUserName').addClass('success');
    }

    if (password.length < 4 || password == 'Password') {
        jQuery('#dvPassword').addClass('error');
        jQuery('#dvPassword').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvPassword').removeClass('error');
        jQuery('#dvPassword').addClass('success');
    }

    if (!error) {
        return true;
    }
    else {
        return false;
    }
}
function validateForgotPassword() {
    var error = 0;
    var email = jQuery('#txtEmail').val();
    if (email.length < 1 || email == 'Email') {
        jQuery('#dvEmail').addClass('error');
        jQuery('#dvEmail').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvEmail').removeClass('error');
        jQuery('#dvEmail').addClass('success');
    }
    if (!error) {
        return true;
    }
    else {
        return false;
    }
}
$(window).load(function () {
    sidemenuheight();
    
    // if(document.getElementById("ddlLanguage") != undefined)
    // {
        $('html, body').animate({
        scrollTop: $("body").offset().top
        }, 0);
    //}
	//$("body").scrollTop(0);
});
function sidemenuheight() {
    var max = 1;
    $("#container .equal-height").each(function () {
        var i = $(this).index(i);
        var height1 = $(this).height();
        max = height1 > max ? height1 : max;
        //alert(max);
    })
    //$("#container .equal-height").css("min-height", "100px");
    $("#container .equal-height").css("min-height", max);
}
function changeoption()
{
    if ($("#rd_Text").is(":checked"))
    {
        $("#dvText").show();
        $("#dvPageImage").hide();
        $("#dvPageImageLink").hide();
    }
    else if ($("#rd_Image").is(":checked")) {
        $("#dvText").hide();
        $("#dvPageImage").show();
        $("#dvPageImageLink").show();
    }
    else {
        $("#dvText").show();
        $("#dvPageImage").show();
        $("#dvPageImageLink").show();
    }
}


function changeoption_help_type(type)
{
    if (type=="Image")
    {
        $("#dvText").hide();
        $("#dvImageTitle").show();
        $("#dvImageMain").show();
        $("#dvSubImage").show();
        $("#dvqa").hide();
    }
    else if (type=="QA") {
        //alert("QA Call");
        $("#dvText").hide();
        $("#dvImageTitle").hide();
        $("#dvImageMain").hide();
        $("#dvSubImage").hide();
        $("#dvqa").show();
        
        
    }
    else {
        $("#dvText").show();
        $("#dvImageTitle").hide();
        $("#dvImageMain").hide();
        $("#dvqa").hide();
    }
}

function add_one_image_items(){
    $("#plus_button").remove();
    $("#dvSubImage").append("<div id='dvDetailImage1' class='control-group'><label class='control-label'><input type='file' name='ctl00$ContentPlaceHolder1$DealDetailImage1' id='DealDetailImage1' style='margin-top:-10px;'><div style='margin-top:8px'><a href='viewimage_link.html' style='position: absolute;''>View Image</a></div></label><div class='controls'><div id='ContentPlaceHolder1_Div1'><div id='ContentPlaceHolder1_dvdropzonedd1' data-canvas='true' data-width='500' data-image='' data-url='imagehandlers/dealdetailimagehandler1.aspx' class='dropzone' style='min-width: 350px; min-height: 0px; box-sizing: border-box; height: 70px; width: 350px;' data-ghost='false' data-ajax='true' data-height='500'><textarea  name='game_short_description' id='game_short_description' class='span4 required' rows='2' style='position: absolute;'></textarea></div></div></div></div><div align='right' id='plus_button'><a class='btn btn-info add' onclick='add_one_image_items()''><i class='icon-plus icon-white'></i></a></div>");
}

function add_one_qa_items(){
    $("#plus_button_qa").remove();
    $("#dvSub_QA").append("<label class='control-label help-label'>Question</label><div class=''><div id='ContentPlaceHolder1_Div1'><div id='ContentPlaceHolder1_dvdropzonedd1' data-canvas='true' data-width='500' data-image='' data-url='imagehandlers/dealdetailimagehandler1.aspx' class='dropzone' style='min-width: 350px; min-height: 0px; box-sizing: border-box; height: 40px; width: 700px;' data-ghost='false' data-ajax='true' data-height='500'><input type='text' name='question_1' id='question_1' class='span9 required'></div></div></div><label class='control-label help-label'> Answer</label><div class='controls' style='margin-left:100px;'><div id='ContentPlaceHolder1_Div1'><div id='ContentPlaceHolder1_dvdropzonedd1' data-canvas='true' data-width='0' data-image='' data-url='imagehandlers/dealdetailimagehandler1.aspx' class='dropzone' style='min-width: 350px; min-height: 0px; box-sizing: border-box; height: 60px; width: 700px;' data-ghost='false' data-ajax='true' data-height='500'><textarea class='span9' rows=''></textarea></div></div></div>");
    
    $("#dvSub_QA").append(" <div align='right' id='plus_button_qa'><a class='btn btn-info add' onclick='add_one_qa_items()'><i class='icon-plus icon-white'></i></a></div>");
}