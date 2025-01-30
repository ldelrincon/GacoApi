using Microsoft.IdentityModel.Tokens;
using gaco_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using gaco_api.Models.DTOs.Responses.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace gaco_api.Customs
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;

        public Utilidades(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }

        public byte[] GetFileBytes(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("La ruta del archivo no puede estar vacía.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("El archivo no existe.", filePath);
            }

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al leer el archivo en {filePath}.", ex);
            }
        }

        public string EncriptarSHA256(string texto)
        {
            using(var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenerarJWT(UsuarioResponse usuario)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo!),
                new Claim(ClaimTypes.Name, $"{usuario.Nombres} {usuario.Apellidos}"!), // Opcional: nombre del usuario
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(10), // Tiempo de expiración
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public bool ValidarToken(string token)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
            };

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenExpiredException stee)
            {
                Console.WriteLine(stee.Message);
                return false;
            }
            catch (SecurityTokenInvalidSignatureException stise)
            {
                Console.WriteLine(stise.Message);
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GuardarArchivoBase64Async(string carpeta, string extension, string base64)
        {
            try
            {
                // Crear la carpeta si no existe
                //local
                //var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), carpeta);
                // produccion
                var rutaCarpeta = Path.Combine("/home/Evidencias", carpeta);
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                // Generar un nombre único para el archivo
                var nombreUnico = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                var rutaArchivo = $"{rutaCarpeta}/{nombreUnico}.{extension}";

                // Convertir el Base64 a bytes y guardar el archivo
                var bytes = Convert.FromBase64String(base64);
                await File.WriteAllBytesAsync(GetPhysicalPath(rutaArchivo), bytes);
                //await File.WriteAllBytesAsync(rutaArchivo, bytes);

                // return rutaArchivo;
                return $"{carpeta}/{nombreUnico}.{extension}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el archivo: {ex.Message}");
            }
        }

        public async Task<string> ObtenerBase64Async(string rutaArchivo)
        {
            //rutaArchivo = ObtenerUrlArchivo(rutaArchivo);

            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException("La ruta del archivo no puede estar vacía o ser nula.", nameof(rutaArchivo));
            }

            if (!File.Exists(rutaArchivo))
            {
                // throw new FileNotFoundException("El archivo especificado no existe.", rutaArchivo);
                return string.Empty;
            }

            try
            {
                // Leer el archivo como bytes
                byte[] archivoBytes = await File.ReadAllBytesAsync(rutaArchivo);

                // Convertir a Base64
                return Convert.ToBase64String(archivoBytes);
            }
            catch (Exception ex)
            {
                // Registrar el error si es necesario y lanzar una excepción más descriptiva
                throw new InvalidOperationException($"Error al convertir el archivo a Base64: {ex.Message}", ex);
            }
        }

        public decimal GetBase64SizeInKB(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return 0;
            }

            // Decodificar la cadena Base64 en un arreglo de bytes
            byte[] byteArray = Convert.FromBase64String(base64String);

            // Obtener el tamaño en bytes y convertir a KB
            decimal sizeInBytes = byteArray.Length;
            return sizeInBytes / 1024.0m;
        }

        public string GetFullUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                throw new ArgumentException("La ruta relativa no puede estar vacía.");

            // Obtener el esquema (http o https) y el host (incluido el puerto si es necesario)
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Combinar con la ruta relativa
            return $"{baseUrl}{relativePath}";
        }

        public string GetPhysicalPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) )
               return string.Empty;

            // Combinar ContentRootPath con la ruta relativa
            string physicalPath = Path.Combine(_environment.ContentRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            //if (!File.Exists(physicalPath))
            //{
            //    return string.Empty;
            //}
                return physicalPath;
        }

        //public string ObtenerUrlArchivo(string rutaRelativa)
        //{
        //    if (string.IsNullOrWhiteSpace(rutaRelativa))
        //    {
        //        throw new ArgumentException("La ruta relativa no puede estar vacía.", nameof(rutaRelativa));
        //    }

        //    // Normalizar la ruta relativa (reemplazar separadores de directorios)
        //    rutaRelativa = rutaRelativa.Replace("\\", "/");

        //    // URL base del servidor
        //    var request = _httpContextAccessor?.HttpContext?.Request;
        //    string urlBase = $"{request.Scheme}://{request.Host}";

        //    // Combinar la URL base con la ruta relativa
        //    return $"{urlBase}/{rutaRelativa}";
        //}
    }
}
