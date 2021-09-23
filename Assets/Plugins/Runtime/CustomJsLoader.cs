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
        private string root = "";
        public static TsConfig config => TsConfig.instance;
        public CustomJsLoader() { }
        static string genPath => TsConfig.instance.rootPath + "/dist~";

        static string DistPath( string baseDir, string path ) =>
            Regex.Replace( baseDir + "/src/", @"^Assets\/", "/" ) + path;

        public CustomJsLoader( string root )
        {
            this.root = root;
        }

        string log( params string[] log )
        {
            if ( config.isReportLoaderLog ) {
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
            if ( config.isReportLoaderLog ) Debug.Log( $"require: {filepath}" );
            if ( filepath.StartsWith( "node_modules/" ) ) {
                return File.Exists( $"{Application.dataPath}/../{filepath}" );
            }

            if ( Application.isEditor &&
                config.sourcePath.Any( s => File.Exists( log( genPath + DistPath( s, filepath ) ) ) ) ) {
                return true;
            }
            else {
                try {
                    if ( null != config.sourcePath.Select( s => log( "Res" + DistPath( s, filepath + ".txt" ) ) )
                        .FirstOrDefault( s =>
                            Addressables.LoadResourceLocationsAsync( s ).WaitForCompletion().Any() ) ) {
                        if ( config.isReportLoaderLog ) Debug.Log( $"{filepath} addressable exists." );
                        return true;
                    }
                }
                catch ( TargetException e ) {
                    Debug.Log( e.Message );
                }

                string pathToUse = this.PathToUse( filepath );
                var asset = UnityEngine.Resources.Load( pathToUse );
                if ( asset == null ) {
                    if ( config.isReportLoaderLog ) Debug.Log( $"{filepath} not exist" );
                }
                else {
                    if ( config.isReportLoaderLog ) Debug.Log( $"{filepath} resource exists." );
                }

                return asset != null;
            }
        }

        public string ReadFile( string filepath, out string debugpath )
        {
            debugpath = root + "/" + filepath;
            if ( filepath.StartsWith( "node_modules/" ) ) {
                debugpath = Path.GetFullPath( $"{Application.dataPath}/../{filepath}" );
                if ( File.Exists( debugpath ) ) {
                    return File.ReadAllText( debugpath );
                }

                return null;
            }

            if ( Application.isEditor ) {
                var path = config.sourcePath.FirstOrDefault( s => File.Exists( genPath + DistPath( s, filepath ) ) );
                if ( path != null ) {
                    debugpath = Path.GetFullPath( genPath + DistPath( path, filepath ) );
                    if ( config.isReportLoaderLog ) Debug.Log( $"debugPath: {debugpath}" );
                    return File.ReadAllText( debugpath );
                }
            }

            UnityEngine.TextAsset file = null;
            try {
                var path = config.sourcePath.Select( s => "Res" + DistPath( s, filepath + ".txt" ) )
                    .FirstOrDefault( s => Addressables.LoadResourceLocationsAsync( s ).WaitForCompletion().Any() );
                if ( path != null ) {
                    file = Addressables.LoadAssetAsync<TextAsset>( path ).WaitForCompletion();
                    debugpath = Path.GetFullPath( genPath + Regex.Replace( path, @"^Res\/(.+)\.txt$", "$1" ) );
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
                debugpath = Path.GetFullPath( $"{Application.dataPath}/../{AssetDatabase.GetAssetPath( file )}" );
            }
#endif
            return file?.text;
        }
    }
}