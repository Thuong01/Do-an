using Models.Enums;
using Models.Models;

namespace Datas.ViewModels
{
    public class BaseSpecification
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(string? filter, Status? status, string? sorting)
        {
            Filter = filter;
            Status = status;
            Sorting = sorting;
        }

        public string? Filter { get; set; }
        public Status? Status { get; set; } = Models.Enums.Status.All;
        public string? Sorting { get; set; } = "name";

        protected void ApplyFilter(string? filter)
        {

        }

    }
}
