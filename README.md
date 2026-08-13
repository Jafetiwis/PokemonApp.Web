# Pokédex Web App (.NET 8 MVC)

Aplicación web desarrollada en .NET 8 que consume la **PokeApi** para gestionar un catálogo 
interactivo de Pokémon con capacidades de filtrado, paginación a nivel de servidor, exportación 
a Excel y envío de notificaciones por correo electrónico en formato HTML.

---

### Guía de configuración y Envío de Correos

Para probar el funcionamiento del módulo de correo electrónico y el servicio SMTP: 

1. Abre el archivo **`appsettings.json`** ubicado en la raíz del proyecto.
2. Localiza la sección *`SmtpSettings`**:
	```json
	"SmtpSettings": {
	"Server": "smtp.gmail.com",
	"Port": 587,
	"SenderEmail": "tu_correo@gmail.com"
	"Password": "tu_contraseña_de_aplicacion"
	}

# Instrucciones para credenciales reales:

SenderEmail: Ingresa tu dirección de correo electrónico(ej. Gmail)

Password: Debes generar e ingresar una Contraseña de Aplicación de 16 
caracteres proporcionada por tu proveedor de correo (en el caso de Gmail, 
se genera desde la sección de seguridad de tu cuenta Google con la verificación 
en dos pasos activada). No utilices tu contraseña personal habitual.

//Enlace myaccount.google.com/security

En la barra de busqueda de la parte superior busca y selecciona Contraseñas de aplicaciones. 
Es posible que te pida volver a escribir tu contraseña de Google por seguridad.

En la pantalla que aparece, verás un formulario para crear una nueva contraseña:
En el campo Seleccionar aplicación, elige Otra (nombre personalizado).
Escribe un nombre para identificarla, por ejemplo: PokedexApp

Dar click al botón de Generar.
Se generará una contraseña de 16 letras, esas se deben pegar en password del appsettings.json.

Poner su correo en la clase HomeController.cs, línea 158

Nota de seguridad: Si no se configuran credenciales válidas, el sistema cuenta 
con un bloque try-catch de protección que atrapará la excepción SMTP de forma segura, 
evitando caídas en el servidor local y notificando la alerta visualmente en la interfaz.

# Desiciones Técnicas y Justificación

1. Elección de la líbrería de Excel: ClosedXML.
Se optó por utilizar ClosedXML para la generación de reportes .xlsx.

2. ¿Por qué se eligió?
Permite manipular archivos de Excel de manera sumamente intuitiva mediante una API 
orientada a objetos en C#, sin necesidad de tener Office instalado en el servidor y 
con un rendimiento excelente en memoria a través de MemoryStream.

3. Alternativas descartadas:
Microsoft.Office.Interop.Excel: Se descartó por completo porque requiere que el servidor 
host tenga instalado Office, además de consumir una cantidad excesiva de recursos y generar 
bloqueos de hilos (COM Interop).

EPPlus: Se descartó debido a los cambios recientes en su modelo de licenciamiento comercial 
(licencia non-commercial vs comercial), lo que la vuelve menos viable para entornos de producción corporativos.


Estrategia de paginación manual a nivel de servidor.
1. Se implementó la paginación y el filtrado por nombre/tipo utilizando saltos y límites directos
sobre los listados de la API.

2. ¿Por qué se eligió?
La PokeAPI maneja catálogos extensos pero ofrece endpoints estructurados por tipo y parámetros de paginación 
(limit y offset). Procesar esto de forma controlada evita saturar el navegador del cliente con miles de registros de golpe.

3. Alternativas descartadas:
Cargar todos los 1300+ Pokémon de golpe en el cliente: Se descartó porque incrementa drásticamente
el tiempo de carga inicial (First Contentful Paint) y consume memoria innecesaria en el DOM del navegador.


Gestión y resiliencia ante Timeouts de la PokeAPI
1. Me decidí por el uso de HttpClient tipado mediante inyección de dependencias combinado con validaciones de
códigos de estado (IsSuccessStatusCode) y bloques de protección en las peticiones asíncronas.

2. ¿Por qué se eligió?:
Garantiza que si la PokeAPI experimenta latencia o intermitencia temporal, la aplicación web no se congele 
ni propague un error crítico sin control al usuario final.

3. Alternativas descartadas:
Llamadas bloqueantes síncronas (.Result o .Wait()): Descartadas por completo ya que bloquean los hilos 
del pool de servidores web (Thread Starvation), reduciendo la concurrencia de la aplicación.