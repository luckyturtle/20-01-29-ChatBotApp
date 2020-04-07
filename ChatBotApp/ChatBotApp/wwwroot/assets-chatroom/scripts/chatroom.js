var is_first_customer_selected = 1;
$(document).ready(function () {

    $(".validate-label").css('display', 'none');
    //$(".app-footer__inner").addClass('actived-theme');
    displaySideBar();
    //$('#setting_fixed_footer').prop('checked', true).trigger('change');
    //$('#setting_fixed_footer').trigger('click');
    resizeAction();
    //$("html,body").animate({ scrollTop: 0 }, "slow");
    //$(".msg_history").animate({ scrollTop: 0 }, "slow");
    current_agent = $("#current_userid").val();
    $(".app-container").addClass("fixed-footer");
    $("#agent" + current_agent + " a").addClass("mm-active");
    if ($("#nimda").val() == 1) {
        is_first_customer_selected = 0;
        AgentThreading = current_agent;
        displayRoom(1, "");
    }
    if ($("#current_avatar").val() == "NULL") {
        $("#current_avatar_img").html(getIconByName($("#current_username").val(),''));
        if ($("#edit_avatarPreview").attr("src") == "NULL") {
            $("#edit_avatarPreview").attr("src","/assets-chatroom/images/avatars/default.png");
        }
    }
});

$(window).resize(function () {
    resizeAction();
});
function resizeAction() {
    $(".msg_history").css('height', eval($(".scrollbar-sidebar").css('height').replace('px', '')) - 110);
    $(".app-container").removeClass('closed-sidebar');
    $(".close-sidebar-btn").removeClass('is-active');
    $(".close-sidebar-btn").addClass('is-active');
}

function readURL(input, f) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#' + f).attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}
$("#edit_avatar").change(function () {
    readURL(this, "edit_avatarPreview");
});
$("#edit-avatar-change-btn").click(function () {
    $("#edit_avatar").trigger('click');
    return false;
});
$("#add_avatar").change(function () {
    readURL(this, "add_avatarPreview");
});
$("#add-avatar-change-btn").click(function () {
    $("#add_avatar").trigger('click');
    return false;
});



function attachedFileClickAction() {
    if (current_type) $('#attached_file').trigger('click');
}
$('#attached_file').on('change', function () {
    //alert($(this).val());
    //$(".show-toastr-example1").trigger('click');
    $("#toast-container .toast-message").html($(this).val());
    $("#toast-container").css('display', "block");
    $("#message-ping-obj").focus();
});
function closeAttachedFileAction() {
    $("#toast-container").css('display', "none");
}

function agentSelectAction(ai, f) {
    current_type = f;
    $(".app-sidebar__inner ul li").each(function (obj) {
        $(this).find("a").removeClass("mm-active");
    });
    current_agent = ai;
    $("#agent" + current_agent + " a").addClass("mm-active");
    if (!current_type) {
        $('.app-footer__inner').removeClass('actived-theme');
        $("#customer_profile_view").css("display", "none");

        var agents = JSON.parse($("#agentListJson").val());
        for (var i = 0; i < agents.length; i++) {
            if (agents[i].id == current_agent) {
                $("#current_avatar_img").html(agents[i].avatar == "NULL" ? getIconByName(agents[i].name) : "<img width=\"35\" class=\"rounded-circle\" src=\"" + agents[i].avatar + "\" alt=\"\">");
                //$("#current_avatar_img").attr("src", agents[i].avatar);
                $("#current_username_div").html(agents[i].name);
                $("#current_nickname_div").html(agents[i].nickName);
                if ($("#nimda").val()) {
                    $('#edit_avatar').prop('src', agents[i].avatar);
                    $("#edit_fullname").val(agents[i].fullName);
                    $("#edit_nickname").val(agents[i].nickName);
                    $("#edit_username").val(agents[i].name);
                    $("#edit_userid").val(agents[i].id);
                }
                break;
            }
        }
    }else {
        $('.app-footer__inner').addClass('actived-theme');

        var customers = JSON.parse($("#customerListJson").val());
        for (var i = 0; i < customers.length; i++) {
            if (customers[i].id == current_agent) {
                if (customers[i].name == "&nbsp;") customers[i].name = "";
                $("#edit_fullname_c").val(customers[i].fullName);
                $("#edit_nickname_c").val(customers[i].nickName);
                $("#edit_email_c").val(customers[i].email);
                $("#edit_phone_c").val(customers[i].phone);
                $("#edit_username_c").val(customers[i].name);
                break;
            }
        }
        $("#customer_profile_view").css("display","block");
    }
    

    
    /*
    if (current_type) {
        $("#current_customeravatar_img").attr("src", '/assets-chatroom/images/avatars/bot.png');
        $("#current_customername_div").html('');
        $("#current_customernickname_div").html('');
        current_customer = -1;
    } else if (current_customer=="-1") {
        var customer = JSON.parse($("#customerListJson").val());
        if(customer.length) {
            $("#current_customeravatar_img").attr("src", customer[0].avatar);
            $("#current_customername_div").html(customer[0].name);
            $("#current_customernickname_div").html(customer[0].nickName);
        }
    }
    */
    AgentThreading = current_agent;
    displayRoom(1, "");
}

