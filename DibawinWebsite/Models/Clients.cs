﻿using DibawinWebsite.Areas.Identity.Data;
using DibawinWebsite.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DibawinWebsite.Models
{
    public class Clients : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string LatinTitle { get; set; }
        public bool Status { get; set; }
        public DateTime RegDateTime { get; set; }
        public string DefinedByUserId { get; set; }
        public string ModifiedByUsers { get; set; } //comination of 'UserId' and 'ModifiedDateTime'
        //=========================================================================================
        public string UserId { get; set; }
        public string ClientCode { get; set; }
        public string ManagerFullName { get; set; }
        public bool IsRealPersonality { get; set; }
        public byte[] Logo { get; set; }
        public byte[] LogoTiny { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("DefinedByUserId")]
        public virtual ApplicationUser DefinedByUser { get; set; }

        public ICollection<ClientAddress> ClientAddress { get; set; }
        public ICollection<Projects> Projects { get; set; }

    }
}
