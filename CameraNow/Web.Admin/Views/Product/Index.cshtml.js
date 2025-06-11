
$(document).ready(function () {
    var hideCreationTime = false;

    // Format number to ###.### (Vietnam style)
    function formatNumber(num) {
        return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }

    function cleanNumber(str) {
        return str.replace(/\./g, "").replace(/[^0-9]/g, "");
    }

    $('#Price').on('input', function () {
        let val = $(this).val();
        val = cleanNumber(val); // bỏ dấu . và ký tự không phải số
        if (val) {
            $(this).val(formatNumber(val));
        } else {
            $(this).val('');
        }
    })

    $('#Promotion_Price').on('input', function () {
        let val = $(this).val();
        val = cleanNumber(val); // bỏ dấu . và ký tự không phải số
        if (val) {
            $(this).val(formatNumber(val));
        } else {
            $(this).val('');
        }
    })


    $("#chk_hide_show_status").on("change", function () {
        $('.hiden-status-col').toggle();

    });

    $('#chk_hide_show_creation_date').on('click', function () {
        $('.hiden-creation-date-col').toggle();
    })

    $('#check_all').on('change', function () {
        if ($(this).is(':checked'))
            $('#product_datatable tbody tr input[type="checkbox"]').prop('checked', true)
        else
            $('#product_datatable tbody tr input[type="checkbox"]').prop('checked', false)
    });

    //$('#product_datatable tbody input[type=checkbox]').on('change', function (e) {

    //})

    // Image
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
                preview.append(wrap);
            };
            reader.readAsDataURL(file);
        })
    })

    $(document).on('click', '.btnDelImg', function () {
        var index = $(this).parent().parent().data('index');

        removeFile($('#image')[0], index);

        // Nếu số lượng ảnh dưới 9, mở lại khả năng tải ảnh
        if (more_image.length < 9) {
            $('#moreimage').removeAttr('disabled');
            $('#choose-more-img').removeClass('text-body-tertiary');
        }

        $(this).parent().parent().remove();

        // Cập nhật lại data-index cho các .preview-img-wrap còn lại
        $('.preview-img-wrap').each(function (index) {
            $(this).attr('data-index', index);
        });
    });

    $('#image2').on('change', function (e) {
        var preview = $('#preview-img2');

        $.each(e.target.files, (index, file) => {
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
                            .addClass('btnDelImg2')
                            .append(
                                $("<i>").addClass("fa-regular fa-trash-can")
                            )
                    );

                wrap.append(imageContainer, actions);

                //imgElement.addClass('col-4')
                preview.append(wrap);
            };
            reader.readAsDataURL(file);
        })
    })

    $(document).on('click', '.btnDelImg2', function () {
        var index = $(this).parent().parent().data('index');

        removeFile($('#image2')[0], index);

        // Nếu số lượng ảnh dưới 9, mở lại khả năng tải ảnh
        if (more_image.length < 9) {
            $('#moreimage').removeAttr('disabled');
            $('#choose-more-img').removeClass('text-body-tertiary');
        }

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

            console.log(input.files);
        }
    }

    var more_image = [];
    $('#moreimage').on('change', function (e) {
        var preview = $('#preview-moreimg');

        for (let i = 0; i < e.target.files.length; i++) {
            if (more_image.length >= 9) {
                break;
            }

            more_image.push(e.target.files[i])

            const index = more_image.length == 0 ? 0 : more_image.length - 1;
            DisplayImage(e.target.files[i], preview, index);

        }

        if (more_image.length >= 9) {
            $(this).attr('disabled', true);
            $('#choose-more-img').addClass('text-body-tertiary')
        }
    })

    $(document).on('click', '.btnDelImgOfMoreImg', function () {
        var index = $(this).parent().parent().data('index');

        more_image.splice(index, 1);

        UpdateImgCount();

        removeFile($('#moreimage')[0], index);

        // Nếu số lượng ảnh dưới 9, mở lại khả năng tải ảnh
        if (more_image.length < 9) {
            $('#moreimage').removeAttr('disabled');
            $('#choose-more-img').removeClass('text-body-tertiary');
        }

        $(this).parent().parent().remove();

        // Cập nhật lại data-index cho các .preview-img-wrap còn lại
        $('.preview-img-wrap').each(function (index) {
            $(this).attr('data-index', index);
        });
    });

    function DisplayImage(file, preview, index) {
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
                        .addClass('btnDelImgOfMoreImg')
                        .append(
                            $("<i>").addClass("fa-regular fa-trash-can")
                        )
                );

            wrap.append(imageContainer, actions);

            //imgElement.addClass('col-4')
            preview.append(wrap);

            UpdateImgCount();
        };
        reader.readAsDataURL(file);
    }

    function UpdateImgCount() {
        var img_count = more_image.length;
        $('#img-count').html(img_count)
    }

    $('#createForm').on('submit', function (e) {
        e.preventDefault();
        $('#Price').each(function () {
            const raw = cleanNumber($(this).val());
            $(this).val(raw);
        });
        $('#Promotion_Price').each(function () {
            const raw = cleanNumber($(this).val());
            $(this).val(raw);
        });

        $('.loading').show();

        let formData = new FormData(this);

        var keysToDelete = [];
        formData.forEach(function (value, key) {
            if (key === 'MoreImage') {
                keysToDelete.push(key);
            }
        });

        // Xóa các mục 'MoreImage' khỏi formData
        keysToDelete.forEach(function (key) {
            formData.delete(key);
        });

        for (let i = 0; i < more_image.length; i++) {
            formData.append('MoreImage', more_image[i]);
        }

        // Log the FormData entries to see the contents
        let formDataEntries = [];
        for (let pair of formData.entries()) {
            formDataEntries.push({ key: pair[0], value: pair[1].name ? pair[1].name : pair[1] });
        }

        // Xử lý phần Size và Quantity
        const sizesWithQuantities = [];
        let isValid = true;

        $('.inpSizeWrap').each(function () {
            const size = $(this).find('input[name="Sizes"]').val();
            const quantity = $(this).find('input[name="Size_Quantities"]').val();

            // Kiểm tra thiếu size hoặc thiếu quantity
            if ((size && !quantity) || (!size && quantity)) {
                isValid = false;
            }

            if (size && quantity) {
                sizesWithQuantities.push({
                    size: size,
                    quantity: quantity
                });
            }
        });

        // Nếu không hợp lệ thì cảnh báo và không gửi form
        if (!isValid) {
            $('.loading').hide();
            alert("Vui lòng nhập đầy đủ thông tin Size và Số lượng cho từng dòng.");
            return;
        }

        // Xoá các key cũ nếu có
        formData.delete('Sizes');
        formData.delete('Quantities');
        formData.delete('SizesWithQuantities');

        // Đưa JSON chuỗi vào FormData
        formData.append('SizesQuantity', JSON.stringify(sizesWithQuantities));

        console.log(formDataEntries)

        // //Thực hiện submit với dữ liệu mới
        $.ajax({
            url: 'Create',
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('.loading').hide();
                    window.location.href = "/Product";
                }
                else {
                    Toastify({
                        text: "Thêm mới sản phẩm thất bại!",
                        duration: 3000,
                        gravity: "bottom", // `top` or `bottom`
                        position: "right", // `left`, `center` or `right`
                        style: {
                            background: "linear-gradient(to right, #ff4e50, #f44336)", // đỏ chuyển đỏ đậm
                        },
                    }).showToast();
                    $('.loading').hide();
                }
            },
            error: function (response) {
                $('.loading').hide();
                console.error(response);
            }
        })
    })
    // End Image    

    $('.btn-del').on('click', function () {
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
                    url: "Product/Delete",
                    type: 'POST',
                    data: { id: product_id },
                    success: function (response) {
                        if (response.success) {
                            var page = localStorage.getItem('current_page') || 1;
                            var search_input = $('#search_input').val();
                            var query = `?page=${page}`;

                            if (search_input && search_input.trim() !== '')
                                query += `&filter_text=${encodeURIComponent(search_input)}`;

                            window.location.href = `/Product${query}`;

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

    $('#btn_del_multi').on('click', function () {
        var list = $('#product_datatable tbody input[type=checkbox]:checked');
        var ids = [];
        $.each(list, function (i, e) {
            ids.push($(e).data('id'));
        })

        if (list.length <= 0) {
            Swal.fire("Vui lòng chọn sản phẩm để xóa!");
        }
        else {
            Swal.fire({
                title: "Xóa nhiều sản phẩm?",
                text: "Bạn có chắc muốn xóa " + ids.length + " sản phẩm?",
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
                        url: "Product/DeleteRange",
                        type: 'POST',
                        data: { ids: ids },
                        success: function (response) {
                            var page = localStorage.getItem('current_page') || 1;
                            var search_input = $('#search_input').val();
                            var query = `?page=${page}`;
                            if (response.success) {

                                if (search_input && search_input.trim() !== '')
                                    query += `&filter_text=${encodeURIComponent(search_input)}`;

                                window.location.href = `/Product${query}`;

                                $('.loading').hide();
                            }
                            else {
                                $('.loading').hide();
                            }

                        },
                        error: function (err) {
                            $('.loading').hide();
                            console.error(err);
                        }
                    })
                }
            });
        }
    })


    //$('.product_datatable_row').dblclick(function () {
    //    window.location.href = `/Details/${$(this).data('id')}`;
    //})

    $('#editForm').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        var keysToDelete = [];
        formData.forEach(function (value, key) {
            if (key === 'MoreImage') {
                keysToDelete.push(key);
            }
        });

        // Xóa các mục 'MoreImage' khỏi formData
        keysToDelete.forEach(function (key) {
            formData.delete(key);
        });

        for (let i = 0; i < more_image.length; i++) {
            formData.append('MoreImage', more_image[i]);
        }

        const imgs = $('.preview-img-items');
        let imgs_list = [];
        imgs.each(function (index) {
            imgs_list.push($(this).attr('src'));
        })

        for (let i = 0; i < imgs_list.length; i++) {
            formData.append('Images', imgs_list[i]);
        }

        // Log the FormData entries to see the contents
        let formDataEntries = [];
        for (let pair of formData.entries()) {
            formDataEntries.push({ key: pair[0], value: pair[1].name ? pair[1].name : pair[1] });
        }

        // Xử lý phần Size và Quantity
        const sizesWithQuantities = [];
        let isValid = true;

        $('.inpSizeWrap').each(function () {
            const size = $(this).find('input[name="Product_Sizes"]').val();
            const quantity = $(this).find('input[name="Size_Quantities"]').val();

            // Kiểm tra thiếu size hoặc thiếu quantity
            if ((size && !quantity) || (!size && quantity)) {
                isValid = false;
            }

            if (size && quantity) {
                sizesWithQuantities.push({
                    size: size,
                    quantity: quantity
                });
            }
        });

        // Nếu không hợp lệ thì cảnh báo và không gửi form
        if (!isValid) {
            $('.loading').hide();
            alert("Vui lòng nhập đầy đủ thông tin Size và Số lượng cho từng dòng.");
            return;
        }

        // Xoá các key cũ nếu có
        formData.delete('Product_Sizes');
        formData.delete('Quantities');
        formData.delete('SizesWithQuantities');

        // Đưa JSON chuỗi vào FormData
        formData.append('SizesQuantity', JSON.stringify(sizesWithQuantities));

        console.log(formDataEntries)

        $('.loading').show();

        $.ajax({
            type: 'POST',
            url: 'Update',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('.loading').hide();
                    window.location.href = `/Product`;
                }
                else {
                    $('.loading').hide();
                    console.log(response?.message);

                    Toastify({
                        text: "Cập nhật thông tin sản phẩm thất bại!",
                        duration: 3000,
                        gravity: "bottom", // `top` or `bottom`
                        position: "right", // `left`, `center` or `right`
                        style: {
                            background: "linear-gradient(to right, #ff4e50, #f44336)", // đỏ chuyển đỏ đậm
                        },
                    }).showToast();
                }

            },
            error: function (response) {
                $('.loading').hide();
                console.error(response);
            }
        })
    })
})