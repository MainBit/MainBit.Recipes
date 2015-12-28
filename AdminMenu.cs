using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace MainBit.Recipes {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                //.AddImageSet("templates")
                .Add(T("Recipes"), "5.0", item => item.Action("List", "Admin", new { id = "Recipe", area = "Contents"})
                    .Permission(StandardPermissions.SiteOwner));
        }
    }
}
