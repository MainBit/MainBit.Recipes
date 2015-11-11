﻿using System.Collections.Generic;
using System.Linq;
using Orchard.AuditTrail.Helpers;
using Orchard.AuditTrail.Models;
using Orchard.AuditTrail.Services;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Environment.Extensions;

namespace MainBit.Recipes.AuditTrail.Providers
{
    [OrchardFeature("MainBit.Recipes.AuditTrail")]
    public class RecipeAuditTrailEventShapes : IShapeTableProvider {
        private readonly Work<IContentManager> _contentManager;
        private readonly IDiffGramAnalyzer _analyzer;

        public RecipeAuditTrailEventShapes(Work<IContentManager> contentManager, IDiffGramAnalyzer analyzer)
        {
            _contentManager = contentManager;
            _analyzer = analyzer;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("AuditTrailEvent").OnDisplaying(context => {
                var record = (AuditTrailEventRecord)context.Shape.Record;

                if (record.Category != "Recipe")
                    return;

                var eventData = (IDictionary<string, object>)context.Shape.EventData;
                var contentItemId = eventData.Get<int>("ContentId");
                var previousContentItemVersionId = eventData.Get<int>("PreviousVersionId");
                var previousVersionXml = eventData.GetXml("PreviousVersionXml");
                var diffGram = eventData.GetXml("DiffGram");
                var contentItem = _contentManager.Value.Get(contentItemId, VersionOptions.AllVersions);
                var previousVersion = previousContentItemVersionId > 0 ? _contentManager.Value.Get(contentItemId, VersionOptions.VersionRecord(previousContentItemVersionId)) : default(ContentItem);

                if (diffGram != null) {
                    var diffNodes = _analyzer.Analyze(previousVersionXml, diffGram).ToArray();
                    context.Shape.DiffNodes = diffNodes;
                }

                context.Shape.ContentItemId = contentItemId;
                context.Shape.ContentItem = contentItem;
                context.Shape.PreviousVersion = previousVersion;
                context.Shape.Tenants = eventData.Get<string>("Tenants").Split(new char[] { ',' });
            });

            builder.Describe("AuditTrailEventActions").OnDisplaying(context => {
                var record = (AuditTrailEventRecord)context.Shape.Record;

                if (record.Category != "Recipe")
                    return;

                var eventData = (IDictionary<string, object>)context.Shape.EventData;
                var contentItemId = eventData.Get<int>("ContentId");
                var previousContentItemVersionId = eventData.Get<int>("PreviousVersionId");
                var contentItem = _contentManager.Value.Get(contentItemId, VersionOptions.AllVersions);
                var previousVersion = previousContentItemVersionId > 0 ? _contentManager.Value.Get(contentItemId, VersionOptions.VersionRecord(previousContentItemVersionId)) : default(ContentItem);

                context.Shape.ContentItemId = contentItemId;
                context.Shape.ContentItem = contentItem;
                context.Shape.PreviousVersion = previousVersion;
            });
        }
    }
}