# Exception Handling Middleware

A flexible and customizable exception handling middleware for ASP.NET Core applications that provides consistent error responses across all services.

## Features

- ✅ Global exception handling for all API requests
- ✅ Consistent API response format using `ApiResponse` wrapper
- ✅ Automatic mapping of custom `AppException` types to HTTP status codes
- ✅ Support for standard .NET exceptions (ArgumentException, UnauthorizedAccessException, etc.)
- ✅ Configurable exception details exposure (for development vs production)
- ✅ Custom exception handlers and loggers
- ✅ Stack trace inclusion control
- ✅ Error code to HTTP status code mapping

## Quick Start

### Basic Usage

In your `Program.cs`, add the middleware after routing but before authorization:

```csharp
var app = builder.Build();

// Add exception handling middleware
app.UseExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Environment-Specific Configuration

**Development Environment:**
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandlingDevelopment(); // Shows detailed errors
}
else
{
    app.UseExceptionHandlingProduction(); // Hides sensitive details
}
```

### Custom Configuration

```csharp
app.UseExceptionHandling(options =>
{
    options.IncludeExceptionDetails = app.Environment.IsDevelopment();
    options.IncludeStackTrace = app.Environment.IsDevelopment();
    options.ExposeGenericExceptionMessages = false;
    options.IncludeJsonIndentation = true;
});
```

## Configuration Options

### ExceptionHandlingOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludeExceptionDetails` | `bool` | `false` | Include detailed exception information in response |
| `IncludeStackTrace` | `bool` | `false` | Include stack trace in error response |
| `ExposeGenericExceptionMessages` | `bool` | `false` | Expose exception messages for non-AppException errors |
| `IncludeJsonIndentation` | `bool` | `false` | Format JSON output with indentation |
| `ErrorCodeMapper` | `Func<int, int>?` | `null` | Custom function to map error codes to HTTP status codes |
| `CustomExceptionHandler` | `Func<...>?` | `null` | Custom handler to modify response before sending |
| `CustomLogger` | `Action<...>?` | `null` | Custom logging function |

## Response Format

