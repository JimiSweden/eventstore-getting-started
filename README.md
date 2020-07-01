# Getting started with EventStore on Windows 10
(2020-06-28)

A beginner stumbling on the first line... I really started to feel stupid 🙄 
(if you want the "quick fix" try using https://127.0.0.1:2113/ - but please read about the configration problems I had as it might be good knowledge ot have if you stumble on the same errors)

Following  https://eventstore.com/docs/getting-started/index.html for windows (and also docker with no success)

I had problems to understand why I could not connect to http://127.0.0.1:2113/ when following the guide. 

I tried search phrases like "evenstore admin ui not available" and "eventstore admin ui Unable to connect" 


Running the command with --dev mode didn't work for me, so I tried out with self signed cert (and it din't work either)
cmd: EventStore.ClusterNode.exe --db ./db --log ./logs --dev
- documentation says 
"Admin UI is visible under 2113 port, navigate to http://127.0.0.1:2113/ in your web browser to see it."
- I didn't assume it would work with https because it doesn't tell (in hindsight it is not so strange since you seem to be required to have a certificate, but it's not hinted that dev mode would not work in hhtp mode)

https://eventstore.com/docs/getting-started/index.html?tabs=tabid-1%2Ctabid-dotnet-client%2Ctabid-dotnet-client-connect%2Ctabid-4#discover-event-store-via-admin-ui
- documentation says "Admin UI is visible under 2113 port, navigate to http://127.0.0.1:2113/ in your web browser to see it."

# With self signed certificate. 
The config file example in documentation on geteventstore is not working (probably for an older version).
https://eventstore.com/docs/server/setting-up-ssl/index.html?tabs=tabid-6%2Ctabid-8#setting-up-ssl-on-windows


- Removing the property ExtSecureTcpPort (due to error - # ExtSecureTcpPort throws EventStore.Rags.OptionException: The option ExtSecureTcpPort is not a known option)
- gives next error - System.Exception: TrustedRootCertificatesPath must be specified unless development mode (--dev) is set

googling on TrustedRootCertificatesPath (not found when searching evenstore.com)
pointed me to https://github.com/EventStore/EventStore/pull/2335 
> - Detailed changes
> The PR makes the following changes:
> Add a mandatory TrustedRootCertificatesPath configuration parameter. The certificate store will be expanded with  those root certificates before validation. Thus, root certificates no longer need to be installed on the system.
> ...

ok, so How do I add the path to trusted root certificates? 
I'm not sure but it looks like it has to be the path on disk (first started to look for path to the windows certification managers "Trusted Root Certification Authorities", but didn't really find anything obvious)

## Finally 
Here is a working config file , as mentioned in the documentation the three dashes for start and end are important, as well as spaces between the property and value.


Config file 
```yaml
---
# config.yaml
CertificateStoreLocation: CurrentUser
CertificateStoreName: My
CertificateThumbPrint: FE4CE0CA77CB55A053DB7F460AA79B4EB18B1036
CertificateSubjectName: CN=eventstore.com
## if you have the cert in folder 'c:\eventsource'
TrustedRootCertificatesPath: "c:\\eventsource"
## or as below if you have the cert in 'current folder'/trusted-root-cert
## TrustedRootCertificatesPath: "./trusted-root-cert"
## below throws "not supported command
## ExtSecureTcpPort: 1115
---

```


start command running the config file 
```cmd
EventStore.ClusterNode.exe --db ./db --log ./logs --config ./config.yaml
```
(NOTE: in the documentation https://eventstore.com/docs/server/command-line-arguments/index.html#yaml-files,
it says "o tell Event Store to use a different configuration file, you pass the file path on the command line with --config=filename, or use the CONFIG enivornment variable.",
both --config=config.yaml and '--config ./config.yaml' works (in powershell), but to be consistent with the other parameters I used the latter (i.e validated the flag '--config=config.yaml' after I managed to get the store running and accessing the Admin UI.



> you should see a line like this "Trusted root certificate file loaded: "eventstore-selfsigned.cer"
when the store is starting
