$(function () {

    var elems = $('body *');

    // Найти индекс с использованием базового DOM API
    var index = elems.index(document.getElementById("oblock"));
    console.log("Индекс, найденный с использованием DOM-элемента: " + index);

    // Найти индекс с использованием другого объекта jQuery
    index = elems.index($('#oblock'));
    console.log("Индекс, найденный с использованием объекта jQuery: " + index);


    $('.edit').hide();
    $('.edit-case').on('click', function () {
        var tr = $(this).parents('tr:first');
        tr.find('.edit, .read').toggle();
    });
    $('.update-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        var gender = tr.find('#Gender').val();
        var phone = tr.find('#PhoneNo').val();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "http://localhost:55627/Default2/Edit",
            data: JSON.stringify({ "id": id, "gender": gender, "phone": phone }),
            dataType: "json",
            success: function (data) {
                tr.find('.edit, .read').toggle();
                $('.edit').hide();
                tr.find('#gender').text(data.person.Gender);
                tr.find('#phone').text(data.person.Phone);
            },
            error: function (err) {
                alert("error");
            }
        });
    });
    $('.cancel-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var id = $(this).prop('id');
        tr.find('.edit, .read').toggle();
        $('.edit').hide();
    });
    $('.delete-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://localhost:55627/Default2/Delete/" + id,
            dataType: "json",
            success: function (data) {
                alert('Delete success');
                window.location.href = "http://localhost:55627/Default2/Index";
            },
            error: function () {
                alert('Error occured during delete.');
            }
        });
    });
});