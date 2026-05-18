# 🏎️ F1OS (Formula 1 Operating System)

![Logo de F1OS](assets/logo.png)

## 🏁 Descripció
**F1OS** és un sistema operatiu de 64 bits desenvolupat sobre el framework **Cosmos (C# Open Source Managed Operating System)**. 

Inspirat en l'enginyeria de precisió de la Fórmula 1, aquest sistema busca la màxima optimització, velocitat de resposta i una arquitectura modular que permeti un rendiment "pole position" en cada procés. El projecte ha evolucionat d'una interfície de línia de comandes (CLI) a un entorn completament gràfic (GUI) amb capacitats de connectivitat en xarxa.

---

## 👥 Membres del Grup
El "Pit Wall" d'aquest projecte està format per:
* **Jefferson Méndez** 🏎️
* **Biel Duran** 🔧

---

## 🛠️ Estructura del Repositori
Per mantenir el garatge ordenat i facilitar el manteniment, hem organitzat el projecte de la següent manera:
* `src/`: Codi font del Kernel i biblioteques del sistema. Modularitzat per separar la lògica de la interfície gràfica, el sistema de fitxers, la xarxa i el servidor FTP.
* `docs/`: Documentació tècnica, manuals d'usuari i guies de configuració de xarxa.
* `assets/`: Recursos gràfics, logotips, icones i paletes de colors per a la GUI.

---

## 🚀 Tecnologies utilitzades
* **Llenguatge:** C# (.NET Core)
* **Kernel Base:** Cosmos Kit
* **Interfície Gràfica:** Cosmos Graphic Subsystem
* **Arquitectura:** x86/x64
* **Àudio:** Cosmos Audio Driver
* **Xarxa:** Cosmos Network Stack (TCP/IP & FTP Server)

---

## 🆕 Funcionalitats recents

### 📺 Subsistema Gràfic (Grand Prix GUI)
Hem abandonat el mode text per implementar una interfície visual immersiva utilitzant el **Cosmos Graphic Subsystem**:
* **Pantalla de Benvinguda (Warm-up Lap):** Renderitzat inicial del logotip de F1OS, texts de benvinguda i barres de càrrega utilitzant formes geomètriques i la paleta de colors oficial de la competició.
* **Interfície de l'Usuari (Dashboard):** Disseny d'una pantalla atractiva combinant formes, rectangles i colors per estructurar el tauler de control, escrivint el text directament sobre els elements gràfics per a una experiència d'usuari neta i moderna.
* **Adaptació del Sistema:** S'ha refactoritzat i modificat el codi previ per fer que totes les funcionalitats anteriors siguin totalment operatives dins de l'entorn gràfic.

### 🌐 Telemetria i Xarxa (Pit-to-Car Communication)
El sistema ara és capaç d'interactuar amb el món exterior gràcies a la pila de xarxa de Cosmos:
* **IP Estàtica:** Configuració de xarxa que permet assignar una adreça IP estàtica a la màquina virtual.
* **Comanda d'Adreça:** Comanda integrada a la interfície per mostrar de manera instantània la IP actual del sistema quan l'usuari ho sol·liciti.

### 📂 Servidor FTP (Data Download Link)
* **Publicació de Directoris:** Posada en marxa d'un **servidor FTP** intern que publica un directori del sistema de fitxers de F1OS.
* **Connexió Externa:** Suport per a connexions des de clients externs (com ara *FileZilla*) per poder pujar, descarregar i gestionar els fitxers del sistema remotament des del sistema operatiu amfitrió.

---

## 💾 Funcionalitats Heretades (i adaptades a la GUI)
* **Race History:** Historial i recuperació de les últimes 5 comandes executades per guanyar temps a la línia de boxs.
* **Pit Wall Radio:** Sistema d'àudio amb melodia de benvinguda (*Startup Sound*) i *feedback* sonor per a accions correctes i errors.
* **Gestió de Fitxers:** Suport per a la creació, llistat (`ls`), eliminació i navegació de directoris.
* **Engine Stats (Calculadora):** Operacions aritmètiques completes mitjançant la comanda `calc` (Suma, Resta, Multiplicació, Divisió, `mod` i `sqrt`).

---

## 📄 Llicència
Aquest projecte està sota la llicència **MIT**. Pots consultar el fitxer `LICENSE` per a més detalls.

---

### 💡 Propers passos
> ⚠️ **Nota de l'equip d'enginyers:** Totes les novetats gràfiques i de xarxa s'estan desenvolupant en una **nova branca a GitHub** per assegurar l'estabilitat de la branca principal. Els propers passos se centraran en la seguretat del servidor FTP, la implementació d'usuaris amb contrasenya per al Pit Wall i la millora de la taxa de refresc de la GUI.
