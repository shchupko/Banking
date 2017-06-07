
$(document).ready(function () {
    $("#register_form").validate({
        rules: {
            Login: {
                required: true,
                minlength: 2,
                maxlength: 12
            },
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                rangelength: [6, 24]
            },
            ConfirmPassword: {
                required: true,
                equalTo: "#Password"
            }
        },
        content: {   //??? not use
            Password: {
                text: 'Allowed letters, numbers and symbols "-!./\$,?:&*;%()+=№#_[]"',
                title: {
                    text: 'Password'
                }
            }
        },
        messages: {
            Login: {
                required: "Укажите свое имя!",
                minlength: "Не менее 2 символов",
                maxlength: "Не более 12 символов"
            },
            Email: {
                required: "Нужно указать email адрес",
                email: "e-mail введен неверно"
            },
            Password: {
                text: 'Allowed letters, numbers and symbols "-!./\$,?:&*;%()+=№#_[]"',
                required: "Надо ввести пароль",
                rangelength: "От 6 до 24 символов"

            },
            ConfirmPassword: {
                required: "Подтвердите пароль",
                equalTo: "Подтверждение не принято"
            }
        },
        submitHandler: function (form) {
            $(form).ajaxSubmit({
                target: "#output",
                timeout: 3000
            });
        },
        success: function (label) {

            // $(label).addClass("valid").text("Ok!");
            //$(label).html("nice name");
            //element.parent().find("label[for='" + element.attr("name") + "']")
        },
        //errorPlacement: function(error, element) {
        //    error.appendTo(
        //    element.parent()
        //    .find("label[for='" + element.attr("name") + "']")
        //    .find("em")
        //    );
        //    //changeTip(element);//назначаем обработчик на ошибки
        //    //            return true;
        //}
        errorPlacement: function (error, element) {
            console.log('called', this);
            var cornersAt = ['left center', 'top left'], // Set positioning based on the elements position in the form
                cornersMy = ['right bottom', 'bottom right'],
                flipIt = $(element).parents('div.right').length > 0,
                position = {
                    at: cornersAt[flipIt ? 0 : 1],
                    my: cornersMy[flipIt ? 0 : 1]
                },
                offset = flipIt ? 6 : 35;

            $(element).qtip('destroy');

            if (!($(element).hasClass('error'))) {
                //    //$(element).addClass("valid");
                //    $(element).qtip('content.text');
                var r = cont[$(element)[0].name];
            }
            else {
                $(element).filter(':not(.valid)').qtip({ // Apply the tooltip only if it isn't valid      elem.filter(':not(.valid)').qtip({
                    overwrite: false,
                    content: error,
                    position: position,
                    show: {
                        event: false,
                        ready: true
                    },
                    hide: false,
                    style: { // Make it red... the classic error colour!
                        classes: 'ui-tooltip-error ui-tooltip-rounded',
                        tip: {
                            corner: true,
                            offset: offset
                        }
                    }
                }).qtip('option', 'content.text', error);
            }
        } // closes errorPlacement
    });


    $.validator.addMethod("equal", function (value, element, params) {
        return value == params;
    });

});

//$(function () {

//    var elems = $('body *');

//    // Найти индекс с использованием базового DOM API
//    var index = elems.index(document.getElementById("oblock"));
//    console.log("Индекс, найденный с использованием DOM-элемента: " + index);

//    // Найти индекс с использованием другого объекта jQuery
//    index = elems.index($('#oblock'));
//    console.log("Индекс, найденный с использованием объекта jQuery: " + index);


//    $('.edit').hide();
//    $('.edit-case').on('click', function () {
//        var tr = $(this).parents('tr:first');
//        tr.find('.edit, .read').toggle();
//    });
//    $('.update-case').on('click', function (e) {
//        e.preventDefault();
//        var tr = $(this).parents('tr:first');
//        id = $(this).prop('id');
//        var gender = tr.find('#Gender').val();
//        var phone = tr.find('#PhoneNo').val();
//        $.ajax({
//            type: "POST",
//            contentType: "application/json; charset=utf-8",
//            url: "http://localhost:55627/Default2/Edit",
//            data: JSON.stringify({ "id": id, "gender": gender, "phone": phone }),
//            dataType: "json",
//            success: function (data) {
//                tr.find('.edit, .read').toggle();
//                $('.edit').hide();
//                tr.find('#gender').text(data.person.Gender);
//                tr.find('#phone').text(data.person.Phone);
//            },
//            error: function (err) {
//                alert("error");
//            }
//        });
//    });
//    $('.cancel-case').on('click', function (e) {
//        e.preventDefault();
//        var tr = $(this).parents('tr:first');
//        var id = $(this).prop('id');
//        tr.find('.edit, .read').toggle();
//        $('.edit').hide();
//    });
//    $('.delete-case').on('click', function (e) {
//        e.preventDefault();
//        var tr = $(this).parents('tr:first');
//        id = $(this).prop('id');
//        $.ajax({
//            type: 'POST',
//            contentType: "application/json; charset=utf-8",
//            url: "http://localhost:55627/Default2/Delete/" + id,
//            dataType: "json",
//            success: function (data) {
//                alert('Delete success');
//                window.location.href = "http://localhost:55627/Default2/Index";
//            },
//            error: function () {
//                alert('Error occured during delete.');
//            }
//        });
//    });
//});

