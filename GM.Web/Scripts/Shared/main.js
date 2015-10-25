
$(window).load(function () {
    jQuery(document).ready(function ($) {

        $('[data-popup-target]').click(function (event) {
            //Close all open popups first
            $('.popup').removeClass('visible');
            $('html').addClass('overlay');
            var activePopup = $(this).attr('data-popup-target');
            $(activePopup).addClass('visible');
            event.preventDefault();

        });


        $(document).keyup(function (e) {
            if (e.keyCode == 27 && $('html').hasClass('overlay')) {
                clearPopup();
            }
        });

        $('.js-page-change-submit').keypress(function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                var url = $(this).data("url");
                if (url.indexOf("?") > -1) {
                    location.href = url + "&page=" + $(this).val();
                }
                else {
                    location.href = url + "?page=" + $(this).val();
                }
            }
        });

        $('[data-toggle="tooltip"]').tooltip();

        $('.popup-exit').click(function () {
            clearPopup();
        });

        $('.popup-overlay').click(function () {
            clearPopup();
        });

        function clearPopup() {
            $('.popup.visible').removeClass('visible');
            $('html').removeClass('overlay');

            setTimeout(function () {
                $('.popup').removeClass('transitioning');
            }, 200);
        }

    });
});//]]>  


//Disabling scroll wheel increment number
$(':input[type=number]').on('mousewheel', function (e) {
    $(this).blur();
});


/// Single reward slider 
// Generate the navigation dots

var createDots = function () {
    var nav = $('#slider-nav ul');
    var slides = $('.slide-reward').length;
    for (var i = 0; i < slides; i++) {
        nav.append('<li class="dot">&bull;</li>');
    }
    $('.dot').first().addClass("active-dot");
};

// Next slide transition

var next = function () {
    var activeSlide = $('.active-slide');
    var nextSlide = activeSlide.next('.slide-reward');
    if (nextSlide.length == 0) {
        nextSlide = $('.slide-reward').first();
    } activeSlide.fadeOut('slow').removeClass("active-slide");
    nextSlide.fadeIn(500).addClass("active-slide");

    var activeDot = $('.active-dot');
    var nextDot = activeDot.next('.dot');
    if (nextDot.length == 0) {
        nextDot = $('.dot').first();
    }
    activeDot.removeClass("active-dot");
    nextDot.addClass("active-dot");
};
var prev = function () {
    var activeSlide = $('.active-slide');
    var prevSlide = activeSlide.prev('.slide-reward');
    if (prevSlide.length == 0) {
        prevSlide = $('.slide-reward').last();
    }
    activeSlide.fadeOut('slow').removeClass("active-slide");
    prevSlide.fadeIn(500).addClass("active-slide");

    var activeDot = $('.active-dot');
    var prevDot = activeDot.prev('.dot');
    if (prevDot.length == 0) {
        prevDot = $('.dot').last();
    }
    activeDot.removeClass("active-dot");
    prevDot.addClass("active-dot");
};
var clickDot = function (dot) {
    var activeSlide = $('.active-slide');
    var i = $(dot).index() + 1;
    var nextSlide = $('.slide-reward:nth-child(' + i + ')');
    activeSlide.fadeOut('slow').removeClass("active-slide");
    nextSlide.fadeIn(500).addClass("active-slide");
    $('.active-dot').removeClass("active-dot");
    $(dot).addClass("active-dot");
};
var createSlide = function (url) {
    if ($('.slide-reward').length < 10) {
        // Create the slide
        var slide = $('<div class="slide-reward"></div>');
        var img = $('<img src="' + url + '" class="slide-img" />');
        slide.append(img);
        $('#slides').append(slide);
        // Create the dot
        var nav = $('#slider-nav ul');
        var dot = '<li class="dot">&bull;</li>';
        nav.append(dot);
        $('.dot').last().click(function () {
            clickDot(this);
        });
    }
};

var main = function () {
    var mili = 2000;
    var dialog = false;
    createDots();

    // Move automaticaly through the slides
    slideLoop = setInterval(next, mili);
    $('#slider').mouseenter(function () {
        $('.arrow').fadeIn('slow');
        if (!dialog) {
            clearInterval(slideLoop);
        }
    });
    $('#slider').mouseleave(function () {
        $('.arrow').fadeOut('slow');
        if (!dialog) {
            slideLoop = setInterval(next, mili);
        }
    });
    $('.right').click(next);
    $('.left').click(prev);
    // Clicking a dot move to the corresponding slide
    $('.dot').click(function () {
        clickDot(this);
    });


};

