# Guía de Testing

## Estructura

```
CuyControl.Tests/
├── Unit/
│   ├── Services/
│   ├── Repositories/
│   └── Validators/
├── Integration/
│   └── Controllers/
├── Fixtures/
└── Helpers/
```

## Ejecución

### Ejecutar todas las pruebas
```
dotnet test
```

### Ejecutar pruebas específicas
```
dotnet test --filter "FullyQualifiedName~CuyControl.Tests.Unit.Services"
```

### Con cobertura de código
```
dotnet test /p:CollectCoverageMetrics=true /p:CoverageFileName=coverage.xml
```

## Patrones

### Tests Unitarios

Usar AAA (Arrange, Act, Assert):
```csharp
[TestMethod]
public void CrearCuy_ConDatosValidos_Exitoso()
{
	// Arrange
	var cuy = new Cuy { Codigo = "C001", ... };

	// Act
	var resultado = _service.CrearCuy(cuy);

	// Assert
	Assert.IsTrue(resultado.Exitoso);
}
```

### Tests de Integración

```csharp
[TestClass]
public class CuyControllerTests
{
	private readonly ApplicationDbContext _context;
	private readonly CuyController _controller;

	[TestInitialize]
	public void Setup()
	{
		// Usar InMemory DB
		_context = new ApplicationDbContext(
			new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("TestDb")
				.Options
		);
	}
}
```

## Mocking

Usar Moq para dependencias:
```csharp
var mockRepo = new Mock<ICuyRepository>();
mockRepo.Setup(r => r.GetByIdAsync(1))
	.ReturnsAsync(new Cuy { Id = 1 });
```

## Cobertura Mínima

- Servicios: 80%
- Repositorios: 70%
- Controllers: 60%

## Análisis de Calidad

```
dotnet analyze
```

