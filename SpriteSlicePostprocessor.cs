using System.IO;
using UnityEngine;
using UnityEditor;

public class SpriteSlicePostprocessor : AssetPostprocessor {

    private const string SPRITE_PATH_24 = "Assets/Sprites/24/";
    private const int TILESIZE_24 = 24;

    private const string SPRITE_PATH_32 = "Assets/Sprites/32/";
    private const int TILESIZE_32 = 32;

    private void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
        // Add more calls here to check other directories for sprites + the sizes to slice them
        PostprocessBySize(texture, SPRITE_PATH_24, TILESIZE_24);
        PostprocessBySize(texture, SPRITE_PATH_32, TILESIZE_32);
    }

    private void PostprocessBySize(Texture2D texture, string dirPath, int size)
    {
        if (!this.assetPath.Contains(dirPath))
            return;

        int numColumns = texture.width / size;
        int numRows = texture.height / size;
        int numSprites = numColumns * numRows;
            
        TextureImporter importer = assetImporter as TextureImporter;
        if (importer.spritesheet.Length == numSprites)  // Weak heuristic but come at me bro
            return;

        SpriteMetaData[] spritesheet = new SpriteMetaData[numSprites];
        string spriteName = Path.GetFileNameWithoutExtension(assetPath);
        for (int i = 0; i < numSprites; i++)
        {
            Rect newRect = new Rect((i % numColumns) * size, (texture.height - size) -  (size * (i / numColumns)), size, size);
            spritesheet[i] = new SpriteMetaData()
            {
                name = string.Format("{0}_{1}", spriteName, i),
                rect = newRect
            };
        }
        importer.spritesheet = spritesheet;
        importer.spriteImportMode = SpriteImportMode.Multiple;

        importer.SaveAndReimport();
    }
}
