using System.Threading.Tasks;

namespace Apex.RaspberryPiServer.Sensors
{
    public interface ICameraService
    {
        Task<byte[]> GetImage();
    }
}