using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Orchard.Recipes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Recipes.Models
{
    public class RecipePart : ContentPart
    {
        internal LazyField<Recipe> _recipe = new LazyField<Recipe>();
        public Recipe Recipe {
            get { return _recipe.Value; }
        }

        public string RecipeText {
            get { return this.Retrieve(p => p.RecipeText); }
            set { this.Store(x => x.RecipeText, value); }
        }
    }
}