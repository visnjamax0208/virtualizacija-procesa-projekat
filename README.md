# GalaxyPPG

GalaxyPPG je WCF klijent-server aplikacija za razmenu i osnovnu obradu PPG/HRV podataka sa nosivih uredjaja. Projekat koristi CSV zapise inspirisane GalaxyPPG skupom podataka i prikazuje tok obrade od citanja fajlova do slanja podataka serveru, skladistenja i detekcije jednostavnih alarma.

## Opis resenja

Aplikacija je podeljena na tri projekta:

- `GalaxyPPG.Common` sadrzi zajednicke modele, WCF ugovor servisa i model greske.
- `GalaxyPPG.Server` sadrzi WCF servis, upis podataka u fajl sistem i obradu alarma.
- `GalaxyPPG.Client` sadrzi konzolni klijent, CSV parser i primer ulaznih podataka.

Klijent cita CSV fajlove za puls, PPG/BVP signal i akcelerometar. Procitani redovi se pretvaraju u `SensorRecord` objekte i pakuju u `MeasurementPacket`. Paket se zatim salje WCF servisu. Server proverava podatke, snima ih u strukturisan folder i generise alarme ako su vrednosti van zadatih pragova.

## Tehnologije i oblasti

U projektu su primenjeni:

- WCF servis i WCF klijent
- `DataContract` i `ServiceContract`
- `FaultContract` za obradu gresaka
- konfiguracija kroz `Web.config` i `App.config`
- `FileStream`, `StreamReader` i `StreamWriter`
- `MemoryStream` za raw CSV upload
- `IDisposable` za klase koje rade sa resursima
- delegati i dogadjaji za alarmni model

## Podaci

U specifikaciji projekta naveden je GalaxyPPG dataset:

```text
https://zenodo.org/records/14635823
```

Zbog velicine, kompletan dataset nije dodat u repozitorijum. U projektu se nalaze mali primeri CSV fajlova u folderu:

```text
GalaxyPPG.Client/SampleData/P01/GalaxyWatch
```

Primeri pokrivaju:

- `HR.csv` - puls
- `BVP.csv` - PPG/BVP signal
- `ACC.csv` - akcelerometar

## Pokretanje

1. Otvoriti `GalaxyPPG.sln` u Visual Studio-u.
2. Buildovati solution.
3. Pokrenuti `GalaxyPPG.Server` preko IIS Express-a.
4. Proveriti adresu servisa:

   ```text
   http://localhost:51500/GalaxyPpgService.svc
   ```

5. Pokrenuti `GalaxyPPG.Client`.

Klijent koristi adresu servisa iz `GalaxyPPG.Client/App.config`.

## Izlazni fajlovi

Server upisuje obradjene pakete u:

```text
GalaxyPPG.Server/App_Data/GalaxyPPG
```

Raw CSV upload se cuva u podfolderu:

```text
GalaxyPPG.Server/App_Data/GalaxyPPG/raw
```

Alarmi se upisuju u:

```text
GalaxyPPG.Server/App_Data/GalaxyPPG/alarms.log
```

Ovi izlazni fajlovi su generisani tokom rada aplikacije i nisu namenjeni za commit.

## Alarmni uslovi

Server proverava nekoliko jednostavnih uslova:

- slab PPG signal
- prekomeran pokret na osnovu ACC magnitude
- puls van konfigurisanog opsega

Pragovi se nalaze u `GalaxyPPG.Server/Web.config` i mogu se menjati bez promene koda.
