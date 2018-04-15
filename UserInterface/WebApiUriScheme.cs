using Application;
using Domain.Aggregate.Auction;
using System;
using System.Web.Http.Routing;

namespace UserInterface
{
    public class WebApiUriScheme : IUriScheme
    {
        private readonly UrlHelper _urlHelper;

        public WebApiUriScheme(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public Uri ToUri(ReverseAuctionAggregate reverseAuction)
        {
            // TODO: Test this.  It's tough to debug because of the anonymous object.
            return new Uri(_urlHelper.Link("ReverseAuction", new { reverseAuction.Id }));
        }
    }
}
