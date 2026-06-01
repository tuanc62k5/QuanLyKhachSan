namespace DoAn.Utilities
{
    public class Functions
    {
        public static int _KH_ID = 0;
        public static string _TenKhach = "";
        public static string _Email = "";
        public static string _VaiTro = "";

        public static bool IsLogin()
        {
            return _KH_ID > 0;
        }
    }
}