In order to build this example app add a file named keys.json and mark it as an embedded resource.

This file will contain your facebook and/or google client id and secret codes in the following format:

[
  {
    "name": "Google",
    "client_secret": "your google client secret",
    "client_id": "your google client id",
    "scopes":  "profile"
  },
  {
    "name": "Facebook",
    "client_secret": "",
    "client_id": "your facebook client id",
    "scopes":  "public_profile"
  }
]