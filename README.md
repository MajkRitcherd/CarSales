# 🚗 CarSales

Desktopová aplikace pro správu prodejů automobilů. Aplikace umožňuje evidovat prodejní data aut, spravovat informace o výrobcích a provádět analýzu prodejů včetně výpočtu DPH a statistik prodejů.

## 📋 Funkce

- **Správa vozidel** - Přidávání, zobrazování a správa prodaných automobilů
- **Správa výrobců** - Evidence automobilových značek a jejich vozidel
- **Výpočty cen** - Automatický výpočet ceny s DPH na základě sazby DPH
- **Analýza dat** - Zobraní statistiky prodejů a víkendových prodejů
- **Import/Export dat** - Možnost načíst a uložit data v XML nebo CSV formátu
- **Uživatelské rozhraní** - Přehledné WPF rozhraní s tabelárním zobrazením dat

## 🛠️ Požadavky

- **.NET 10.0** (Windows)
- Visual Studio 2022 nebo novější (pro vývoj)
- Windows 7 SP1 nebo novější (pro spuštění)

## 📥 Instalace a spuštění lokálně

### 1. Klonování repozitáře
```bash
git clone https://github.com/MajkRitcherd/CarSales.git
cd CarSales
```

### 2. Otevření v Visual Studiu
1. Otevřete **Visual Studio 2022**
2. Zvolte `File → Open → Project/Solution`
3. Vyberte soubor `CarSales.sln`

### 3. Obnovení balíčků
Visual Studio automaticky obnoví NuGet balíčky. Pokud ne, spusťte:
```bash
dotnet restore
```

### 4. Spuštění aplikace
- Stiskněte **F5** nebo klikněte na tlačítko `Run` v Visual Studiu

## 🗂️ Struktura projektu

```
CarSales/
├── Models/              # Datové modely (Vehicle, Manufacturer, SalesData)
├── Views/               # XAML UI komponenty a code-behind
├── ViewModels/          # ViewModel logika (MVVM pattern)
├── Services/            # Služby (FileService, atd.)
├── CarSales.csproj      # Projektový soubor
└── README.md            # Tento soubor
```

## 📦 Použité knihovny

- **CommunityToolkit.Mvvm** (v8.4.2) - MVVM framework
- **CsvHelper** (v33.1.0) - Práce s CSV soubory
- **WPF** - Windows Presentation Foundation pro UI

## 🚀 Použití

1. **Spusťte aplikaci**
2. **Načtěte data** - Klikněte na "Načíst" pro import XML nebo CSV souboru
3. **Přidejte vozidlo** - Vyplňte údaje o značce, modelu, ceně a datu prodeje
4. **Prohlédněte statistiky** - Přepínejte mezi záložkami pro zobrazení prodejů a víkendových prodejů
5. **Uložte data** - Klikněte na "Uložit" pro export do XML nebo CSV

## 📄 Formáty dat

### XML formát
```xml
<?xml version="1.0" encoding="utf-8"?>
<SalesData>
  <Manufacturer>
    <Name>Škoda</Name>
    <Vehicles>
      <Vehicle>
        <Model>Oktávia</Model>
        <NetPrice>500000</NetPrice>
        <VatPercent>21</VatPercent>
        <SoldOn>2026-06-01</SoldOn>
      </Vehicle>
    </Vehicles>
  </Manufacturer>
</SalesData>
```

### CSV formát
```
Značka,Model,Cena bez DPH,DPH,Cena s DPH,Datum prodeje
Škoda,Oktávia,500000,21,605000,2026-06-01
```

## 📝 Poznámky

- Aplikace ukládá data v paměti během spuštění
- Při zavření bez uložení budou změny ztraceny
- DPH se počítá automaticky ze sazby uvedené v datech

## 👨‍💻 Autor

[MajkRitcherd](https://github.com/MajkRitcherd)

## 📄 Licence

Projekt není licencován. Volně použitelný kód.
