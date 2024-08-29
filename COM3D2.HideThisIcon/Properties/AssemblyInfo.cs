using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("COM3D2.HideThisIcon")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("COM3D2.HideThisIcon")]
[assembly: AssemblyCopyright("Copyright © 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("Neutral")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4d8a9123-80d0-42db-858c-9c6e5165300d")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// This is the major & minor version with an asterisk (*) appended to auto increment numbers.
[assembly: AssemblyVersion(COM3D2.COM3D2.HideThisIcon.PluginInfo.PLUGIN_VERSION + ".*")]
[assembly: AssemblyFileVersion(COM3D2.COM3D2.HideThisIcon.PluginInfo.PLUGIN_VERSION)]

// These two lines tell your plugin to not worry about accessing private variables/classes whatever.
// It requires a publicized stub of the library with those private objects though. 
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
