using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Apex.RaspberryPiServer.Helpers;
using Microsoft.Extensions.Options;

namespace Apex.RaspberryPiServer.Sensors
{
    public class SensorService : ISensorService
    {
        readonly RaspberryPiSettings settings;

        public SensorService(IOptions<RaspberryPiSettings> raspberryPiSettings)
        {
            settings = raspberryPiSettings.Value;
        }

        public async Task<double> ReadDistance()
        {
            double distance;
            Stopwatch timeWatcher = new Stopwatch();

            var trigger = Pi.Gpio[settings.ProximityTriggerPin];
            trigger.PinMode = GpioPinDriveMode.Output;
            var echo = Pi.Gpio[settings.ProximityEchoPin];
            echo.PinMode = GpioPinDriveMode.Input;
            await trigger.WriteAsync(GpioPinValue.Low);

            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            timeWatcher.Reset();

            //Send pulse
            trigger.Write(GpioPinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            trigger.Write(GpioPinValue.Low);
            var t = Task.Run(() =>
            {
                //Receive pulse
                while (echo.ReadValue() != GpioPinValue.High) { }
                timeWatcher.Start();

                while (echo.ReadValue() == GpioPinValue.High) { }
                timeWatcher.Stop();

                //Calculating distance
                distance = timeWatcher.Elapsed.TotalSeconds * 17000;
                return Math.Round(distance, 2);
            });

            bool didComplete = t.Wait(TimeSpan.FromMilliseconds(100));
            if (didComplete)
            {
                return Math.Round(t.Result, 2);
            }
            else
            {
                return settings.ProximityMaxDistance; // if no response, assumes the distance is MAX_DISTANCE             
            }
        }

        public async Task<double> ReadTemperature()
        {
            #region bits explained
            // To line everything up for ease of reading back (on byte boundary) we 
            // will pad the command start bit with 7 leading "0" bits

            // Write 0000000S GDDDxxxx xxxxxxxx
            // Read  ???????? ?????N98 76543210
            // S = start bit
            // G = Single / Differential
            // D = Chanel data 
            // ? = undefined, ignore
            // N = 0 "Null bit"
            // 9-0 = 10 data bits

            // 0000 01 = 7 pad bits, start bit
            // 1000 0000 = single ended, channel bit 2, channel bit 1, channel bit 0, 4 clocking bits
            // 0000 0000 = 8 clocking bits
            #endregion

            var writeBuffer = new byte[3] { 0b00000001, 0b10000000, 0b00000000 };   /* we will hold the command to send to the chipbuild this in the constructor for the chip we are using */
            var readBuffer = new byte[3] { 0b00000000, 0b00000000, 0b00000000 };    /* this is defined to hold the output data*/

            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
            readBuffer = await Pi.Spi.Channel0.SendReceiveAsync(writeBuffer);

            return SensorHelper.ToTemperature(readBuffer);
        }

        public async Task<double> ReadLuminosity()
        {
            var writeBuffer = new byte[3] { 0b00000001, 0b10010000, 0b00000000 };
            var readBuffer = new byte[3] { 0b00000000, 0b00000000, 0b00000000 };

            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
            readBuffer = await Pi.Spi.Channel0.SendReceiveAsync(writeBuffer);

            return SensorHelper.ToLuminosity(readBuffer);
        }

        public async Task<double> ReadInfrared()
        {
            var writeBuffer = new byte[3] { 0b00000001, 0b10100000, 0b00000000 };
            var readBuffer = new byte[3] { 0b00000000, 0b00000000, 0b00000000 };

            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
            readBuffer = await Pi.Spi.Channel0.SendReceiveAsync(writeBuffer);

            return SensorHelper.ToInfrared(readBuffer);
        }

        public async Task<bool> HasInfrared(int id)
        {
            GpioPin pin;
            pin = Pi.Gpio[id];
            pin.PinMode = GpioPinDriveMode.Input;
            var val = await pin.ReadValueAsync();
            return val == GpioPinValue.High;
        }
    }
}