$(document).ready(main);

///Google map
function initialize() {
    var mapProp = {
        center: new google.maps.LatLng(51.508742, -0.120850),
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
}
//google.maps.event.addDomListener(window, 'load', initialize);

//// Sort options
$("#settings, #admin_text").on("click", function () {
    $("#menu").fadeToggle("fast");
});
$("#menu a").on("click", function () {
    $("#menu").hide();
});


// Update profile (for simple user)
$(".account-updates .toggle label, .push-notification .toggle label, .push-to-facebook .toggle label").click(function () {
    var target = $(this).closest(".toggle");
    var valueCurrent = $(target).find("input:hidden").val();

    if (valueCurrent.toLowerCase() === "true") {
        $(target).find("input:hidden").val("false");
        $(target).find("input[type=checkbox]").val("false");
    } else {
        $(target).find("input:hidden").val("true");
        $(target).find("input[type=checkbox]").val("false");
    }
});

// Update reward (for supplier)
$(".p-biss-toggle .toggle label, .p-biss-toggle-display-p .toggle label, .p-biss-mobile-redeem .toggle label").click(function () {
    var target = $(this).closest(".toggle");
    var valueCurrent = $(target).find("input:hidden").val();

    if (valueCurrent.toLowerCase() === "true") {
        $(target).find("input:hidden").val("false");
        $(target).find("input[type=checkbox]").val("false");
    } else {
        $(target).find("input:hidden").val("true");
        $(target).find("input[type=checkbox]").val("false");
    }
});



// Rewards
$(".redeem-all-rewards .reward-item .js-add-to-cart").click(function () {
    var trigger = $(this);
    var id = trigger.closest(".reward-item").data("id");
    $.ajax({
        url: "/Account/CartToAdd",
        data: { 'id': id, 'quantity': 1 },
        type: "POST",
        success: function (data) {
            if (data === "True") {
                trigger.text("Added");
                var rcount = parseInt($(".js-added-reward").text());
                $(".js-added-reward").removeClass("hide").text(rcount + 1);
                $(trigger).removeClass("js-add-to-cart");
            }
        },
        error: function (e) {
            alert("Error occured: " + e);
        }
    });
    return false;
});

// Reward details
$(".row.single-reward .js-add-to-cart").click(function () {
    var trigger = $(this);
    var id = trigger.data("id");
    $.ajax({
        url: "/Account/CartToAdd",
        data: { 'id': id, 'quantity': 1 },
        type: "POST",
        success: function (data) {
            if (data === "True") {
                trigger.text("Added");
                var rcount = parseInt($(".js-added-reward").text());
                $(".js-added-reward").removeClass("hide").text(rcount + 1);
                $(trigger).removeClass("js-add-to-cart");
            }
        },
        error: function (e) {
            alert("Error occured: " + e);
        }
    });
    return false;
});

// Cart
$('.js-quantity-update input').change(function () {
    $('#myCartForm').submit();
});

$('.js-reward-remove').click(function () {
    $(this).closest('tr').find('.js-quantity-update input').val('-1');
    $('#myCartForm').submit();
});

// Home page - registration popups

var postCodeNotMatching = $("#PostCodeIsNotMatching").val() == "True";
var enterFbRgstrPostCode = $("#EnterFbRgstrPostCode").val() == "True";
var supplierRgstrNotValid = $("#SupplierRegistrationIsNotValid").val() == "True";
var rgstrNotValid = $("#RegistrationIsNotValid").val() == "True";

//Show modal dialog if post code missing
if (enterFbRgstrPostCode) {
    $("#mdlFbRgstrEnterCode").addClass("visible");
}
//Show modal dialog if user registration not valid
if (rgstrNotValid) {
    $("#signin").addClass("visible");
}
//Show modal dialog if supplier registration not valid
if (supplierRgstrNotValid) {
    $("#suplier-signin-popup").addClass("visible");
}

//----------------------------SIGN IN POPUP

$('#showMailRgtrForm').click(function () {
    $(this).parent().remove();
    $("#mailRgtrForm").removeClass("hide");
    $(".modal .privacy").addClass("hide");
    $(".modal form .privacy").removeClass("hide");
    $(".modal.fade.in").css("top", "10px");
});


$("#mailRgtrForm").on("submit", function () {
    if (!validateForm()) {
        return false;
    }
    return true;
});

//----------------------------SIGN IN POPUP AUSPOST

$(".js-auspost form").on("submit", function (e) {
    if ($(".js-email").val().indexOf("@auspost.com.au") > 0) {
        $(".js-email").removeClass("error");
        return true;
    }
    $(".js-email").addClass("error");
    return false
});

//----------------------------

function validateForm() {
    var isValid = true;

    $("#signin.popup .form-group").each(function () {
        if ($(this).find("input").val().trim().length == 0) {
            $(this).addClass("error");
            isValid = false;
        } else {
            $(this).removeClass("error");
        }
    });
    return isValid;
}

//------------------------SUPPLIER SIGN IN POPUP

$("#supplierRgtrForm").on("submit", function () {
    if (!validateForm()) {
        return false;
    }
    return true;
});

function validateForm() {
    var isValid = true;

    $("#suplier-signin-popup.popup .form-group").each(function () {
        if ($(this).find("input").val().trim().length == 0) {
            $(this).addClass("error");
            isValid = false;
        } else {
            $(this).removeClass("error");
        }
    });
    return isValid;
}

//------------------------CODE NOT FOUND POPUP

//Show modal dialog if post code missing
if (postCodeNotMatching) {
    $("#mdlCantJoinSite").addClass("visible");
    $("#mdlCantJoinSite .btn").click(function () {
        window.location.href = window.location.href;
    });
}


//------------------------ ALMOST DONE FB REGISTER (ENTER POST CODE)

$("#fbRegSubmit").click(function () {

    var postcode = $("#mdlFbRgstrEnterCode").find("#fbRegPostcode").val();
    if (!checkPostCode(postcode)) {
        $("#mdlFbRgstrEnterCode").find("#fbRegPostcode").closest(".form-group").addClass("error");
        return false;
    }

    $.ajax({
        url: "/Home/FacebookLoginVerifyPostCode", //TODO remove this to web config!!! create web config transformation for stage site 
        data: { 'code': postcode },
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.success == false) {
                $("#mdlFbRgstrEnterCode").removeClass("visible");
                $("#mdlCantJoinSite").addClass("visible");
                $("#mdlCantJoinSite .btn").click(function () {
                    window.location.href = window.location.origin;
                });
            } else {
                //success
                window.location.href = data.result;
            }
        },
        error: function (e) {
            alert("Error occured: " + e);
        }
    });
    return false;
});

