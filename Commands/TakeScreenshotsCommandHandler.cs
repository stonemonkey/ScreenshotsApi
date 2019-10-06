using System.Threading;
using System.Threading.Tasks;
using CoreHtmlToImage;
using MediatR;

namespace Screenshots.Commands
{
    public class TakeScreenshotsCommandHandler : AsyncRequestHandler<TakeScreenshotsCommand>
    {
        private readonly ScreenshotsAsAServiceContext _dbContext;
        private readonly HtmlConverter _converter;
 
        public TakeScreenshotsCommandHandler(
            ScreenshotsAsAServiceContext dbContext, HtmlConverter converter)
        {
            _dbContext = dbContext;
            _converter = converter;
        }

        protected override async Task Handle(
            TakeScreenshotsCommand command, CancellationToken cancellationToken) 
        {
            foreach(var url in command.Urls)
            {
                var screenshot = GetNewScreenshot(url);
                if (screenshot != null)
                {
                    await InsertOrUpdateAsync(screenshot);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        private Screenshot GetNewScreenshot(string url)
        {
            byte[] bytes;
            try 
            {
                bytes = _converter.FromUrl(url, 1920, ImageFormat.Jpg);
            }
            catch 
            {
                return null;
            }
            return new Screenshot { Url = url, Bytes = bytes };
        }

        private async Task InsertOrUpdateAsync(Screenshot screenshot)
        {
            var existingScreenshot = await _dbContext.Screenshots.FindAsync(screenshot.Url);
            if (existingScreenshot == null)
            {
                await _dbContext.AddAsync(screenshot);
            }
            else
            {
               _dbContext.Entry(existingScreenshot).CurrentValues.SetValues(screenshot);
            }
        }
    }
}
