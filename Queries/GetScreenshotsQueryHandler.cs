using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Screenshots.Queries
{
    public class GetScreenshotsQueryHandler : IRequestHandler<GetScreenshotQuery, GetScreenshotResult>
    {
        private readonly ScreenshotsAsAServiceContext _dbContext;
 
        public GetScreenshotsQueryHandler(ScreenshotsAsAServiceContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetScreenshotResult> Handle(
            GetScreenshotQuery query, CancellationToken cancellationToken)
        {
            var screenshot = await _dbContext.Screenshots
                .SingleOrDefaultAsync(x => x.Url == query.Url);
            if (screenshot is null)
            {
                return null;
            }
            return CreateQueryResult(screenshot);
        }

        private static GetScreenshotResult CreateQueryResult(Screenshot screenshot)
        {
            return new GetScreenshotResult
            {
                Bytes = screenshot.Bytes,
                MimeType = "image/jpeg",
            };
        }
    }
}
