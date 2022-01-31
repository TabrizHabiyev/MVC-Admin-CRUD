
$(".edidForm").each(function () {

    $(this).bind("submit", function (event) {

        event.preventDefault();

        let Data = $(this).serializeArray();

        var _Data = {
            ProductId: parseInt(Data[0].value),
            Text: Data[2].value,
            Id: parseInt(Data[1].value)
        }

        axios({
            method: 'post',
            url: 'https://localhost:44361/comment/edit',
            data: _Data
        }).then(function (response) {
            if (response.data == "ok") {
                $("#edidResponse").removeClass("alert alert-danger");
                $("#edidResponse").text("Your comment has been successfully updated").addClass("alert alert-success");
            } else {
                $("#edidResponse").removeClass("alert alert-success");
                $("#edidResponse").text(response.data).addClass("alert alert-danger");
            }

        });;

    });

});


