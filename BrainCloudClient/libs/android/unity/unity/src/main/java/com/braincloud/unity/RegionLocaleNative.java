package com.braincloud.unity;

import java.util.*;
import android.content.*;
import android.telephony.*;

public class RegionLocaleNative
{
    // ripped from http://stackoverflow.com/questions/3659809/where-am-i-get-country
    /**
     * Get ISO 3166-1 alpha-2 country code for this device or empty string if not found
     * @param context Context reference to get the TelephonyManager instance from
     * @return country code or empty string
     */
    public static String GetUsersCountryLocale(Context context)
    {
        try {
            final TelephonyManager tm = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
            final String simCountry = tm.getSimCountryIso();
            if (simCountry != null && simCountry.length() == 2) { // SIM country code is available
                return simCountry.toLowerCase(Locale.US);
            }
            else if (tm.getPhoneType() != TelephonyManager.PHONE_TYPE_CDMA) { // device is not 3G (would be unreliable)
                String networkCountry = tm.getNetworkCountryIso();
                if (networkCountry != null && networkCountry.length() == 2) { // network country code is available
                    return networkCountry.toLowerCase(Locale.US);
                }
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        // This is the proper way, but sim card will give us more accurate info if it's available.
        return Locale.getDefault().getCountry();
    }
}
