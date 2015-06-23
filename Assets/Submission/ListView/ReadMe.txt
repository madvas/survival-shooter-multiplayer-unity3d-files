----------------------------------------------
            ListView for Unity UI
       Copyright © 2014 Endgame Studios
                Version 1.7
        http://www.endgamestudios.com
         listview@endgamestudios.com
----------------------------------------------

Thank you for purchasing ListView for Unity UI!

If you have any questions, comments or feature requests, please contact us at listview@endgamestudios.com.

We'd love to hear from you!


---------------------------------------
 How to use the ListView
---------------------------------------

- Make sure your scene has Canvas and EventSystem objects placed in the hierarchy.
  (If these are not present, you can create them via GameObject->UI.)
- Instantiate the ListView prefab by dragging it from the Prefabs folder in the project view onto the Canvas object in the hierarchy.
- The ListView object you just created is now ready to use!
- You can configure its visual appearance in the Inspector, or via code.
- You can add and remove columns and items via code (please refer to the samples supplied with the package to learn how).


-----------------
 Version History
-----------------

1.0.0:
- Initial revision.

1.1:
- Workarounds for Unity beta issues.

1.2:
- Added an item hover event.

1.3:
- Fixed issues related to multiple ListViews on screen.

1.5:
- Added support for images and custom controls in subitems.

1.6:
- Bug fix: Inserting an item when the last item/row is selected results in a crash.
- Removal of redundant compilation warnings.
- Addition of runtime warnings in the case where the listview is initialised incorrectly
  (e.g. adding columns/items before the listview has received a Start event,
  adding items before adding columns).

1.7:
- Bug fix: Redundant scroll bars appear sometimes when last column width == -2.
- Bug fix: When last column width == -2 and there is no vertical scroll bar,
  the column width should extend to the right edge of the control.

1.8:
- Bug fix: In some samples, clicking on the column header caused a crash in resize mode.

1.9:
- The ListView now only creates hierarchy elements for the visible
  portion of the items list, rather than the whole list. This allows
  support for very large lists.
- The ListView can now be scrolled by dragging anywhere within the control
  (implemented using a ScrollRect).
- The ListView is now defined within namespace Endgame to avoid naming
  conflicts.

1.10:
- The grid line colour and thickness settings can now be optionally applied
  to the column headers (by setting ListView.ColumnHeaderGridLines == true).

1.11:
- ListView.GetItemButtons is now public.

1.12:
- Changed the vertical scroll bar to "bottom to top" orientation to fix a Unity bug 
  where clicking outside the scroll bar thumb would cause scrolling in the wrong direction.

1.13:
- Fixed a bug where some column header properties didn't force a rebuild.

1.14:
- The column header height can now be set in the editor.

1.15:
- Fixed a bug with the scroll position not updating correctly due to using
  "bottom to top" scrolling.

1.16:
- Fixed a bug with autosizing columns and virtual listview behaviour.

1.17:
- Fixed a bug with listview items popping offscreen in certain cases.

1.18:
- Fixed a bug with mouse wheel scrolling.

1.19:
- Fixed a bug with clearing the listview.
 
1.20:
- Added gamepad support.

1.21:
- Fixed a bug with a render mode check being done in Awake instead of Start.

1.22:
- Fixed a bug with updating the selection after removing an item.

1.23:
- Fixed an exception thrown when gamepad inputs aren't present.

1.24:
- Keyboard and gamepad input now works with Unity UI's navigation system.

1.25:
- Improvements to the navigation integration.
