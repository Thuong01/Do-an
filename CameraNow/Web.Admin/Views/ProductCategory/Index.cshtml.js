$(function () {
    var product_id;

    $("#chk_hide_show_status").on("change", function () {
        $('.hiden-status-col').toggle();
    });

    $('#chk_hide_show_creation_date').on('click', function () {
        $('.hiden-creation-date-col').toggle();
    })

    $('#deleteModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        product_id = button.data('id');

        console.log(product_id)
    });

    // === Image ===\\
    $('#image').on('change', function (e) {
        var preview = $('#preview-img');

        var files = e.target.files;

        $.each(files, (index, file) => {
            var reader = new FileReader();
            reader.onload = function (event) {
                const wrap = $("<div>")
                    .addClass('preview-img-wrap')
                    .attr('data-index', index);

                const imageContainer = $('<div>').append($('<img>').attr('src', event.target.result));

                const actions = $('<div>')
                    .addClass('preview-img-action')
                    .append(
                        $('<span>')
                            .addClass('btnDelImg')
                            .append(
                                $("<i>").addClass("fa-regular fa-trash-can")
                            )
                    );
                wrap.append(imageContainer, actions);
                //imgElement.addClass('col-4')
                preview.html(wrap);
            };
            reader.readAsDataURL(file);
        })
    })

    $(document).on('click', '.btnDelImg', function () {
        var index = $(this).parent().parent().data('index');
        removeFile($('#image')[0], index);
        $(this).parent().parent().remove();
        // Cập nhật lại data-index cho các .preview-img-wrap còn lại
        $('.preview-img-wrap').each(function (index) {
            $(this).attr('data-index', index);
        });
    });


    function removeFile(input, indexToRemove) {
        if (input.files.length > 0) {
            const dt = new DataTransfer();
            const filesArray = Array.from(input.files).filter((file, index) => index !== indexToRemove);
            filesArray.forEach(file => {
                dt.items.add(file);
            });
            input.files = dt.files;
        }
    }

    // === End Image ===\\
    $('#check_all').on('change', function () {
        if ($(this).is(':checked'))
            $('#category_datatable tbody tr input[type="checkbox"]').prop('checked', true)
        else 
            $('#category_datatable tbody tr input[type="checkbox"]').prop('checked', false)
    });

    $('#del_multi_btn').on('click', function () {
        var data_id = $('tbody tr input[type="checkbox"]:checked');

        if (!data_id || data_id.length <= 0) {
            new Notyf().error('Chọn ít nhất một bản ghi!');
            return;
        }

        let dataIds = $('tbody tr input[type="checkbox"]:checked').map(function () {
            return $(this).data('id');
        }).get();

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $('.loading').show();

                $.ajax({
                    url: 'ProductCategory/DeleteRange',
                    data: { ids: dataIds },
                    type: 'POST',
                    success: function (response) {
                        if (response.success) {
                            $('.loading').hide();
                            window.location.href = '/ProductCategory';
                        }
                    },
                    error: function (err) {
                        $('.loading').hide();
                        console.error(err)
                    }
                })
            }
        });

        
    });

    $('.btn-del').on('click', function () {
        var product_id = $(this).data('id');

        Swal.fire({
            title: "Xóa danh mục sản phẩm?",
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
                    url: "ProductCategory/Delete",
                    type: 'POST',
                    data: { id: product_id },
                    success: function (response) {
                        if (response.success) {
                            window.location.href = '/ProductCategory';
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

    $('#confirmDelete').on('click', function () {
        $('.loading').show();
        $.ajax({
            url: 'ProductCategory/Delete',
            type: "POST",
            data: { id: product_id },
            success: function (response) {
                if (response.success) {
                    window.location.reload();
                    $('.loading').hide();
                    $('#deleteModal').modal('hide');
                    //location.reload();
                } else {

                }
            }
        })
    })
});
