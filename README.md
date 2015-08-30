# DeviceOAuth2
Limited input device OAuth 2 flow for .NET

OAuth flow for scenarios with limited access to input devices or web browsers, like console apps, or IoT devices.

    IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, "scope", "client_id", "client_secret");

    auth.AuthenticatePrompt += (o, e) =>
    {
        Console.WriteLine("Go to this url on any computer:");
        Console.WriteLine(e.VerificationUrl);
        Console.WriteLine("And enter this code:");
        Console.WriteLine(e.UserCode);
    };

    var token = await auth.Authenticate(null);

    
