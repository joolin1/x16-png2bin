# x16-Png2Bin
This is a help for importing sprites, tiles and images into your Commander X16 application. It converts a png image file to a binary file that you can use directly in your assembly program. It only works with images that have a color depth of four bits (= 16 colors). This is a .NET application to be used from the command line and therefore only works in Windows.

1. Draw your sprites/tiles/image in an application like paint.net. If you are drawing sprites or tiles, set the bitmap to the width of your sprites/tiles and then place them all vertically. (Not very practical, I know...)

2. Save the image, use the png format with a bit depth of 4.

3. Use the command line application Png2Binary with the file as argument to convert the image. You will get two files as a result. The first is a binary file with the image data. The second file is a text file with all colors used.

4. Include the binary file in your source code. In Acme assembler, this is an example of what you write: _mytiles: !bin "mytiles.bin". 

5. Paste the text file in your source code. Now you can easily adjust colors if you wish. It can for example look like this:
_mytilespalette: !word $0000, $0331, $0333, $0663, $0555, $0775, $0777, $0AA5, $0DD7, $0FF7, $0AAA, $0FFA, $0DDD, $0FFD, $0FFF, $0000

Remember that if you load the data into RAM, it has to be copied to VRAM, otherwise VERA can't access it.The palette should be copied into the VERA palette beginning at $F1000. Exactly where depends on which palette offset you are using for your sprites/tiles. For example, if sprites are using a palette offset of two, the palette should be copied to a destination beginning at $F1000 + 2 (offset) * 16 (number of colors) * 2 (number of bytes for each color) = $F1040.   

To access different tiles/sprites. Remember that every byte contains two pixels (4bpp = 4 bits per pixel). If you for exemple have made a series of tiles in the size of 16x16 pixels every tile will be 16 x 16 / 2 bytes = 128 bytes. This means the second tile can be accessed att address _mytiles + 128, the third att _mytiles + 256 and so on.

