using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Tsg.UI.Main.BlockchainIntegration;
using TSG.ServiceLayer;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {

            var config = GlobalConfiguration.Configuration;
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(config);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));


            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();


            Injector.Bootstrap(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //ModelMetadataProviders.Current = new HtmlExtensions.IgnoreValidationModelMetaDataProvider();

            AutoMapperInitializator.InitMap();
            GlobalConfiguration.Configuration.Filters.Add(new LogExceptionFilterAttribute());

        }

        //Create filter
        public class LogExceptionFilterAttribute : ExceptionFilterAttribute
        {
            public override void OnException(HttpActionExecutedContext context)
            {
                _logger.Error(context.Exception);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            _logger.Error(ex);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            string path = app.Context.Request.Url.PathAndQuery;
            int pos = path.IndexOf("%20%3C", StringComparison.Ordinal);
            if (pos > -1)
            {
                path = path.Substring(0, pos);
                app.Context.RewritePath(path);
            }
            _logger.Debug($"Query is: Type [{app.Context.Request.RequestType}] - {app.Context.Request.Url.AbsoluteUri}");
        }
        //public override void Init()
        //{
        //    base.AuthenticateRequest += OnAuthenticateRequest;
        //}

        //private void OnAuthenticateRequest(object sender, EventArgs eventArgs)
        //{
        //    if (HttpContext.Current.User!=null)
        //    {
        //        if (HttpContext.Current.User.Identity.IsAuthenticated)
        //        {
        //            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        //            var decodedTicket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
        //            //var decodedTicket = FormsAuthentication.Decrypt(cookie.Value);
        //            var roles = decodedTicket.UserData.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        //            var principal = new GenericPrincipal(HttpContext.Current.User.Identity, roles);
        //            HttpContext.Current.User = principal;
        //        }
        //    }
        //}
    }
}