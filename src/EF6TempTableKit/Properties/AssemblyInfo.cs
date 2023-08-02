using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EF6TempTableKit")]
[assembly: AssemblyDescription("Extends EF6 with temp tables")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Zoran Blagojevic")]
[assembly: AssemblyProduct("EF6TempTableKit")]
[assembly: AssemblyCopyright("Copyright © 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4ecee5ef-a20d-4d0e-9abd-40e9eaa27459")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("3.0.*")] //File version
[assembly: AssemblyInformationalVersion("3.1.0.0")] //Product version

#if DEBUG
[assembly: InternalsVisibleTo("EF6TempTableKit.Test")]
#endif