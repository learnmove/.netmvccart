using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopingCart.Models
{
    [Bind(Exclude="ItemId")]
    public class Item
    {
        [ScaffoldColumn(false)]
        public int ItemId { get; set; }
        [DisplayName("分類")]
        public int CategoryId { get; set; }
        [DisplayName("廠商")]
        public int ProducerId { get; set; }
        [Required(ErrorMessage ="必須填寫價格")]
        [Range(0.1,100,ErrorMessage ="價格0.1~100")]
        public decimal Price { get; set; }
        [StringLength(1024)]
        [DisplayName("圖片網址")]
        public string ItemArtUrl { get; set; }
        [StringLength(160)]
        [Required(ErrorMessage = "必須填寫名稱")]

        public string Title { get; set; }

        public  virtual Producer Producer { get; set; }

        public virtual Category Category { get; set; }
    }
}