﻿using System.ComponentModel;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class AuthorFilterModel

    {

		public int Id { get; set; }

		[DisplayName("Từ khóa ...")]
		public string Keyword { get; set; }
		public string FullName { get; set; }
		public string Notes { get; set; }
		public string Email { get; set; }
        public string ImageUrl { get; set; }



    }
}