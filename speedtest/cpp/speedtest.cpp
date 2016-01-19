// Copyright 2011 Google Inc.
// All Rights Reserved.
// Author: fraser@google.com

#include <QtCore>
#include "diff_match_patch.h"
#include "diff_match_patch_test.h"

QString readFile(const char *filename) {
  QFile file(filename);
  file.open(QIODevice::ReadOnly);
  QTextStream in(&file);
  QString text = in.readAll();
  file.close();
  return text;
}

int main(int argc, char **argv) {
  Q_UNUSED(argc);
  Q_UNUSED(argv);
  QString text1 = readFile("speedtest1.txt");
  QString text2 = readFile("speedtest2.txt");

  diff_match_patch dmp = diff_match_patch();
  dmp.Diff_Timeout = 0.0;

  // Execute one reverse diff as a warmup.
  dmp.diff_main(text2, text1, false);

  QTime t;
  t.start();
  dmp.diff_main(text1, text2, false);
  qDebug("Elapsed time: %d ms", t.elapsed());
  return 0;
}
