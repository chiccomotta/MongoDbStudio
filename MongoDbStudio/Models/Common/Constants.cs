using System.Collections.Generic;

namespace MongoDbStudio.Models.Common
{
    public static class Constants
    {
    }

    public static class ApiMessages
    {
        public const string RedHatError =
            "Invalid token: Token Header kid not found in keys. Please check token params (audience ecc..)";
    }

    public static class ApiConstants
    {
        public const string CorrelationKey = "CORRELATION_KEY";
        public const string ParentCorrelationHeader = "parent-correlation";
        public const string KO = "KO";
        public const string OK = "OK";
        public const string BAD_REQUEST = "Bad Request";
        public const string NOT_FOUND = "Not Found";
        public const string ENTITY_UPDATE_OK = "Entity update OK";
        public const string ENTITY_UPDATE_KO = "Entity update KO";
        public const string ENTITY_CREATE_OK = "Entity create OK";
        public const string ENTITY_CREATE_KO = "Entity create KO";
    }

    public static class AuthorizationConstants
    {
        public const string Scope = "scope";
        public const string AccessToken = "access_token";
        public const string ClientId = "clientId";
        public const string ClientIdOther = "azp";
    }

    public static class RedHat
    {
        public const int ExpiredCache = 4;
    }
}