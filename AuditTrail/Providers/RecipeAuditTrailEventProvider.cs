using System.Globalization;
using System.Linq;
using Orchard.AuditTrail.Helpers;
using Orchard.AuditTrail.Services;
using Orchard.AuditTrail.Services.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace MainBit.Recipes.AuditTrail.Providers {
    [OrchardFeature("MainBit.Recipes.AuditTrail")]
    public class RecipeAuditTrailEventProvider : AuditTrailEventProviderBase {
        private readonly IContentManager _contentManager;
        public RecipeAuditTrailEventProvider(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public const string Executed = "Executed";

        public static Filters CreateFilters(int contentId, IUpdateModel updateModel) {
            return new Filters(updateModel) {
                {"content", contentId.ToString(CultureInfo.InvariantCulture)}
            };
        }

        public override void Describe(DescribeContext context) {
            context.For("Recipe", T("Recipes"))
                .Event(this, Executed, T("Executed"), T("A content item of recipe type was executed."), enableByDefault: true);

            context.QueryFilter(QueryFilter);
            context.DisplayFilter(DisplayFilter);
        }

        private void QueryFilter(QueryFilterContext context) {
            if (!context.Filters.ContainsKey("content"))
                return;

            var contentId = context.Filters["content"].ToInt32();
            context.Query = context.Query.Where(x => x.EventFilterKey == "content" && x.EventFilterData == contentId.ToString());
        }

        private void DisplayFilter(DisplayFilterContext context) {
            var contentItemId = context.Filters.Get("content").ToInt32();
            if (contentItemId != null) {
                var contentItem = _contentManager.Get(contentItemId.Value, VersionOptions.AllVersions);
                var filterDisplay = context.ShapeFactory.AuditTrailFilter__ContentItem(ContentItem: contentItem);

                context.FilterDisplay.Add(filterDisplay);
            }
        }
    }
}