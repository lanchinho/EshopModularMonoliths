using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions.Handler;

public sealed class CustomexceptionHandler(IProblemDetailsService problemDetailsService,
	ILogger<CustomexceptionHandler> logger)
	: IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
	{
		logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}",
			exception.Message, DateTime.UtcNow);

		(string Details, string Title, int StatusCode) = exception switch
		{
			InternalServerException =>
			(
				exception.Message,
				exception.GetType().Name,
				context.Response.StatusCode = StatusCodes.Status500InternalServerError
			),
			ValidationException =>
			(
				exception.Message,
				exception.GetType().Name,
				context.Response.StatusCode = StatusCodes.Status400BadRequest
			),
			BadRequestException =>
			(
				exception.Message,
				exception.GetType().Name,
				context.Response.StatusCode = StatusCodes.Status400BadRequest
			),
			NotFoundException =>
			(
				exception.Message,
				exception.GetType().Name,
				context.Response.StatusCode = StatusCodes.Status404NotFound
			),
			_ =>
			(
				exception.Message,
				exception.GetType().Name,
				context.Response.StatusCode = StatusCodes.Status500InternalServerError
			)
		};

		var problemDetails = new ProblemDetails
		{
			Title = Title,
			Detail = Details,
			Status = StatusCode,
			Instance = context.Request.Path
		};

		if (exception is ValidationException validationException)
			problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = context,
			Exception = exception,
			ProblemDetails = problemDetails
		});
	}
}
