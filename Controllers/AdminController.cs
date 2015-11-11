using MainBit.MultiTenancy.Services;
using MainBit.Recipes.Handlers;
using MainBit.Recipes.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MultiTenancy.Services;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MainBit.Recipes.Controllers
{
    [Admin]
    public class AdminController : Controller {
        private readonly Orchard.Recipes.Services.IRecipeManager _recipeManager;
        private readonly ITenantService _tenantService;
        private readonly ITenantWorkContextAccessor _tenantWorkContextAccessor;
        private readonly MainBit.Recipes.Services.IRecipeManager _mainBitRecipeManager;
        private readonly IRecipeEventHandler _recipeEventHandler;

        public AdminController(
            IOrchardServices services,
            Orchard.Recipes.Services.IRecipeManager recipeManager,
            ITenantService tenantService,
            ITenantWorkContextAccessor tenantWorkContextAccessor,
            MainBit.Recipes.Services.IRecipeManager mainBitRecipeManager,
            IRecipeEventHandler recipeEventHandler)
        {
            Services = services;
            _tenantService = tenantService;
            _tenantWorkContextAccessor = tenantWorkContextAccessor;
            _mainBitRecipeManager = mainBitRecipeManager;
            _recipeEventHandler = recipeEventHandler;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Execute(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to execute recipes")))
                return new HttpUnauthorizedResult();

            var recipe = _mainBitRecipeManager.Get(id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            var viewModel = new RecipeExecuteViewModel
            {
                Id = id,
                Title = recipe.As<ITitleAspect>().Title,
                TenantSettings = _tenantService.GetTenants()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Execute")]
        public ActionResult ExecutePOST(int id, string moduleId, string name, IEnumerable<string> tenantNames)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to execute recipes")))
                return new HttpUnauthorizedResult();

            var recipe = _mainBitRecipeManager.Get(id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            foreach (var tenantName in tenantNames)
            {
                var tenantWorkContext = _tenantWorkContextAccessor.GetContext(tenantName);
                var tenantRecipeManager = tenantWorkContext.Resolve<Orchard.Recipes.Services.IRecipeManager>();

                try
                {
                    tenantRecipeManager.Execute(recipe.Recipe);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error while executing recipe {0} in {1} - in tenant {2}", moduleId, name, tenantName);
                    Services.Notifier.Error(T("Recipes contains {0} unsupported module installation steps.", recipe.Recipe.Name));
                }
            }

            var recipeExecuteContentContext = new RecipeExecuteContentContext(recipe.ContentItem, tenantNames);
            _recipeEventHandler.Executed(recipeExecuteContentContext);

            Services.Notifier.Information(T("The recipe {0} was executed successfully.", recipe.Recipe.Name));

            return RedirectToAction("Execute", new { id, moduleId, name });
        }
    }
}