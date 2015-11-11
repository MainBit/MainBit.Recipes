using Orchard.Environment.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Recipes.ViewModels
{
    public class RecipeExecuteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<ShellSettings> TenantSettings { get; set; }
    }
}