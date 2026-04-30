namespace Fucoffee.Helpers
{
    public static class SessionHelper
    {
        public static bool IsLoggedIn(IHttpContextAccessor accessor)
            => accessor.HttpContext?.Session.GetString("Username") != null;

        public static bool IsAdmin(IHttpContextAccessor accessor)
            => accessor.HttpContext?.Session.GetString("Role") == "Admin";

        public static int GetUserId(IHttpContextAccessor accessor)
            => accessor.HttpContext?.Session.GetInt32("UserId") ?? 0;
    }
}