All exceptions are converted to the standard `ApiResponse` format:

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "result": { /* data */ },
  "error": null
}
```

### Error Response (Production)
```json
{
  "success": false,
  "message": "Resource not found",
  "result": null,
  "error": null
}
```

### Error Response (Development with details)
```json
{
  "success": false,
  "message": "Resource not found",
  "result": null,
  "error": {
    "type": "NotFoundException",
    "errorCode": 404,
    "detail": { "resourceId": "123" },
    "stackTrace": "..."
  }
}
```

## Exception Types

### Built-in AppException Types

All these exceptions automatically map to appropriate HTTP status codes:

- `BadException` → 400 Bad Request
- `UnauthorizedException` → 401 Unauthorized
- `ForbiddenException` → 403 Forbidden
- `NotFoundException` → 404 Not Found
- `ValidationException` → 400 Bad Request

**Example Usage:**
```csharp
public async Task<WordDto> GetWordById(int id)
{
    var word = await _context.Words.FindAsync(id);
    if (word == null)
    {
        throw new NotFoundException($"Word with ID {id} not found", new { wordId = id });
    }
    return _mapper.Map<WordDto>(word);
}
```

### Standard .NET Exceptions

The middleware also handles standard .NET exceptions:

- `UnauthorizedAccessException` → 401
- `ArgumentException` / `ArgumentNullException` → 400
- `InvalidOperationException` → 400
- `NotImplementedException` → 501
- `TimeoutException` → 408
- All others → 500 Internal Server Error

## Advanced Customization

### Custom Error Code Mapping

Map your internal error codes to HTTP status codes:

```csharp
app.UseExceptionHandling(options =>
{
    options.ErrorCodeMapper = errorCode => errorCode switch
    {
        1001 => 400, // Business validation error
        1002 => 409, // Conflict
        2001 => 503, // Service unavailable
        _ => errorCode // Use original code
    };
});
```

### Custom Exception Handler

Modify the response before it's sent to the client:

```csharp
app.UseExceptionHandling(options =>
{
    options.CustomExceptionHandler = async (context, exception, statusCode, response) =>
    {
        // Add custom headers
        context.Response.Headers.Add("X-Error-Id", Guid.NewGuid().ToString());
        
        // Log to external service
        await LogToExternalService(exception);
        
        // Modify response for specific cases
        if (exception is DatabaseException)
        {
            return (503, new ApiResponse("Database temporarily unavailable", null));
        }
        
        return null; // Use default handling
    };
});
```

### Custom Logger

Implement custom logging logic:

```csharp
app.UseExceptionHandling(options =>
{
    options.CustomLogger = (exception, statusCode, context, logger) =>
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;
        
        logger.Log(
            logLevel,
            exception,
            "API Error: {Method} {Path} - {StatusCode} - {UserId}",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            context.User?.Identity?.Name ?? "Anonymous"
        );
    };
});
```

## Service-Specific Customization

Each service can customize the middleware behavior in its `Program.cs`:

**Content Service Example:**
```csharp
app.UseExceptionHandling(options =>
{
    options.IncludeExceptionDetails = app.Environment.IsDevelopment();
    options.IncludeStackTrace = false; // Never expose stack traces
    options.CustomExceptionHandler = async (ctx, ex, code, resp) =>
    {
        // Content service specific handling
        if (ex is ContentValidationException)
        {
            // Custom handling for content validation
            return (422, new ApiResponse("Content validation failed", ex.Message));
        }
        return null;
    };
});
```

**Auth Service Example:**
```csharp
app.UseExceptionHandling(options =>
{
    options.ExposeGenericExceptionMessages = false; // Extra security
    options.CustomLogger = (ex, code, ctx, logger) =>
    {
        // Log auth-specific details
        logger.LogWarning(
            "Auth Error: {Code} - IP: {IP} - User: {User}",
            code,
            ctx.Connection.RemoteIpAddress,
            ctx.User?.Identity?.Name ?? "Unknown"
        );
    };
});
```

## Best Practices

1. **Use AppException derivatives** for business logic errors with known status codes
2. **Configure for environment**: Detailed errors in development, minimal in production
3. **Custom logging**: Implement service-specific logging requirements
4. **Security**: Never expose stack traces or sensitive data in production
5. **Consistent responses**: Let the middleware handle all exceptions for consistency

## Example Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;
    
    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TopicDto>>> GetTopic(int id)
    {
        // No try-catch needed - middleware handles all exceptions
        var topic = await _topicService.GetTopicById(id);
        return Ok(ApiResponse<TopicDto>.New(topic, "Topic retrieved successfully"));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TopicDto>>> CreateTopic(CreateTopicDto dto)
    {
        // ValidationException thrown by service will be caught by middleware
        var topic = await _topicService.CreateTopic(dto);
        return Ok(ApiResponse<TopicDto>.New(topic, "Topic created successfully"));
    }
}
```

## Migration from Existing Code

1. **Remove try-catch blocks** from controllers (middleware handles exceptions)
2. **Replace custom error responses** with throwing appropriate exceptions
3. **Add middleware** to `Program.cs`
4. **Configure options** based on environment
5. **Test** all error scenarios

**Before:**
```csharp
try
{
    var word = await _context.Words.FindAsync(id);
    if (word == null)
        return NotFound(new { message = "Word not found" });
    return Ok(word);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting word");
    return StatusCode(500, new { message = "Internal error" });
}
```

**After:**
```csharp
var word = await _context.Words.FindAsync(id);
if (word == null)
    throw new NotFoundException($"Word with ID {id} not found");
return Ok(ApiResponse<Word>.New(word));
```

## Troubleshooting

**Q: Middleware not catching exceptions?**  
A: Ensure middleware is registered early in the pipeline, before `UseAuthentication()` and `UseAuthorization()`.

**Q: Getting 500 instead of custom status code?**  
A: Make sure you're throwing `AppException` derivatives, not generic `Exception`.

**Q: Stack traces showing in production?**  
A: Set `IncludeStackTrace = false` and `IncludeExceptionDetails = false` in production configuration.

## License

Part of the Genzy Base Library.
