Aquí tens una proposta per actualitzar el teu **README.md**. He integrat les noves funcionalitats seguint la temàtica de Fórmula 1, mantenint l'estil net i professional que ja tenies.

-----

# 🏎️ F1OS (Formula 1 Operating System)

## 🏁 Descripció

**F1OS** és un sistema operatiu de 64 bits desenvolupat sobre el framework **Cosmos (C\# Open Source Managed Operating System)**.

Inspirat en l'enginyeria de precisió de la Fórmula 1, aquest sistema busca la màxima optimització, velocitat de resposta i una arquitectura modular que permeti un rendiment "pole position" en cada procés.

-----

## 👥 Membres del Grup

El "Pit Wall" d'aquest projecte està format per:

  * **Jefferson Méndez** 🏎️
  * **Biel Duran** 🔧

-----

## 🛠️ Estructura del Repositori

Per mantenir el garatge ordenat, utilitzem la següent estructura:

  * `src/`: Codi font del Kernel i biblioteques del sistema.
  * `docs/`: Documentació tècnica i manuals d'usuari.
  * `assets/`: Recursos gràfics, logotips i icones.

-----

## 🚀 Tecnologies utilitzades

  * **Llenguatge:** C\# (.NET Core)
  * **Kernel Base:** Cosmos Kit
  * **Arquitectura:** x86/x64
  * **Àudio:** Cosmos Audio Driver (PCSpeaker/AudioMixer)

-----

## 🆕 Funcionalitats recents

### 🔊 Sistema d'Àudio (Pit Wall Radio)

Hem implementat la gestió de so per millorar el *feedback* de l'usuari, seguint la guia oficial de COSMOS:

  * **Startup Sound:** Una melodia de benvinguda al carregar el sistema.
  * **Command Success:** Notificació sonora quan una comanda s'executa correctament.
  * **Error Alert:** Senyal acústic per indicar que una operació ha fallat.

### 🎨 Identitat Visual (ASCII Logo)

  * **Branding d'inici:** Ara, en arrencar el sistema, es desplega un logotip en **ASCII Art** que reforça la identitat de F1OS abans de donar el control a l'usuari.

### 📂 Gestió de Fitxers i Directoris

  * Suport per a la creació, llistat (`ls`), eliminació i navegació entre directoris.

### 🧮 Unitat Aritmètica (Engine Stats)

  * Nova comanda `calc` amb suport per a: Suma, Resta, Multiplicació, Divisió, Mòdul (`mod`) i Arrel quadrada (`sqrt`).

### ⌨️ Configuració i Energia

  * **Teclat:** Suport total per al layout estàndard espanyol.
  * **Energia:** Control de tancament (`retire`) i reinici (`restart`) del sistema.

-----

## 📄 Llicència

Aquest projecte està sota la llicència **MIT**. Pots consultar el fitxer `LICENSE` per a més detalls.

-----