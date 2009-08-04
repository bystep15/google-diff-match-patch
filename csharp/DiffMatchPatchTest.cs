/*
 * Copyright 2008 Google Inc. All Rights Reserved.
 * Author: fraser@google.com (Neil Fraser)
 * Author: anteru@developer.shelter13.net (Matthaeus G. Chajdas)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Diff Match and Patch -- Test Harness
 * http://code.google.com/p/google-diff-match-patch/
 */

using DiffMatchPatch;
using System.Collections.Generic;
using System;
using System.Text;
using NUnit.Framework;

namespace nicTest
{
    [TestFixture()]
    public class diff_match_patchTest : diff_match_patch
    {
        [Test()]
        public void patch_toTextTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            string strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n  laz\n";
            List<Patch> patches;
            patches = target.patch_fromText(strp);
            String result = target.patch_toText(patches);
            Assert.AreEqual(strp, result);

            strp = "@@ -1,9 +1,9 @@\n-f\n+F\n oo+fooba\n@@ -7,9 +7,9 @@\n obar\n-,\n+.\n  tes\n";
            patches = target.patch_fromText(strp);
            result = target.patch_toText(patches);
            Assert.AreEqual(strp, result);
        }

        [Test()]
        public void patch_splitMaxTest()
        {
            // Assumes that Match_MaxBits is 32.
            diff_match_patchTest target = new diff_match_patchTest();
            List<Patch> patches;
            patches = target.patch_make("abcdef1234567890123456789012345678901234567890123456789012345678901234567890uvwxyz", "abcdefuvwxyz");
            target.patch_splitMax(patches);
            var result = target.patch_toText(patches);
            Assert.AreEqual("@@ -3,32 +3,8 @@\n cdef\n-123456789012345678901234\n 5678\n@@ -27,32 +3,8 @@\n cdef\n-567890123456789012345678\n 9012\n@@ -51,30 +3,8 @@\n cdef\n-9012345678901234567890\n uvwx\n", result);

            patches = target.patch_make("1234567890123456789012345678901234567890123456789012345678901234567890", "abc");
            target.patch_splitMax(patches);
            Assert.AreEqual("@@ -1,32 +1,4 @@\n-1234567890123456789012345678\n 9012\n@@ -29,32 +1,4 @@\n-9012345678901234567890123456\n 7890\n@@ -57,14 +1,3 @@\n-78901234567890\n+abc\n", target.patch_toText(patches));

            patches = target.patch_make("abcdefghij , h : 0 , t : 1 abcdefghij , h : 0 , t : 1 abcdefghij , h : 0 , t : 1", "abcdefghij , h : 1 , t : 1 abcdefghij , h : 1 , t : 1 abcdefghij , h : 0 , t : 1");
            target.patch_splitMax(patches);
            Assert.AreEqual("@@ -2,32 +2,32 @@\n bcdefghij , h : \n-0\n+1\n  , t : 1 abcdef\n@@ -29,32 +29,32 @@\n bcdefghij , h : \n-0\n+1\n  , t : 1 abcdef\n", target.patch_toText(patches));
        }

        [Test()]
        public void diff_linesToCharsTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Convert lines down to characters
            List<String> tmpVector = new List<String>();
            tmpVector.Add("");
            tmpVector.Add("alpha\n");
            tmpVector.Add("beta\n");
            var result = target.diff_linesToChars("alpha\nbeta\nalpha\n", "beta\nalpha\nbeta\n");
            Assert.AreEqual("\u0001\u0002\u0001", result[0]);
            Assert.AreEqual("\u0002\u0001\u0002", result[1]);
            CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

            tmpVector.Clear();
            tmpVector.Add("");
            tmpVector.Add("alpha\r\n");
            tmpVector.Add("beta\r\n");
            tmpVector.Add("\r\n");
            result = target.diff_linesToChars("", "alpha\r\nbeta\r\n\r\n\r\n");
            Assert.AreEqual("", result[0]);
            Assert.AreEqual("\u0001\u0002\u0003\u0003", result[1]);
            CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

            tmpVector.Clear();
            tmpVector.Add("");
            tmpVector.Add("a");
            tmpVector.Add("b");
            result = target.diff_linesToChars("a", "b");
            Assert.AreEqual("\u0001", result[0]);
            Assert.AreEqual("\u0002", result[1]);
            CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

