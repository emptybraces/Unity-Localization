# Unity_Localization
A text based localization system.
Words are displayed on the TMP_Text from a word list defined in a text file prepared for each language in StreamingAssets, and can switched as needed regardless of Editor or Runtime.

# Install
Intall via Unity Package Manager:
```
https://github.com/emptybraces/Unity_Localization.git?path=Assets/upm
```

![localiation](https://github.com/emptybraces/Unity_Localization/assets/1441835/d76783a1-0493-4049-bad3-215ae9b4eccb)

# Structure
This code accomplishes the language switching by dynamically replacing the Fallback FontAsset prepared for each language. Create an intermediate FontAsset for each theme(system, dialogues, etc) you want to use. This FontAsset is only intermediate data for the fallback, so the atlas textures should empty. Or if you want to use the same font for all languages for alphabets and numbers, bake them in ASCII code. Materials are created for this intermediate font data and are shared by all Fallback FontAsset. Then, using the TmproLocalize component, the TMP_FontAsset for each language can be loaded into the Fallback FontAsset to display the characters.

# Setup
- Configuration file must be created in "Assets/Localization/Create Localization Settings".
- From the configuration file, set the languages to be supported, file prefixes for word lists, etc. Next, set the font data (TMP_FontAsset) for each language and press the button below to register it in Addressables.

![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/20063736-7528-4e2c-b0fe-90a46a4dd7dd)

- Place a text file that defines the word list for each language to be supported in the StreamingAssets folder. Keys and values are separated by spaces or tabs. The key can also have an array value that must begin with a space or tab.

![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/cbe15108-09d1-48a7-af7e-dcd328c4e83d)

- AssetPostprocessor is implemented, and the word list file in the folder specified in Settings will automatically create LID.cs when detect to updated. It can also be run manually from "Assets/Localization/Create LID.cs".

![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/e3f33611-fa33-45ca-9456-8923b1b0ad80)