function checkPostCode(code) {
    var pattern = /^\d{4}$/;
    if (pattern.test(code)) {
        return true;
    } else {
        return false;
    }
}

//------------------------ change address popup
//
$("#trigger-change-address-section").click(function () {
    if ($("#change-address-section").hasClass("hide")) {
        $("#change-address-section").removeClass("hide");
    } else
        $("#change-address-section").addClass("hide");
    return false;
});

//--------------------------

if ($("#problemWithAddress").length > 0) {
    $("#mdlProblemWithAddress").addClass("visible");
}

//--------------------------Add members (registration)

$("#addNewMember").click(function () {

    var visibleInputsAreValid = true;
    $("form .member").not(".hide").each(function () {

        var target = $(this).find("input");
        var targetErrorElement = target.parent();
        if ($.trim(target.val()) == "") {
            targetErrorElement.addClass("error");
            visibleInputsAreValid = false;
        }

    });

    // If all fields entered show next member to enter
    if (visibleInputsAreValid) {
        $(".member.hide:first").removeClass("hide");
    }
});

// Add additional members
$("#addUserRgtrForm input[type='submit']").click(function () {
    var valid = true;
    $("#addUserRgtrForm").find("input").each(function () {
        if ($.trim($(this).val()) == "") {
            valid = false;
        }
    });

    if (valid) {

        $.ajax({
            url: "/Account/AdditionalUsersAjax",
            data: {
                "firstname": $("#HouseholdMemberModel_FirstName").val(),
                "lastname": $("#HouseholdMemberModel_LastName").val(),
                "email": $("#HouseholdMemberModel_Email").val()
            },
            type: "POST",
            success: function (data) {
                if (data == "True") {
                    $("#add-user-popup").removeClass("visible");
                    $("#invitation-sent-popup").addClass("visible");
                } else {
                    return false;
                }
                return false;
            },
            error: function (e) {
                alert("Error occured: " + e);
            }
        });

        return false;
    }
    return true;
});

