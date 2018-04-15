using Application;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Repr = Application.Representation;

namespace UserInterface.Controllers
{
    public class ReverseAuctionController : ApiController
    {
        private readonly ReverseAuction.Create _create;

        public ReverseAuctionController(
            ReverseAuction.Create create)
        {
            _create = create;
        }

        public Repr.PaginatedSequence<Repr.ReverseAuction> Get()
        {
            throw new NotImplementedException();
        }

        public Repr.ReverseAuction Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(Repr.ReverseAuction request)
        {
            Repr.ReverseAuction response = _create.From(request);

            return Created(response.Uri, response);
        }
    }
}
