using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace gaco_api.Models.DTOs.Responses
{
    public class GenericResponse<T>
    {
        public bool isError { get; set; }
        public T? Object { get; set; }
        public string? MsgError { get; set; }

        public static GenericResponse<List<string>> FromModelState(ModelStateDictionary modelState)
        {
            var errores = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new GenericResponse<List<string>>
            {
                isError = false,
                MsgError = "Error en los datos enviados.",
                Object = errores
            };
        }
    }
}
