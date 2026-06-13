
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

## Catálogo y Firma de Endpoints (API JSON)

| Método | Ruta HTTP | Descripción | Parámetros Requeridos |
| :--- | :--- | :--- | :--- |
| **GET** | `/` | Pantalla de bienvenida con estado del servicio e instrucciones de la API. | Ninguno |
| **POST** | `/api/paquetes/nuevo` | Registra un envío en SQLite, calcula costos y genera código `ENV-`. | **Query:** `ubicacionInicial`<br>**Body (JSON):** Datos del paquete |
| **GET** | `/api/paquetes/rastreo/{codigo}` | Recupera la información de un paquete y su árbol de historial completo. | **Path:** Código de rastreo |
| **POST** | `/api/pruebaapi/actualizar` | Procesa cambios de estado, controla flujos de negocio e intentos fallidos. | **Query:** `codigo`, `nuevoEstadoId`, `ubicacion`, `esIntentoFallido` |
| **GET** | `/api/pruebaapi/multiples-intentos` | Endpoint de auditoría. Retorna los paquetes en estado crítico o con fallos. | Ninguno |
| **GET** | `/api/pruebaapi/departamentos` | Recupera los 18 departamentos de Guatemala almacenados en la semilla. | Ninguno |