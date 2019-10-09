using System.Linq;
using MediatR;

namespace Screenshots.Commands
{
    public class TakeScreenshotsCommand : IRequest<TakeScreenshotsResult>
    {
        public string[] Urls { get; set; }

        public bool IsValid()
        {
            return Urls is null || Urls.Any();
        }
    }
}
