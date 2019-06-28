using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Apex.RaspberryPiServer.Sensors
{
    public class LedService : ILedService
    {
        readonly RaspberryPiSettings settings;

        public LedService(IOptions<RaspberryPiSettings> raspberryPiSettings)
        {
            settings = raspberryPiSettings.Value;
        }

        public async Task LedOn(int pin)
        {
            var led = Pi.Gpio[pin];
            led.PinMode = GpioPinDriveMode.Output;
            await led.WriteAsync(GpioPinValue.High);
        }

        public async Task LedOff(int pin)
        {
            var led = Pi.Gpio[pin];
            led.PinMode = GpioPinDriveMode.Output;
            await led.WriteAsync(GpioPinValue.Low);
        }

        public void LedDim(int pin, int delay)
        {
            var led = Pi.Gpio[pin];
            led.PinMode = GpioPinDriveMode.PwmOutput;
            led.PwmMode = PwmMode.Balanced;
            led.PwmClockDivisor = 2;

            for (var i = 0; i <= settings.LedDimmingSteps; i++)
            {
                led.PwmRegister = (int)led.PwmRange / settings.LedDimmingSteps * i;
                Thread.Sleep(delay);
            }

            for (var i = 0; i <= settings.LedDimmingSteps; i++)
            {
                led.PwmRegister = (int)led.PwmRange - ((int)led.PwmRange / settings.LedDimmingSteps * i);
                Thread.Sleep(delay);
            }
        }

        public async Task LedFlash(int pin, int delay)
        {
            var led = Pi.Gpio[pin];
            led.PinMode = GpioPinDriveMode.Output;
            await led.WriteAsync(GpioPinValue.High);
            Thread.Sleep(delay);
            await led.WriteAsync(GpioPinValue.Low);
        }
    }
}