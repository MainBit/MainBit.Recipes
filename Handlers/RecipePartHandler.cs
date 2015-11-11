using System;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Security;
using Orchard.Services;
using Orchard.Recipes.Services;
using MainBit.Recipes.Models;

namespace MainBit.Recipes.Handlers
{
    [UsedImplicitly]
    public class RecipePartHandler : ContentHandler {
        private readonly IRecipeParser _recipeParser;

        public RecipePartHandler(IRecipeParser recipeParser)
        {
            _recipeParser = recipeParser;

            OnLoading<RecipePart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<RecipePart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

        }

        protected void LazyLoadHandlers(RecipePart part) {
            part._recipe.Loader(() => _recipeParser.ParseRecipe(part.RecipeText));           
        }
    }
}
