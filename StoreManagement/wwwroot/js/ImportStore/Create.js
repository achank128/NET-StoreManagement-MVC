$(document).ready(function () {
    var newProductItemHtml = "";
    $.ajax({
        url: '/Product/GetAll',
        type: 'GET',
    }).done(function (result) {
        newProductItemHtml = `
        <tr class="product-item">
            <td>
                <select class="Product form-select">${result.data.map((p) => {
            return `<option value="${p.id}">${p.productName}</option>`
        }).join('')
            }</select>
            </td>
            <td><input type="number" class="ProductPrice form-control" readonly/></td>
            <td><input type="number" class="Quantity form-control" /></td>
            <td><input type="number" class="Price form-control" /></td>
            <td class="text-center align-middle">
                <button class="btn btn-outline-danger btn-sm"><i class="bi bi-x"></i></button>
            </td>
        </tr>`
        $("#add-product-item").click((e) => {
            e.preventDefault();
            $("#product-items").append(newProductItemHtml);
            setProductItem(result)
        })
        setProductItem(result)
    });
    $('body').on('change', '.Price', (e) => {
        console.log(e)
    });

    $('body #table').on('change', '.Quantity', (e) => {
        console.log(e)
    })
});

function setProductItem(result) {
    $('.product-item').each((i, e) => {
        var productId = $(e).find('.Product :selected').val();
        var productPrice = result.data.find(p => p.id == productId).price
        $(e).find('.ProductPrice').val(productPrice);

        $(e).find('.Product').on('change', () => {
            var productId = $(e).find('.Product :selected').val();
            var productPrice = result.data.find(p => p.id == productId).price
            $(e).find('.ProductPrice').val(productPrice);
        })
        $(e).find('.btn').on('click', () => {
            $(e).remove();
        })
    })
}

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