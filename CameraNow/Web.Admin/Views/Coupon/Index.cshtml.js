$(document).ready(function () {
    $('#check_all').on('change', function () {
        if ($(this).is(':checked')) {
            $('#couponDatatable tbody input[type="checkbox"]').prop('checked', true);
        }
        else {
            $('#couponDatatable tbody input[type="checkbox"]').prop('checked', false);
        }
    })

    var id;
    //$('.btn_Del').on('click', function () {
    //    id = $(this).data('id');

    //    $.ajax({
    //        url: `https://localhost:7100/api/CouponAPI/${id}`,
    //        data: { id: id },
    //        success: function (response) {
    //            console.log(response);

    //            var data = response;
    //            var htmls = `
    //                <p>Bạn có chắc muốn xóa mã giảm giá có mã '${data.code}'</p>
    //            `;

    //            $('#confirmDelModalBody').html(htmls);
    //        },
    //        error: function (err) {
    //            console.error(err);
    //        }

    //    })
    //});

    $('.btn_Del').on('click', function () {
        var product_id = $(this).data('id');

        Swal.fire({
            title: "Xóa sản phẩm?",
            text: "Bạn có chắc muốn xóa!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            cancelButtonText: "Không!",
            confirmButtonText: "Có, xóa!"
        }).then((result) => {
            if (result.isConfirmed) {
                $('.loading').show();

                $.ajax({
                    url: "Coupon/Delete",
                    type: 'POST',
                    data: { id: product_id },
                    success: function (response) {
                        if (response.success) {
                            //var page = localStorage.getItem('current_page') || 1;
                            //var search_input = $('#search_input').val();
                            //var query = `?page=${page}`;

                            //if (search_input && search_input.trim() !== '')
                            //    query += `&filter_text=${encodeURIComponent(search_input)}`;

                            window.location.href = `/Coupon`;
                        }

                        $('.loading').hide();
                    },
                    error: function (err) {
                        $('.loading').hide();
                        console.error(err);
                    }
                })
            }
        });
    });

    $('#btn_confirmDelete').on('click', function () {
        $('.loading').show();

        $.ajax({
            url: 'Coupon/Delete',
            data: { id: id },
            type: 'POST',
            success: function (response) {
                if (response.success) {
                    $('.loading').hide();
                    window.location.href = '/Coupon';
                }

                id = null;
            },
            error: function (err) {
                $('.loading').hide();
                id = null;

            }
        })
    });

    let setTimer;
    let $searchInput = $('#search_input');
    $searchInput.on('keydown', function () {
        clearTimeout(setTimer);
    })

    $searchInput.on('keyup', function () {
        setTimer = setTimeout(function () {
            $('#frm_search').submit();
        }, 1000);
    })
})