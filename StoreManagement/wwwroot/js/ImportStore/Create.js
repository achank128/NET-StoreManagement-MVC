$(document).ready(function () {
    //Get product item
    var newProductItemHtml = "";
    $.ajax({
        url: '/Product/GetData',
        type: 'GET',
    }).done(function (result) {
        newProductItemHtml = `
        <tr class="product-item">
            <td>
                <select class="Product form-select" required>${result.data.map((p) => {
            return `<option value="${p.id}">${p.productName}</option>`
        }).join('')
            }</select>
            </td>
            <td><input type="number" class="ProductPrice form-control" readonly/></td>
            <td><input type="number" class="Quantity form-control" min="0" required/></td>
            <td><input type="number" class="Price form-control" min="0" required/></td>
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


    //Caculate total
    $('#table').on('change', '.Price', (e) => {
        caculateTotal()
    });

    $('#table').on('change', '.Quantity', (e) => {
        caculateTotal()
    })


    //Save
    $('.needs-validation').each((i, form) => {
        form.addEventListener('submit', event => {
            event.preventDefault()
            if (!form.checkValidity()) {
                event.stopPropagation()
                notyf.error('Vui lòng nhập đầy đủ thông tin.');
            } else {
                handleSave()
            }
            form.classList.add('was-validated')
        }, false)
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

function caculateTotal() {
    var total = 0;
    $('.product-item').each((i, e) => {
        var item = {
            quantity: $(e).find('.Quantity').val(),
            price: $(e).find('.Price').val(),
        }
        total += Number(item.quantity) * Number(item.price);
    });
    $("#Total").val(total)
}

function handleSave() {
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
        error: (e) => {
            notyf.error('Lưu dữ liệu không thành công');
        },
    }).done(function (result) {
        location.replace("/ImportStore")
    })
}
