# Plain Text vs. Structured Content #

The diff, match and patch algorithms in this library are **plain text only**.  Attempting to feed HTML, XML or some other structured content through them may result in problems.  Consider the case where a series of patches are applied to HTML content on a best-effort basis.  One could be left with a `<B>` tag that starts but doesn't end, text falling between a `</TD>` and a `<TD>`, or a syntactically invalid tag missing a bracket.

The correct solution is to use a tree-based diff, match and patch.  These employ totally different algorithms.  I'm afraid I can't help you there.

## Doing it anyway ##

However, depending on the task, there are sometimes some interesting ways to use text-based algorithms on structured content.

One method is to strip the tags from the HTML using a simple regex or node-walker.  Then diff the HTML content against the text content.  Don't perform any diff cleanups.  This diff enables one to map character positions from one version to the other (see the `diff_xIndex` function).  After this, one can apply all the patches one wants against the plain text, then safely map the changes back to the HTML.  The catch with this technique is that although text may be freely edited, HTML tags are immutable.

Another method is to walk the HTML and replace every opening and closing tag with a Unicode character.  Check the Unicode spec for a range that is not in use.  During the process, create a hash table of Unicode characters to the original tags.  The result is a block of text which can be patched without fear of inserting text inside a tag or breaking the syntax of a tag.  One just has to be careful when reconverting the content back to HTML that no closing tags are lost.