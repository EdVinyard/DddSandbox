using System.Web.Http;
using System.Web.Http.Dispatcher;
using UserInterface.App_Start;
using WebApi.StructureMap;

namespace UserInterface
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Based on a combination of
            //   - http://structuremap.github.io/integrations/ which recommends
            //     https://github.com/mikeobrien/WebApi.StructureMap, and
            //   - Jacob Calder's https://github.com/jcalder/WebApiExample,
            // use StructureMap as the WebAPI DependencyResolver.
            GlobalConfiguration.Configuration.UseStructureMap(c =>
            {
                c.AddRegistry<UserInterface.DependencyRegistry>();
                c.AddRegistry<Application.DependencyRegistry>();
                c.AddRegistry<Persistence.DependencyRegistry>();

                // I'd like to avoid scanning, or even *registering* Domain
                // Services.  Right now, Domain Services are neither separated
                // into interface and implementation, nor pure-virtual classes
                // that could be sub-classed automatically (e.g. by a 
                // mocking/stubbing/faking tool) or manually.  This could 
                // present challenges when testing elaborate or complex business
                // processes implemented within the Domain.
                //
                // My ideal is that the Domain is tested as a whole, only mocking
                // dependencies that must be satisfied *outside* the Domain itself.
                // This is in the neighborhood of "TDD, where did it all go wrong"
                // (https://vimeo.com/68375232) and "Is TDD dead?" 
                // (https://youtu.be/z9quxZsLcfo).
                //
                //c.Scan(a =>
                //{
                //    // KLUDGE: I *hate* scanning, but I have only a few 
                //    // options available:
                //    //   (1) scan -- MAGIC!
                //    //   (2) add StructureMap reference to Domain -- DIRTY!
                //    //   (3) evolve the Domain.IDependencies interface to
                //    //       allow Domain to explicitly register dependencies
                //    //       but remain ignorant of StructureMap -- I think
                //    //       this is The Right Way To Do It, but it's more
                //    //       work than I want to embark on at this moment.
                //    a.AssemblyContainingType<Domain.AssemblyMarker>();
                //    a.WithDefaultConventions();
                //});
            });

            UseNHibernateSessionPerRequestControllerActivator();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private static void UseNHibernateSessionPerRequestControllerActivator()
        {
            // KLUDGE: The preceding extension method, .UseStructureMap(), 
            // configured a ControllerActivator, but I don't see a way to 
            // hook into it to add a new NHibernate Session to the per-
            // request Nested StructureMap Container before the Controller 
            // is constructed.  Instead, replace StructureMapControllerActivator
            // with our own NHibernateSessionPerRequestControllerActivator
            // after that method concludes, because the other setup it
            // performs (e.g., setting up StructureMap as the WebAPI 
            // DependencyResolver) is still desirable.

            // This doesn't seem to do anything at all!
            //
            // GlobalConfiguration.Configuration.Services.Replace(
            //    typeof(IHttpControllerActivator), 
            //    new NHibernateSessionPerRequestControllerActivator());
            //
            // so instead get access to the global Container...
            var globalContainer = GlobalConfiguration.Configuration.DependencyResolver
                .GetService<StructureMap.IContainer>();
            
            //var before = globalContainer.WhatDoIHave(typeof(IHttpControllerActivator));

            // ...ditch the ControllerActivator provided by the 
            // WebApi.StructureMap library...
            globalContainer.EjectAllInstancesOf<IHttpControllerActivator>();
            // ...and use one that sets up a separate NHibernate Session for
            // each request.
            globalContainer.Configure(c =>
            {
                c.For<IHttpControllerActivator>()
                    .Use<NHibernateSessionPerRequestControllerActivator>()
                    .Singleton();
            });

            //var after = globalContainer.WhatDoIHave(typeof(IHttpControllerActivator));
        }
    }
}
