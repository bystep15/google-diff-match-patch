# Introduction #

The patch algorithms generate (using `patch_toText()`) and parse (using `patch_fromText()`) a textual diff format which looks a lot like the [Unidiff format](http://en.wikipedia.org/wiki/Diff#Unified_format).  However the formats have three differences.  These differences are only important if one plans to parse this text.

## Example ##

Text 1: `There are two things that are more difficult than making an after-dinner speech: climbing a wall which is leaning toward you and kissing a girl who is leaning away from you.`

Text 2: ``````Churchill``````` talked about climbing a wall which is leaning toward you and kissing a woman who is leaning away from you.`

This example contains two relatively well-separated edits.  Using this library will result in the following two patches:

```
@@ -1,84 +1,28 @@
-There are two things that are more difficult than making an after-dinner speech:
+%60Churchill%60 talked about
  cli
@@ -136,12 +80,13 @@
 g a 
-girl
+woman
  who
```

## 1. Character Based ##

The Diff Match Patch library is character-based (unlike GNU Diff and Patch which are line-based).

## 2. Encoded Characters ##

Special characters are encoded using %xx notation.  The set of characters which are encoded matches JavaScript's `encodeURI()` function, with the exception of spaces which are not encoded.

## 3. Rolling context ##

GNU Diff and Patch assume that patches may be applied selectively or in arbitrary order.  The Diff Match Patch library assumes patches will be applied serially from start to finish.  The result is that each patch contains context information which assume that the previous patches have been applied.

This design decision stems from a fundamental limitation of the matching algorithm.  Generally, the Bitap match can only work with text that is 32 characters long.  Thus patches (context plus deletions, excluding insertions) can only be 32 characters long.  Long patches (such as the one above) will be automatically split up into 32 character chunks with overlapping contexts during application:

```
@@ -1,32 +1,4 @@
-There are two things that ar
 e mo
@@ -29,32 +1,4 @@
-e more difficult than making
  an 
@@ -57,28 +1,28 @@
- an after-dinner speech:
+%60Churchill%60 talked about
  cli
@@ -136,12 +80,13 @@
 g a 
-girl
+woman
  who
```

In these cases each fragmented patch overlaps neatly with the previous patch.