// second example
    //$(document).ready(function () {
    //    $("#register_form").validate({
    //        errorClass: "errormessage",
    //        onkeyup: false,
    //        errorClass: 'error',
    //        validClass: 'valid',
    //        rules: {
    //            Login: { required: true, minlength: 3 },
    //            Password: { required: true, minlength: 3 }
    //        },
    //        errorPlacement: function (error, element) {
    //            // Set positioning based on the elements position in the form
    //            var elem = $(element),
    //                corners = ['left center', 'right center'],
    //                flipIt = elem.parents('span.right').length > 0;

    //            // Check we have a valid error message
    //            if (!error.is(':empty')) {
    //                // Apply the tooltip only if it isn't valid
    //                elem.filter(':not(.valid)').qtip({
    //                    overwrite: false,
    //                    content: error,
    //                    position: {
    //                        my: corners[flipIt ? 0 : 1],
    //                        at: corners[flipIt ? 1 : 0],
    //                        viewport: $(window)
    //                    },
    //                    show: {
    //                        event: false,
    //                        ready: true
    //                    },
    //                    hide: false,
    //                    style: {
    //                        classes: 'ui-tooltip-red' // Make it red... the classic error colour!
    //                    }
    //                })

    //                // If we have a tooltip on this element already, just update its content
    //                .qtip('option', 'content.text', error);
    //            }

    //                // If the error is empty, remove the qTip
    //            else { elem.qtip('destroy'); }
    //        },
    //        success: $.noop, // Odd workaround for errorPlacement not firing!,
    //        submitHandler: function () {
    //            console.log("success");
    //            return false;
    //        }
    //    })
    //});

// third example
//    $(function () {

//        // Don't allow browser caching of forms
//        $.ajaxSetup({ cache: false });

//        // Wire up the click event of any dialog links
//        $("#register_form").on('click', function () {
//            var element = $(this);

//            // Retrieve values from the HTML5 data attributes of the link        
//            var dialogTitle = element.attr('data-dialog-title');
//            var updateTargetId = '#' + element.attr('data-update-target-id');
//            var updateUrl = element.attr('data-update-url');

//            // Generate a unique id for the dialog div
//            var dialogId = 'uniqueName-' + Math.floor(Math.random() * 1000)
//            var dialogDiv = "<div id='" + dialogId + "'></div>";

//            // Load the form into the dialog div
//            $(dialogDiv).load(this.href, function () {
//                $(this).dialog({
//                    modal: true,
//                    resizable: false,
//                    title: dialogTitle,
//                    buttons: {
//                        "submit": function () {
//                            // Manually submit the form                        
//                            var form = $('form', this);
//                            $(form).submit();
//                        },
//                        "Cancel": function () {
//                            $(this).dialog('close');
//                        }
//                    },
//                    close: function () {
//                        // Remove all qTip tooltips
//                        $('.qtip').remove();

//                        // It turns out that closing a jQuery UI dialog
//                        // does not actually remove the element from the
//                        // page but just hides it. For the server side 
//                        // validation tooltips to show up you need to
//                        // remove the original form the page
//                        $('#' + dialogId).remove();
//                    }
//                });

//                // Enable client side validation
//                $.validator.unobtrusive.parse(this);

//                // Setup the ajax submit logic
//                wireUpForm(this, updateTargetId, updateUrl);
//            });
//            return false;
//        });
//    });

//function wireUpForm(dialog, updateTargetId, updateUrl) {
//    $('form', dialog).submit(function () {

//        // Do not submit if the form
//        // does not pass client side validation
//        if (!$(this).valid())
//            return false;

//        // Client side validation passed, submit the form
//        // using the jQuery.ajax form
//        $.ajax({
//            url: this.action,
//            type: this.method,
//            data: $(this).serialize(),
//            success: function (result) {
//                // Check whether the post was successful
//                if (result.success) {
//                    // Close the dialog 
//                    $(dialog).dialog('close');

//                    // Reload the updated data in the target div
//                    $(updateTargetId).load(updateUrl);
//                } else {
//                    // Reload the dialog to show model errors                    
//                    $(dialog).html(result);

//                    // Enable client side validation
//                    $.validator.unobtrusive.parse(dialog);

//                    // Setup the ajax submit logic
//                    wireUpForm(dialog, updateTargetId, updateUrl);
//                }
//            }
//        });
//        return false;
//    });
//}