$("form .skip").click(function () {
    window.location.href = $(this).data("url");
});


//-----------------Update profile uplad image

$('input[id=photoUpload]').change(function (event) {
    $("#profileImg").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
});

//Barcode image upload
$('input[id=photoBarCodeUpload]').change(function (event) {
    $("#barcodeImg").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $("#barcodeImg").after('<span class="delete"></span>');

    $(".p-biss-barcode .delete").on("click", function (e) {
        e.preventDefault();
        var $trigger = $(this).closest("div");
        $trigger.find("input[type=hidden]").val(null);
        $trigger.find("img").attr('src', '/Images/reward-page-barcode.jpg');
        $trigger.find(".delete").remove();
    });
});
$(".p-biss-barcode .delete").on("click", function (e) {
    e.preventDefault();
    var $trigger = $(this).closest("div");
    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/reward-page-barcode.jpg');
    $trigger.find(".delete").remove();
});

//Update profile images
$('.p-biss-profile-image input[type=file]').on("change", function (event) {
    var $trigger = $(this).closest("div");
    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $trigger.find("img").after('<span class="delete"></span>');
    $trigger.find(".uploadphotos").remove();

    $(".profile-image-holder .delete").on("click", function (e) {
        e.preventDefault();
        $trigger.find("input[type=hidden]").val(null);
        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
        $trigger.find("img").after('<div class="img-overlay"></div>');
        $trigger.find(".delete").remove();
        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
    });
});
$(".profile-image-holder .delete").on("click", function (e) {
    e.preventDefault();
    var $trigger = $(this).closest("div");
    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
    $trigger.find("img").after('<div class="img-overlay"></div>');
    $trigger.find(".delete").remove();
    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
});

//Update logo image
$('.p-biss-logo input[type=file]').change(function (event) {
    $(this).closest(".p-biss-logo").find("img.partner-logo-size").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $(this).closest(".p-biss-logo").find("img").after('<span class="delete"></span>');

    $(".p-biss-logo .delete").on("click", function (e) {
        e.preventDefault();
        var $trigger = $(this).closest("div");
        $trigger.find("input[type=hidden]").val(null);
        $trigger.find("img").attr('src', '/Images/partner-profile-logo.jpg');
        $trigger.find(".delete").remove();
        //$trigger.find("img").after('<span class="photoicon"></span>');
    });
});
$(".p-biss-logo .delete").on("click", function (e) {
    e.preventDefault();
    var $trigger = $(this).closest("div");
    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-logo.jpg');
    $trigger.find(".delete").remove();
    //$trigger.find("img").after('<span class="photoicon"></span>');
});

//challenge
$('.admin-challenge-logo input[type=file]').change(function (event) {
    $(this).closest(".admin-challenge-logo").find("img.admin-challenge-logo-size").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $(this).closest(".admin-challenge-logo").find("img").after('<span class="delete"></span>');
    $(".admin-challenge-logo .delete").on("click", function (e) {
        e.preventDefault();
        var $trigger = $(this).closest("div");
        $trigger.find("input[type=hidden]").val(null);
        $trigger.find("img").attr('src', '/Images/admin-create-challenge-logo.jpg');
        $trigger.find(".delete").remove();
        //$trigger.find("img").after('<span class="photoicon"></span>');
    });
});

//Challenges
$(".js-scroll-to-instructions").click(function () {
    $('html,body').animate({
        scrollTop: $(".js-instructions").offset().top
    },
        'slow');
});

$(".js-scroll-to-instructions-learn").click(function () {
    $('html,body').animate({
        scrollTop: $(".js-about-learn").offset().top
    },
        'slow');
});


$('a.backtochallenge').click(function () {
    $('.popup.visible').removeClass('visible');
    $('html').removeClass('overlay');
    event.preventDefault();

});

