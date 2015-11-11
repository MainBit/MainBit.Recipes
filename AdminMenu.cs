using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.Modules {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .Add(T("Modules"), menu => menu
                    .Add(T("MainBit Recipes"), "3", item => item.Action("List", "Admin", new { area = "MainBit.Recipes" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                    .Add(T("Database recipes"), "4", item => item.Action("List", "Admin", new { id = "Recipe", area = "Contents"}).Permission(StandardPermissions.SiteOwner).LocalNav())
                    );
        }
    }
}
