# DeviceOAuth2
Limited input device OAuth 2 flow for .NET

OAuth flow for devies with limited access to input devices or web browsers, like console apps, or IoT devices.

    var keys = GetAppCredentials("Facebook");
    IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

    auth.WaitingForConfirmation += (o, e) =>
    {
        Console.CursorLeft = 0;
        Console.Write(e + " seconds left");
    };
    auth.AuthenticatePrompt += (o, e) =>
    {
        Console.WriteLine("");
        Console.WriteLine("Go to this url on any computer:");
        Console.WriteLine(e.VerificationUrl);
        Console.WriteLine("And enter this code:");
        Console.WriteLine(e.UserCode);
        Console.WriteLine("");
    };

    Console.WriteLine("Authenticating...");

    try
    {
        var token = await auth.Authenticate(null);

        await ShowUserProfile(token);
    }
    catch (AggregateException e)
    {
        Console.WriteLine("Error:");
        foreach (var inner in e.InnerExceptions)
        {
            Console.WriteLine(inner.Message);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Error:");
        Console.WriteLine(e.Message);
    }