$(".js-claim-points").click(function () {
    var trigger = $(this);
    var id = trigger.closest(".js-main-data").data("id");
    $.ajax({
        url: "/Challenges/ClaimPoints",
        data: { 'id': id },
        type: "POST"
    });
    return false;
});

$(".js-claim-points-pledge").click(function () {
    var trigger = $(this);
    var id = trigger.closest(".js-main-data").data("id");
    var url = trigger.data("url");
    $.ajax({
        url: "/Challenges/ClaimPointsForPledge",
        data: { 'id': id },
        type: "POST",
        success: function (data) {
            if (data === "True") {
                //trigger.remove();
                //$(".js-claim-points-disabled").removeClass("hidden");
                //$(".nextsteps-popup").removeClass("visible");
                window.location = url;
            }
        },
        error: function (e) {
            alert("Error occured: " + e);
        }
    });
    return false;
});

$(".js-claim-points-learn").click(function () {
    var trigger = $(this);
    var id = trigger.closest(".js-main-data").data("id");
    $.ajax({
        url: "/Challenges/ClaimPointsForLearn",
        data: { 'id': id },
        type: "POST",
        success: function (data) {
            if (data === "True") {
                trigger.remove();
                $(".js-claim-points-disabled").removeClass("hidden");
                $('.popup').removeClass('visible');
                $('html').addClass('overlay');
                var activePopup = $(this).attr('data-popup-target');
                $(activePopup).addClass('visible');
                $("#earn-points-congratulations-popup").addClass("visible");
            }
        },
        error: function (e) {
            alert("Error occured: " + e);
        }
    });
    return false;
});

$(".js-add-points-action").click(function () {
    var $trigger = $(this);
    var $main = $(".js-main-data");
    var id = $main.data("id");
    var $input = $trigger.closest("#entercode-popup").find("#promoCodeValue");
    var value = $input.val();
    $('.popup').removeClass('visible');
    $.ajax({
        url: "/Challenges/AddPoints",
        data: { 'id': id, 'promocode': value },
        type: "POST",
        success: function (data) {
            if (data === "True") {
                $("#good-promo-code").addClass("visible");
            }
            else {
                $("#bad-promo-code").addClass("visible");
            }
        },
        error: function (e) {
            $("#bad-promo-code").addClass("visible");

        }
    });
    return false;
});

$(".admin-challenge-logo .delete").on("click", function (e) {




    e.preventDefault();
    var $trigger = $(this).closest("div");
    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/admin-create-challenge-logo.jpg');
    $trigger.find(".delete").remove();
    //$trigger.find("img").after('<span class="photoicon"></span>');
});
//challenge profile
//$('.admin-challenge-profile-image input[type=file]').on("change", function (event) {
//    var $trigger = $(this).closest("div");
//    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
//    $trigger.find("img").after('<span class="delete"></span>');
//    $trigger.find(".uploadphotos").remove();

//    $(".admin-challenge-image-holder .delete").on("click", function (e) {
//        e.preventDefault();
//        $trigger.find("input[type=hidden]").val(null);


//        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
//        $trigger.find("img").after('<div class="img-overlay"></div>');
//        $trigger.find(".delete").remove();
//        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
//    });

//});

//$(".admin-challenge-image-holder .delete").on("click", function (e) {
//    e.preventDefault();
//    var $trigger = $(this).closest("div");

//    $trigger.find("input[type=hidden]").val(null);
//    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
//    $trigger.find("img").after('<div class="img-overlay"></div>');
//    $trigger.find(".delete").remove();
//    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
//});
$('#divPhoto1 input[type=file]').on("change", function (event) {
    var $trigger = $('#divPhoto1');
    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $trigger.find("img").after('<span class="delete1"></span>');
    $trigger.find(".uploadphotos").remove();

    $("#divPhoto1 .delete1").on("click", function (e) {
        e.preventDefault();
        $trigger.find("input[type=hidden]").val(null);


        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
        $trigger.find("img").after('<div class="img-overlay"></div>');
        $trigger.find(".delete1").remove();
        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
        $("#Photo1").val("");
    });

});

