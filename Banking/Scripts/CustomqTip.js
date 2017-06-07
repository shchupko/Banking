


$(document).ready(function() 
{
        //настрйока валидации
    $("#login_form").validate({
		ignore: ".ignore, :hidden",
		focusInvalid: true,
		hideErrElem : "#validate_error",
		errorPlacement: function(error,element) {
			//назначаем обработчик на ошибки
			changeTip(element);
			return true;
		},
		success: function (label) {
		    label.addClass("valid").text("Ok!");
		    $(label).html("nice name");
		},
		rules: {
			'Login'		: {
				required : true,
				loginRule: true
			},
			'Password'		: {
				required : true,
				passRule: true,
				minlength: 6
			},
			'ConfirmPassword': {
			    required: true,
			    ConfirmPasswordRule: true
			},
			'Email': {
			    required: true,
			    EmailRule: true
			},
			'Address': {
			    required: true,
			    AddressRule: true
			},
			'Captcha': {
			    required: true,
			    CaptchaRule: true
			}
		}
	});
	$("#subm_id").click(function() {
		var valid = $("#register_form").valid();
		if(valid)
		{
			document.forms["form_reg"].submit()
	    }
	});

	jQuery.validator.addMethod('loginRule', function(value, element, param)
	{
		if((new RegExp("^[0-9a-zA-Z\_]{3,}$", "").test(value)))
		{
			return true;
		}

		return false;
	});

	jQuery.validator.addMethod("passRule", function(value, element)
	{
		if((/^[a-zA-Z\d\-\!\.\/\$\\\,\?\:\&\*\;\@@\%\(\)\+\=\№\#\_\[\]]{6,}$/.test(value)))
		{
			return true;
		}

		return false;
	});

	//для того чтобы выводить ошибки при валидации обернем все подсказки в объект, чтобы было удобнее ими управлять
	fields_tip = {
		//основные параметры для подсказок
		tip_w : 300,
		tip_r : 3,
		tip_color : 'light',
		tip_show : 'mouseover focus',
		tip_show_ready: false,
        tip_show_solo: true,
		tip_hide : 'mouseout click blur',
		tip_border_w : 0,
		tip_screen : false,
		tip_hide_delay : 0,

		//настройка внешнего вида подсказок qtip

		//поле логин
		Login : function() {
			$('*[name="Login"]').qtip({
				content: {
				    text: 'Allowed letters, numbers and symbol "_"',
					title: {
						text: 'Login'
					}
				}

			});
		},
		login_destruct : function() {
			$('*[name="Login"]').qtip('destroy');
		},
		//поле пароль
		Password : function() {
			$('*[name="Password"]').qtip({
				content: {
				    text: 'Allowed letters, numbers and symbols "-!./\$,?:&*;@@%()+=№#_[]"',
					title: {
					    text: 'Password'
					}
				},
				
			});
		},
		users_password_destruct : function() {
			$('*[name="Password"]').qtip('destroy');
		},
		//инициализация подсказок
		init : function() {
			this.Login();
			$('*[name="Login"]').mouseover();
			this.Password();
			$('*[name="Password"]').mouseover();
		},

		destructor : function() {
			this.login_destruct();
			this.users_password_destruct();
		}

	};

	fields_tip.init();
});

function changeTip(element)
{
	//удаляем текущую подсказку
	$(element).qtip('destroy');
	
	if(!($(element).hasClass('error')))
	{
		//если класса ошибки у элемента нет, то делаем ему нормальную подсказку
		//создаем обычную
		fields_tip.tip_show_ready = false;
		fields_tip.tip_color = 'light';
		
		fields_tip[$(element)[0].name].call(fields_tip);
		$('*[name="' + $(element)[0].name + '"]').mouseover();
	}
	else
	{
		fields_tip.tip_color = 'red';
		fields_tip.tip_show_ready = true;

		//создаем ошибку
		fields_tip[$(element)[0].name].call(fields_tip);
		$('*[name="' + $(element)[0].name + '"]').mouseover();
        //скрытие подсказки, можно сделать любой эффект
		setTimeout('$(\'*[name="' + $(element)[0].name + '"]\').qtip("hide");', 3000);
	}
}
