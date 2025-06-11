const FormatPrice = (price, format) => {
    switch (format) {
        case 'VNĐ':
            return Number(price)?.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
        case 'USD':
            return Number(price)?.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
        default:
            return Number(price);
    }
};

const FormatLongDate = (dateStr) => {
    const date = new Date(dateStr);

    const options = { year: 'numeric', month: 'long', day: 'numeric' };

    const formattedDate = date.toLocaleDateString('en-GB', options);

    return formattedDate;
};

const FormatLongDateTime = (dateStr) => {
    const date = new Date(dateStr);

    // Định dạng ngày và tháng
    const day = date.getDate();
    const month = date.toLocaleString('en-US', { month: 'short' }).toUpperCase();
    const year = date.getFullYear();

    // Định dạng giờ, phút và AM/PM
    let hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12; // Đổi sang giờ 12 giờ

    // Kết hợp các phần thành chuỗi kết quả
    const formattedDate = `${day} ${month} ${year}, ${hours}:${minutes} ${ampm}`;

    return formattedDate;
};

function toSnakeCase(str) {
    // Bảng chuyển đổi ký tự có dấu thành không dấu
    const from = 'áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ';
    const to = 'aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd';

    // Chuẩn hóa chuỗi: Bỏ dấu tiếng Việt
    str = str
        .split('')
        .map((char, i) => {
            let index = from.indexOf(char);
            return index !== -1 ? to[index] : char;
        })
        .join('');

    // Thay thế khoảng trắng và ký tự đặc biệt thành "_"
    return str
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '_') // Ký tự không phải chữ cái, số => "_"
        .replace(/^_+|_+$/g, ''); // Xóa ký tự "_" ở đầu và cuối
}

function generateOrderCode() {
    const now = new Date();
    const year = now.getFullYear().toString().slice(-2); // Lấy 2 số cuối của năm
    const month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng (cộng 1 vì getMonth() trả về từ 0-11)
    const day = String(now.getDate()).padStart(2, '0'); // Ngày

    const randomChars = Math.random().toString(36).substring(2, 10).toUpperCase(); // Lấy 8 ký tự ngẫu nhiên

    return `${year}${month}${day}${randomChars}`;
}

export { FormatPrice, FormatLongDate, FormatLongDateTime, toSnakeCase, generateOrderCode };
