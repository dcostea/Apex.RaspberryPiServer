using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using System.Threading.Tasks;
using System;
using Apex.RaspberryPiServer.Models;
using Apex.RaspberryPiServer.Sensors;
using Apex.RaspberryPiServer.Enums;
using Microsoft.Extensions.Options;
using Apex.RaspberryPiServer.Helpers;

namespace Apex.RaspberryPiServer.Hubs
{
    public class SensorHub : Hub
    {
        private readonly SensorsDBContext context;
        private readonly RaspberryPiSettings settings;
        private readonly ISensorService sensor;
        private readonly ILedService led;

        public static Source Source { get; private set; }
        private static bool isStreaming;

        public SensorHub(SensorsDBContext sensorsDBContext, IOptions<RaspberryPiSettings> raspberryPiSettings, ISensorService sensorService, ILedService ledService)
        {
            context = sensorsDBContext;
            settings = raspberryPiSettings.Value;
            sensor = sensorService;
            led = ledService;
        }

        public async Task StartStreaming()
        {
            ConsoleHelper.WriteHeader("Streaming started.");
            isStreaming = true;
            await led.LedOn(settings.BlueLedPin);
            await Clients.All.SendAsync("streamingStarted", Source.ToString());            
        }

        public async Task StopStreaming()
        {
            ConsoleHelper.WriteHeader("Streaming stopped.");
            isStreaming = false;
            await led.LedOff(settings.BlueLedPin);
            await Clients.All.SendAsync("streamingStopped");
        }

        public async Task ChangeSource(string source)
        {
            Source = Enum.Parse<Source>(source);

            //switch (Source)
            //{
            //    case Source.StandBy:
            //        break;
            //    case Source.Lighter:
            //        break;
            //    case Source.FlashLight:
            //        break;
            //    case Source.Infrared:
            //        break;
            //    default:
            //        break;
            //}

            ConsoleHelper.WriteHeader($"{Source} Reading at {settings.ReadingDelay} ms");

            await Clients.All.SendAsync("streamingSource", Source.ToString());
        }

        public ChannelReader<Reading> SensorsTick()
        {
            var channel = Channel.CreateUnbounded<Reading>();
            _ = WriteToChannel(channel.Writer);
            return channel.Reader;

            async Task WriteToChannel(ChannelWriter<Reading> writer)
            {
                while (isStreaming)
                {
                    await led.LedFlash(settings.RedLedPin, settings.FlashingDelay);

                    var luminosity = await sensor.ReadLuminosity();
                    await Task.Delay(settings.ReadingDelay);

                    var temperature = await sensor.ReadTemperature();
                    await Task.Delay(settings.ReadingDelay);

                    var infrared = await sensor.ReadInfrared();
                    await Task.Delay(settings.ReadingDelay);
                    //if (infrared > settings.InfraredDistance) 
                    //{
                    //    Led.LedFlash(settings.BlueLedPin, settings.FlashingDelay);
                    //}

                    var distance = await sensor.ReadDistance();
                    await Task.Delay(settings.ReadingDelay);
                    //if (distance < settings.ProximityDistance) 
                    //{
                    //    Led.LedFlash(settings.BlueLedPin, settings.FlashingDelay);
                    //}

                    ConsoleHelper.HighlightLine($"lux={luminosity} temp={temperature} infra={infrared} dist={distance} source={Source}");

                    var reading = new Reading
                    {
                        Luminosity = luminosity,
                        Temperature = temperature,
                        Infrared = infrared,
                        Distance = distance,
                        Source = Source.ToString()
                    };

                    await writer.WriteAsync(reading);
                    await context.Readings.AddAsync(reading);

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
