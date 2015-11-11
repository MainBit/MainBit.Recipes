using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Recipes.Handlers
{
    public class RecipeExecuteContentContext : ContentContextBase
    {
        public RecipeExecuteContentContext(ContentItem contentItem, IEnumerable<string> tenantNames)
            : base(contentItem)
        {
            TenantNames = tenantNames;
        }

        public IEnumerable<string> TenantNames { get; private set; }
    }
}