namespace FuturoDoTrabalho.Api.Enums
{
    // ====================================================================================
    // ENUM: UserRole
    // ====================================================================================
    // Defines all available user roles/profiles in the system
    // Each role determines access levels and authorization for API endpoints
    // Used in authorization filters and business logic validation
    // ====================================================================================
    public enum UserRole
    {
        // System administrator with complete access to all features and data
        Admin = 1,

        // Manager with access to employee records and departmental reports
        Gerente = 2,

        // Regular employee with limited access (typically read/write personal data)
        Funcionario = 3,

        // Read-only viewer with access to view data but cannot modify anything
        Viewer = 4
    }
}