function selectCustomerAction(id) {
    current_customer = id;
    var customer = JSON.parse($("#customerListJson").val());
    for (var i = 0; i < customer.length; i++) {
        if (customer[i].id == current_customer) {
            $("#current_customeravatar_img").attr("src", customer[i].avatar);
            $("#current_customername_div").html(customer[i].name);
            $("#current_customernickname_div").html(customer[i].nickName);
            break;
        }
    }
}

$("#message-ping-obj").on("keyup", function (e) {
    if (!current_type) return;
    if (e.keyCode == 13) {
        pingAction();
    }
});

function getChatLogMessage(id, f, agentmsg, attach, nowdate) {
    var h = "";
    if (f) {
        h += "<div id=\"" + id + "\" class=\"incoming_msg\">" +
            "<div class=\"received_msg\">" +
            "<div class=\"received_withd_msg\">" +
            "<p>" +
            "" + agentmsg + "" +
            "</p>" +
            (attach == "" || attach == null || attach == "undefined" || attach == "null" ? "" : (attach.indexOf("http://") > -1 || attach.indexOf("https://") > -1 ? "<embed src=\"" + attach + "\" style=\"max-width:100%;\"/>":"<embed src=\"attach\\" + attach + "\" style=\"max-width:100%;\"/>")) +
            "<span class=\"time_date\">" + nowdate + "</span></div>" +
            "</div>" +
            "</div>";
    } else {
        h += "<div id=\"" + id + "\" class=\"outgoing_msg\">" +
            "<div class=\"sent_msg\">" +
            "<p>" +
            "" + agentmsg + "" +
            "</p>" +
            (attach == "" || attach == null || attach == "undefined" || attach == "null" ? "" : (attach.indexOf("http://") > -1 || attach.indexOf("https://") > -1 ? "<embed src=\"" + attach + "\" style=\"max-width:100%;\"/>" :"<embed src=\"attach\\" + attach + "\" style=\"max-width:100%;\"/>")) +
            "<span class=\"time_date\">" + nowdate + "</span></div>" +
            "</div>";
    }
    return h;
}

function addPingAction(f, agentmsg, attach, d) {
    var dateObj = d;
    var month = dateObj.getUTCMonth() + 1; //months from 1-12
    var day = dateObj.getUTCDate();
    var year = dateObj.getUTCFullYear();
    var week = dateObj.getDay();//sun-0
    var time = dateObj.getHours() + ":" + dateObj.getMinutes();
    var newdate = month + "/" + day + "/" + year + "/" + week + "/" + time;
    var nowdate = dateObj.getHours() + ":" + dateObj.getMinutes() + " " + getAmOrPm(dateObj.getHours()) + " | " + month + "/" + day + "/" + year;
    var lastdate = dateObj.getHours() + ":" + dateObj.getMinutes() + " " + getAmOrPm(dateObj.getHours());//getLastTime(newdate);

    var h = $(".msg_history").html();
    var id = new Date().getTime();
    $(".msg_history").html(h + getChatLogMessage(id, f, agentmsg, attach, nowdate + (f ? "" : " | " + $('#current_username').val())));
    try {
        //var h = eval($("#" + id).css("height").split("px")[0]);
        //if (h > 0) ChatLogTotalHeight += h;
    } catch (error) { }
    //$(".msg_history").scrollTo(1000);// = $(".scrollbar-container").scrollHeight;
    //alert($(".msg_history").scrollTop);
    //$(".msg_history").animate({ scrollTop: $("#" + id).offset().top + 10 }, "slow");
    $(".msg_history").animate({ scrollTop: $(".msg_history").prop("scrollHeight") }, "slow");
    var len = 35;
    if (agentmsg.length > len) agentmsg = agentmsg.substring(0, len - 3) + "...";
    $("#agent" + current_agent + " a div div div .lastest_message").html(agentmsg);
    $("#agent" + current_agent + " a .lastest_datetime").html(lastdate);
}
/**
 * generate icon bt user name
 * */
function getIconByName(name, status) {
    var arr = name.split(" ");//ALT
    var color_arr = (new String("PRIMARY SECONDARY SUCCESS INFO WARNING DANGER FOCUS DARK")).toLowerCase().split(" ");
    var first_letter = arr[0].charAt(0).toUpperCase();
    var icon_name = first_letter + (arr.length > 1 ? arr[1].charAt(0) : "").toUpperCase();
    var icon_color = color_arr[(first_letter.charCodeAt(0) - 65) % color_arr.length];
    var html = "<div class=\"badge badge-pill badge-" + icon_color + " left-panel-generated-icon\">" + icon_name;
    if (status == "2") html += "<div role=\"presentation\" tabindex=\"-1\" title=\""+name+"\" aria-hidden=\"true\" style=\"/* position:absolute;*/display:flex;flex-direction:column;flex-grow:0;flex-shrink:0;overflow:hidden;/*align-items:center;*/bottom:-1px;right:-1px;margin-left: 15px; margin-top: 2px; border-radius: 6.5px; border-color: rgb(199, 237, 252); border-style: solid; border-width: 2px; width: 13px; height: 13px; justify-content: center; background-color: rgb(77, 217, 101); cursor: pointer;\"></div>";
    html+="</div>";
    return html;
}
