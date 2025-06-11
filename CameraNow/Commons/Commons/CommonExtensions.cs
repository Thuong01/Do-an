using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace Commons.Commons
{
    public static class CommonExtensions
    {
        //public static string password = "nmwk cwae byjq buap";// "thqfbkirbjaepzqm";
        //public static string Email = "anh038953@gmail.com"; // "tranvananh.works@gmail.com";

        public static string password = "thqfbkirbjaepzqm";
        public static string Email = "tranvananh.works@gmail.com";

        public static bool SendMail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com"; //host name
                    smtp.Port = 587; //port number
                    smtp.EnableSsl = true; //whether your smtp server requires SSL
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential()
                    {
                        UserName = Email,
                        Password = password
                    };
                }
                MailAddress fromAddress = new MailAddress(Email, name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);
                rs = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rs = false;
            }
            return rs;
        }

        public static string GenerateSEOTitle(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var slug = input.ToLower();

            // Đổi ký tự có dấu thành không dấu
            slug = Regex.Replace(slug, "[áàảạãăắằẳẵặâấầẩẫậ]", "a");
            slug = Regex.Replace(slug, "[éèẻẽẹêếềểễệ]", "e");
            slug = Regex.Replace(slug, "[iíìỉĩị]", "i");
            slug = Regex.Replace(slug, "[óòỏõọôốồổỗộơớờởỡợ]", "o");
            slug = Regex.Replace(slug, "[úùủũụưứừửữự]", "u");
            slug = Regex.Replace(slug, "[ýỳỷỹỵ]", "y");
            slug = Regex.Replace(slug, "[đ]", "d");

            // Xóa ký tự đặc biệt
            slug = Regex.Replace(slug, @"[\|\~\!\@\#\|\$\%\^\&\*\(\)\+\=\,\./\?\>\<\'\""\:\;]", "");

            // Đổi khoảng trắng thành ký tự gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-");

            // Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
            slug = Regex.Replace(slug, @"-+", "-");

            // Xóa các ký tự gạch ngang ở đầu và cuối
            slug = slug.Trim('-');

            return slug;
        }
    }
}
