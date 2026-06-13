# Despliegue en Producción (Render)

La API se encuentra completamente desplegada, configurada y operativa en la nube a través de la infraestructura de Render.

* **Enlace Base de la API:**   [https://examenfinal-analisissistemas.onrender.com](https://examenfinal-analisissistemas.onrender.com)

### 1. Estado de la Aplicación (Health Check)
* **Método:** `GET`
* **URL:** `https://examenfinal-analisissistemas.onrender.com/`
* **Descripción:** Devuelve un JSON con el estado operacional del sistema, la confirmación de la conexión a la base de datos SQLite y el listado de endpoints disponibles.

### 2. Registrar un Nuevo Paquete
* **Método:** `POST`
* **URL:** `https://examenfinal-analisissistemas.onrender.com/api/paquetes`
---

# Historias de Usuario (Product Backlog)

### HU01: Generación de Código de Rastreo Único
* **Como:** Operador del Sistema / Administrador.
* **Quiero:** Que el sistema asigne automáticamente un código único con el formato `ENV-YYYYMMDD-XXXX` al registrar un paquete.
* **Para:** Identificar de forma inequívoca cada envío a nivel nacional sin riesgo de duplicidad.
* **Criterios de Aceptación:**
  * El formato debe iniciar obligatoriamente con el prefijo `ENV-`.
  * Debe capturar la fecha actual del servidor en formato compacto (`YYYYMMDD`).
  * Debe incluir un correlativo secuencial diario de 4 dígitos (`XXXX`).

### HU02: Cálculo Automático del Costo de Envío
* **Como:** Operador de Oficina.
* **Quiero:** Que el sistema calcule automáticamente el costo del envío al procesar el departamento de destino.
* **Para:** Evitar la manipulación o errores manuales en los cobros y agilizar la atención en ventanilla.
* **Criterios de Aceptación:**
  * El costo debe computarse en caliente según la regla del `departamentoDestinoId` enviado.
  * El paquete no puede persistirse en la base de datos si el costo es menor o igual a cero.

### HU03: Registro del Historial de Ubicación Inicial
* **Como:** Operador de Oficina.
* **Quiero:** Que al registrar un nuevo paquete se cree obligatoriamente su primer movimiento en el historial.
* **Para:** Saber con exactitud en qué oficina física o central regional se capturó y resguardó el paquete por primera vez.
* **Criterios de Aceptación:**
  * El endpoint de creación debe exigir obligatoriamente una cadena para la ubicación geográfica actual.
  * Se debe insertar una fila en la tabla `HistorialEstados` vinculada al estado de control "Registrado" (Id = 1) de forma automática.

### HU04: Rastreo en Tiempo Real (JSON)
* **Como:** Cliente / Destinatario.
* **Quiero:** Consultar el código de rastreo en el endpoint público del servidor.
* **Para:** Conocer el estado actual de mi paquete y auditar todo su recorrido histórico sin tener que llamar por teléfono a soporte.
* **Criterios de Aceptación:**
  * La petición de rastreo debe retornar el objeto del paquete junto con su colección interna completa de `HistorialEstados`.
  * Los estados en el historial deben devolverse ordenados cronológicamente por fecha (`FechaActualizacion`).
  * Si el código buscado no existe en SQLite, se debe responder un código de estado `HTTP 404 NotFound` con un mensaje claro.

### HU05: Cobertura Restringida a Departamentos Semilla
* **Como:** Administrador del Sistema.
* **Quiero:** Que el sistema cuente con una lista estricta de 18 departamentos de Guatemala precargados en la base de datos.
* **Para:** Validar que no se registren ni se cobren envíos hacia zonas o regiones fuera de la cobertura geográfica de la empresa.
* **Criterios de Aceptación:**
  * La base de datos debe autogenerar los 18 departamentos de cobertura al aplicar la migración inicial (`HasData`).
  * Si se intenta registrar un paquete con un ID de departamento inexistente, el motor relacional de SQLite debe rechazar la transacción por integridad referencial.

### HU06: Flujo Secuencial y Estricto de Estados
* **Como:** Coordinador de Logística.
* **Quiero:** Validar que las actualizaciones de los estados de los paquetes sigan un orden lógico predefinido.
* **Para:** Impedir que los operadores o repartidores marquen accidentalmente estados incongruentes (ej. de "Registrado" directo a "Entregado").
* **Criterios de Aceptación:**
  * El servicio de negocio debe verificar el peso u orden numérico del estado actual contra el propuesto antes de guardar cambios.
  * El flujo solo puede avanzar secuencialmente, bloqueando retrocesos ilógicos a menos que se trate de un desvío por devolución autorizada.

### HU07: Control de Intentos Fallidos de Entrega
* **Como:** Repartidor / Mensajero.
* **Quiero:** Marcar un cambio de estado indicando explícitamente si la entrega fue un "Intento Fallido".
* **Para:** Registrar la bitácora física de incidencias en la dirección de entrega de forma transparente.
* **Criterios de Aceptación:**
  * El endpoint de actualización debe admitir una bandera booleana (`esIntentoFallido = true`).
  * Cada intento fallido debe acumularse y registrarse con su respectiva ubicación física actual y observaciones del repartidor.

### HU08: Transición Automática a "En Devolución" al Tercer Fallo
* **Como:** Sistema Automatizado de Negocio.
* **Quiero:** Cambiar el estado del paquete automáticamente a "En Devolución" al procesar el tercer intento de entrega fallido.
* **Para:** Sacar el paquete de la ruta de distribución activa de inmediato y programar su retorno seguro al remitente.
* **Criterios de Aceptación:**
  * El contador de reintentos fallidos debe evaluarse de forma automática en cada actualización.
  * Al confirmarse el 3er fallo consecutivo, la API forzará el cambio del estado general del paquete a "En Devolución" (Id = 4), ignorando el estado que el repartidor haya enviado originalmente.

### HU09: Reporte de Alertas de Paquetes Críticos
* **Como:** Gerente de Operaciones.
* **Quiero:** Consultar un listado consolidado de todos los paquetes que registren reintentos de entrega o incidencias acumuladas.
* **Para:** Gestionar soluciones proactivas con los remitentes, mitigar pérdidas de paquetería y disminuir la tasa de reclamos.
* **Criterios de Aceptación:**
  * El endpoint de monitoreo debe realizar un filtrado dinámico sobre SQLite devolviendo solo paquetes con incidencias registradas.
  * Debe incluir una cabecera de conteo total (`conteo`) y el arreglo estructurado en formato JSON plano.

### HU10: Persistencia de Semillas de Estados de Control
* **Como:** Desarrollador / DevOps.
* **Quiero:** Que los 5 estados core del negocio ("Registrado", "EnReparto", "Entregado", "En Devolucion", "Devuelto") se inicialicen mediante migraciones.
* **Para:** Garantizar que los IDs de control lógico funcionen de manera idéntica en cualquier contenedor Docker, entorno de desarrollo local o producción en Render.
* **Criterios de Aceptación:**
  * Al arrancar la aplicación por primera vez en cualquier entorno limpio, la tabla `Estados` debe poblarse automáticamente con los registros del 1 al 5.
  * Los valores de prioridad lógica (`Orden`) deben persistirse de manera estricta para asegurar el correcto funcionamiento de las validaciones de flujo de la HU06.

---
Aquí tienes el informe completo estructurado exactamente en formato **README.md** con bloques de código Markdown listos para que los agregues directamente a tu repositorio o documento de entrega:

```markdown
# INFORME TÉCNICO: BITÁCORA DE DESARROLLO Y ASEGURAMIENTO DE CALIDAD
### 🎓 Ingeniería en Sistemas - Séptimo Semestre (UMG)
**Estudiante:** Cristian Chamo  
**Proyecto:** Examen Final – Sistema de Envíos Rápidos GT  

---

## 🤖 1. Identificación de la Inteligencia Artificial Utilizada
* **Modelo Utilizado:** Gemini (Modelo de lenguaje avanzado de Google).
* **Rol en el proyecto:** Copiloto de desarrollo (*Pair Programming*), depurador de entornos de ejecución de .NET y asesor de infraestructura en la nube.

---

## 📝 2. Detalle de Prompts Enviados y Flujo de Trabajo

A continuación, se presentan las instrucciones y capturas lógicas de los prompts reales utilizados para guiar la resolución de los problemas del proyecto:

### 🔹 Fase 1: Diagnóstico de Errores de Compilación y Versiones (.NET 9)
* **Contexto del Prompt:** Se le compartieron a la IA los logs de error de la consola donde el compilador fallaba debido a que la suite de herramientas locales (`Dotnet Tools`) arrastraba configuraciones experimentales en conflicto con el SDK estable de .NET 9, junto con dependencias rotas en las rutas relativas del archivo `.csproj`.
* **Respuesta y Solución:** La IA detectó el desfase y guio la edición manual de los metadatos del proyecto de pruebas. Se forzó el uso estricto de Entity Framework Core en su versión estable `9.0.0` y se reparó la referencia cruzada utilizando la ruta de escape de nivel `..\..\`.

### 🔹 Fase 2: Sincronización de Lógica de Negocio en Pruebas Unitarias (xUnit)
* **Contexto del Prompt:** Se envió el código base del simulador de persistencia. El sistema fallaba al evaluar la regla de negocio más importante (el cambio automático a estado *"En Devolución"* tras acumular 3 intentos de entrega fallidos) debido a la volatilidad de la base de datos en memoria.
* **Respuesta y Solución:** Se reestructuró la suite de pruebas unitarias (`PaqueteServiceTests.cs`). La IA aplicó técnicas de desacoplamiento de caché de entidades para obligar al motor `UseInMemoryDatabase` a evaluar los impactos de forma aislada y real, logrando exitosamente el estado verde (3/3 pruebas aprobadas).

### 🔹 Fase 3: Purga de Binarios y Preparación del Entorno
* **Contexto del Prompt:** *“no quiero subir los obj y bien, dame el comando para quitarlos”*
* **Respuesta y Solución:** Se proveyó un script nativo de automatización para la terminal de comandos de Windows (PowerShell) con el fin de desintegrar de forma masiva los directorios residuales del compilador local, aliviando el peso del proyecto antes de empaquetar:
```powershell
  Get-ChildItem -Directory -Recurse -Include bin, obj | Remove-Item -Recurse -Force

```

Adicionalmente, se configuró el archivo estricto de exclusión `.gitignore`.

### 🔹 Fase 4: Despliegue de la API para Consumo Externo

* **Contexto del Prompt:** *“Ya, ahora en render no usare swgagger pero mi catedratico tiene que hacer peticiones a la API, como le hago o como debe de aparecer”*
* **Respuesta y Solución:** La IA rediseñó el punto de entrada en el `Program.cs` reemplazando la amarra de red local de `localhost` por una directiva de escucha universal `http://*:{port}`, permitiendo que la API capture de forma dinámica la variable de entorno asignada por el balanceador de carga de Render.

---

## 🛠️ 3. Correcciones Realizadas (Historial de Errores vs. Soluciones)

| Error de Origen (Log de Consola) | Causa Raíz Encontrada | Solución Técnica Aplicada |
| --- | --- | --- |
| **`error CS0117 / CS1061`** | Propiedades inexistentes o cruzadas (`CostoEnvio` / `Precio`) en el armado manual de objetos del test. | Se saneó el mock de datos reduciendo la carga de inicialización únicamente a propiedades primitivas seguras (`Id` y `Nombre`). |
| **`warning xUnit2002`** | Uso inválido del analizador `Assert.NotNull()` sobre una tupla estructurada de valores nativos. | Se reemplazó por la aserción booleana explícita `Assert.True(resultado.Success)`. |
| **`Port Scan Timeout (Render)`** | Trabar la aplicación en un puerto IP cableado (`localhost:5000`) inaccesible para internet. | Configuración de mapeo dinámico de sockets mediante `Environment.GetEnvironmentVariable("PORT")`. |

---

## 🧠 4. Reflexión Académica del Estudiante

El desarrollo de este examen final reafirmó que el ciclo de vida del software va mucho más allá de escribir código que funcione a nivel local. Enfrentar errores de infraestructura, dependencias corruptas y diferencias entre entornos de desarrollo (Local) y producción (Nube) representa el verdadero reto de la ingeniería.

La interacción estratégica con la Inteligencia Artificial bajo la modalidad de **Pair Programming** fue un catalizador crítico. En lugar de automatizar ciegamente el trabajo, el uso de la IA me obligó a actuar como un arquitecto de software: analizando logs densos, interpretando el comportamiento de la memoria del servidor de pruebas de xUnit y tomando decisiones de optimización de DevOps (como la purga de las carpetas `bin` y `obj` mediante comandos y la implementación de un archivo `.gitignore` robusto).

El resultado final no es solo un código que aprueba el examen, sino una API REST limpia, escalable, con puertos elásticos configurados para entornos reales de nube y respaldada por un escudo de pruebas unitarias automatizadas que garantizan que las reglas críticas del negocio funcionen exactamente bajo cualquier condición de estrés.
---

## Catálogo y Firma de Endpoints (API JSON)

| Método | Ruta HTTP | Descripción | Parámetros Requeridos |
| :--- | :--- | :--- | :--- |
| **GET** | `/` | Pantalla de bienvenida con estado del servicio e instrucciones de la API. | Ninguno |
| **POST** | `/api/paquetes/nuevo` | Registra un envío en SQLite, calcula costos y genera código `ENV-`. | **Query:** `ubicacionInicial`<br>**Body (JSON):** Datos del paquete |
| **GET** | `/api/paquetes/rastreo/{codigo}` | Recupera la información de un paquete y su árbol de historial completo. | **Path:** Código de rastreo |
| **POST** | `/api/pruebaapi/actualizar` | Procesa cambios de estado, controla flujos de negocio e intentos fallidos. | **Query:** `codigo`, `nuevoEstadoId`, `ubicacion`, `esIntentoFallido` |
| **GET** | `/api/pruebaapi/multiples-intentos` | Endpoint de auditoría. Retorna los paquetes en estado crítico o con fallos. | Ninguno |
| **GET** | `/api/pruebaapi/departamentos` | Recupera los 18 departamentos de Guatemala almacenados en la semilla. | Ninguno |