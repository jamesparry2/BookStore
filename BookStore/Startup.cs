using Microsoft.Owin;
using Owin;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(BookStore.Startup))]
namespace BookStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            create();
        }

        public void create()
        {

            BookStore.Models.ApplicationDbContext context = new Models.ApplicationDbContext();

            var roleManger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<Models.ApplicationUser>(new UserStore<Models.ApplicationUser>(context));

            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            role.Name = "Admin";
            roleManger.Create(role);

            var user = new Models.ApplicationUser();
            user.Email = "testjames@email.com";
            user.UserName = user.Email;

            string userPassword = "Pass@Word1";

            var chkUser = UserManager.Create(user, userPassword);

            if (chkUser.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
