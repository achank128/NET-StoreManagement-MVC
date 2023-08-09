var newProductItemHtml = "";

$(document).ready(function () {
    $.ajax({
        url: '/Product/GetAll',
        type: 'GET',
    }).done(function (result) {
        newProductItemHtml = `
        <tr class="product-item">
            <td>
                <select class="Product form-select">${
                    result.data.map((p) => {
                        return `<option value="${p.id}">${p.productName}</option>`
                    }).join('') 
                }</select>
            </td>
            <td><input type="number" class="Quantity form-control" /></td>
            <td><input type="number" class="Price form-control" /></td>
            <td class="text-center align-middle">
                <button class="btn btn-outline-danger btn-sm"><i class="bi bi-x"></i></button>
            </td>
        </tr>`
        $("#product-items").append(newProductItemHtml);
        $("#add-product-item").click(function (e) {
            e.preventDefault();
            $("#product-items").append(newProductItemHtml);
        });
    });
});


$('#save').click(function (e) {
    e.preventDefault();
    var data = {
        importerName: $("#ImporterName").val(),
        supplier: $("#Supplier").val(),
        importDate: $("#ImportDate").val(),
        total: 0,
        listProducts: []
    };

    $('.product-item').each((i, e) => {
        var item = {
            productId: $(e).find('.Product :selected').val(),
            quantity: $(e).find('.Quantity').val(),
            price: $(e).find('.Price').val(),
        }
        data.total += Number(item.quantity) * Number(item.price);
        data.listProducts.push(item);
    })

    $.ajax({
        url: '/ImportStore/CreateImportStore',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(data),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    }).done(function (result) {
        location.replace("/ImportStore")
    });
});