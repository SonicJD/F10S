# 🏎️ F1OS (Formula 1 Operating System)

![Logo de F1OS](assets/logo.png)

## 🏁 Descripció
**F1OS** és un sistema operatiu de 64 bits desenvolupat sobre el framework **Cosmos (C# Open Source Managed Operating System)**. 

Inspirat en l'enginyeria de precisió de la Fórmula 1, aquest sistema busca la màxima optimització, velocitat de resposta i una arquitectura modular. El projecte ha fet el gran salt de l'entorn de text abstracte a una interfície completament visual mitjançant el **Cosmos Graphic Subsystem (GUI)**, integrant a més una pila de xarxa funcional amb suport per a transferència de fitxers en temps real mitjançant un **Servidor FTP**.

---

## 👥 Membres del Grup (The Pit Wall)
* **Jefferson Méndez** 🏎️ (Enginyer de Telemetria i Sistemes Gràfics)
* **Biel Duran** 🔧 (Enginyer de Motor de Fitxers i Xarxa)

---

## 💻 Arquitectura i Anàlisi Tècnica del Kernel

El nucli del sistema (`Kernel.cs`) s'ha estructurat seguint les demandes del subsistema gràfic i el comportament síncron dels serveis de xarxa de Cosmos. A continuació s'expliquen els pilars tècnics de la implementació actual:

### 1. Inicialització d'Infraestructura (`BeforeRun`)
Abans d'arrancar el bucle principal, el sistema munta l'ecosistema en memòria:
* **Sistema de Fitxers Virtual (VFS):** S'instancia `CosmosVFS` i es registra a través del `VFSManager` per mapar la unitat del disc de memòria (`0:\`).
* **Subsistema Gràfic de Cosmos:** S'aixeca un buffer de pantalla completa (`FullScreenCanvas`) configurat amb una resolució nativa de **1024x768 píxels** i una profunditat de color de **32 bits (ColorDepth32)** per admetre el renderitzat de paletes RGB riques.
* **Xarxa Estàtica:** Es crida al mètode `NetworkInit()` el qual detecta la targeta de xarxa virtual (`NetworkDevice.Devices`). S'assigna una **IP estàtica (192.168.93.2)** mitjançant `IPConfig.Enable()` juntament amb la seva màscara de xarxa i la porta d'enllaç per a comunicacions externes.

### 2. Màquina d'Estats del Bucle Principal (`Run`)
El mètode `Run()` actua com el motor de cicles del cotxe. Està dividit en **3 estats d'execució exclusius** gestionats per banderes booleanes:

* **Estat 1: Warm-up Lap (Boot Screen):** Si `bootScreen == true`, es bloqueja la pantalla renderitzant l'escut ASCII de F1OS en color vermell escuderia. El sistema espera de forma asíncrona un esdeveniment de teclat (`ConsoleKeyEx.Enter`) per alliberar el sistema, activar l'àudio de benvinguda i passar al mode normal.
* **Estat 2: Pit Lane Mode (FTP Server Listening):** Una limitació inherent de la llibreria `CosmosFtpServer` és que el mètode `.Listen()` és **bloquejant**. Quan l'usuari executa `ftpstart`, la bandera `ftpListening` s'activa. El bucle `Run()` desvia tot el flux a aquest estat, pinta la interfície gràfica d'espera FTP i crida a `ftpServer.Listen()`. El sistema es congela intencionadament en aquest punt processant les peticions de paquets del client (com *FileZilla*). Quan el client es desconnecta, el mètode retorna el control, es destrueix la instància amb `.Dispose()` i el sistema torna de forma segura al mode terminal gràfic.
* **Estat 3: Grand Prix Mode (Terminal Normal):** Si no hi ha pantalles de booteig ni servidors actius, el Kernel llegeix l'entrada de teclat (`HandleKeyboard`) gestionant caràcters i esdeveniments de retrocés (`Backspace`), i pinta la línia de comandes gràfica reflectint el buffer del terminal gràfic a 22 píxels de salt de línia.

---

## 🏁 Guia de Comandes per a la Presentació

Aquesta és la llista de comandes de telemetria i control que es poden testejar en directe durant la demo del sistema operatiu, dividides per la seva responsabilitat a boxes:

### ⚙️ Comandes de Sistema i Telemetria
| Comanda | Descripció Tècnica | Metàfora F1 |
| :--- | :--- | :--- |
| `team` | Mostra la versió actual del sistema operatiu (F1OS v1.0). | Identificació de l'Escuderia |
| `telemetry` | Comprova l'estat del sistema, la estabilitat de la RAM i si el FTP està online o offline. | Telemetria del xassís |
| `lap` | Imprimeix la data i l'hora exacta del sistema a través del rellotge de temps real. | Cronòmetre de Volta |
| `ip` | Mostra l'adreça IP estàtica configurada a la interfície de xarxa. | Ràdio d'equip (Canal de dades) |

### 📂 Comandes del Sistema de Fitxers (Garatge de Dades)
| Comanda | Descripció Tècnica | Metàfora F1 |
| :--- | :--- | :--- |
| `grid` | Llista tots els directoris `[D]` i fitxers `[F]` de la ruta actual (equivalent a `ls`). | Graella de Sortida (Grid) |
| `drs <ruta>` | Canvia el directori de treball actual (equivalent a `cd`). Admet l'ús de `..` per retrocedir. | Obertura de DRS (Avançar/Moure's) |
| `build <nom>` | Crea un nou directori en la ruta actual (equivalent a `mkdir`). | Construcció de components |
| `crash <nom>` | Elimina un directori de forma recursiva del disc (equivalent a `rmdir`). | Accident (Eliminació del cotxe) |
| `engine <fitxer> <text>`| Crea o sobreescriu un fitxer de text amb el contingut especificat. | Forjar el Motor (Escriure codi) |
| `radio <fitxer>` | Llegeix i mostra per pantalla el contingut d'un fitxer de text (equivalent a `cat`). | Missatge de Ràdio (Escoltar ràdio) |
| `rm <fitxer>` | Elimina un fitxer de text del sistema de fitxers. | Retirar peça defectuosa |

