using GowBoard.Models.Service;
using GowBoard.Models.Service.Interface;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace GowBoard
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            container.RegisterType<IMemberService, MemberService>();
            container.RegisterType<IBoardService, BoardService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<ICommentService, CommentService>();

            // Set resolver for MVC
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}