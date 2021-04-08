namespace Blog.Test.Services
{
    using System.Threading.Tasks;
    using Blog.Services.Images;
    using Blog.Services.Web;
    using FakeItEasy;
    using Fakes;
    using Xunit;

    public class ImageServiceTest
    {
        [Fact]
        public void CalculateOptimalSizeShouldReturnMinimumSizeWhenSizeIsLessThanTheAllowedMinimum()
        {
            // Arrange
            const int minimumSize = 100;
            const int originalSize = 200;
            const int resizeSize = 50;

            var imageService = new ImageService(null, null);

            // Act
            var (width, height) = imageService
                .CalculateOptimalSize(resizeSize, resizeSize, originalSize, originalSize);

            // Assert
            Assert.Equal(minimumSize, width);
            Assert.Equal(minimumSize, height);
        }

        [Fact]
        public async Task UpdateImageShouldDownloadImageAndResizeItToCorrectDestination()
        {
            // Arrange
            const string imageUrl = "TestImageUrl";
            const string destination = "TestDestination";

            const int size = 200;

            bool FileDownloaded=false;
            bool ImageResized = false;

            string DownloadDestination=null;
            string ImageSource = null;
            string ImageDestination = null;



            //var webClientService = new FakeWebClientService();
            var webClientService = A.Fake<IWebClientService>();

            A.CallTo(() => webClientService
            .DownloadFile(A<string>.Ignored, A<string>.Ignored))
            .Invokes((string imageUrl, string dDestination) =>
            {
                FileDownloaded = true;
                DownloadDestination = dDestination;                
            });

            var imageProcessorService = A.Fake<IImageProcessorService>();


            A.CallTo(() => imageProcessorService.Resize(A<string>.Ignored, A<string>.Ignored, A<int>.Ignored, A<int>.Ignored))
                .Invokes((string imageUrl, string destination, int w, int h) =>
                {
                    ImageResized = true;
                    ImageSource = imageUrl;
                    ImageDestination = destination;
                });
            
            var imageService = new ImageService(webClientService, imageProcessorService);

            // Act
            await imageService.UpdateImage(imageUrl, destination, size, size);

            // Assert
            var imageDestination = $"{destination}.jpg";

            Assert.True(FileDownloaded);
            Assert.Equal($"{destination}.jpg", DownloadDestination);
            Assert.True(ImageResized);
            Assert.Equal(imageDestination, ImageSource);
            Assert.Equal($"{destination}_optimized.jpg",ImageDestination);
        }
    }
}
