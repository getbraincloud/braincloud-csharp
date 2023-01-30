using System.Reflection;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("websocket-sharp")]
[assembly: AssemblyDescription("A C# implementation of the WebSocket protocol client and server")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("websocket-sharp.dll")]
[assembly: AssemblyCopyright("sta.blockhead")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

#if !(DOT_NET)
[assembly: AssemblyVersion("1.0.2")] //.* does not support unity 2020 deterministic builds. The .* here is not even necessary for us to have either. 
#else 
[assembly: AssemblyVersion("1.0.2.*")]
#endif

// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
