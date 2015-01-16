﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34014
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataSync.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DataSync.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Config.xml ähnelt.
        /// </summary>
        internal static string ConfigurationFile {
            get {
                return ResourceManager.GetString("ConfigurationFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die DataSync.exe 
        ///Startet &quot;UI&quot; Modus und lädt bereits eingegebene Sync Pairs aus Configuration File.
        ///
        ///DataSync.exe { (SyncPair1) (SyncPair2) … (SyncPairN) }
        ///[/r|/recursiv] 
        ///[/log:logfilename] 
        ///[/logsize:logfilesize]
        ///[/blockcomparefilesize:filesize|/bcfs:filesize] 
        ///[/blocksize:blocksize|/bs:blocksize] 
        ///[/ps|/parallelsync]
        ///
        ///(SyncPair) = SourceFolder&gt;TargetFolder1| … |TargetFolderN[&lt;ExceptFolder1| … |ExceptFolderN] ähnelt.
        /// </summary>
        internal static string HelpConsole {
            get {
                return ResourceManager.GetString("HelpConsole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Instructions:
        ///- ADDPAIR {PAIRNAME}  
        ///- DELETEPAIR {PAIRNAME}
        ///- CLEARPAIRS
        ///- EXIT
        ///- SWITCH {OPTIONNAME} [ON|OFF]
        ///     {OPTIONNAME}: RECURSIV | PARALLELSYNC | LOGVIEW | JOBSVIEW	
        ///- SET  {OPTIONNAME} {VALUE}
        ///     {OPTIONNAME}:  LOGSIZE | BLOCKCOMPAREFILESIZE | BLOCKSIZE	
        ///- LOGTO {LOGFILENAME}
        ///- LISTPAIRS
        ///- SHOWPAIRDETAIL {PAIRNAME}
        ///- HELP ähnelt.
        /// </summary>
        internal static string HelpInstruction {
            get {
                return ResourceManager.GetString("HelpInstruction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Enter for EXIT ähnelt.
        /// </summary>
        internal static string Program_Main_EnterForEXIT {
            get {
                return ResourceManager.GetString("Program_Main_EnterForEXIT", resourceCulture);
            }
        }
    }
}
