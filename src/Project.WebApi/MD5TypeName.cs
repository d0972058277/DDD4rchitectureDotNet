using System.Security.Cryptography;
using System.Text;

namespace Project.WebApi
{
    public class MD5TypeName
    {
        public static string Get<T>()
        {
            return Get(typeof(T));
        }

        public static string Get(Type type)
        {
            using var md5 = MD5.Create();
            var hashedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(type.FullName!));
            var hashedString = BitConverter.ToString(hashedBytes);
            var name = hashedString.Replace("-", "") + "_" + type.Name;
            return name;
        }
    }
}
