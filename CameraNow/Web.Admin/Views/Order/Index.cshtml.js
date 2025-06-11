$(document).ready(function () {
    $('.btnConfirmOrder').on('click', function () {
        $('.loading').show();
        const orderId = $(this).data('orderid');
        const userId = $(this).data('userid');
        const status = $(this).data('status');

        ChangeStatus(orderId, userId, status);        
    })

    $('.btnShippingOrder').on('click', function () {
        $('.loading').show();
        const orderId = $(this).data('orderid');
        const userId = $(this).data('userid');
        const status = $(this).data('status');

        ChangeStatus(orderId, userId, status);
    })

    $('.btnCancelOrder').on('click', function () {
        $('.loading').show();
        const orderId = $(this).data('orderid');
        const userId = $(this).data('userid');
        const status = $(this).data('status');

        ChangeStatus(orderId, userId, status);
    })


    function ChangeStatus(orderId, userId, status) {
        $.ajax({
            url: '/Order/ChangeOrderStatus',
            data: { orderid: orderId, userId: userId, status: status },
            type: 'POST',
            success: function (response) {
                if (response.success) {
                    $('.loading').hide();
                    window.location.href = "/Order";
                }
            },
            error: function (error) {
                console.log(error);
                $('.loading').hide();
            }
        })
    }
})