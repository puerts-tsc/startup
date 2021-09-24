using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Puerts;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Pipeline.Tasks;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime
{
    public class CustomJsLoader : ILoader
    {
        private string root = TsConfig.instance.outputPath + "/";
        public static TsConfig config => TsConfig.instance;
        public CustomJsLoader() { }

        static string DistPath( string baseDir, string path ) =>
            Regex.Replace( baseDir + "/src/", @"^Assets\/", "/" ) + path;

        public CustomJsLoader( string root )
        {
            this.root = root.TrimEnd( new char[] { '\\', '/' } ) + "/";
        }

        string log( params string[] log )
        {
            if ( config.isLogRequirePath ) {
                Debug.Log( string.Join( ", ", log ) );
            }

            return log[0];
        }

        private string PathToUse( string filepath )
        {
            return filepath.EndsWith( ".cjs" ) ? filepath.Substring( 0, filepath.Length - 4 ) : filepath;
        }

        public bool FileExists( string filepath )
        {
            //if ( config.isReportLoaderLog ) Debug.Log( $"require: {filepath}" );
            if ( filepath.StartsWith( "node_modules/" ) ) {
                return File.Exists( $"{Application.dataPath}/../{filepath}" )
                    .Of( t => Debug.Log( $"require: {filepath} => {t}" ) );
            }

            if ( Application.isEditor && null != config.sourcePath
                .FirstOrDefault( s => File.Exists( root + DistPath( s, filepath ) ) ).Of( s => {
                    var path = $"{s}/src/{filepath.Replace( ".js", ".ts" )}";
                    if ( !File.Exists( path ) && File.Exists( path + "x" ) ) {
                        path += "x";
                    }

                    Debug.Log( $"require: {filepath} => {path}" );
                }, s => !s.IsNullOrEmpty() && config.isLogRequirePath ) ) {
                return true;
            }
            else {
                try {
                    if ( null != config.sourcePath.Select( s => "Res" + DistPath( s, filepath + ".txt" ) )
                        .FirstOrDefault( s =>
                            Addressables.LoadResourceLocationsAsync( s ).WaitForCompletion().Any() ) ) {
                        if ( config.isLogRequirePath ) Debug.Log( $"require: {filepath} => [Res/] " );
                        return true;
                    }
                }
                catch ( TargetException e ) {
                    Debug.Log( e.Message );
                }

                string pathToUse = this.PathToUse( filepath );
                var asset = UnityEngine.Resources.Load( pathToUse );
                if ( asset == null ) {
                    if ( config.isLogRequirePath ) Debug.Log( $"{filepath} not exist" );
                }
                else {
                    if ( config.isLogRequirePath )
                        Debug.Log( $"require: {filepath} => ".Tap( s => {
                        #if UNITY_EDITOR
                            return s + AssetDatabase.GetAssetPath( asset );
                        #endif
                            return $"Resources/{s}";
                        } ) );
                }

                return asset != null;
            }
        }

        public string ReadFile( string filepath, out string debugpath )
        {
            debugpath = Path.GetFullPath( root + filepath );
            if ( filepath.StartsWith( "node_modules/" ) ) {
                debugpath = Path.GetFullPath( filepath );
                if ( File.Exists( debugpath ) ) {
                    return File.ReadAllText( debugpath );
                }
                else {
                    Debug.LogError( $"{filepath} not exists" );
                }

                return null;
            }

            if ( Application.isEditor ) {
                var path = config.sourcePath.FirstOrDefault( s => File.Exists( root + DistPath( s, filepath ) ) );
                if ( path != null ) {
                    debugpath = Path.GetFullPath( root + DistPath( path, filepath ) );
                    return File.ReadAllText( debugpath );
                }
            }

            UnityEngine.TextAsset file = null;
            try {
                var path = config.sourcePath.Select( s => "Res" + DistPath( s, filepath + ".txt" ) )
                    .FirstOrDefault( s => Addressables.LoadResourceLocationsAsync( s ).WaitForCompletion().Any() );
                if ( path != null ) {
                    file = Addressables.LoadAssetAsync<TextAsset>( path ).WaitForCompletion();
                    debugpath = Path.GetFullPath( root + Regex.Replace( path, @"^Res\/(.+)\.txt$", "$1" ) );
                }
            }
            catch ( TargetException e ) {
                Debug.Log( e.Message );
            }

            if ( file == null ) {
                string pathToUse = this.PathToUse( filepath );
                file = (UnityEngine.TextAsset)UnityEngine.Resources.Load( pathToUse );
            }
        #if UNITY_EDITOR
            if ( file != null ) {
                debugpath = Path.GetFullPath( AssetDatabase.GetAssetPath( file ) );
            }
        #endif
            if ( config.isLogDebugPath && file != null ) Debug.Log( $"debugPath: {debugpath}" );
            return file?.text;
        }
    }
}