$('#divPhoto2 input[type=file]').on("change", function (event) {
    var $trigger = $('#divPhoto2');
    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $trigger.find("img").after('<span class="delete2"></span>');
    $trigger.find(".uploadphotos").remove();

    $("#divPhoto2 .delete2").on("click", function (e) {
        e.preventDefault();
        $trigger.find("input[type=hidden]").val(null);


        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
        $trigger.find("img").after('<div class="img-overlay"></div>');
        $trigger.find(".delete2").remove();
        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
        $("#Photo2").val("");
    });

});

$('#divPhoto3 input[type=file]').on("change", function (event) {
    var $trigger = $('#divPhoto3');
    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $trigger.find("img").after('<span class="delete3"></span>');
    $trigger.find(".uploadphotos").remove();

    $("#divPhoto3 .delete3").on("click", function (e) {
        e.preventDefault();
        $trigger.find("input[type=hidden]").val(null);


        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
        $trigger.find("img").after('<div class="img-overlay"></div>');
        $trigger.find(".delete3").remove();
        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
        $("#Photo3").val("");
    });

});

$('#divPhoto4 input[type=file]').on("change", function (event) {
    var $trigger = $('#divPhoto4');
    $trigger.find("img").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
    $trigger.find("img").after('<span class="delete4"></span>');
    $trigger.find(".uploadphotos").remove();

    $("#divPhoto4 .delete4").on("click", function (e) {
        e.preventDefault();
        $trigger.find("input[type=hidden]").val(null);


        $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
        $trigger.find("img").after('<div class="img-overlay"></div>');
        $trigger.find(".delete4").remove();
        $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
        $("#Photo4").val("");
    });

});

$("#divPhoto1 .delete1").on("click", function (e) {
    e.preventDefault();
    var $trigger = $('#divPhoto1');

    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
    $trigger.find("img").after('<div class="img-overlay"></div>');
    $trigger.find(".delete1").remove();
    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
    $("#Photo1").val("");
});

$("#divPhoto2 .delete2").on("click", function (e) {
    e.preventDefault();
    var $trigger = $('#divPhoto2');

    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
    $trigger.find("img").after('<div class="img-overlay"></div>');
    $trigger.find(".delete2").remove();
    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
    $("#Photo2").val("");
});
$("#divPhoto3 .delete3").on("click", function (e) {
    e.preventDefault();
    var $trigger = $('#divPhoto3');

    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
    $trigger.find("img").after('<div class="img-overlay"></div>');
    $trigger.find(".delete3").remove();
    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
    $("#Photo3").val("");
});
$("#divPhoto4 .delete4").on("click", function (e) {
    e.preventDefault();
    var $trigger = $('#divPhoto4');

    $trigger.find("input[type=hidden]").val(null);
    $trigger.find("img").attr('src', '/Images/partner-profile-image-static.jpg');
    $trigger.find("img").after('<div class="img-overlay"></div>');
    $trigger.find(".delete4").remove();
    $trigger.find(".custom-file-upload").after('<label class="uploadphotos" for="Photo">Upload image</label>');
    $("#Photo3").val("");
});



//$("#settings, #admin_text").on("click", function () {
//    $("#menu").fadeToggle("fast");
//});
$("#menu a").on("click", function () {
    $("#menu").hide();
});
//// challenge sort

$("#sortsettings, #sortadmin_text").on("click", function () {
    $("#sortmenu").fadeToggle("fast");
});
$("#sortmenu a").on("click", function () {
    $("#sortmenu").hide();
});
//// challenge status

$("#statussettings, #statusadmin_text").on("click", function () {
    $("#statusmenu").fadeToggle("fast");
});
$("#statusmenu a").on("click", function () {
    $("#statusmenu").hide();
});
//// challenge category

$("#categorysettings, #categoryadmin_text").on("click", function () {
    $("#categorymenu").fadeToggle("fast");
});
$("#categorymenu a").on("click", function () {
    $("#categorymenu").hide();
});
//// download csv

$("#csvsettings, #csvadmin_text").on("click", function () {
    $("#csvmenu").fadeToggle("fast");
});
$("#csvmenu a").on("click", function () {
    $("#csvmenu").hide();
});
//// download pdf

$("#pdfsettings, #pdfadmin_text").on("click", function () {
    $("#pdfmenu").fadeToggle("slow");
});
$("#pdfmenu a").on("click", function () {
    $("#pdfmenu").hide();
});






