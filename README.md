Getting Real 1. semester virksomhedsprojekt Datamatiker Online maj 2026
Gruppe 14: Sofus Boelskifte Pedersen, Henrik Carlsen, Andronikos Noa Papadakis & Isabella Larsson Seedorff

Et klienthåndterings- og bookingsystem udviklet til Andreas' firma ManuVision (papamanu.dk), som udbyder kropsterapibehandlinger. 
Systemet er en prototype som kan køres som enten konsolapp eller wpf

Login til konsolapp:
Medarbejder Id: AP
Kode: Papas
(casesensitiv)

Login til wpf:
Brugernavn: admin
Adgangskode: 1234

Kræver .NET 10 
Åbn wpfGettingRealG14This.slnx
Sæt hhv wpfGettingRealG14This eller GetReal Startup Project alt efter om du ønsker at se wpf-versionen eller konsolappen

Projektstruktur:
GetReal: Model-laget (Class Library) med domæneklasser, interfaces, repositories og services

wpfGettingRealG14This: WPF-applikationen med ViewModels, Views og Helpers

Data gemmes automatisk i JSON-filer (clients.json og clientnotes.json)

Vi er bevidste om at der er uhørt mange kommentare i koden - men disse er lavet som vores egne interne noter, som vi ønsker at beholde
til fremtidige semestre.
