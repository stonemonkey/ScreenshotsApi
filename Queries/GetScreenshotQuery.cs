using MediatR;

namespace Screenshots.Queries
{
    public class GetScreenshotQuery : IRequest<GetScreenshotResult>
    {
        public string Url { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Url);
        }
    }
}
