using NHibernate;
using StructureMap;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using WebApi.StructureMap;

namespace UserInterface.App_Start
{
    public class NHibernateSessionPerRequestControllerActivator : 
        IHttpControllerActivator
    {
        private readonly DefaultHttpControllerActivator _defaultActivator;

        public NHibernateSessionPerRequestControllerActivator()
        {
            _defaultActivator = new DefaultHttpControllerActivator();
        }

        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            request.GetDependencyScope().GetService<IContainer>().Configure(x =>
            {
                // copied from https://github.com/mikeobrien/WebApi.StructureMap/blob/master/src/WebApi.StructureMap/HttpControllerActivatorProxy.cs
                x.For<HttpRequestMessage>().Use(request);
                x.For<HttpControllerDescriptor>().Use(controllerDescriptor);
                x.For<HttpRequestContext>().Use(request.GetRequestContext());
                x.For<IHttpRouteData>().Use(request.GetRouteData());

                x.For<ISession>().Use(c => c.GetInstance<ISessionFactory>().OpenSession());
                // RULE: Transactions are managed by Application Services, so
                // we don't need to start a transaction here and resolve it
                // at the "end" of the request.
            });

            return _defaultActivator.Create(
                request, 
                controllerDescriptor, 
                controllerType);
        }
    }
}
