# DeviceOAuth2 API

There are two patterns that can be used with [DeviceOAuth](xref:DeviceOAuth2.DeviceOAuth):

## Event Based

Events on the [IDeviceOAuth2](xref:DeviceOAuth2.IDeviceOAuth2) interface are raised as communication with the auth endpoint occurs.

    IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Google, "scope", "client_id", "client_secret");

    auth.PromptUser += (o, e) =>
    {
        Console.WriteLine("Go to this url on any computer:");
        Console.WriteLine(e.VerificationUri);
        Console.WriteLine("And enter this code:");
        Console.WriteLine(e.UserCode);
    };

    var token = await auth.Authorize(null);

## Stepwise

[IDeviceOAuth2Stepwise](xref:DeviceOAuth2.IDeviceOAuth2) returns state data and requires client code to call methods in the correct order.

    IDeviceOAuth2Stepwise auth = new DeviceOAuth(EndPointInfo.Google, "scope", "client_id", "client_secret");

    var info = await auth.StartAuthorization();

    Console.WriteLine("Go to this url on any computer:");
    Console.WriteLine(info.VerificationUri);
    Console.WriteLine("And enter this code:");
    Console.WriteLine(info.UserCode);

    var token = await auth.WaitForUserConsent(info);