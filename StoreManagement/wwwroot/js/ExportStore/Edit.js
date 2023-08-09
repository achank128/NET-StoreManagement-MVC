var newProductItemHtml = ""

$(document).ready(function () {
    $.ajax({
        url: '/Product/GetAll',
        type: 'GET',
    }).done(function (result) {
        newProductItemHtml = `
        <tr class="product-item">
            <td>
                <select id="Product" class="form-select">${
                    result.data.map((p) => {
                        return `<option value="${p.id}">${p.productName}</option>`
                    }).join('') 
                }</select>
            </td>
            <td><input id="Quantity" type="number" class="form-control" /></td>
            <td><input id="Price" type="number" class="form-control" /></td>
            <td class="text-center align-middle">
                <button class="btn btn-outline-danger btn-sm"><i class="bi bi-x"></i></button>
            </td>
        </tr>`
        $("#add-product-item").click(function (e) {
            e.preventDefault();
            $("#product-items").append(newProductItemHtml);
        })
    });
});


$('#save').click(function (e) {
    e.preventDefault();
    var data = {
        id: $("#ExportStoreId").val(),
        exporterName: $("#ExporterName").val(),
        customer: $("#Customer").val(),
        exportDate: $("#ExportDate").val(),
        total: 0,
        listProducts: []
    };

    $('.product-item').each((i, e) => {
        var item = {
            id: $(e).find('#ProductItemId')?.val(),
            productId: $(e).find('#Product :selected').val(),
            quantity: $(e).find('#Quantity').val(),
            price: $(e).find('#Price').val(),
        }
        data.total += Number(item.quantity) * Number(item.price);
        data.listProducts.push(item);
    })


    $.ajax({
        url: '/ExportStore/EditExportStore',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(data),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    }).done(function (result) {
        location.replace("/ExportStore")
    });
});