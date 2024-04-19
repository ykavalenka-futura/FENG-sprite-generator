## Futura Engeniering sprite generator

### How to use:

Build project, open console and run application `FEngSpriteGenerator.exe` with next arguments:

``` <input_directory> <output_directory> [align] [output_filename] [padding_value] ```

### Argumenst:

```
<input_directory>     Path to the directory containing input images.
<output_directory>    Path to the directory where the generated sprite will be saved
-align                Optional. Alignment of the generated sprite (V for vertical, H for horizontal). Defaults to `Vertical`
-output_filename      Optional. Name of the generated sprite file. Defaults to `sprite_result.png`.
-padding_value        Optional. Padding between images in the sprite. Defaults to `10` pixels.
```