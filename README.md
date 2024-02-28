# Unity_Localization
A text based localization system.
Words are displayed on the TMP_Text from a word list defined in a text file prepared for each language in StreamingAssets, and can switched as needed regardless of Editor or Runtime.

# Install
Intall via Unity Package Manager:
```
https://github.com/emptybraces/Unity_Localization.git?path=Assets/upm
```
![localization](https://github.com/emptybraces/Unity_Localization/assets/1441835/c563f22b-5f5c-4732-8c2e-c64d115b2f16)

# Structure
This code accomplishes the language switching by dynamically replacing the Fallback FontAsset prepared for each language. Create an base FontAsset for each theme(system, dialogues, etc) you want to use. It's almost for the fallback setter, so the atlas textures should empty or bake ASCII code use the same font for all languages for alphabets and numbers. Materials are created for this base FontAsset and are shared by all fallbacks. Then, using the TmproLocalize component, the FontAsset for each language can be loaded into the fallback to display the characters.

# Setup
- Configuration file must be created in "Assets/Localization/Create Localization Settings".
- In configuration, set the languages to be supported and file prefix for word lists. Next, set the TMP_FontAsset for each language and press the button below to register it in Addressables.

![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/964b3dcd-68ec-47f8-adc0-e4d5593f9893)

- Place a text file in the StreamingAssets folder, these files defining wordlist for languages you set above. File name format is "[PREFIX]_word.txt".
- Keys and values are separated by spaces or tabs. The key can also have an array value that must begin with a space or tab.
  
![files](https://github.com/emptybraces/Unity_Localization/assets/1441835/daa5a4b9-7a0d-4883-b39c-cf2e9604704a)


- AssetPostprocessor is implemented, and the word list file in the folder specified in Settings will automatically create LID.cs when detect to updated. It can also be run manually from "Assets/Localization/Create LID.cs".

![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/e3f33611-fa33-45ca-9456-8923b1b0ad80)
