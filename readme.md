## Futura Engineering sprite generator

### How to use:

Build project, open console and run application `FEngSpriteGenerator.exe` with next arguments:

``` 
--{ArgumentKey.InputDir}=<value> --{ArgumentKey.OutputDir}=<value> 
[--{ArgumentKey.Align}=<value>] [--{ArgumentKey.OutputFileName}=<value>] [--{ArgumentKey.PaddingValue}=<value>] 
[--{ArgumentKey.SpriteCssClassName}=<value>] [--{ArgumentKey.SpriteCssItemFormat}=<value>]
[--{ArgumentKey.SpriteCssClassName}=<value>] [--{ArgumentKey.SpriteCssItemFormat}=<value>]
[--{ArgumentKey.OutputCssFileName}=<value>] [--{ArgumentKey.OutputCssDir}=<value>] 
```

### Argumenst:

```
--InputDir              - Path to the directory containing input images.");
--OutputDir             - Path to the directory where the generated sprite will be saved.");
--Align                 - Optional. Alignment of the generated sprite (V for vertical, H for horizontal). Defaults to 'Vertical'.");
--OutputFileName        - Optional. Name of the generated sprite file. Defaults to 'sprite_result.png'.");
--PaddingValue          - Optional. Padding between images in the sprite. Defaults to '10' pixels.");
--SpriteCssClassName    - Optional. Name of base sprite css class. Defaults to 'sprite'.");
--SpriteCssItemFormat   - Optional. Name of generated sprite class format. Defaults to '.sprite-icon-{0}'.");
--OutputCssFileName     - Optional. Exported css file name. Defaults to '{defaultOutputCssFileName}'.");
--OutputCssDir          - Optional. Path to the directory to export generated css. Defaults to output directory.");
```


### How to use with project build?

```
...

<Target Name="BeforeBuild" Condition="'$(Configuration)' == 'Debug'">
    <Exec Command="$(SolutionDir)FEngSpriteGenerator.exe --InputDir=$(ProjectDir)Images\Img16 --OutputDir=$(ProjectDir)Images\Sprites --OutputFileName=nav16.png --SpriteCssClassName=sprites16 --OutputCssFileName=nav16sprites.css --OutputCssDir=$(ProjectDir)StyleSheets --SpriteCssItemFormat=&quot;.sprite-{0} .txt&quot;" />
  </Target>
...

```
This example demonstrates that we can utilize a command to invoke the generation of new sprites and styles for them, each time prior to project compilation. However, this applies only to the local version. Additionally, there is a hack to specify spaces for generating complex sprite classes.