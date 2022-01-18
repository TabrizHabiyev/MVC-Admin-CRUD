$("#form").submit(function () {
    var jqxhr = $.post('/AdminArea/Product/Create', $('#form').serialize())
        .success(function () {
            var loc = jqxhr.getResponseHeader('Location');
            var a = $('<a/>', { href: loc, text: loc });
            $('#message').html(a);
        })
        .error(function () {
            $('#message').html("Error posting the update.");
        });
    return false;
});


//Axios create Product
function validateForm(){


    // var fileUpload = $("#product_image").get(0);  
    // var files = fileUpload.files;  


    // var FormData = {
    //     Photos: files,
    //     Price: document.getElementById("price").value,
    //     Name: document.getElementById("name").value,
    //     CategoryId: 5,
    // }


    // $.post("/AdminArea/Product/Create",{
    //     Photos: files,
    //     Price: document.getElementById("price").value,
    //     Name: document.getElementById("name").value,
    //     CategoryId: 5,
    // })

            // axios({
            //     method : 'post',
            //     url : '/AdminArea/Product/Create',
            //     data : FormData,

            // })
            // .then((res)=>{
            //     console.log(res);
            // })
            // .catch((err) => {throw err});
    return false;
}


