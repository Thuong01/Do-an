$(function () {

    // Khi nhấn nút "Upload" thì mở hộp thoại chọn file
    $('#upload-btn').click(function (e) {
        e.preventDefault();
        $('#ImageFile').click();
    });

    // Khi chọn file => hiển thị ảnh preview
    $('#ImageFile').change(function (e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (event) {
                $('#previewImage').attr('src', event.target.result);
                $("#Image_yn").val(true); // Đánh dấu là đã có ảnh
                $("#Delete_yn").val(false);
            };
            reader.readAsDataURL(file);
        }
    });

    // Khi nhấn "Remove" => reset ảnh về default + clear file input
    $('#remove-btn').click(function (e) {
        e.preventDefault();
        $('#ImageFile').val('');
        $("#Image_yn").val(false); // Đánh dấu là đã có ảnh
        $("#Delete_yn").val(true);
        $('#previewImage').attr('src', '/img/avatar_default.jpg');
    });


    $('#change-password-btn').on('click', () => {
        $('#outerPass').removeClass('d-none');
        $('#cancel-password-btn').removeClass('d-none');

    });

    $('#cancel-password-btn').on('click', () => {
        $('#outerPass').addClass('d-none');
        $('#cancel-password-btn').addClass('d-none');

    });

    function validatePassword(input) {
        const errorElement = $('#outerMsgPass');
        const inputElement = $('#password');

        // Reset styles and error
        errorElement.hide();
        inputElement.removeClass('invalid valid');

        // Validation rules
        const options = {
            RequireDigit: true,
            RequireLowercase: true,
            RequireNonAlphanumeric: true,
            RequireUppercase: true,
            RequiredLength: 6,
            RequiredUniqueChars: 1
        };

        let errors = [];

        // Check length
        if (input.length < options.RequiredLength) {
            errors.push(`<span class="error fs-6 text">Password must be at least ${options.RequiredLength} characters long.</span>`);
        }

        // Check for digit
        if (options.RequireDigit && !/\d/.test(input)) {
            errors.push('<span  class="error fs-6 text">Password must contain at least one digit (0-9).</span>');
        }

        // Check for lowercase
        if (options.RequireLowercase && !/[a-z]/.test(input)) {
            errors.push('<span class="error fs-6 text">Password must contain at least one lowercase letter (a-z).</span>');
        }

        // Check for uppercase
        if (options.RequireUppercase && !/[A-Z]/.test(input)) {
            errors.push('<span class="error fs-6 text">Password must contain at least one uppercase letter (A-Z).</span>');
        }

        // Check for non-alphanumeric
        if (options.RequireNonAlphanumeric && !/[^a-zA-Z0-9]/.test(input)) {
            errors.push('<span class="error fs-6 text">Password must contain at least one non-alphanumeric character.</span>');
        }

        // Check for unique characters
        if (options.RequiredUniqueChars > 0) {
            const uniqueChars = new Set(input).size;
            if (uniqueChars < options.RequiredUniqueChars) {
                errors.push(`<span class="error fs-6 text">Password must contain at least ${options.RequiredUniqueChars} unique character(s).<span>`);
            }
        }

        // Display errors if any
        if (errors.length > 0) {
            errorElement.html(errors.join(' ')).show();
            inputElement.addClass('invalid');
            $('#update-profile-button').prop('disabled', true);
        } else {
            inputElement.addClass('valid');
            $('#update-profile-button').prop('disabled', false);
            errorElement.html(errors.join(' ')).show();
        }

    }

    $('#Password').on('input', function () {
        const input = $(this).val();
        validatePassword(input);
        
    });

    $('#profile-form').submit(function (e) {
        e.preventDefault();
        $('.loading').show();

        var form = $('#profile-form')[0];
        var formData2 = new FormData(form);

        //// Kiểm tra xem có file được chọn không
        //var imageFile = $('input[name="ImageFile"]')[0].files[0];
        //if (!imageFile) {
        //    formData2.append('Image_yn', false);  
        //}

        $.ajax({
            url: '/Account/EditProfile', // Thay bằng API thực tế
            type: 'POST',
            data: formData2,
            processData: false, // Không xử lý dữ liệu
            contentType: false, // Không thiết lập contentType
            success: function (response) {
                if (response.success) {
                    Toastify({
                        text: "Cập nhật thông tin tài khoản thành công!",
                        duration: 3000,
                        gravity: "bottom", // `top` or `bottom`
                        position: "right", // `left`, `center` or `right`
                        style: {
                            background: "linear-gradient(to right, #00b09b, #96c93d)",
                        },

                    }).showToast();
                    $('.loading').hide();
                    // Có thể redirect hoặc hiển thị thông báo

                    // Reload trang sau khi thông báo hiển thị
                    setTimeout(function () {
                        location.reload();
                    }, 1000); // Đợi 3 giây để người dùng thấy thông báo
                }
                else {
                    if (response.result && response.result.key && response.result.key == 'Password') {
                        var msg = response.result.value.split(',');
                        console.log(msg);
                    }

                    Toastify({
                        text: "Cập nhật thông tin tài khoản thất bại!",
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
            error: function (xhr) {
                //alert("Có lỗi xảy ra: " + xhr.responseText);
                Toastify({
                    text: "Cập nhật thông tin tài khoản thất bại!",
                    duration: 3000,
                    gravity: "bottom", // `top` or `bottom`
                    position: "right", // `left`, `center` or `right`
                    style: {
                        background: "linear-gradient(to right, #ff4e50, #f44336)", // đỏ chuyển đỏ đậm
                    },
                }).showToast();
                $('.loading').hide();
            }
        });
    });
})