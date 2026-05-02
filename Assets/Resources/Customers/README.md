# Character Generation File Layout

## NoseMouths and Eyes

These can just be piled into their own directories for now, I might break into subfolders later though to handle spoiled versions and animations

## Bases, Clothes, Hair

Since these are dependent on the body base the file layout is a bit more complicated. The `Template` Directory is an empty skeleton of what each body folder should look like. Names of files don't matter except for hair files, to make sure they end up in the right order the front sprite name should begin with `0`, the back sprite's name should begin with `1`, and the shadow sprite's name should begin with `2`. The only folders who's names matter are the folders in `Characters`, and the `Hair` and `Clothing` folders in each body folder. The algorithm will first pick a random body folder, pick a color variant from the files in that folder, pick a clothes sprite from the `Clothes` folder within the body folder, and pick a random folder from the `Hair` folder which will contain the 3 layers of the chosen hair. See the diagram below.

### Body folder diagram

```
Assets/Resources/Characters/Bases/
├── BodyA/ 
│   ├── ColorVariant1.png
│   ├── ColorVariant2.png
│   ├── Clothes/
│   │   ├── Clothes1.png
│   │   └── Clothes2.png
│   └── Hair/
│       ├── Hair1/
│       │   ├── Hair1Front.png
│       │   ├── Hair1Back.png
│       │   └── Hair1Shadow.png
│       └── Hair2/
│           ├── Hair2Front.png
│           ├── Hair2Back.png
│           └── Hair2Shadow.png
└── BodyB/
    └── ...
```
