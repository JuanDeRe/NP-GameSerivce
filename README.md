# Nightwatch Protocol - GameService

https://github.com/JuanDeRe/NP-GameService/actions/workflows/ci.yml/badge.svg](https://github.com/JuanDeRe/NP-GameService/actions/workflows/ci.yml)

`GameService` es el servicio autoritativo encargado de administrar el estado de las partidas de Nightwatch Protocol.

Este servicio mantiene en memoria las partidas activas, valida acciones de gameplay y calcula el estado oficial de los jugadores. El cliente no envía posiciones finales, sino intenciones de acción; el GameService valida esas acciones y devuelve eventos autoritativos.

## Responsabilidades principales

- Crear partidas.
- Mantener estado activo de partidas en memoria.
- Registrar jugadores y roles.
- Validar movimiento de animatrónicos.
- Calcular posición oficial.
- Validar límites del mapa.
- Bloquear movimiento inválido.
- Exponer endpoints internos para pruebas y comunicación con otros servicios.

## Tecnologías

- C# / .NET
- ASP.NET Core Web API
- xUnit para pruebas unitarias

## Estructura

```text
GameService/
├── Controllers/
├── Contracts/
├── Factories/
├── Models/
├── Services/
├── Stores/
├── test/
│   └── GameService.Tests/
├── GameService.csproj
├── GameService.sln
└── README.md
```