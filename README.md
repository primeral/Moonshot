# Luna

Luna is a Jellyfin/Moonfin plugin fork focused on improved media-stack integration, Seerr session bootstrap, and compatibility with login-agnostic authentication flows.

This repository is currently based on the Moonfin server plugin and carries the Luna-specific work needed for the biiitch ecosystem sandbox.

## Current alpha focus

- Luna/Moonfin server plugin work
- Seerr session bootstrap support
- Seerr API key configuration UI
- Sandbox build metadata and validation notes
- Compatibility with Multi-Auth and SMS-based Quick Connect workflows

## Design goals

Luna should remain login-agnostic.

It should not become an identity provider. It should work with Jellyfin local login, Jellyfin Quick Connect, Multi-Auth, future OIDC flows, and Seerr+QC without requiring every native client platform to be rebuilt or sideloaded.

## Related projects

- Multi-Auth: Jellyfin plugin for SMS Quick Connect, SMS login authorization, and future OIDC.
- Seerr+QC: Seerr fork with Jellyfin Quick Connect support added/restored.
- SMS-Gateway Server: self-hosted SMS transport server.
- SMS-Gateway Client for Android: Android SMS device client.

## Fork status

This is an active Luna fork of the Moonfin plugin. Some internal names, namespaces, package names, and comments may still reference Moonfin during the alpha transition.
