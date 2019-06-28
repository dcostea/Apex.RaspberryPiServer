$(document).ready(function () {

    //document.getElementById("hello1").play(); 

    // choose the source type (StandBy, Lighter, FlashLight, Infrared) 
    $(".source").click(function () {
        connection.invoke("StopStreaming");
        connection.invoke("ChangeSource", this.id);
        connection.invoke("StartStreaming");
        $("#state")[0].innerHTML = "State: " + this.value;
        //document.getElementById('confirmation1').play(); 
    });

    $(".none").click(function () {
        populateData("?", "?", "?", "?"); 
        connection.invoke("StopStreaming");
        $("#state")[0].innerHTML = "State: Offline";
        document.getElementById('denial1').play(); 
    });

    const connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Error)
        .withUrl("/sensor")
        .build();

    connection.on("streamingSource", function (source) {
        console.log("STREAMING SOURCE: " + source);
    });

    connection.on("streamingStarted", function () {
        console.log("STREAMING STARTED");

        connection.stream("SensorsTick").subscribe({
            close: false,
            next: sensors => {
                populateData(sensors);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished streaming");
            }
        });
    });

    connection.on("streamingStopped", function () {
        console.log("STREAMING STOPPED");
        populateData("?", "?", "?", "?");         
    });

    connection.start();
});

function populateData(sensors) {

    //// too much infrared energy!
    //if (sensors.infrared > 0)
    //{
    //    //document.getElementById('danger2').play(); 
    //}

    //// proximity alert!
    //if (sensors.distance < 20)
    //{
    //    //document.getElementById('warning1').play(); 
    //}

    //// too hot and too bright!
    //if (sensors.luminosity > 70 || sensors.temperature > 70)
    //{
    //    //document.getElementById('info1').play(); 
    //}
    if (sensors !== undefined)
    {
        $("#lux").html(`${sensors.luminosity} %`);
        $("#temp").html(`${sensors.temperature} &deg;C`);
        $("#infra").html(`${sensors.infrared} %`);
        $("#dist").html(`${sensors.distance >= 400 ? '? cm' : sensors.distance + ' cm'}`);
    }
}
