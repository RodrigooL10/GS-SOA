using System.Text.RegularExpressions;
using FuturoDoTrabalho.Api.ValueObjects;

namespace FuturoDoTrabalho.Api.ValueObjects
{
    // ====================================================================================
    // VALUE OBJECT: CPF
    // ====================================================================================
    // Represents a validated Brazilian CPF (Cadastro de Pessoa Física)
    // Ensures only valid CPFs can be created through format and checksum validation
    // Provides strong typing for CPF values throughout the application
    // Brazilian CPF format: XXX.XXX.XXX-XX (11 digits)
    // ====================================================================================
    public class CPF
    {
        // ====================
        // PROPERTIES
        // ====================
        // The validated CPF number (stored as 11 digits without formatting)
        public string Numero { get; private set; }

        // ====================
        // CONSTRUCTOR (Private)
        // Prevents direct instantiation - use Create() factory method instead
        // ====================
        private CPF(string numero)
        {
            // Store only numeric digits (removes formatting)
            Numero = Regex.Replace(numero, @"\D", "");
        }

        // ====================
        // FACTORY METHOD: Create
        // Validates and creates a CPF instance if valid
        // Returns Result<CPF> allowing caller to handle validation errors
        // Validates format (11 digits), uniqueness rules, and checksum digits
        // ====================
        public static Result<CPF> Create(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return Result<CPF>.Failure("CPF não pode estar vazio");

            // Remove non-numeric characters
            var apenasNumeros = Regex.Replace(numero, @"\D", "");

            if (apenasNumeros.Length != 11)
                return Result<CPF>.Failure("CPF deve conter exatamente 11 dígitos");

            // Check if all digits are the same (invalid CPF pattern)
            if (apenasNumeros.All(c => c == apenasNumeros[0]))
                return Result<CPF>.Failure("CPF inválido");

            // Validate checksum digits
            if (!ValidarDigitosVerificadores(apenasNumeros))
                return Result<CPF>.Failure("CPF inválido");

            return Result<CPF>.Success(new CPF(numero));
        }

        // ====================
        // METHOD: ValidarDigitosVerificadores
        // Calculates and validates the two check digits of the CPF
        // Uses the standard Brazilian CPF validation algorithm
        // ====================
        private static bool ValidarDigitosVerificadores(string cpf)
        {
            // First check digit calculation
            var soma1 = 0;
            var multiplicador1 = 10;

            for (int i = 0; i < 9; i++)
            {
                soma1 += int.Parse(cpf[i].ToString()) * multiplicador1;
                multiplicador1--;
            }

            var resto1 = soma1 % 11;
            var digito1 = resto1 < 2 ? 0 : 11 - resto1;

            // Second check digit calculation
            var soma2 = 0;
            var multiplicador2 = 11;

            for (int i = 0; i < 10; i++)
            {
                soma2 += int.Parse(cpf[i].ToString()) * multiplicador2;
                multiplicador2--;
            }

            var resto2 = soma2 % 11;
            var digito2 = resto2 < 2 ? 0 : 11 - resto2;

            // Verify both check digits match
            return cpf[9] == char.Parse(digito1.ToString()) && 
                   cpf[10] == char.Parse(digito2.ToString());
        }

        // ====================
        // METHOD: Formatado
        // Returns the CPF formatted as XXX.XXX.XXX-XX
        // ====================
        public string Formatado()
        {
            return $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}";
        }

        // ====================
        // OVERRIDE: Equals
        // Compares two CPF instances based on their numeric values
        // ====================
        public override bool Equals(object? obj)
        {
            if (obj is not CPF cpf)
                return false;

            return Numero == cpf.Numero;
        }

        // ====================
        // OVERRIDE: GetHashCode
        // Returns hash code based on CPF number for use in collections
        // ====================
        public override int GetHashCode()
        {
            return Numero.GetHashCode();
        }

        // ====================
        // OVERRIDE: ToString
        // Returns the formatted CPF string representation
        // ====================
        public override string ToString()
        {
            return Formatado();
        }
    }
}
