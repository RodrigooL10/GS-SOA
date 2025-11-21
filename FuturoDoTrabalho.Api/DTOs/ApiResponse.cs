namespace FuturoDoTrabalho.Api.Dtos
{
    // ====================================================================================
    // DTO: API RESPONSE - Resposta Genérica<T>
    // ====================================================================================
    // Define a estrutura padrão de resposta para TODOS os endpoints da API
    // Garante consistência nas respostas: sempre com success, message, data e timestamp
    // Padrão RESTful que facilita tratamento de erros e sucesso no cliente
    // ====================================================================================
    public class ApiResponse<T>
    {
        // ==================== PROPRIEDADES ====================
        /// <summary>Indica se a operação foi bem-sucedida (true/false)</summary>
        public bool Success { get; set; }

        /// <summary>Mensagem descritiva: sucesso, erro ou validação</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Dados retornados pela operação (genérico T)</summary>
        public T? Data { get; set; }

        /// <summary>Timestamp em UTC quando a resposta foi gerada</summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ====================
        // FACTORY METHOD: SuccessResponse
        // Creates a successful API response with the provided data and message
        // ====================
        public static ApiResponse<T> SuccessResponse(T? data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        // ====================
        // FACTORY METHOD: ErrorResponse
        // Creates an error API response with the provided error message and optional data
        // ====================
        public static ApiResponse<T> ErrorResponse(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    // ====================
    // CLASS: ApiResponse (non-generic)
    // Represents a standard API response without generic data payload
    // Used for operations that don't need to return specific data (like delete or update status checks)
    // ====================
    public class ApiResponse
    {
        // ====================
        // PROPERTIES
        // ====================
        // Indicates whether the operation completed successfully
        public bool Success { get; set; }

        // Descriptive message about the operation result (success or error details)
        public string Message { get; set; } = string.Empty;

        // UTC timestamp when the response was generated
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ====================
        // FACTORY METHOD: SuccessResponse
        // Creates a successful API response with the provided message
        // ====================
        public static ApiResponse SuccessResponse(string message = "Operação realizada com sucesso")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }

        // ====================
        // FACTORY METHOD: ErrorResponse
        // Creates an error API response with the provided error message
        // ====================
        public static ApiResponse ErrorResponse(string message)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
