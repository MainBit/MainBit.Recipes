using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.Recipes {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("RecipePart", part => part
                .Attachable()
                .WithDescription("Turns a type into a recipe provider."));

            ContentDefinitionManager.AlterTypeDefinition("Recipe", type => type
                .WithPart("CommonPart",
                    p => p
                        .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false")
                        .WithSetting("DateEditorSettings.ShowDateEditor", "false"))
                .WithPart("IdentityPart")
                .WithPart("TitlePart")
                .WithPart("RecipePart"));

            return 2;
        }
    }
}