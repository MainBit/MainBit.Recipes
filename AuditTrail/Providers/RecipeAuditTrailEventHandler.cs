using MainBit.Recipes.Handlers;
using MainBit.Recipes.Services;
using Orchard;
using Orchard.AuditTrail.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MainBit.Recipes.AuditTrail.Providers
{
    [OrchardFeature("MainBit.Recipes.AuditTrail")]
    public class TenantRecipeEventHandler : IRecipeEventHandler {
        private readonly IAuditTrailManager _auditTrailManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IContentManager _contentManager;


        public TenantRecipeEventHandler(IAuditTrailManager auditTrailManager, IWorkContextAccessor wca, IContentManager contentManager)
        {
            _auditTrailManager = auditTrailManager;
            _wca = wca;
            _contentManager = contentManager;
        }

        public void Executed(RecipeExecuteContentContext context)
        {
            var properties = new Dictionary<string, object> {
                {"Content", context.ContentItem as IContent}
            };

            var metaData = _contentManager.GetItemMetadata(context.ContentItem);
            var eventData = new Dictionary<string, object> {
                {"ContentId", context.ContentItem.Id},
                {"ContentIdentity", metaData.Identity.ToString()},
                {"ContentType", context.ContentItem.ContentType},
                {"VersionId", context.ContentItem.Version},
                {"VersionNumber", context.ContentItem.VersionRecord != null ? context.ContentItem.VersionRecord.Number : 0},
                {"Published", context.ContentItem.VersionRecord != null && context.ContentItem.VersionRecord.Published},
                {"Title", metaData.DisplayText},
                {"Tenants", string.Join(",", context.TenantNames.ToArray())}
            };

            _auditTrailManager.CreateRecord<RecipeAuditTrailEventProvider>(
                RecipeAuditTrailEventProvider.Executed,
                _wca.GetContext().CurrentUser,
                properties,
                eventData,
                eventFilterKey: "content",
                eventFilterData: context.ContentItem.Id.ToString(CultureInfo.InvariantCulture));
        }

    }
}