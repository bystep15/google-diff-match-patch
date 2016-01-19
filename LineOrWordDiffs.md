# Introduction #

The difference algorithm in this library operates in character mode.  This produces the most detailed diff possible.  However, for some applications one may wish to take a coarser approach.

Note that the following applies to all language ports _except_ Lua.  Lua currently has no capability to handle Unicode.  The examples below are in JavaScript, but the procedure is identical in other languages.

# Line Mode #

Computing a line-mode diff is really easy.

```
function diff_lineMode(text1, text2) {
  var dmp = new diff_match_patch();
  var a = dmp.diff_linesToChars_(text1, text2);
  var lineText1 = a[0];
  var lineText2 = a[1];
  var lineArray = a[2];

  var diffs = dmp.diff_main(lineText1, lineText2, false);

  dmp.diff_charsToLines_(diffs, lineArray);
  return diffs;
}
```

What's happening here is that the texts are rebuilt by the `diff_linesToChars` function so that each line is represented by a single Unicode character.  Then these Unicode characters are diffed.  Finally the diff is rebuilt by the `diff_charsToLines` function to replace the Unicode characters with the original lines.

Adding `dmp.diff_cleanupSemantic(diffs);` before returning the diffs is probably a good idea so that the diff is more human-readable.

# Word Mode #

Computing a word-mode diff is exactly the same as the line-mode diff, except you will have to make a copy of `diff_linesToChars` and call it `diff_linesToWords`.  Look for the line that identifies the next line boundary:
```
  lineEnd = text.indexOf('\n', lineStart);
```
Change this line to look for any runs of whitespace, not just end of lines.

Despite the name, the `diff_charsToLines` function will continue to work fine in word-mode.