namespace Persistence
{
    public class DependencyRegistry : StructureMap.Registry
    {
        public DependencyRegistry() : base()
        {
            For<NHibernate.ISessionFactory>()
                .Use(ctx => Persistence.Config.Database.BuildSessionFactory())
                .Singleton();

            For<Domain.Aggregate.Auction.IReverseAuctionRepository>()
                .Use<ReverseAuctionRepository>();
        }
    }
}
