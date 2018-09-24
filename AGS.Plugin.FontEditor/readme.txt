Manual
======


1. Explanation
---------------
This is an editor for wfn fonts, as well as for sci fonts.
The functions depends on the usage as a plugin for AGS (AdventureGameStudio)
or as a stand alone version.


2. Terms and Definitions
------------------------
p = plugin
s = stand alone


3. Features
-----------
- Edit wfn fonts                                                                (p/s)
- Edit sci fonts                                                                (  s)
- The fonts can outlined [Button 'Outline']                                     (p/s)
- Convert between sci and wfn fonts [Button 'Convert selected']                 (  s)
  the field text height is only for sci fonts interesting. 
  It set the space between two text lines and not 
  the textheight itself
- Shift the characters to up, down, left, right [Buttons 'U', 'L', 'D', 'R']    (p/s)
- Invert an character [Button 'Invert']                                         (p/s)
- Clear an character (with black) [Button 'Clear']                              (p/s)
- Fill an character (with white) [Button 'Fill']                                (p/s)
- Swap an character horizontally or vertically [Buttons 'Swap H', 'Swap V']     (p/s)
- Outline the complete font [Button 'Outline Font']                             (p/s)
- Show a grid for better font designing                                         (p/s)
- Multiple Redo/Undo [Right click on the character selection field]             (p/s)
- Copy to and paste from the clipboard                                          (p/s)
  [Right click on the character selection field]
- Zoomable edit field (2–40)                                                    (p/s)
- Swap the left color and the right color (click on the connected arrows)       (p/s)

4. Start
--------
Start the program, or copy the plugin to your AGS path and start AGS.

4.1 Stand alone
---------------
After you have started the program stand alone, you have to click open and select a path.
In the list on the left side, it shows you all the fonts, you can edit 
(wfn and sci [version 1.2.0.0 and higher]).

4.2 Plugin
----------
Start AGS and load a game. On the pane you found an entry named 'FontEditor'. Open it and
it will show all editable fonts. Under 'Fonts' you found all fonts, used by your game.
It is possible, that you find more than just the ones under 'FontEditor', because AGS is able
to handle ttf fonts as well, but not this editor.
Double click on an entry under 'FontEditor' to edit the selected wfn font, or right click under 'Fonts'
and select 'Edit font (with WFN-FontEditor)'.

5. Workflow (usage)
-------------------
After you have selected a font, you have to click on an character on the left side (selection field) to show it
zoomed. You can select the check box 'Show grid' for a better editing feeling. Now yo can click in the (zoomed) 
field to edit the selected character. It updates the character in the left selection field. By dragging your mouse
while holding the mouse button down, you can edit more pixel in a row. 

6. Tips
-------
If you want to make a font and a outline font too, it yould be good you have a border around your characters. That
means, one pixel on every side of the character is black. After you designed your font (or converted it from ttf
by using the TtfToWfnSci converter), save it and make a copy (with a file manager). Name the copy correct (file convention)
with a number higher and reopen the WFN-FontEditor. Than select the correct font and click 'Outline Font'. 
Check the characters an you will be done.
