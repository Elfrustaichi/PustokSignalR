$(document).on("click", ".single-modal-btn", function (e) {
    e.preventDefault();

    let url = $(this).attr("href");

    fetch(url)
        .then(response => {

            if (response.ok) {
                return response.text()
            }
            else {
                alert("Xeta bas verdi!")
            }
        })
        .then(data => {
            $("#quickModal .modal-dialog").html(data)
            $("#quickModal").modal('show');
        })
   
})

$(document).on("click", ".single-basket-button", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url)
        .then(response => {
            if (!response.ok) {
                alert("xeta bas verdi")
            }
            return response.text()
        })
        .then(data => {
            $(".cart-block").html(data)
        })
})

$(document).on("click", ".removefrombasket", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url)
        .then(response => {
            if (!response.ok) {
                alert("xeta bas verdi")
                return
            }
            return response.text()
        })
        .then(data => {
            $(".cart-block").html(data)
        })
})
