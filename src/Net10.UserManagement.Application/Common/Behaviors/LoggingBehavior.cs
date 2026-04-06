using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Net10.UserManagement.Application.Common.Behaviors;

public partial class LoggingBehavior<TRequest, TResponse> (ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        LogProcessingRequest(_logger, requestName);

        var stopwatch = Stopwatch.StartNew();
        
        try 
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();        
            LogHandledRequest(_logger, requestName, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogErrorHandlingRequest(_logger, ex, requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing {RequestName}")]
    private static partial void LogProcessingRequest(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handled {RequestName} in {ElapsedMilliseconds} ms")]
    private static partial void LogHandledRequest(ILogger logger, string requestName, long elapsedMilliseconds);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error handling {RequestName} in {ElapsedMilliseconds}ms")]
    private static partial void LogErrorHandlingRequest(ILogger logger, Exception ex, string requestName, long elapsedMilliseconds);
}
