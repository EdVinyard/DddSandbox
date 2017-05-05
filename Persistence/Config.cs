using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Configuration;

namespace Persistence
{
    class Config
    {
        public static FluentConfiguration Database { get { return MsSqlDatabase; } }

        public static bool ShowSql
        {
            get
            {
                bool showSql;
                var appSetting = ConfigurationManager.AppSettings["NHibernate.ShowSql"];
                return (null != appSetting)
                    && bool.TryParse(appSetting, out showSql)
                    && showSql;
            }
        }

        private static readonly ConnectionStringSettings MsSqlDbConnectionString =
            ConfigurationManager.ConnectionStrings["UnitOfWorkTest"];

        protected static FluentConfiguration MsSqlDatabase
        {
            get
            {
                var msSqlConfiguration = MsSqlConfiguration
                    .MsSql2012
                    .ConnectionString(MsSqlDbConnectionString.ConnectionString);

                if (ShowSql)
                {
                    msSqlConfiguration
                        .ShowSql()
                        .FormatSql();
                }

                return Fluently.Configure()
                    .Database(msSqlConfiguration)
                    .Mappings(m => m.FluentMappings
                        .AddFromAssemblyOf<RouteMap>());
            }
        }
    }
}
