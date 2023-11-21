using Lib;

namespace Api.Config;

public class Secrets
{
    public static readonly string JWT_ISSUER;
    public static readonly string JWT_SECRET;

    public static readonly string DB_CONNECTION_STRING;
    public static readonly string GOOGLE_CLIENT_ID;
    public static readonly string GOOGLE_CLIENT_SECRET;

    static Secrets()
    {
        JWT_ISSUER = Env.GetRequired("JWT_ISSUER");
        JWT_SECRET = Env.GetRequired("JWT_SECRET");
        GOOGLE_CLIENT_ID = Env.GetRequired("GOOGLE_CLIENT_ID");
        GOOGLE_CLIENT_SECRET = Env.GetRequired("GOOGLE_CLIENT_SECRET");

        DB_CONNECTION_STRING = Env.GetRequired("DB_CONNECTION_STRING");
    }
}