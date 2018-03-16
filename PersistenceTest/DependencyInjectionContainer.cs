﻿using StructureMap;
using StructureMap.Pipeline;
using Persistence;
using System;

namespace PersistenceTest
{
    // TODO: Delete this whole file?  We don't seem to need it.

    public class DependencyInjectionContainer
    {
        private IContainer _instance;
        public IContainer Instance
        {
            get
            {
                if (null == _instance)
                {
                    throw new InvalidOperationException("You must invoke " +
                        "DependencyInjectionCotnainer.Setup() before .Instance.");
                }

                return _instance;
            }
            private set { _instance = value; }
        }

        public void SetUp(Action<ConfigurationExpression> configure = null)
        {
            Instance = new Container(x =>
            {
                x.For<NHibernate.ISessionFactory>()
                    .Use(ctx => Config.Database.BuildSessionFactory())
                    .Singleton();
                configure?.Invoke(x);
            });

            System.Console.WriteLine(Instance.WhatDoIHave());
        }
    }
}
