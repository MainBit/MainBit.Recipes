using System.Collections.Generic;
using Orchard.AuditTrail.Services;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Events;
using Orchard.AuditTrail.Providers.Users;

namespace MainBit.Recipes.Handlers
{

    public interface IRecipeEventHandler : IEventHandler
    {
        void Executed(RecipeExecuteContentContext context);
    }
}