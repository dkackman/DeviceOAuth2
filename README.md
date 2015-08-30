# DeviceOAuth2
Limited input device OAuth 2 flow for .NET

OAuth flow for devies with limited access to input devices or web browsers, like console apps, or IoT devices.

    var keys = GetAppCredentials("Facebook");
    IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

    auth.AuthenticatePrompt += (o, e) =>
    {
        Console.WriteLine("Go to this url on any computer:");
        Console.WriteLine(e.VerificationUrl);
        Console.WriteLine("And enter this code:");
        Console.WriteLine(e.UserCode);
    };

    var token = await auth.Authenticate(null);

    
