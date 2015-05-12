using System.Runtime.InteropServices;

public class RegionLocale
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string _GetUsersCountryLocale();
#endif

    protected static string m_countryLocale = "";
    public static string UsersCountryLocale
    {
        get
        {
            if (m_countryLocale == "")
            {
                GetCountryLocale();
            }
            return m_countryLocale;
        }
    }

    protected static void GetCountryLocale()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        m_countryLocale = _GetUsersCountryLocale();
#endif
    }
}
