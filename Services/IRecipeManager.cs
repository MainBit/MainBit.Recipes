using MainBit.Recipes.Models;
using MainBit.Recipes.Services.Models;
using Orchard;
using Orchard.Collections;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MainBit.Recipes.Services
{
    public interface IRecipeManager : IDependency {

        RecipePart Get(int id);
    }

    public class RecipeManager : IRecipeManager {
        private readonly IContentManager _contentManager;

        public RecipeManager(
            IContentManager contentManager,
            IRecipeParser recipeParser)
        {
            _contentManager = contentManager;
        }

        public RecipePart Get(int id)
        {
            return _contentManager.Get<RecipePart>(id);
        }
    }
}