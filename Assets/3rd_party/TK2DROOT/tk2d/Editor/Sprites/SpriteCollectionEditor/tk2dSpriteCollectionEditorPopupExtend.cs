using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class tk2dSpriteCollectionEditorPopup
{
    public void PackMap(Object[] objs)
    {
        HandleDroppedPayload(objs);

        spriteCollectionProxy.DeleteUnusedData();
        spriteCollectionProxy.CopyToTarget();
        tk2dSpriteCollectionBuilder.ResetCurrentBuild();

        foreach (var t in _spriteCollection.textureParams)
        {
            t.anchor = tk2dSpriteCollectionDefinition.Anchor.LowerLeft;
        }

        if (!tk2dSpriteCollectionBuilder.Rebuild(_spriteCollection))
        {
            EditorUtility.DisplayDialog("Failed to commit sprite collection",
                "Please check the console for more details.", "Ok");
        }
        spriteCollectionProxy.CopyFromSource();
    }
}
