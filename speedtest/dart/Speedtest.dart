import 'dart:html';
import 'DiffMatchPatch.dart';

void main() {
  document.query('#launch').on.click.add((e) {
    launch();
  });
  document.query('#outputdiv').innerHTML = '';
}

void launch() {
  String text1 = document.query('#text1').value;
  String text2 = document.query('#text2').value;

  DiffMatchPatch dmp = new DiffMatchPatch();
  dmp.Diff_Timeout = 0.0;

  // No warmup loop since it risks triggering an 'unresponsive script' dialog.
  Date date_start = new Date.now();
  List<Diff> d = dmp.diff_main(text1, text2, false);
  Date date_end = new Date.now();

  var ds = dmp.diff_prettyHtml(d);
  document.query('#outputdiv').innerHTML =
      '$ds<BR>Time: ${date_end.difference(date_start)} (h:mm:ss.mmm)';
}
