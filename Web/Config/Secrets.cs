using Lib;

namespace Web.Config;

/// <summary>
/// Provides access to sensitive configuration secrets used in the application.
/// </summary>
public class Secrets
{
    /// <summary>
    /// JWT Issuer used in token generation and validation.
    /// </summary>
    public static readonly string JWT_ISSUER;

    /// <summary>
    /// JWT Secret used in token generation and validation.
    /// </summary>
    public static readonly string JWT_SECRET;

    /// <summary>
    /// Database connection string for establishing connections to the database.
    /// </summary>
    public static readonly string DB_CONNECTION_STRING;

    /// <summary>
    /// Google OAuth client ID for authentication.
    /// </summary>
    public static readonly string GOOGLE_CLIENT_ID;

    /// <summary>
    /// Google OAuth client secret for authentication.
    /// </summary>
    public static readonly string GOOGLE_CLIENT_SECRET;

    // Static constructor to initialize static readonly fields from environment variables.
    static Secrets()
    {
        JWT_ISSUER = Env.GetRequired("JWT_ISSUER");
        JWT_SECRET = Env.GetRequired("JWT_SECRET");

        DB_CONNECTION_STRING = Env.GetRequired("DB_CONNECTION_STRING");

        GOOGLE_CLIENT_ID = Env.GetRequired("GOOGLE_CLIENT_ID");
        GOOGLE_CLIENT_SECRET = Env.GetRequired("GOOGLE_CLIENT_SECRET");
    }
}
