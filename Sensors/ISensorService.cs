using System.Threading.Tasks;

namespace Apex.RaspberryPiServer.Sensors
{
    public interface ISensorService
    {
        Task<bool> HasInfrared(int id);
        Task<double> ReadDistance();
        Task<double> ReadInfrared();
        Task<double> ReadLuminosity();
        Task<double> ReadTemperature();
    }
}