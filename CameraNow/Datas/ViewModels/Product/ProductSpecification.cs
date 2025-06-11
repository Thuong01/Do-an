using Models.Enums;

namespace Datas.ViewModels.Product
{
    public class ProductSpecification : BaseSpecification
    {
        public ProductSpecification()
        {
        }

        public ProductSpecification(string? filter, Status? status, string? sorting, string? category, DateTime? createdDate) 
            : base(filter, status, sorting)
        {
            Category = category;
            CreatedDate = createdDate;
        }

        public string? Category { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
