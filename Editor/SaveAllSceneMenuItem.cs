using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Kogane.Internal
{
    internal static class SaveAllSceneMenuItem
    {
        [MenuItem( "Kogane/すべてのシーンを保存" )]
        private static void Save()
        {
            var isOk = EditorUtility.DisplayDialog
            (
                title: "",
                message: "すべてのシーンを保存しますか？",
                ok: "はい",
                cancel: "いいえ"
            );

            if ( !isOk ) return;

            var sceneSetups = EditorSceneManager.GetSceneManagerSetup();

            // FindAssets は Packages フォルダも対象になっているので
            // Assets フォルダ以下のシーンのみを抽出
            var scenePathArray = AssetDatabase
                    .FindAssets( "t:scene" )
                    .Select( x => AssetDatabase.GUIDToAssetPath( x ) )
                    .Where( x => x.StartsWith( "Assets/" ) )
                    .ToArray()
                ;

            try
            {
                var count = scenePathArray.Length;

                for ( var i = 0; i < count; i++ )
                {
                    var scenePath = scenePathArray[ i ];
                    var number    = i + 1;

                    EditorUtility.DisplayCancelableProgressBar
                    (
                        title: "すべてのシーンを保存",
                        info: $"{number} / {count} {scenePath}",
                        progress: ( float )number / count
                    );

                    var scene = EditorSceneManager.OpenScene( scenePath );

                    EditorSceneManager.MarkSceneDirty( scene );
                    EditorSceneManager.SaveScene( scene );
                }

                EditorUtility.DisplayDialog( "", "すべてのシーンを保存しました", "OK" );
            }
            finally
            {
                // Untitled なシーンは復元できず、SceneSetup[] の要素数が 0 になる
                // Untitled なシーンを復元しようとすると下記のエラーが発生するので if で確認
                // ArgumentException: Invalid SceneManagerSetup:
                if ( 0 < sceneSetups.Length )
                {
                    EditorSceneManager.RestoreSceneManagerSetup( sceneSetups );
                }

                EditorUtility.ClearProgressBar();
            }
        }
    }
}