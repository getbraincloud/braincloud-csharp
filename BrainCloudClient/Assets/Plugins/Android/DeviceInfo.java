// Georgy Treshchev 2024.
// FF Reality

// Note: Save this file in UTF-8 without BOM format to ensure successful compilation

package com.Plugins.AndroidNative;

//import androidx.annotation.Keep;
import android.app.Activity;

import android.os.Build;
import android.provider.Settings;
import android.app.Activity;
import android.content.Context;
import java.io.File;
import android.content.res.Resources;
//import androidx.core.os.ConfigurationCompat;
import android.location.Location;
import android.location.LocationManager;
import java.net.InetAddress;
import java.net.UnknownHostException;
import android.content.res.Configuration;

import android.os.LocaleList;
import android.telephony.TelephonyManager;
import android.util.Log;

// NSD Service
import android.os.IBinder;
import android.app.Service;
import android.net.nsd.NsdManager;
import android.net.nsd.NsdServiceInfo;

//@Keep
public class DeviceInfo {

	static NsdManager nsdManager;
	static NsdServiceInfo nsdServiceInfo;
	static NsdManager.RegistrationListener listener;
	static Context context;

//@Keep
	public static void startNsdService(final Activity activity, int Port) {
				
		context = activity;
				
		nsdManager = (NsdManager)context.getSystemService(Context.NSD_SERVICE);

		nsdServiceInfo = new NsdServiceInfo();
		nsdServiceInfo.setServiceName("NdiNsdService");
		nsdServiceInfo.setServiceType("_ndi._tcp.");
		nsdServiceInfo.setPort(Port);

		nsdManager.registerService(nsdServiceInfo, NsdManager.PROTOCOL_DNS_SD, new NsdManager.RegistrationListener() {

			@Override
			public void onRegistrationFailed(NsdServiceInfo nsdServiceInfo, int i) {}

			@Override
			public void onUnregistrationFailed(NsdServiceInfo nsdServiceInfo, int i) {}

			@Override
			public void onServiceRegistered(NsdServiceInfo nsdServiceInfo) {}

			@Override
			public void onServiceUnregistered(NsdServiceInfo nsdServiceInfo) {}
		});
	}	

//    @Keep
    public static String GetGeoLocation(final Activity activity)
    {
        Context context = activity;
        LocationManager mLocationManager = (LocationManager)context.getSystemService(context.LOCATION_SERVICE);
        Location locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
        Location locationNet = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
    
        long GPSLocationTime = 0;
        if (null != locationGPS) { GPSLocationTime = locationGPS.getTime(); }
    
        long NetLocationTime = 0;
    
        if (null != locationNet) {
            NetLocationTime = locationNet.getTime();
        }
    
        if ( 0 < GPSLocationTime - NetLocationTime ) {
            return locationGPS.toString();
        }
        else {
            return locationNet.toString();
        }
    }

//    @Keep
    public static boolean IsInternetAvailable() {
        try {
            InetAddress address = InetAddress.getByName("www.google.com");
            return !address.equals("");
        } catch (UnknownHostException e) { }
        return false;
    }

    /**
	 * 0 - Undefined
	 * 1 - Light
	 * 2 - Dark
	*/
//	@Keep
	public static byte GetCurrentSystemTheme(final Activity activity) {

		if (Build.VERSION.SDK_INT < Build.VERSION_CODES.Q) {
			return -1;
		}
        Configuration config = activity.getResources().getConfiguration();
        
		switch (config.uiMode & config.UI_MODE_NIGHT_MASK) {
			case Configuration.UI_MODE_NIGHT_NO:
				return 0;
			case Configuration.UI_MODE_NIGHT_YES:
				return 1;
		}
		return -1;
	}
	
	/** Path to "storage/emulated/0/Android/data/data/%APP_PACKAGE_NAME%/" */
//    @Keep
    public static String GetExternalPath(final Activity activity) {
    	Context context = activity;
    	File file = context.getExternalFilesDir(null);
    	String PathStr = file.getPath();
    	PathStr += "/";
    
    	return PathStr;
    }
    
//    @Keep
    public static String GetUniqueID(Activity activity) {
    	return Settings.Secure.getString(activity.getContentResolver(), Settings.Secure.ANDROID_ID);
    }

//	@Keep
	public static String GetCountryCode(Activity activity){
		Context context = activity;
		String countryCode = "";
        TelephonyManager telephonyManager =(TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        LocaleList locales = context.getResources().getConfiguration().getLocales();

        Log.d("CountryGetter", "Country from SIM " + telephonyManager.getSimCountryIso());
        Log.d("CountryGetter", "Country from Network " + telephonyManager.getNetworkCountryIso());
        Log.d("CountryGetter", "Country from Locale " + locales.get(0).getCountry());

        countryCode = telephonyManager.getSimCountryIso();
        if(countryCode == ""){
            countryCode = telephonyManager.getNetworkCountryIso();
        }
        if(countryCode == ""){
            countryCode = locales.get(0).getCountry();
        }

        return countryCode;
	}

//	@Keep
	public static String GetCountryCodeFromSIM(Activity activity){
		Context context = activity;
        TelephonyManager telephonyManager =(TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);

		return telephonyManager.getSimCountryIso();
	}

//	@Keep
	public static String GetCountryCodeFromNetwork(Activity activity){
		Context context = activity;
        TelephonyManager telephonyManager =(TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);

		return telephonyManager.getNetworkCountryIso();
	}

//	@Keep
	public static String GetCountryCodeFromLocale(Activity activity){
		Context context = activity;
		LocaleList locales = context.getResources().getConfiguration().getLocales();

		return locales.get(0).getCountry();
	}

//	@Keep
	public static String GetOSVersion() {
		return System.getProperty("os.version");
	}

//	@Keep
	public static int GetSDKVersion() {
		return Build.VERSION.SDK_INT;
	}

//	@Keep
	public static String GetBrand() {
		return Build.BRAND;
	}

//	@Keep
	public static String GetModel() {
		return Build.MODEL;
	}

//	@Keep
	public static String GetProduct() {
	    return Build.PRODUCT;
	}

//	@Keep
// 	public static String GetLanguage()	{
// 		return ConfigurationCompat.getLocales(Resources.getSystem().getConfiguration()).get(0).getDefault().getDisplayLanguage();
// 	}

//	@Keep
// 	public static String GetLanguageCode()	{
// 		return ConfigurationCompat.getLocales(Resources.getSystem().getConfiguration()).get(0).getDefault().toLanguageTag();
// 	}
}