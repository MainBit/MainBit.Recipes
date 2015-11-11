using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using MainBit.Recipes.Models;
using MainBit.Recipes.Services;
using MainBit.Recipes.ViewModels;
using Orchard.Utility.Extensions;
using Orchard.Recipes.Services;

namespace MainBit.Recipes.Drivers {
    public class RecipePartDriver : ContentPartDriver<RecipePart> {
        private readonly ITransactionManager _transactions;
        private readonly IRecipeParser _recipeParser;

        public RecipePartDriver(
            IRecipeParser recipeParser,
            ITransactionManager transactions) {
            
            _recipeParser = recipeParser;
            _transactions = transactions;
            T = NullLocalizer.Instance;
        }

        Localizer T { get; set; }

        protected override DriverResult Display(RecipePart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_Recipe_Execute_SummaryAdmin",
                             () => shapeHelper.Parts_Recipe_Execute_SummaryAdmin()));
        }

        protected override DriverResult Editor(RecipePart part, dynamic shapeHelper)
        {
            var viewModel = new RecipePartViewModel {
                RecipeText = part.RecipeText
            };

            return ContentShape("Parts_Recipe_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Recipe", Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(RecipePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new RecipePartViewModel {
                RecipeText = part.RecipeText
            };

            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                try {
                    var recipe = _recipeParser.ParseRecipe(viewModel.RecipeText);
                    part.RecipeText = viewModel.RecipeText;
                }
                catch (Exception ex) {
                    updater.AddModelError("", T("Recipe processing error: {0}", ex.Message));
                }
            }

            return ContentShape("Parts_Recipe_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Recipe", Model: viewModel, Prefix: Prefix));
        }

        protected override void Exporting(RecipePart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).Add(new XCData(part.RecipeText));
        }

        protected override void Importing(RecipePart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            var recipeElement = context.Data.Element(part.PartDefinition.Name);

            if (recipeElement != null)
                part.RecipeText = recipeElement.Value;
        }
    }
}