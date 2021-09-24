using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Runtime;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Puerts
{
    public static class Gen
    {
        [MenuItem( "NodeTSC/生成 Extensions", false, -102 )]
        static void GenerateExtensions()
        {
            var mb = Puerts.Editor.Generator.Utils.extensionMethods;
            var output = "";
            var tt = new Dictionary<string, HashSet<string>>();
            mb.ForEach( tk => {
                if ( !tk.Key.IsInterface && !tk.Key.IsAbstract && !tk.Key.IsGenericType ) {
                    tt[tk.Key.GetFriendlyName()] = new HashSet<string>();
                    tk.Value.ForEach( v => {
                        //Debug.Log(tk.Key.GetFriendlyName());
                        if ( !v.DeclaringType.GetFriendlyName().StartsWith( "System." ) && v.DeclaringType.IsPublic &&
                            tk.Key.IsPublic && tk.Key.IsClass ) {
                            tt[tk.Key.GetFriendlyName()].Add( v.DeclaringType.GetFriendlyName() );
                        }
                    } );
                }
            } );
            var names = new List<string>();
            tt.ForEach( tk => {
                var key = tk.Key;
                if ( key == "float" ) key = "System.Single";
                if ( key == "string" ) key = "System.String";
                if ( key == "bool" ) key = "System.Boolean";
                if ( key == "long" ) key = typeof(Int64).FullName;
                if ( key == "double" ) key = typeof(System.Double).FullName;
                if ( !key.Contains( "[" ) && !key.Contains( "<" ) && !key.Contains( "&" ) &&
                    !key.Contains( "Unity.Entities" ) && key != "System.Reflection.ParameterInfo" &&
                    !key.StartsWith( "UnityEditor." ) && key != "UnityEngine.Rendering.CommandBuffer" ) {
                    names.Add( key.Split( '.' ).FirstOrDefault() );
                    tk.Value.Where( s => !s.StartsWith( "UnityEditor." ) ).ForEach( value => {
                        output += $"    $extension({key}, {value})\n";
                        names.Add( value.Split( '.' ).FirstOrDefault() );
                    } );
                }
            } );
            var ns = names.Where( s => !string.IsNullOrEmpty( s ) ).Distinct();
            var head = $@"
import {{ {string.Join( ", ", ns )} }} from 'csharp';
import {{ $extension }} from 'puerts';
export default function() {{

    console.log(""init extensions"");
";
            var path = TsConfig.instance.rootPath + "/src/gen/extensions.ts";
            Directory.CreateDirectory( Path.GetDirectoryName( path ) ?? "" );
            File.WriteAllText( path, $"{head}\n{output}\n}}" );
        }

        public static string HoverTreeClearMark( string input )
        {
            input = Regex.Replace( input, @"/\*[\s\S]*?\*/", "", RegexOptions.IgnoreCase );
            input = Regex.Replace( input, @"^\s*//[\s\S]*?$", "", RegexOptions.Multiline );
            input = Regex.Replace( input, @"^\s*$\n", "", RegexOptions.Multiline );
            input = Regex.Replace( input, @"^\s*//[\s\S]*", "", RegexOptions.Multiline );
            return input;
        }

        [MenuItem( "NodeTSC/一键生成 Bindings 和 Using", false, -105 )]
        public static void GenerateDTS()
        {
            Puerts.Editor.Generator.Menu.GenerateDTS();
            Puerts.Editor.GeneratorUsing.GenerateUsingCode();
            var start = DateTime.Now;
            var src = Path.Combine( Configure.GetCodeOutputDirectory(), "Typing/csharp" );
            //var saveTo = TsConfig.instance.rootPath + "/Typing";
            var dts = File.ReadAllText( $"Assets/Scripts/extra/index.d.ts" );
            // 替换掉type后面有个*的报错
            var content = dts + "\n" + File.ReadAllText( $"{src}/index.d.ts" );
            content = Regex.Replace( content, @"(\w)\*([^\*])", "$1 $2", RegexOptions.Multiline );
            //new Regex( @"(\w)\*([^\*])" ).Replace( content, "$1 $2" );
            content = HoverTreeClearMark( content );
            File.WriteAllText( $"{src}/index.d.ts", "//@ts-nocheck\n" + content );

            //GenerateExtensions();
            //ExamplesCfg.TestUsingAction();
            //Debug.Log( "finished! use " + ( DateTime.Now - start ).TotalMilliseconds + " ms" );
            AssetDatabase.Refresh();
        }
    }
}