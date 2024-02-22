# Unity_Localization
A text-based localization system.
Words are displayed on the TMP_Text from a word list defined in a text file prepared for each language in StreamingAssets, and can switched as needed regardless of Editor or Runtime.

# Install
Intall via Unity Package Manager:
```
https://github.com/emptybraces/Unity_Localization.git?path=Assets/upm
```

![localiation](https://github.com/emptybraces/Unity_Localization/assets/1441835/d76783a1-0493-4049-bad3-215ae9b4eccb)

# How
- Configuration file must be created in "Assets/Localization/Create Localization Settings".
- From the configuration file, set the languages to be supported, file prefixes for word lists, etc. Next, set the font data (TMP_FontAsset) for each language and press the button below to register it in Addressables.
![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/20063736-7528-4e2c-b0fe-90a46a4dd7dd)

- Place a text file that defined word list for each language to be supported in the StreamingAssets file.
![image](https://github.com/emptybraces/Unity_Localization/assets/1441835/cbe15108-09d1-48a7-af7e-dcd328c4e83d)


