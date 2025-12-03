using System.Security.Cryptography;
using System.Text;

namespace TradeSync.Core.Logic
{
    public static class SecurityHelper
    {
        public static string Protect(string clearText)
        {
            if (string.IsNullOrEmpty(clearText)) return clearText;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(clearText);
                byte[] protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.LocalMachine);
                return Convert.ToBase64String(protectedBytes);
            }
            catch { return clearText; }
        }

        public static string Unprotect(string protectedText)
        {
            if (string.IsNullOrEmpty(protectedText)) return protectedText;
            try
            {
                byte[] bytes = Convert.FromBase64String(protectedText);
                byte[] clearBytes = ProtectedData.Unprotect(bytes, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(clearBytes);
            }
            catch { return protectedText; }
        }
    }
}