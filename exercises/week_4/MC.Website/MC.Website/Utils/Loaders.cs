using MC.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MC.Website.Utils
{
    public static class Loaders
    {
        private static ApplicationDbContext dbApp = new ApplicationDbContext();

        public static IEnumerable<SelectListItem> RolesToSelectItem()
        {
            return dbApp.Roles.Select(x => new SelectListItem { Value = x.Name, Text = x.Name });
        }
    }
}