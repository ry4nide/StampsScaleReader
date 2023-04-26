using HidLibrary;

// Based on answer provided at https://stackoverflow.com/questions/10407128/how-to-read-the-weight-from-a-weight-usb-scale
// Uses HidLibrary: https://github.com/mikeobrien/HidLibrary
// Install-Package hidlibrary

Console.WriteLine("Starting StampsScaleReader...");
decimal? ounces = null;
bool? isStable = null;
GetStampsComModel2500iScaleWeight(out ounces, out isStable);
Console.WriteLine("ounces: " + ounces?.ToString() ?? "NA");
Console.WriteLine("isStable: " + isStable?.ToString() ?? "NA");

void GetStampsComModel2500iScaleWeight(out decimal? ounces, out bool? isStable)
{
    isStable = null;
    ounces = null;

    //var hidDeviceListEnum = HidDevices.Enumerate(); // Get list of ALL HID devices
    var hidDeviceListEnum = HidDevices.Enumerate(0x1446, 0x6A73);   // Get these specific device types
    var hidDeviceList = hidDeviceListEnum.ToArray();

    if (hidDeviceList.Length > 0)
    {
        int waitTries;

        var scale = hidDeviceList[0];
        waitTries = 0;

        // For some reason, the scale isn't always immediately available
        // after calling Open(). Let's wait for a few milliseconds before
        // giving up.
        while (!scale.IsConnected && waitTries < 10)
        {
            Thread.Sleep(50);
            waitTries++;
        }

        if (scale.IsConnected)
        {
            var inData = scale.Read(250);
            ounces = (Convert.ToDecimal(inData.Data[4]) +
                Convert.ToDecimal(inData.Data[5]) * 256) / 10;
            isStable = inData.Data[1] == 0x4;
        }

        scale.Dispose();
    }
}