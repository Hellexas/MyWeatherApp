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

---

## Funkcionalumų panaudojimas kode

Šioje skiltyje pateikiamas C# kalbos ir .NET platformos funkcionalumų sąrašas bei jų realizacijos vietos projekte:

### 1. Objektinis Programavimas (OOP) ir Tipai
* **Nuosava sąsaja (`Interface`)**: `IWeatherService` (faile `IWeatherService.cs`) – apibrėžia orų paslaugos kontraktą.
* **Abstrakti klasė**: `ForecastItemBase` (faile `ForecastItemBase.cs`) – bazinė klasė, suteikianti bendrą funkcionalumą prognozių elementams.
* **„Sealed“ klasė**: `WeatherService` (faile `WeatherApi.cs`) – klasė uždaryta paveldėjimui, siekiant užtikrinti logikos vientisumą.
* **Statinis konstruktorius**: `WeatherService` klasėje (faile `WeatherApi.cs`) – naudojamas vienkartiniam `HttpClient` iniciavimui (`static WeatherService()`).
* **Dalijamos klasės (`Partial class`)**: `MainPage` (faile `MainPage.xaml.cs`) – standartinis MAUI/WPF šablonas UI klasėms.
* **Standartinių sąsajų implementacija**:
    * **`IComparable<T>`**: `DailyForecastItem` klasėje (faile `DailyForecastItem.cs`) – leidžia rikiuoti prognozes.
    * **`IEquatable<T>`**: `DailyForecastItem` klasėje (faile `DailyForecastItem.cs`) – efektyviam objektų lyginimui.
    * **`IFormattable`**: `DailyForecastItem` klasėje (faile `DailyForecastItem.cs`) – lanksčiam tekstiniam atvaizdavimui.

### 2. Sintaksė ir Operatoriai
* **`switch` su `when` raktiniu žodžiu**: `WeatherCodeHelper.cs` metode `GetWeatherIcon` – naudojamas orų kodų grupavimui (pvz., `case int n when (n >= 1 && n <= 3):`).
* **`Range` tipas (Indeksų rėžiai)**: `WeatherViewModel.cs` metode `ProcessHourlyForecast` – naudojama sintaksė `hourly.Time[..24]` paimti pirmas 24 valandas.
* **„Pattern Matching“ ir `is` operatorius**: `DailyForecastItem.cs` metode `Equals` (`if (obj is DailyForecastItem other)`) ir `WeatherCodeHelper.cs`.
* **Operatorių perdengimas (`Operator overloading`)**: `DailyForecastItem.cs` – perdengti `>` ir `<` operatoriai temperatūrų lyginimui.
* **`params` raktinis žodis**: `ForecastItemBase.cs` metode `GetFormattedData` – leidžia perduoti kintamą argumentų skaičių.
* **Bitinės operacijos**: Naudojamos `DailyForecastItem.cs` metode `GetHashCode` (paprastai `^` operatorius maišos kodo generavimui).
* **Operatoriai `?.`, `??`, `??=`**: Plačiai naudojami null reikšmių saugikliams, pvz., `WeatherViewModel.cs` (`WeatherData?.Current`).

### 3. Duomenys ir Metodai
* **Inicializacija naudojant `out`**: `DailyForecastItem.cs` metode `Deconstruct` (arba `TryParse` kvietimuose `WeatherViewModel`).
* **Dekonstruktorius (`Deconstructor`)**: `DailyForecastItem.cs` – leidžia išskaidyti objektą į kintamuosius (`var (min, max, date) = item`).
* **Pasirinktiniai ir pavadinti argumentai**: `IWeatherService.GetWeatherAsync` (numatytasis parametras `timezone`) ir kviečiant metodą `WeatherViewModel.cs` (`timezone: "Europe/Vilnius"`).
* **Delegatai ir Lambda funkcijos**: Naudojami `RelayCommand` apibrėžimuose (`WeatherViewModel.cs`) ir LINQ užklausose (`.Where(x => ...)`).
* **Kolekcijos (`System.Collections.Generic`)**: `Dictionary` naudojamas `WeatherService.cs`, o `ObservableCollection` – `WeatherViewModel.cs`.

### 4. Architektūra
* **Keli moduliai (Assemblies)**: Projektas atskirtas į `MyWeatherApp.Core` (logika) ir `MyWeatherApp` (UI platforma).
