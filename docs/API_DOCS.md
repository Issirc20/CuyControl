# Documentación de API

## Base URL

```
http://localhost:5000/api
```

## Autenticación

Los endpoints requieren token JWT o sesión autenticada.

Ejemplo de header:
```
Authorization: Bearer {token}
```

## Endpoints Disponibles

### Dashboard API

#### GET /api/dashboard/stats
Obtiene estadísticas principales.

Response 200:
```json
{
  "uptime": "string",
  "users": 0,
  "totalCuyes": 0,
  "totalVentas": 0
}
```

### Cuyes

#### GET /api/cuyes
Obtiene lista de cuyes.

Query Parameters:
- skip (int): Paginación
- take (int): Cantidad

Response 200:
```json
[
  {
	"id": 1,
	"codigo": "C001",
	"sexo": "M",
	"peso": 1.5,
	"estado": "Activo"
  }
]
```

#### POST /api/cuyes
Crear nuevo cuy.

Body:
```json
{
  "codigo": "C002",
  "sexo": "H",
  "peso": 1.2,
  "jaulaId": 1
}
```

### Ventas

#### GET /api/ventas
Obtiene ventas registradas.

#### POST /api/ventas
Registra nueva venta.

Body:
```json
{
  "cuyId": 1,
  "cantidad": 1,
  "precioUnitario": 50.00,
  "nombreComprador": "Juan Perez"
}
```

## Códigos HTTP

- 200: OK
- 201: Creado
- 400: Solicitud inválida
- 401: No autorizado
- 403: Acceso denegado
- 404: No encontrado
- 500: Error del servidor

