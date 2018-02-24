using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;

namespace Persistence
{
    public class Config
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

        private static readonly ConnectionStringSettings MsSqlDb =
            ConfigurationManager.ConnectionStrings["DddExample"];

        internal static string MsSqlDbConnectionString => MsSqlDb.ConnectionString;

        protected static FluentConfiguration MsSqlDatabase
        {
            get
            {
                var msSqlConfiguration = MsSqlConfiguration
                    .MsSql2012
                    .ConnectionString(MsSqlDbConnectionString);

                if (ShowSql)
                {
                    msSqlConfiguration
                        .ShowSql()
                        .FormatSql();
                }

                return Fluently.Configure()
                    .Database(msSqlConfiguration)
                    .Mappings(m => m.FluentMappings
                        .AddFromAssemblyOf<NHibernateMappingMarker>())
                    .ExposeConfiguration(c => new SchemaExport(c).Create(
                        useStdOut: false, 
                        execute: true));
            }
        }
    }
}
