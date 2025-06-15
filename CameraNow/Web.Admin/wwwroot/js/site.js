// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('.nav-collapse').on('click', function () {
        $(this).find('.fa-caret-down').toggleClass('rotate-180');
    })
})

function GetNotification() {
    $('#contentNoti').html('');

    $.ajax({
        url: '/Home/GetNotification',
        type: 'GET',
        success: function (res) {
            if (res.success) {
                if (res.result.length == 0) {
                    $('#contentNoti').append('<li class="text-center mb-2 mt-2">Không có thông báo nào</li>');
                    return;
                }
                else {
                    for (var i = 0; i < res.result.length; i++) {
                        var notification = res.result[i];
                        var notificationItem = `<li class="notification-item">
                            <i class="bi bi-exclamation-circle text-warning"></i>
                            <div>
                                <h4>${notification.title}</h4>
                                <p>${notification.content}</p>
                                <p>${notification.createdDate}</p>
                            </div>
                        </li>`

                        $('#contentNoti').append(notificationItem);
                    }
                }
            } else {
                alert('Không thể lấy thông báo');
            }
        },
        error: function () {
            alert('Có lỗi khi gọi API.');
        }
    });
}