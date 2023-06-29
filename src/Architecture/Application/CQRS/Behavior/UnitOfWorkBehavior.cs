using Architecture.Application.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.CQRS.Behavior
{
    public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

        public UnitOfWorkBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IBaseCommand)
                return await HandleCommand(request, next, cancellationToken);
            else
                return await next();
        }

        private async Task<TResponse> HandleCommand(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                if (_unitOfWork.HasActiveTransaction)
                    return await ContinueTransaction(request, next);
                else
                    return await BeginTransaction(request, next, cancellationToken);
            }
            catch (Exception ex)
            {
                var typeName = request.GetGenericTypeName();

                if (cancellationToken.IsCancellationRequested)
                    _logger.LogInformation(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);
                else
                    _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }

        private async Task<TResponse> BeginTransaction(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var transactionId = _unitOfWork.TransactionId!.Value;

            _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transactionId, typeName, request);

            var response = await next();

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            await _unitOfWork.CommitAsync(cancellationToken);
            sw.Stop();

            _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName} costs {ElapsedMilliseconds}ms", transactionId, typeName, sw.ElapsedMilliseconds);

            return response;
        }

        private async Task<TResponse> ContinueTransaction(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Continue transaction {TransactionId} for {CommandName} ({@Command})", _unitOfWork.TransactionId!.Value, typeName, request);

            var response = await next();

            return response;
        }
    }
}