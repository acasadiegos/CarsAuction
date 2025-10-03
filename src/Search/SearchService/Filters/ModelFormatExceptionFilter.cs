using Application.Commons.Bases.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;

namespace SearchService.Filters
{
    public class ModelFormatExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = new BaseResponse<object>();

            // Manejar errores de validación de modelo/binding
            if (context.Exception is JsonException ||
                context.Exception is FormatException ||
                context.ModelState?.IsValid == false)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Error en el formato de los datos enviados";
                response.Data = null;

                // Extraer errores del ModelState si están disponibles
                var errors = new List<ErrorDetail>();

                if (context.ModelState != null)
                {
                    foreach (var modelError in context.ModelState)
                    {
                        foreach (var error in modelError.Value.Errors)
                        {
                            errors.Add(new ErrorDetail
                            {
                                PropertyName = ExtractPropertyName(modelError.Key),
                                ErrorMessage = ExtractUserFriendlyMessage(error.ErrorMessage)
                            });
                        }
                    }
                }
                else
                {
                    // Si no hay ModelState, crear un error genérico
                    errors.Add(new ErrorDetail
                    {
                        PropertyName = "Request",
                        ErrorMessage = "Formato de datos inválido. Verifique los tipos de datos enviados."
                    });
                }

                response.Errors = errors;

                context.Result = new BadRequestObjectResult(response);
                context.ExceptionHandled = true;
                return;
            }
        }

        private string ExtractPropertyName(string modelKey)
        {
            // Extraer el nombre de la propiedad del path JSON (ej: "$.records" -> "records")
            if (modelKey.StartsWith("$."))
            {
                return modelKey.Substring(2);
            }
            return modelKey;
        }

        private string ExtractUserFriendlyMessage(string originalMessage)
        {
            // Convertir mensajes técnicos a mensajes más amigables
            if (originalMessage.Contains("could not be converted to System.Int32"))
            {
                return "El valor debe ser un número entero válido";
            }
            if (originalMessage.Contains("could not be converted to System.DateTime"))
            {
                return "El valor debe ser una fecha válida";
            }
            if (originalMessage.Contains("could not be converted to System.Boolean"))
            {
                return "El valor debe ser verdadero o falso";
            }

            // Mensaje genérico para otros casos
            return "Formato de dato inválido";
        }
    }
}
