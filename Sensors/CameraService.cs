using System.Threading.Tasks;
using Unosquare.RaspberryIO;

namespace Apex.RaspberryPiServer.Sensors
{
    public class CameraService : ICameraService
    {
        public Task<byte[]> GetImage()
        {
            return Pi.Camera.CaptureImageJpegAsync(640, 480);
        }
    }
}