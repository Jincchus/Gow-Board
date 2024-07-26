using GowBoard.Controllers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GowBoard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Index");

            if (httpException != null)
            {
                routeData.Values.Add("statusCode", httpException.GetHttpCode());
            }
            else
            {
                routeData.Values.Add("statusCode", 500);
            }

            // ���� �α��� ���⿡ �߰��� �� �ֽ��ϴ�.
            // Logger.Log(exception);

            Server.ClearError();

            IController errorController = new ErrorController();
            var requestContext = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorController.Execute(requestContext);
        }
    }
}
