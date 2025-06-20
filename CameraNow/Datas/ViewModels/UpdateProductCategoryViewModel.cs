﻿using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class UpdateProductCategoryViewModel : AuditableViewModel
    {
        public Guid ID { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Alias")]
        public string? Alias { get; set; }

        [Display(Name = "Parent_ID")]
        public Guid? Parent_ID { get; set; }

        [Display(Name = "Image")]
        public string? Image { get; set; }

        public string? Image_Public_Id { get; set; }
    }
}
