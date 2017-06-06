using StructureMap;
using StructureMap.Pipeline;
using Persistence;
using System;

namespace PersistenceTest
{
    public static class DependencyInjectionContainer
    {
        private static IContainer _instance;
        public static IContainer Instance
        {
            get
            {
                if (null == _instance)
                {
                    throw new ApplicationException("You must invoke " +
                        "DependencyInjectionCotnainer.Setup() before .Instance.");
                }

                return _instance;
            }
            private set { _instance = value; }
        }

        public static void SetUp()
        {
            Instance = new Container(x =>
            {
                x.For<NHibernate.ISessionFactory>()
                    .Use(ctx => Config.Database.BuildSessionFactory())
                    .Singleton();
                x.For<NHibernate.ISession>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(ctx => ctx.GetInstance<NHibernate.ISessionFactory>().OpenSession());
            });

            System.Console.WriteLine(Instance.WhatDoIHave());
        }
    }
}
