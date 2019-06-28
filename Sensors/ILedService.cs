using System.Threading.Tasks;

namespace Apex.RaspberryPiServer.Sensors
{
    public interface ILedService
    {
        Task LedOn(int pin);
        Task LedOff(int pin);
        Task LedFlash(int pin, int delay);
        void LedDim(int pin, int delay);
    }
}