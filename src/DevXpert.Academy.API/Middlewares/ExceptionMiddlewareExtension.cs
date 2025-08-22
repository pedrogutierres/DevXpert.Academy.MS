using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace DevXpert.Academy.API.Middlewares
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    ResponseError error;
                    HttpStatusCode errorCode = HttpStatusCode.InternalServerError;

                    var logger = loggerFactory.CreateLogger("ExceptionHandler");

                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature != null)
                    {
                        if (exceptionHandlerFeature.Error is BusinessException businessException)
                        {
                            logger.LogError($"Erro de negócio no servidor: {businessException.AgruparTodasAsMensagens()}");

                            errorCode = HttpStatusCode.BadRequest;

                            error = new ResponseError(
                                      "Erro de negócio",
                                      null,
                                      StatusCodes.Status400BadRequest,
                                      context.Request.HttpContext.Request.Path,
                                      businessException.Message);
                        }
                        else if (exceptionHandlerFeature.Error is ValidationException validationException)
                        {
                            logger.LogError($"Erro de validação de entidade no servidor: {validationException.AgruparTodasAsMensagens()}");

                            errorCode = HttpStatusCode.BadRequest;

                            var errors = new Dictionary<string, IEnumerable<string>>(1)
                            {
                                { validationException.Property, new List<string> { validationException.Message } }
                            };

                            error = new ResponseError(
                                      "Erro de negócio",
                                      null,
                                      StatusCodes.Status400BadRequest,
                                      context.Request.HttpContext.Request.Path,
                                      errors);
                        }
                        else if (exceptionHandlerFeature.Error is SqlException sqlException)
                        {
                            logger.LogError($"Erro de banco de dados: {sqlException.AgruparTodasAsMensagens()}");

                            string msgErro = sqlException.Number switch
                            {
                                -1 => "Sem conexão com o servidor de banco de dados.",
                                5 => "Muitas conexões em uso simultâneamente no servidor, tente novamente.",
                                _ => sqlException.Message
                            };

                            errorCode = HttpStatusCode.BadRequest;

                            error = new ResponseError(
                                      "Erro de banco de dados",
                                      null,
                                      StatusCodes.Status400BadRequest,
                                      context.Request.HttpContext.Request.Path,
                                      msgErro);
                        }
                        else
                        {

                            logger.LogCritical($"Erro interno do servidor: {exceptionHandlerFeature.Error.AgruparTodasAsMensagens()}");

                            var mensagemErro = env.IsProduction()
                                ? "Erro interno no servidor, contate o suporte"
                                : exceptionHandlerFeature.Error.AgruparTodasAsMensagens();

                            error = new ResponseError(
                                       "Erro interno no servidor",
                                       null,
                                       StatusCodes.Status500InternalServerError,
                                       context.Request.HttpContext.Request.Path,
                                       mensagemErro);
                        }
                    }
                    else
                    {
                        logger.LogCritical($"Erro inesperado não encontrou a exception");

                        error = new ResponseError(
                                   "Erro inesperado no servidor",
                                   null,
                                   StatusCodes.Status500InternalServerError,
                                   context.Request.HttpContext.Request.Path,
                                   "Erro interno no servidor, contate o suporte");
                    }

                    context.Response.StatusCode = (int)errorCode;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(error, 
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None }));
                });
            });
        }
    }
}
