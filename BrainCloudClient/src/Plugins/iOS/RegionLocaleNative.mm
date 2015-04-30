//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

extern "C"
{
    extern "C"
	{
		const char * _GetUsersCountryLocale()
		{
			NSString *countryCode = [[NSLocale currentLocale] objectForKey: NSLocaleCountryCode];
			
			return strdup([countryCode UTF8String]);
		}
	}
}