            // More than 256
            int n = 300;
            //StringBuilder lineList = new StringBuilder();
            tmpVector.Clear();
            StringBuilder lineList = new StringBuilder();
            StringBuilder charList = new StringBuilder();
            for (int x = 1; x < n + 1; x++)
            {
                tmpVector.Add(x + "\n");
                lineList.Append(x + "\n");
                charList.Append(Convert.ToChar(x));
            }
            Assert.AreEqual(n, tmpVector.Count);
            String lines = lineList.ToString();
            String chars = charList.ToString();
            Assert.AreEqual(n, chars.Length);
            tmpVector.Insert(0, "");
            result = target.diff_linesToChars(lines, "");
            Assert.AreEqual(chars, result[0]);
            Assert.AreEqual("", result[1]);
            CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);
        }

        [Test()]
        public void diff_halfMatchTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            Assert.IsNull(target.diff_halfMatch("1234567890", "abcdef"));

            CollectionAssert.AreEqual(new string[] { "12", "90", "a", "z", "345678" }, target.diff_halfMatch("1234567890", "a345678z"));

            CollectionAssert.AreEqual(new string[] { "a", "z", "12", "90", "345678" }, target.diff_halfMatch("a345678z", "1234567890"));

            CollectionAssert.AreEqual(new string[] { "12123", "123121", "a", "z", "1234123451234" }, target.diff_halfMatch("121231234123451234123121", "a1234123451234z"));

            CollectionAssert.AreEqual(new string[] { "", "-=-=-=-=-=", "x", "", "x-=-=-=-=-=-=-=" }, target.diff_halfMatch("x-=-=-=-=-=-=-=-=-=-=-=-=", "xx-=-=-=-=-=-=-="));

            CollectionAssert.AreEqual(new string[] { "-=-=-=-=-=", "", "", "y", "-=-=-=-=-=-=-=y" }, target.diff_halfMatch("-=-=-=-=-=-=-=-=-=-=-=-=y", "-=-=-=-=-=-=-=yy"));
        }

        [Test()]
        public void diff_commonSuffixTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Detect and remove any common suffix.
            Assert.AreEqual(0, target.diff_commonSuffix("abc", "xyz"));
            Assert.AreEqual(4, target.diff_commonSuffix("abcdef1234", "xyz1234"));
        }

        [Test()]
        public void diff_commonPrefixTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            Assert.AreEqual(0, target.diff_commonPrefix("abc", "xyz"));
            Assert.AreEqual(4, target.diff_commonPrefix("1234abcdef", "1234xyz"));
        }

        [Test()]
        public void diff_cleanupSemanticLosslessTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Slide diffs to match logical boundaries
            List<Diff> diffs = new List<Diff>();
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(new List<Diff>(), diffs);

            diffs = new List<Diff>
            {
                new Diff(Operation.EQUAL, "AAA\r\n\r\nBBB"),
                new Diff(Operation.INSERT, "\r\nDDD\r\n\r\nBBB"),
                new Diff(Operation.EQUAL, "\r\nEEE")
            };
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(
                new List<Diff>{
                    new Diff(Operation.EQUAL, "AAA\r\n\r\n"),
                    new Diff(Operation.INSERT, "BBB\r\nDDD\r\n\r\n"),
                    new Diff(Operation.EQUAL, "BBB\r\nEEE")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "AAA\r\nBBB"),
                new Diff(Operation.INSERT, " DDD\r\nBBB"),
                new Diff(Operation.EQUAL, " EEE")};
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(
                new List<Diff>{
                    new Diff(Operation.EQUAL, "AAA\r\n"),
                    new Diff(Operation.INSERT, "BBB DDD\r\n"),
                    new Diff(Operation.EQUAL, "BBB EEE")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "The c"),
                new Diff(Operation.INSERT, "ow and the c"),
                new Diff(Operation.EQUAL, "at.")};
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.EQUAL, "The "),
                new Diff(Operation.INSERT, "cow and the "),
                new Diff(Operation.EQUAL, "cat.")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "The-c"),
                new Diff(Operation.INSERT, "ow-and-the-c"),
                new Diff(Operation.EQUAL, "at.")};
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.EQUAL, "The-"),
                new Diff(Operation.INSERT, "cow-and-the-"),
                new Diff(Operation.EQUAL, "cat.")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "ax")};
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "aax")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "xa"),
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "a")};
            target.diff_cleanupSemanticLossless(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.EQUAL, "xaa"),
                new Diff(Operation.DELETE, "a")}, diffs);
        }

        [Test()]
        public void diff_cleanupSemanticTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Cleanup semantically trivial equalities
            List<Diff> diffs = new List<Diff>();
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(new List<Diff>(), diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.INSERT, "b"),
                new Diff(Operation.EQUAL, "cd"),
                new Diff(Operation.DELETE, "e")};
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(
                new List<Diff>{
                    new Diff(Operation.DELETE, "a"),
                    new Diff(Operation.INSERT, "b"),
                    new Diff(Operation.EQUAL, "cd"),
                    new Diff(Operation.DELETE, "e")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "b"),
                new Diff(Operation.DELETE, "c")};
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(
                new List<Diff>{
                    new Diff(Operation.DELETE, "abc"),
                    new Diff(Operation.INSERT, "b")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.EQUAL, "cd"),
                new Diff(Operation.DELETE, "e"),
                new Diff(Operation.EQUAL, "f"),
                new Diff(Operation.INSERT, "g")};
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "abcdef"),
                new Diff(Operation.INSERT, "cdfg")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.INSERT, "1"),
                new Diff(Operation.EQUAL, "A"),
                new Diff(Operation.DELETE, "B"),
                new Diff(Operation.INSERT, "2"),
                new Diff(Operation.EQUAL, "_"),
                new Diff(Operation.INSERT, "1"),
                new Diff(Operation.EQUAL, "A"),
                new Diff(Operation.DELETE, "B"),
                new Diff(Operation.INSERT, "2")};
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "AB_AB"),
                new Diff(Operation.INSERT, "1A2_1A2")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "The c"),
                new Diff(Operation.DELETE, "ow and the c"),
                new Diff(Operation.EQUAL, "at.")};
            target.diff_cleanupSemantic(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.EQUAL, "The "),
                new Diff(Operation.DELETE, "cow and the "),
                new Diff(Operation.EQUAL, "cat.")}, diffs);
        }

        [Test()]
        public void diff_cleanupMergeTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Cleanup a messy diff
            List<Diff> diffs = new List<Diff>();
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>(), diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.INSERT, "c")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.INSERT, "c")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.EQUAL, "c")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.EQUAL, "abc")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.DELETE, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.DELETE, "c")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.DELETE, "abc")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.INSERT, "a"), new Diff(Operation.INSERT, "b"), new Diff(Operation.INSERT, "c")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.INSERT, "abc")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b"), new Diff(Operation.DELETE, "c"), new Diff(Operation.INSERT, "d"), new Diff(Operation.EQUAL, "e"), new Diff(Operation.EQUAL, "f")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.DELETE, "ac"), new Diff(Operation.INSERT, "bd"), new Diff(Operation.EQUAL, "ef")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "abc"), new Diff(Operation.DELETE, "dc")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "d"), new Diff(Operation.INSERT, "b"), new Diff(Operation.EQUAL, "c")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "ba"), new Diff(Operation.EQUAL, "c")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.INSERT, "ab"), new Diff(Operation.EQUAL, "ac")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "c"), new Diff(Operation.INSERT, "ab"), new Diff(Operation.EQUAL, "a")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.EQUAL, "ca"), new Diff(Operation.INSERT, "ba")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.EQUAL, "c"), new Diff(Operation.DELETE, "ac"), new Diff(Operation.EQUAL, "x")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.DELETE, "abc"), new Diff(Operation.EQUAL, "acx")}, diffs);

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "ca"), new Diff(Operation.EQUAL, "c"), new Diff(Operation.DELETE, "b"), new Diff(Operation.EQUAL, "a")};
            target.diff_cleanupMerge(diffs);
            CollectionAssert.AreEqual(new List<Diff>{new Diff(Operation.EQUAL, "xca"), new Diff(Operation.DELETE, "cba")}, diffs);
        }

        [Test()]
        public void diff_cleanupEfficiencyTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            target.Diff_EditCost = 4;
            List<Diff> diffs = new List<Diff> ();
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>(), diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34")};
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "xyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34")};
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "abxyzcd"),
                new Diff(Operation.INSERT, "12xyz34")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "x"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34")};
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "xcd"),
                new Diff(Operation.INSERT, "12x34")}, diffs);

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "xy"),
                new Diff(Operation.INSERT, "34"),
                new Diff(Operation.EQUAL, "z"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "56")};
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "abxyzcd"),
                new Diff(Operation.INSERT, "12xy34z56")}, diffs);

            target.Diff_EditCost = 5;
            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34")};
            target.diff_cleanupEfficiency(diffs);
            CollectionAssert.AreEqual(new List<Diff>{
                new Diff(Operation.DELETE, "abwxyzcd"),
                new Diff(Operation.INSERT, "12wxyz34")}, diffs);
            target.Diff_EditCost = 4;
        }

        [Test()]
        public void diff_prettyHtmlTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Pretty print
            List<Diff> diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "a\n"),
                new Diff(Operation.DELETE, "<B>b</B>"),
                new Diff(Operation.INSERT, "c&d")};

            Assert.AreEqual("<SPAN TITLE=\"i=0\">a&para;<BR></SPAN><DEL STYLE=\"background:#FFE6E6;\" TITLE=\"i=2\">&lt;B&gt;b&lt;/B&gt;</DEL><INS STYLE=\"background:#E6FFE6;\" TITLE=\"i=2\">c&amp;d</INS>",
                target.diff_prettyHtml(diffs));
        }

        [Test()]
        public void diff_charsToLinesTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();

            Assert.AreEqual(new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.EQUAL, "a"));

            // Convert chars up to lines
            List<Diff> diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "\u0001\u0002\u0001"),
                new Diff(Operation.INSERT, "\u0002\u0001\u0002")};
            List<String> tmpVector = new List<String>();
            tmpVector.Add("");
            tmpVector.Add("alpha\n");
            tmpVector.Add("beta\n");
            target.diff_charsToLines(diffs, tmpVector);
            CollectionAssert.AreEqual(new List<Diff> {
                new Diff(Operation.EQUAL, "alpha\nbeta\nalpha\n"),
                new Diff(Operation.INSERT, "beta\nalpha\nbeta\n")}, diffs);

            // More than 256
            int n = 300;
            //StringBuilder lineList = new StringBuilder();
            tmpVector.Clear();
            StringBuilder lineList = new StringBuilder();
            StringBuilder charList = new StringBuilder();
            for (int x = 1; x < n + 1; x++)
            {
                tmpVector.Add(x + "\n");
                lineList.Append(x + "\n");
                charList.Append(Convert.ToChar (x));
            }
            Assert.AreEqual(n, tmpVector.Count);
            String lines = lineList.ToString();
            String chars = charList.ToString();
            Assert.AreEqual(n, chars.Length);
            tmpVector.Insert(0, "");
            diffs = new List<Diff>{new Diff(Operation.DELETE, chars)};
            target.diff_charsToLines(diffs, tmpVector);
            CollectionAssert.AreEqual(new List<Diff>
                {new Diff(Operation.DELETE, lines)}, diffs);
        }

        [Test()]
        public void diff_textTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            List<Diff> diffs = new List<Diff>
            {
                new Diff(Operation.EQUAL, "jump"),
                new Diff(Operation.DELETE, "s"),
                new Diff(Operation.INSERT, "ed"),
                new Diff(Operation.EQUAL, " over "),
                new Diff(Operation.DELETE, "the"),
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, " lazy")
            };

            Assert.AreEqual("jumps over the lazy", target.diff_text1(diffs));
            Assert.AreEqual("jumped over a lazy", target.diff_text2(diffs));
        }

        [Test()]
        public void diff_fromDeltaTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Convert a diff into delta string
            List<Diff> diffs = new List<Diff> {
                new Diff(Operation.EQUAL, "jump"),
                new Diff(Operation.DELETE, "s"),
                new Diff(Operation.INSERT, "ed"),
                new Diff(Operation.EQUAL, " over "),
                new Diff(Operation.DELETE, "the"),
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, " lazy"),
                new Diff(Operation.INSERT, "old dog")};
            String text1 = target.diff_text1(diffs);
            Assert.AreEqual("jumps over the lazy", text1);

            String delta = target.diff_toDelta(diffs);
            Assert.AreEqual("=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog", delta);

            // Convert delta string into a diff
            CollectionAssert.AreEqual(diffs, target.diff_fromDelta(text1, delta));

            // Generates error (19 < 20).
            try
            {
                target.diff_fromDelta(text1 + "x", delta);
                Assert.Fail ("diff_fromDelta: Too long.");
            }
            catch (ArgumentException)
            {
                // Exception expected.
            }

            // Generates error (19 > 18).
            try
            {
                target.diff_fromDelta(text1.Substring(1), delta);
                Assert.Fail ("diff_fromDelta: Too short.");
            }
            catch (ArgumentException)
            {
                // Exception expected.
            }

            // Generates error (%c3%xy invalid Unicode).
            try
            {
                target.diff_fromDelta("", "+%c3%xy");
                Assert.Fail("diff_fromDelta: Invalid character.");
            }
            catch (ArgumentException)
            {
                // Exception expected.
            }

            // Test deltas with special characters
            char zero = (char)0;
            char one = (char)1;
            char two = (char)2;
            diffs = new List<Diff> {
                new Diff(Operation.EQUAL, "\u0680 " + zero + " \t %"),
                new Diff(Operation.DELETE, "\u0681 " + one + " \n ^"),
                new Diff(Operation.INSERT, "\u0682 " + two + " \\ |")};
            text1 = target.diff_text1(diffs);
            Assert.AreEqual("\u0680 " + zero + " \t %\u0681 " + one + " \n ^", text1);

            delta = target.diff_toDelta(diffs);
            // Lowercase, due to UrlEncode uses lower
            Assert.AreEqual("=7\t-7\t+%da%82 %02 %5c %7c", delta, "diff_toDelta: Unicode.");

            CollectionAssert.AreEqual(diffs, target.diff_fromDelta(text1, delta), "diff_fromDelta: Unicode.");

            // Verify pool of unchanged characters
            diffs = new List<Diff> {
                new Diff(Operation.INSERT, "A-Z a-z 0-9 - _ . ! ~ * ' ( ) ; / ? : @ & = + $ , # ")};
            String text2 = target.diff_text2(diffs);
            Assert.AreEqual("A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", text2, "diff_text2: Unchanged characters.");

            delta = target.diff_toDelta(diffs);
            Assert.AreEqual("+A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", delta, "diff_toDelta: Unchanged characters.");

            // Convert delta string into a diff
            CollectionAssert.AreEqual(diffs, target.diff_fromDelta("", delta), "diff_fromDelta: Unchanged characters.");
        }

        [Test()]
        public void diff_xIndexTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Translate a location in text1 to text2
            List<Diff> diffs = new List<Diff> {
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.INSERT, "1234"),
                new Diff(Operation.EQUAL, "xyz")};
            Assert.AreEqual(5, target.diff_xIndex(diffs, 2), "diff_xIndex: Translation on equality.");

            diffs = new List<Diff> {
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "1234"),
                new Diff(Operation.EQUAL, "xyz")};
            Assert.AreEqual(1, target.diff_xIndex(diffs, 3), "diff_xIndex: Translation on deletion.");
        }

        [Test()]
        public void diff_levenshteinTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            List<Diff> diffs = new List<Diff> {
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.INSERT, "1234"),
                new Diff(Operation.EQUAL, "xyz")};
            Assert.AreEqual(4, target.diff_levenshtein(diffs), "diff_levenshtein: Levenshtein with trailing equality.");

            diffs = new List<Diff> {
                new Diff(Operation.EQUAL, "xyz"),
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.INSERT, "1234")};
            Assert.AreEqual(4, target.diff_levenshtein(diffs), "diff_levenshtein: Levenshtein with leading equality.");

            diffs = new List<Diff> {
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.EQUAL, "xyz"),
                new Diff(Operation.INSERT, "1234")};
            Assert.AreEqual(7, target.diff_levenshtein(diffs), "diff_levenshtein: Levenshtein with middle equality.");
        }

        [Test()]
        public void diff_pathTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // First, check footprints are different.
            Assert.IsTrue(target.diff_footprint(1, 10) != target.diff_footprint(10, 1), "diff_footprint:");

            // Single letters
            // Trace a path from back to front.
            List<HashSet<long>> v_map;
            HashSet<long> row_set;
            v_map = new List<HashSet<long>>();
            {
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 1));
                row_set.Add(target.diff_footprint(1, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 2));
                row_set.Add(target.diff_footprint(2, 0));
                row_set.Add(target.diff_footprint(2, 2));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 3));
                row_set.Add(target.diff_footprint(2, 3));
                row_set.Add(target.diff_footprint(3, 0));
                row_set.Add(target.diff_footprint(4, 3));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 4));
                row_set.Add(target.diff_footprint(2, 4));
                row_set.Add(target.diff_footprint(4, 0));
                row_set.Add(target.diff_footprint(4, 4));
                row_set.Add(target.diff_footprint(5, 3));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 5));
                row_set.Add(target.diff_footprint(2, 5));
                row_set.Add(target.diff_footprint(4, 5));
                row_set.Add(target.diff_footprint(5, 0));
                row_set.Add(target.diff_footprint(6, 3));
                row_set.Add(target.diff_footprint(6, 5));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 6));
                row_set.Add(target.diff_footprint(2, 6));
                row_set.Add(target.diff_footprint(4, 6));
                row_set.Add(target.diff_footprint(6, 6));
                row_set.Add(target.diff_footprint(7, 5));
                v_map.Add(row_set);
            }

            List<Diff> diffs = new List<Diff>{
                new Diff(Operation.INSERT, "W"),
                new Diff(Operation.DELETE, "A"),
                new Diff(Operation.EQUAL, "1"),
                new Diff(Operation.DELETE, "B"),
                new Diff(Operation.EQUAL, "2"),
                new Diff(Operation.INSERT, "X"),
                new Diff(Operation.DELETE, "C"),
                new Diff(Operation.EQUAL, "3"),
                new Diff(Operation.DELETE, "D")};
            CollectionAssert.AreEqual(diffs, target.diff_path1(v_map, "A1B2C3D", "W12X3"), "diff_path1: Single letters.");

            // Trace a path from front to back.
            v_map.RemoveAt(v_map.Count - 1);
            diffs = new List<Diff>{
                new Diff(Operation.EQUAL, "4"),
                new Diff(Operation.DELETE, "E"),
                new Diff(Operation.INSERT, "Y"),
                new Diff(Operation.EQUAL, "5"),
                new Diff(Operation.DELETE, "F"),
                new Diff(Operation.EQUAL, "6"),
                new Diff(Operation.DELETE, "G"),
                new Diff(Operation.INSERT, "Z")};
            CollectionAssert.AreEqual(diffs, target.diff_path2(v_map, "4E5F6G", "4Y56Z"), "diff_path2: Single letters.");

            // Double letters
            // Trace a path from back to front.
            v_map = new List<HashSet<long>>();
            {
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 1));
                row_set.Add(target.diff_footprint(1, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 2));
                row_set.Add(target.diff_footprint(1, 1));
                row_set.Add(target.diff_footprint(2, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 3));
                row_set.Add(target.diff_footprint(1, 2));
                row_set.Add(target.diff_footprint(2, 1));
                row_set.Add(target.diff_footprint(3, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 4));
                row_set.Add(target.diff_footprint(1, 3));
                row_set.Add(target.diff_footprint(3, 1));
                row_set.Add(target.diff_footprint(4, 0));
                row_set.Add(target.diff_footprint(4, 4));
                v_map.Add(row_set);
            }
            diffs = new List<Diff>{
                new Diff(Operation.INSERT, "WX"),
                new Diff(Operation.DELETE, "AB"),
                new Diff(Operation.EQUAL, "12")};
            CollectionAssert.AreEqual(diffs, target.diff_path1(v_map, "AB12", "WX12"), "diff_path1: Double letters.");

            // Trace a path from front to back.
            v_map = new List<HashSet<long>>();
            {
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(0, 1));
                row_set.Add(target.diff_footprint(1, 0));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(1, 1));
                row_set.Add(target.diff_footprint(2, 0));
                row_set.Add(target.diff_footprint(2, 4));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(2, 1));
                row_set.Add(target.diff_footprint(2, 5));
                row_set.Add(target.diff_footprint(3, 0));
                row_set.Add(target.diff_footprint(3, 4));
                v_map.Add(row_set);
                row_set = new HashSet<long>();
                row_set.Add(target.diff_footprint(2, 6));
                row_set.Add(target.diff_footprint(3, 5));
                row_set.Add(target.diff_footprint(4, 4));
                v_map.Add(row_set);
            }
            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "CD"),
                new Diff(Operation.EQUAL, "34"),
                new Diff(Operation.INSERT, "YZ")};
            CollectionAssert.AreEqual(diffs, target.diff_path2(v_map, "CD34", "34YZ"), "diff_path2: Double letters.");
        }

        [Test()]
        public void diff_mainTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Perform a trivial diff
            List<Diff> diffs = new List<Diff>{new Diff(Operation.EQUAL, "abc")};
            CollectionAssert.AreEqual(diffs, target.diff_main("abc", "abc", false), "diff_main: Null case.");

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "ab"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "c")};
            CollectionAssert.AreEqual(diffs, target.diff_main("abc", "ab123c", false), "diff_main: Simple insertion.");

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "bc")};
            CollectionAssert.AreEqual(diffs, target.diff_main("a123bc", "abc", false), "diff_main: Simple deletion.");

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.INSERT, "456"), new Diff(Operation.EQUAL, "c")};
            CollectionAssert.AreEqual(diffs, target.diff_main("abc", "a123b456c", false), "diff_main: Two insertions.");

            diffs = new List<Diff>{new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "456"), new Diff(Operation.EQUAL, "c")};
            CollectionAssert.AreEqual(diffs, target.diff_main("a123b456c", "abc", false), "diff_main: Two deletions.");

            // Perform a real diff
            // Switch off the timeout.
            target.Diff_Timeout = 0;
            target.Diff_DualThreshold = 32;
            diffs = new List<Diff>{new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b")};
            CollectionAssert.AreEqual(diffs, target.diff_main("a", "b", false), "diff_main: Simple case #1.");

            diffs = new List<Diff>{new Diff(Operation.DELETE, "Apple"), new Diff(Operation.INSERT, "Banana"), new Diff(Operation.EQUAL, "s are a"), new Diff(Operation.INSERT, "lso"), new Diff(Operation.EQUAL, " fruit.")};
            CollectionAssert.AreEqual(diffs, target.diff_main("Apples are a fruit.", "Bananas are also fruit.", false), "diff_main: Simple case #2.");

            diffs = new List<Diff>{new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "\u0680"), new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "\t"), new Diff(Operation.INSERT, new String (new char[]{(char)0}))};
            CollectionAssert.AreEqual(diffs, target.diff_main("ax\t", "\u0680x" + (char)0, false), "diff_main: Simple case #3.");

            diffs = new List<Diff>{new Diff(Operation.DELETE, "1"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "y"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "2"), new Diff(Operation.INSERT, "xab")};
            CollectionAssert.AreEqual(diffs, target.diff_main("1ayb2", "abxab", false), "diff_main: Overlap #1.");

            diffs = new List<Diff>{new Diff(Operation.INSERT, "xaxcx"), new Diff(Operation.EQUAL, "abc"), new Diff(Operation.DELETE, "y")};
            CollectionAssert.AreEqual(diffs, target.diff_main("abcy", "xaxcxabc", false), "diff_main: Overlap #2.");

            // Sub-optimal double-ended diff.
            target.Diff_DualThreshold = 2;
            diffs = new List<Diff>{new Diff(Operation.INSERT, "x"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.INSERT, "x"), new Diff(Operation.EQUAL, "c"), new Diff(Operation.DELETE, "y"), new Diff(Operation.INSERT, "xabc")};
            CollectionAssert.AreEqual(diffs, target.diff_main("abcy", "xaxcxabc", false), "diff_main: Overlap #3.");

            target.Diff_DualThreshold = 32;
            target.Diff_Timeout = 0.001f;  // 1ms
            String a = "`Twas brillig, and the slithy toves\nDid gyre and gimble in the wabe:\nAll mimsy were the borogoves,\nAnd the mome raths outgrabe.\n";
            String b = "I am the very model of a modern major general,\nI've information vegetable, animal, and mineral,\nI know the kings of England, and I quote the fights historical,\nFrom Marathon to Waterloo, in order categorical.\n";
            // Increase the text lengths by 1024 times to ensure a timeout.
            for (int x = 0; x < 10; x++) {
              a = a + a;
              b = b + b;
            }
            Assert.IsNull(target.diff_map(a, b), "diff_main: Timeout.");
            target.Diff_Timeout = 0;

            // Test the linemode speedup
            // Must be long to pass the 200 char cutoff.
            a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
            b = "abcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\n";
            CollectionAssert.AreEqual(target.diff_main(a, b, true), target.diff_main(a, b, false), "diff_main: Simple.");

            a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
            b = "abcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n";
            String[] texts_linemode = diff_rebuildtexts(target.diff_main(a, b, true));
            String[] texts_textmode = diff_rebuildtexts(target.diff_main(a, b, false));
            CollectionAssert.AreEqual(texts_textmode, texts_linemode, "diff_main: Overlap.");
        }

        [Test()]
        public void match_alphabetTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Initialise the bitmasks for Bitap
            Dictionary<char, int> bitmask = new Dictionary<char, int>();
            bitmask.Add('a', 4); bitmask.Add('b', 2); bitmask.Add('c', 1);
            CollectionAssert.AreEqual(bitmask, target.match_alphabet("abc"), "match_alphabet: Unique.");

            bitmask.Clear();
            bitmask.Add('a', 37); bitmask.Add('b', 18); bitmask.Add('c', 8);
            CollectionAssert.AreEqual(bitmask, target.match_alphabet("abcaba"), "match_alphabet: Duplicates.");
        }

        [Test()]
        public void match_bitapTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();

            // Bitap algorithm
            target.Match_Distance = 100;
            target.Match_Threshold = 0.5f;
            Assert.AreEqual(5, target.match_bitap("abcdefghijk", "fgh", 5), "match_bitap: Exact match #1.");

            Assert.AreEqual(5, target.match_bitap("abcdefghijk", "fgh", 0), "match_bitap: Exact match #2.");

            Assert.AreEqual(4, target.match_bitap("abcdefghijk", "efxhi", 0), "match_bitap: Fuzzy match #1.");

            Assert.AreEqual(2, target.match_bitap("abcdefghijk", "cdefxyhijk", 5), "match_bitap: Fuzzy match #2.");

            Assert.AreEqual(-1, target.match_bitap("abcdefghijk", "bxy", 1), "match_bitap: Fuzzy match #3.");

            Assert.AreEqual(2, target.match_bitap("123456789xx0", "3456789x0", 2), "match_bitap: Overflow.");

            Assert.AreEqual(0, target.match_bitap("abcdef", "xxabc", 4), "match_bitap: Before start match.");

            Assert.AreEqual(3, target.match_bitap("abcdef", "defyy", 4), "match_bitap: Beyond end match.");

            Assert.AreEqual(0, target.match_bitap("abcdef", "xabcdefy", 0), "match_bitap: Oversized pattern.");

            target.Match_Threshold = 0.4f;
            Assert.AreEqual(4, target.match_bitap("abcdefghijk", "efxyhi", 1), "match_bitap: Threshold #1.");

            target.Match_Threshold = 0.3f;
            Assert.AreEqual(-1, target.match_bitap("abcdefghijk", "efxyhi", 1), "match_bitap: Threshold #2.");

            target.Match_Threshold = 0.0f;
            Assert.AreEqual(1, target.match_bitap("abcdefghijk", "bcdef", 1), "match_bitap: Threshold #3.");

            target.Match_Threshold = 0.5f;
            Assert.AreEqual(0, target.match_bitap("abcdexyzabcde", "abccde", 3), "match_bitap: Multiple select #1.");

            Assert.AreEqual(8, target.match_bitap("abcdexyzabcde", "abccde", 5), "match_bitap: Multiple select #2.");

            target.Match_Distance = 10;  // Strict location.
            Assert.AreEqual(-1, target.match_bitap("abcdefghijklmnopqrstuvwxyz", "abcdefg", 24), "match_bitap: Distance test #1.");

            Assert.AreEqual(0, target.match_bitap("abcdefghijklmnopqrstuvwxyz", "abcdxxefg", 1), "match_bitap: Distance test #2.");

            target.Match_Distance = 1000;  // Loose location.
            Assert.AreEqual(0, target.match_bitap("abcdefghijklmnopqrstuvwxyz", "abcdefg", 24), "match_bitap: Distance test #3.");
        }

        [Test()]
        public void match_mainTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            // Full match
            Assert.AreEqual(0, target.match_main("abcdef", "abcdef", 1000), "match_main: Equality.");

            Assert.AreEqual(-1, target.match_main("", "abcdef", 1), "match_main: Null text.");

            Assert.AreEqual(3, target.match_main("abcdef", "", 3), "match_main: Null pattern.");

            Assert.AreEqual(3, target.match_main("abcdef", "de", 3), "match_main: Exact match.");

            Assert.AreEqual(3, target.match_main("abcdef", "defy", 4), "match_main: Beyond end match.");

            Assert.AreEqual(0, target.match_main("abcdef", "abcdefy", 0), "match_main: Oversized pattern.");

            target.Match_Threshold = 0.7f;
            Assert.AreEqual(4, target.match_main("I am the very model of a modern major general.", " that berry ", 5), "match_main: Complex match.");
            target.Match_Threshold = 0.5f;
        }

        [Test()]
        public void patch_patchObjTest()
        {
            // Patch Object
            Patch p = new Patch();
            p.start1 = 20;
            p.start2 = 21;
            p.length1 = 18;
            p.length2 = 17;
            p.diffs = new List<Diff> {
                new Diff(Operation.EQUAL, "jump"),
                new Diff(Operation.DELETE, "s"),
                new Diff(Operation.INSERT, "ed"),
                new Diff(Operation.EQUAL, " over "),
                new Diff(Operation.DELETE, "the"),
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, "\nlaz")};
            String strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n %0alaz\n";
            Assert.AreEqual(strp, p.ToString(), "Patch: toString.");
        }

        [Test()]
        public void patch_fromTextTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            Assert.IsTrue(target.patch_fromText("").Count == 0, "patch_fromText: #0.");

            String strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n %0alaz\n";
            Assert.AreEqual(strp, target.patch_fromText(strp)[0].ToString(), "patch_fromText: #1.");

            Assert.AreEqual("@@ -1 +1 @@\n-a\n+b\n", target.patch_fromText("@@ -1 +1 @@\n-a\n+b\n")[0].ToString(), "patch_fromText: #2.");

            Assert.AreEqual("@@ -1,3 +0,0 @@\n-abc\n", target.patch_fromText("@@ -1,3 +0,0 @@\n-abc\n") [0].ToString(), "patch_fromText: #3.");

            Assert.AreEqual("@@ -0,0 +1,3 @@\n+abc\n", target.patch_fromText("@@ -0,0 +1,3 @@\n+abc\n") [0].ToString(), "patch_fromText: #4.");

            // Generates error.
            try
            {
                target.patch_fromText("Bad\nPatch\n");
                Assert.Fail("patch_fromText: #5.");
            }
            catch (ArgumentException)
            {

                // Exception expected.
            }
        }

        [Test()]
        public void patch_addContextTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            target.Patch_Margin = 4;
            Patch p;
            p = target.patch_fromText("@@ -21,4 +21,10 @@\n-jump\n+somersault\n") [0];
            target.patch_addContext(p, "The quick brown fox jumps over the lazy dog.");
            Assert.AreEqual("@@ -17,12 +17,18 @@\n fox \n-jump\n+somersault\n s ov\n", p.ToString(), "patch_addContext: Simple case.");

            p = target.patch_fromText("@@ -21,4 +21,10 @@\n-jump\n+somersault\n")[0];
            target.patch_addContext(p, "The quick brown fox jumps.");
            Assert.AreEqual("@@ -17,10 +17,16 @@\n fox \n-jump\n+somersault\n s.\n", p.ToString(), "patch_addContext: Not enough trailing context.");

            p = target.patch_fromText("@@ -3 +3,2 @@\n-e\n+at\n")[0];
            target.patch_addContext(p, "The quick brown fox jumps.");
            Assert.AreEqual("@@ -1,7 +1,8 @@\n Th\n-e\n+at\n  qui\n", p.ToString(), "patch_addContext: Not enough leading context.");

            p = target.patch_fromText("@@ -3 +3,2 @@\n-e\n+at\n")[0];
            target.patch_addContext(p, "The quick brown fox jumps.  The quick brown fox crashes.");
            Assert.AreEqual("@@ -1,27 +1,28 @@\n Th\n-e\n+at\n  quick brown fox jumps. \n", p.ToString(), "patch_addContext: Ambiguity.");
        }

        [Test()]
        public void patch_makeTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            List<Patch> patches;
            String text1 = "The quick brown fox jumps over the lazy dog.";
            String text2 = "That quick brown fox jumped over a lazy dog.";
            String expectedPatch = "@@ -1,8 +1,7 @@\n Th\n-at\n+e\n  qui\n@@ -21,17 +21,18 @@\n jump\n-ed\n+s\n  over \n-a\n+the\n  laz\n";
            // The second patch must be "-21,17 +21,18", not "-22,17 +21,18" due to rolling context.
            patches = target.patch_make(text2, text1);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Text2+Text1 inputs");

            expectedPatch = "@@ -1,11 +1,12 @@\n Th\n-e\n+at\n  quick b\n@@ -22,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n  laz\n";
            patches = target.patch_make(text1, text2);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Text1+Text2 inputs");

            List<Diff> diffs = target.diff_main(text1, text2, false);
            patches = target.patch_make(diffs);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Diff input");

            patches = target.patch_make(text1, diffs);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Text1+Diff inputs");

            patches = target.patch_make(text1, text2, diffs);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Text1+Text2+Diff inputs (deprecated)");

            patches = target.patch_make("`1234567890-=[]\\;',./", "~!@#$%^&*()_+{}|:\"<>?");
            Assert.AreEqual("@@ -1,21 +1,21 @@\n-%601234567890-=%5b%5d%5c;',./\n+~!@#$%25%5e&*()_+%7b%7d%7c:%22%3c%3e?\n",
                target.patch_toText(patches),
                "patch_toText: Character encoding.");

            diffs = new List<Diff>{
                new Diff(Operation.DELETE, "`1234567890-=[]\\;',./"),
                new Diff(Operation.INSERT, "~!@#$%^&*()_+{}|:\"<>?")};
            CollectionAssert.AreEqual(diffs,
                target.patch_fromText("@@ -1,21 +1,21 @@\n-%601234567890-=%5B%5D%5C;',./\n+~!@#$%25%5E&*()_+%7B%7D%7C:%22%3C%3E?\n") [0].diffs,
                "patch_fromText: Character decoding.");

            text1 = "";
            for (int x = 0; x < 100; x++)
            {
                text1 += "abcdef";
            }
            text2 = text1 + "123";
            expectedPatch = "@@ -573,28 +573,31 @@\n cdefabcdefabcdefabcdefabcdef\n+123\n";
            patches = target.patch_make(text1, text2);
            Assert.AreEqual(expectedPatch, target.patch_toText(patches), "patch_make: Long string with repeats.");
        }

        [Test()]
        public void patch_addPaddingTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            List<Patch> patches;
            patches = target.patch_make("", "test");
            Assert.AreEqual("@@ -0,0 +1,4 @@\n+test\n", target.patch_toText(patches), "patch_addPadding: Both edges full.");
            target.patch_addPadding(patches);
            Assert.AreEqual("@@ -1,8 +1,12 @@\n %00%01%02%03\n+test\n %00%01%02%03\n",
                target.patch_toText(patches),
                "patch_addPadding: Both edges full.");

            patches = target.patch_make("XY", "XtestY");
            Assert.AreEqual("@@ -1,2 +1,6 @@\n X\n+test\n Y\n",
                target.patch_toText(patches),
                "patch_addPadding: Both edges partial.");
            target.patch_addPadding(patches);
            Assert.AreEqual("@@ -2,8 +2,12 @@\n %01%02%03X\n+test\n Y%00%01%02\n",
                target.patch_toText(patches),
                "patch_addPadding: Both edges partial.");

            patches = target.patch_make("XXXXYYYY", "XXXXtestYYYY");
            Assert.AreEqual("@@ -1,8 +1,12 @@\n XXXX\n+test\n YYYY\n", target.patch_toText(patches),
                "patch_addPadding: Both edges none.");
            target.patch_addPadding(patches);
            Assert.AreEqual("@@ -5,8 +5,12 @@\n XXXX\n+test\n YYYY\n", target.patch_toText(patches),
                "patch_addPadding: Both edges none.");
        }

        [Test()]
        public void patch_applyTest()
        {
            diff_match_patchTest target = new diff_match_patchTest();
            List<Patch> patches;
            patches = target.patch_make("The quick brown fox jumps over the lazy dog.", "That quick brown fox jumped over a lazy dog.");
            Object[] results = target.patch_apply(patches, "The quick brown fox jumps over the lazy dog.");
            bool[] boolArray = (bool[])results[1];
            String resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
            Assert.AreEqual("That quick brown fox jumped over a lazy dog.\tTrue\tTrue", resultStr, "patch_apply: Exact match.");

            results = target.patch_apply(patches, "The quick red rabbit jumps over the tired tiger.");
            boolArray = (bool[])results[1];
            resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
            Assert.AreEqual("That quick red rabbit jumped over a tired tiger.\tTrue\tTrue", resultStr, "patch_apply: Partial match.");

            results = target.patch_apply(patches, "I am the very model of a modern major general.");
            boolArray = (bool[])results[1];
            resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
            Assert.AreEqual("I am the very model of a modern major general.\tFalse\tFalse", resultStr, "patch_apply: Failed match.");

            patches = target.patch_make("", "test");
            String patchStr = target.patch_toText(patches);
            target.patch_apply(patches, "");
            Assert.AreEqual(patchStr, target.patch_toText(patches), "patch_apply: No side effects.");

            patches = target.patch_make("The quick brown fox jumps over the lazy dog.", "Woof");
            patchStr = target.patch_toText(patches);
            target.patch_apply(patches, "The quick brown fox jumps over the lazy dog.");
            Assert.AreEqual(patchStr, target.patch_toText(patches), "patch_apply: No side effects with major delete.");

            patches = target.patch_make("", "test");
            results = target.patch_apply(patches, "");
            boolArray = (bool[])results[1];
            resultStr = results[0] + "\t" + boolArray[0];
            Assert.AreEqual("test\tTrue", resultStr, "patch_apply: Edge exact match.");

            patches = target.patch_make("XY", "XtestY");
            results = target.patch_apply(patches, "XY");
            boolArray = (bool[])results[1];
            resultStr = results[0] + "\t" + boolArray[0];
            Assert.AreEqual("XtestY\tTrue", resultStr, "patch_apply: Near edge exact match.");

            patches = target.patch_make("y", "y123");
            results = target.patch_apply(patches, "x");
            boolArray = (bool[])results[1];
            resultStr = results[0] + "\t" + boolArray[0];
            Assert.AreEqual("x123\tTrue", resultStr, "patch_apply: Edge partial match.");
        }

        private static String[] diff_rebuildtexts(List<Diff> diffs)
        {
            String[] text = { "", "" };
            foreach (Diff myDiff in diffs)
            {
                if (myDiff.operation != Operation.INSERT)
                {
                    text[0] += myDiff.text;
                }
                if (myDiff.operation != Operation.DELETE)
                {
                    text[1] += myDiff.text;
                }
            }
            return text;
        }
    }
}

