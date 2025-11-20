# MyWeatherApp – Projekto Dokumentacija

Tai yra **.NET MAUI** (Multi-platform App UI) orų programėlė, sukurta naudojant **MVVM** (Model-View-ViewModel) architektūrinį modelį. Programėlė gauna orų duomenis iš viešos API ir pateikia juos vartotojui patogioje sąsajoje su dinamine grafika.

## Architektūra

Projektas yra padalintas į du pagrindinius modulius (assemblies), siekiant atskirti verslo logiką nuo vartotojo sąsajos:

1.  **MyWeatherApp.Core** – Atsakingas už duomenų gavimą, modeliavimą ir verslo taisykles.
2.  **MyWeatherApp** (UI) – Atsakingas už vaizdavimą, būsenos valdymą ir sąveiką su vartotoju.

### Naudojamos technologijos ir bibliotekos
* **.NET MAUI**: Krosplatforminė UI sistema.
* **CommunityToolkit.Mvvm**: MVVM šablonui palengvinti (ObservableObject, RelayCommand).
* **System.Text.Json**: JSON duomenų deserializacijai.
* **Microsoft.Extensions.DependencyInjection**: Priklausomybių injekcijai (DI).

---

## Modulis: MyWeatherApp.Core

Šis modulis yra nepriklausomas nuo UI platformos ir talpina pagrindinę logiką.

### 1. Paslaugos (Services)

#### `IWeatherService` (Sąsaja)
Apibrėžia kontraktą orų duomenų gavimui.
* **Metodas**: `Task<WeatherData> GetWeatherAsync(string? timezone = "Europe/Vilnius")`

#### `WeatherService` (Implementacija)
`sealed` klasė, realizuojanti `IWeatherService`.
* **API Šaltinis**: Naudoja `open-meteo.com` API.
* **Funkcionalumas**:
    * Saugo statinį `HttpClient` efektyviam resursų naudojimui.
    * Turi vidinį žodyną `_cityCoordinates`, susiejantį laiko zonas su geografinėmis koordinatėmis (pvz., Vilnius: 54.72, 25.24).
    * Formuoja užklausas su parametrais: `current`, `hourly`, `daily` orų duomenims.
    * Apdoroja JSON atsakymą ir grąžina `WeatherData` objektą arba `null` klaidos atveju.

### 2. Duomenų Modeliai (Models)

Duomenys deserializuojami į griežtos struktūros klases (`Models/WeatherApi.cs`):

* **`WeatherData`**: Pagrindinis objektas, talpinantis `Current`, `Hourly` ir `Daily` duomenis.
* **`CurrentWeather`**: Dabartinė temperatūra, vėjas, drėgmė, orų kodas.
* **`HourlyWeather`**: Masyvai su 24 valandų prognoze (temperatūra, kritulių tikimybė).
* **`DailyWeather`**: Masyvai su savaitės prognoze (min/max temperatūra).

---

## Modulis: MyWeatherApp (UI)

Šis modulis valdo programėlės atvaizdavimą ir logiką naudojant .NET MAUI.

### 1. ViewModels

#### `WeatherViewModel`
Pagrindinis ViewModelis, paveldintis `ObservableObject`. Jis tarpininkauja tarp `WeatherService` ir vartotojo sąsajos (View).

* **Savybės (Properties)**:
    * `WeatherData`: Saugomi gauti orų duomenys.
    * `IsLoading`: Rodo krovimo būseną.
    * `DailyForecast` / `HourlyForecast`: `ObservableCollection` sąrašai, skirti UI sąrašų atvaizdavimui.
    * `BackgroundBrush`: Dinaminis fonas (gradientas), kintantis pagal orą.
    * Ekrano tekstai (`CurrentWeatherDescription`, `WindDisplay` ir kt.) yra formatuojami ir paruošiami čia.

* **Komandos (Commands)**:
    * `LoadWeatherCommand`: Asinchroniškai kviečia servisą duomenims gauti. Atnaujina UI elementus, konvertuoja orų kodus į piktogramas ir aprašymus.

* **Pagrindinė Logika**:
    * **`UpdateBackground(int code, int isDay)`**: Keičia programėlės fono spalvas (gradientą) priklausomai nuo oro sąlygų (pvz., saulėta – mėlyna, naktis – tamsi, lietus – pilka).
    * **`ProcessDailyForecast`**: Apskaičiuoja savaitės temperatūrų diapazoną ir sugeneruoja duomenis stulpelinei vizualizacijai (`BarStartFactor`, `BarWidthFactor`).
    * **`ProcessHourlyForecast`**: Išfiltruoja artimiausias 24 valandas nuo esamo laiko.
    * **Laikmatis**: `IDispatcherTimer` automatiškai atnaujina duomenis kas 5 minutes.

### 2. Programos Pradžia (Startup)

#### `MauiProgram.cs`
Atsakinga už programėlės konfigūraciją ir priklausomybių injekciją (DI).

* **Registracija**:
    * `IWeatherService` registruojamas kaip `Singleton` (vienas egzempliorius visai programai).
    * `WeatherViewModel` ir `MainPage` registruojami kaip `Transient` (kuriami pagal poreikį).
* **Šriftai**: Užregistruojami "OpenSans" ir "Font Awesome" šriftai ikonoms.

---

## Lokalizacija ir Pagalbininkai

Programėlė naudoja pagalbines klases (Helpers) duomenų transformavimui:
* **`WeatherCodeHelper`**: Konvertuoja skaitinį WMO orų kodą (pvz., 45, 71) į vartotojui suprantamą tekstą ir piktogramą.
* **`AppStrings`**: Resursų failai, naudojami tekstų vertimams (lokalizacijai).

## Kaip paleisti projektą

1.  Įsitikinkite, kad turite įdiegtą **.NET 7/8 SDK** ir **MAUI** darbo krūvį (workload).
2.  Atsidarykite sprendimą (`.sln`) per Visual Studio.
3.  Pasirinkite norimą platformą (Android, iOS, Windows arba MacCatalyst).
4.  Paleiskite projektą (F5).

## Pastabos vystytojams

* Norint pridėti naują miestą, reikia papildyti `_cityCoordinates` žodyną `WeatherService` klasėje.
* API raktas nereikalingas (Open-Meteo yra nemokamas nekomerciniam naudojimui).
