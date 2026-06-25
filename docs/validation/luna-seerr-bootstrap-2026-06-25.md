# Luna / Seerr bootstrap validation - 2026-06-25

## Sandbox topology

- proxmox-jf12-sandbox: 192.168.3.153
- CT120 jellyfin12-rc-lxc: 192.168.3.116
- CT121 seerr-qc-luna-sandbox: 192.168.3.117
- Seerr URL used by Moonfin: http://192.168.3.117:5055

## Validated plugin

- Moonfin plugin directory: /var/lib/jellyfin/plugins/Moonfin_1.9.1.2
- Package: Moonfin.Server-1.9.1.2.zip
- Package MD5: F41994320651E753E50B77C5E537C49D
- Jellyfin loaded plugin: Moonfin 1.9.1.2

## Validated configuration

- SeerrEnabled: true
- SeerrUrl: http://192.168.3.117:5055
- SeerrApiKey: present in /var/lib/jellyfin/plugins/configurations/Moonfin.Server.xml

## Validated browser/API flow

Browser was authenticated to Jellyfin as admin.

Plain fetch without Jellyfin auth header returned 401.

Fetch using Jellyfin browser AccessToken from localStorage worked.

POST /Moonfin/Seerr/Bootstrap returned 200:
- success: true
- seerrUserId / jellyseerrUserId: 1
- displayName: admin
- permissions: 2

GET /Moonfin/Seerr/Status returned 200:
- enabled: true
- authenticated: true
- displayName: admin
- permissions: 2

GET /Moonfin/Seerr/Api/auth/me returned 200 with Seerr admin user JSON.

GET /Moonfin/Seerr/Api/request returned 200 with empty request list and no Radarr/Sonarr service errors.

## Seerr direct UI note

Direct Seerr UI was still stuck at:

Seerr Updated. Please click the button below to reload the application.

This did not block the Luna/Moonfin backend bridge because Seerr backend API was healthy:
- /api/v1/status returned OK.
- /api/v1/auth/me with main.apiKey returned Seerr admin JSON.

## Operational lesson

Do not leave Moonfin backup directories under /var/lib/jellyfin/plugins.

Jellyfin scans nested backup plugin DLLs and can load duplicate Moonfin.Server assemblies, causing PluginConfiguration type-cast failures.

Keep backups under /root/jellyfin-plugin-backups instead.

## Browser console regression snippet

Paste this into a Jellyfin browser console while logged in:

    (() => {
      const creds = JSON.parse(localStorage.getItem("jellyfin_credentials"));
      const server = creds.Servers.find(s => s.AccessToken) || creds.Servers[0];
      const token = server.AccessToken;
      const deviceId = server.DeviceId || "moonfin-luna-browser-test";

      const headers = {
        "X-Emby-Token": token,
        "Authorization": `MediaBrowser Client="Jellyfin Web", Device="Browser", DeviceId="${deviceId}", Version="12.0.0", Token="${token}"`
      };

      fetch("/Moonfin/Seerr/Bootstrap", { method: "POST", headers })
        .then(r => r.text().then(t => console.log("Bootstrap", r.status, t)));

      fetch("/Moonfin/Seerr/Status", { headers })
        .then(r => r.text().then(t => console.log("Status", r.status, t)));

      fetch("/Moonfin/Seerr/Api/auth/me", { headers })
        .then(r => r.text().then(t => console.log("Api auth/me", r.status, t)));

      fetch("/Moonfin/Seerr/Api/request", { headers })
        .then(r => r.text().then(t => console.log("Api request", r.status, t)));
    })();
