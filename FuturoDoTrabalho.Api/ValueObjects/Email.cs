using System.Text.RegularExpressions;

namespace FuturoDoTrabalho.Api.ValueObjects
{
    // ====================================================================================
    // VALUE OBJECT: Email
    // ====================================================================================
    // Represents a validated email address as a domain entity
    // Ensures only valid emails can be created through validation in the Create method
    // Provides strong typing for email values throughout the application
    // ====================================================================================
    public class Email
    {
        // ====================
        // PROPERTIES
        // ====================
        // The validated email address (stored in lowercase and trimmed)
        public string Endereco { get; private set; }

        // ====================
        // CONSTANTS
        // ====================
        // Regular expression pattern for validating email format
        // Matches standard email pattern: something@something.something
        private static readonly string PatternEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // ====================
        // CONSTRUCTOR (Private)
        // Prevents direct instantiation - use Create() factory method instead
        // ====================
        private Email(string endereco)
        {
            Endereco = endereco.ToLower().Trim();
        }

        // ====================
        // FACTORY METHOD: Create
        // Validates and creates an Email instance if valid
        // Returns Result<Email> allowing caller to handle validation errors
        // ====================
        public static Result<Email> Create(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                return Result<Email>.Failure("Email não pode estar vazio");

            if (endereco.Length > 150)
                return Result<Email>.Failure("Email não pode exceder 150 caracteres");

            if (!Regex.IsMatch(endereco, PatternEmail))
                return Result<Email>.Failure("Email inválido");

            return Result<Email>.Success(new Email(endereco));
        }

        // ====================
        // OVERRIDE: Equals
        // Compares two Email instances based on their address values
        // ====================
        public override bool Equals(object? obj)
        {
            if (obj is not Email email)
                return false;

            return Endereco == email.Endereco;
        }

        // ====================
        // OVERRIDE: GetHashCode
        // Returns hash code based on email address for use in collections
        // ====================
        public override int GetHashCode()
        {
            return Endereco.GetHashCode();
        }

        // ====================
        // OVERRIDE: ToString
        // Returns the email address string representation
        // ====================
        public override string ToString()
        {
            return Endereco;
        }
    }
}
