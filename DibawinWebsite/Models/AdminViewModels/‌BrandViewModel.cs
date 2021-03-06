﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DibawinWebsite.Models.AdminViewModels
{
    public class BrandViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "پر کردن این فیلد الزامیست")]
        [DisplayName("نام برند ")]
        public string Name { get; set; }
        public string LatinName { get; set; }
        [DisplayName("توضیحات")]
        public string Description { get; set; }
        public bool Status { get; set; }

    }
}