### 🌐 Comandes de Xarxa i Conexió Externa
| Comanda | Descripció Tècnica | Metàfora F1 |
| :--- | :--- | :--- |
| `ftpstart` | Activa el servidor FTP a la carpeta `0:\ftp` i congela el sistema en espera d'un client extern. | Obrir el pit lane per a descàrrega |
| `ftpstatus` | Informa de l'estat actual, adreça IP i directori arrel del servidor FTP. | Panell de dades de l'enginyer |
| `ftpstop` | Atura el servidor de xarxa i tanca els ports d'escolta d'infraestructura FTP. | Tancar el garatge de telemetria |

### 🛠️ Eines i Memòria (Race History)
| Comanda | Descripció Tècnica | Metàfora F1 |
| :--- | :--- | :--- |
| `calc <op> <n1> <n2>` | Calculadora gràfica que processa operacions aritmètiques básiques: `sum`, `sub`, `mult` i `div`. | Ordinador de consum de benzina |
| `history` | Mostra exactament l'historial de les últimes **5 comandes** emmagatzemades al buffer. | Repetició de telemetria històrica |
| `repeat <índex>` | Executa de nou de forma automàtica la comanda allotjada a la posició de l'historial indicada. | Recomanació de l'estratègia |
| `pitstop` | Neteja per complet totes les línies de missatges impreses al terminal (equivalent a `cls`). | Parada a Boxs (Neteja de pneumàtics) |
| `briefing` | Mostra la guia ràpida de comandes ordenades per categories en la interfície gràfica. | Reunió de pilots (Briefing) |

### 🔌 Control d'Energia
| Comanda | Descripció Tècnica | Metàfora F1 |
| :--- | :--- | :--- |
| `restart` | Tanca l'entorn de manera segura i força un reinici de la màquina (`Power.Reboot()`). | Reiniciar el mapa de motor |
| `retire` | Atura el maquinari i apaga completament l'equip (`Power.Shutdown()`). | Retirar el cotxe del Gran Premi |

---
