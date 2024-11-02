using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace gaco_api.Models.DTOs.Responses
{
    public class DefaultResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // Método estático para generar la respuesta en caso de errores de ModelState.
        public static DefaultResponse<List<string>> FromModelState(ModelStateDictionary modelState)
        {
            var errores = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new DefaultResponse<List<string>>
            {
                Success = false,
                Message = "Error en los datos enviados.",
                Data = errores
            };
        }
    }
}
