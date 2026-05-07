<img width="4096" height="1024" alt="Title D" src="https://github.com/user-attachments/assets/e6444756-725b-41c7-b78a-ba8197e39f22" />

Jedná se o 2D singleplayer hru, jejímž hlavním cílem je postup hráče procedurálně generovaný dungeonem, jehož struktura se při každém průchodu mění. Hráč začíná v oblasti lobby, která slouží k výběru postav, zbraní či nástrojů, jež hráč v průběhu hry odemyká či vyrábí. 

Hra je založena na průzkumu dungeonu, sběru materiálů a překonávání různých nástrah, nepřátel a bossů. Dungeon je rozdělen na několik pater, přičemž každé patro má svůj specifický design a obtížnost. Základní jednotkou generace pater jsou místnosti propojené cestami, do kterých jsou náhodně vloženy objekty, nástrahy a nepřátelé.

Vizuální část hry závisí na pixel art grafice. Kamera je v top-down perspektivě, inspirované hrou Enter the gungeon. Hra bude vyvíjena v herním enginu Unity s využitím programovacího jazyka C#. Součástí dokumentace bude především popis implementace procedurální generace, exportu a spuštění hry.

# Spuštění
## Spuštění .exe
Hra lze spustit jako exportovaný produkt, nebo přímo v Unity. Pro spuštění exportované hry je potřeba z repozitáře tohoto projektu pod složkou [Builds](https://github.com/gyarab/2025-3e-Dungeon_crawler/tree/main/Dungeon_crawler/Builds) dle vašeho operačního systému, například **Windows x64**, stáhnout zazipovanou složku. Po rozbalení stažené složky spusťte soubor `Dungeon Crawler.exe`  pro windows **x64**/**x32** nebo `Dungeon Crawler.x86_64` pro linux. 
Je možné že na linuxu budete muset napsat do terminálu příkazy 
```bash
chmod +x Dungeon_Crawler.x86_64
```
```bash
./Dungeon_Crawler.x86_64.
```
## Spuštění v Unity
Pro spuštění hry v Unity (nedoporučuje se, pouze pro vývoj či kontrolu) stáhněte celý repozitář. 
1) Stáhněte Unity [Hub](https://cloud.unity.com/home/organizations/1374853522376/onboarding/), ve kterém lze stáhnout verzi **Unity 6000.2.10f1**.
2) Zmáčkněte tlačítko `Add` > `Add project from disk`, kde vyberete tento repozitář.
3) Po přidání projektu do Unity Hub lze projekt spustit.
4) Pro spuštění hry v konkrétní scéně je potřeba scénu otevřít ve složce `Assets/Scenes`, kde můžete otevřít například první scénu **MainMenu**.
5) Po vybrání scény se hra zapne stisknutím tlačítka `play` (ikona ▶), které je uprostřed horní lišty.
Hra se zobrazí v okně `Game`, které je při výchozím nastavení skryto pod oknem `Scene`. Tyto okne lze přepínat na horní liště.
Pokud má UI či hra špatné rozlišení či nevypadá jako exportovaný produkt, musí se změnit nastavení rozlišení v horní liště, konkrétně poměr stran nastavit na *16:9* a velikost na *1x*.

Hra spuštěná v Unity nereprezentuje finální produkt, protože editor je dělán pro vývoj. Lze tedy hru jednoduše renderovat špatně či spustit v nezamýšleném stavu.

