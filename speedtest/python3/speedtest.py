#!/usr/bin/python3
#
# Copyright 2010 Google Inc.
# All Rights Reserved.


"""Diff Speed Test
"""

__author__ = "fraser@google.com (Neil Fraser)"

import imp
import gc
import sys
import time
import diff_match_patch as dmp_module
# Force a module reload.  Allows one to edit the DMP module and rerun the test
# without leaving the Python interpreter.
imp.reload(dmp_module)

def main():
  text1 = open("speedtest1.txt").read()
  text2 = open("speedtest2.txt").read()

  dmp = dmp_module.diff_match_patch()
  dmp.Diff_Timeout = 0.0

  # Execute one reverse diff as a warmup.
  dmp.diff_main(text2, text1, False)
  gc.collect()

  start_time = time.time()
  dmp.diff_main(text1, text2, False)
  end_time = time.time()
  print("Elapsed time: %f" % (end_time - start_time))

if __name__ == "__main__":
  main()

