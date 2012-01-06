/*
 * Encode/Decode functions for Dart
 *
 * Copyright 2011 Google Inc.
 * Neil Fraser (fraser@google.com)
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
 */

#library('URI Encode Decode');

/**
 * Implementation of JavaScript's encodeURI function.
 * [text] is the string to escape.
 * Returns the escaped string.
 */
String encodeURI(text) {
  StringBuffer encodedText = new StringBuffer();
  final String whiteList = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' +
                           'abcdefghijklmnopqrstuvwxyz' +
                           '0123456789-_.!~*\'()#;,/?:@&=+\$';
  final String hexDigits = '0123456789ABCDEF';
  for (int i = 0; i < text.length; i++) {
    if (whiteList.indexOf(text[i]) != -1) {
      // This character doesn't need encoding.
      encodedText.add(text[i]);
      continue;
    }
    int charCode = text.charCodeAt(i);
    List<int> byteList = [];
    if (charCode < 0x80) {
      // One-byte code.
      // 0xxxxxxx
      byteList.add(charCode);
    } else if (charCode < 0x800) {
      // Two-byte code.
      // 110xxxxx 10xxxxxx
      byteList.add(charCode >> 6 | 0xC0);
      byteList.add(charCode & 0x3F | 0x80);
    } else if (0xD800 <= charCode && charCode < 0xDC00) {
      // Low surrogate.  Next char must be a high surrogate.
      int nextCharCode = text.length == i + 1 ? 0 : text.charCodeAt(i + 1);
      if (0xDC00 <= nextCharCode && nextCharCode < 0xE000) {
        // Four-byte surrogate pair.
        // 11110xxx 10xxxxxx 10xxyyyy 10yyyyyy
        // Where xxxxxxxxxxx is offset by 1000000 (0x40)
        charCode += 0x40;
        byteList.add(charCode >> 8 & 0x7 | 0xF0);
        byteList.add(charCode >> 2 & 0x3F | 0x80);
        byteList.add(((charCode & 0x3) << 4) |
            (nextCharCode >> 6 & 0xF) | 0x80);
        byteList.add(nextCharCode & 0x3F | 0x80);
      } else {
        throw new
            IllegalArgumentException('URI malformed: Orphaned low surrogate.');
      }
      // Skip next character.
      i++;
    } else if (0xDC00 <= charCode && charCode < 0xE000) {
      throw new 
          IllegalArgumentException('URI malformed: Orphaned high surrogate.');
    } else if (charCode < 0x10000) {
      // Three-byte code.
      // 1110xxxx 10xxxxxx 10xxxxxx
      byteList.add(charCode >> 12 | 0xE0);
      byteList.add(charCode >> 6 & 0x3F | 0x80);
      byteList.add(charCode & 0x3F | 0x80);
    }
    for (int i = 0; i < byteList.length; i++) {
      encodedText.add('%').add(hexDigits[byteList[i] >> 4])
                          .add(hexDigits[byteList[i] & 0xF]);        
    }
  }
  return encodedText.toString();
}

/**
 * Implementation of JavaScript's encodeURIComponent function.
 * [text] is the string to escape.
 * Return the escaped string.
 */
String encodeURIComponent(text) {
  // This is the same as encodeURI except the following are also escaped:
  // #;,/?:@&=+$  -> %23%3B%2C%2F%3F%3A%40%26%3D%2B%24
  text = encodeURI(text);
  return text.replaceAll('#', '%23')
             .replaceAll(';', '%3B')
             .replaceAll(',', '%2C')
             .replaceAll('/', '%2F')
             .replaceAll('?', '%3F')
             .replaceAll(':', '%3A')
             .replaceAll('@', '%40')
             .replaceAll('&', '%26')
             .replaceAll('=', '%3D')
             .replaceAll('+', '%2B')
             .replaceAll('\$', '%24');
}

/**
 * Implementation of JavaScript's decodeURI function.
 * [text] is the string to unescape.
 * Returns the unescaped string.
 */
String decodeURI(text) {
  final String hexDigits = '0123456789ABCDEF';
  // First, break up the text into parts.
  List<String> parts = text.split('%');
  int state = 0;
  int multiByte;  // Temp register for assembling a multi-byte value.
  bool surrogate = false;
  // Skip the first element, it's guaranteed to be a (possibly empty) string.
  for (int i = 1; i < parts.length; i++) {
    String part = parts[i];
    if (part.length < 2) {
      throw new IllegalArgumentException('URI malformed: Missing digits.');
    }
    int hex1 = hexDigits.indexOf(part[0].toUpperCase());
    int hex2 = hexDigits.indexOf(part[1].toUpperCase());
    parts[i] = part.substring(2);
    if (hex1 == -1 || hex2 == -1) {
      throw new IllegalArgumentException('URI malformed: Invalid digits.');
    }
    int charCode = hex1 * 16 + hex2;
    if (state == 0) {
      if (charCode < 0x80) {
        // One-byte code.
        // 0xxxxxxx
        multiByte = charCode;
        state = 0;
      } else if ((charCode & 0xE0) == 0xC0) {
        // Two-byte code.
        // 110xxxxx 10xxxxxx
        multiByte = charCode & 0x1F;
        state = 1;
      } else if ((charCode & 0xF0) == 0xE0) {
        // Three-byte code.
        // 1110xxxx 10xxxxxx 10xxxxxx
        multiByte = charCode & 0xF;
        state = 2;
      } else if ((charCode & 0xF8) == 0xF0) {
        // Four-byte surrogate pair.
        // 11110xxx 10xxxxxx 10xxyyyy 10yyyyyy
        multiByte = charCode & 0x7;
        state = 3;
        surrogate = true;
      } else {
        throw new IllegalArgumentException('URI malformed: Unknown Unicode.');
      }
    } else {
      // All continuation bytes are in the form 10xxxxxx.
      if ((charCode & 0xC0) != 0x80) {
        throw new IllegalArgumentException('URI malformed: Expect 10xxxxxx.');
      }
      multiByte = (multiByte << 6) | (charCode & 0x3F);
      state--;
    }
    if (state == 0) {
      // Character is fully assembled.  Add to string.
      if (surrogate) {
        surrogate = false;
        // Insert surrogate pair.
        // xxxxxxxxxxxyyyyyyyyyy (21 bits)
        // Where xxxxxxxxxxx is offset by 1000000 (0x40)
        int x = (multiByte >> 10) - 0x40 + 0xD800;
        int y = (multiByte & 0x3FF) + 0xDC00;
        if (x >= 0xDC00 || y >= 0xE000) {
          throw new 
              IllegalArgumentException('URI malformed: Invalid surrogate.');
        }
        parts.insertRange(i, 1, new String.fromCharCodes([x, y]));
      } else {
        // Insert a single character.
        parts.insertRange(i, 1, new String.fromCharCodes([multiByte]));
      }
      // Skip the element we just inserted.
      i++;
    } else {
      // This code must be directly followed by another code.
      if (!parts[i].isEmpty()) {
        throw new IllegalArgumentException('URI malformed: Incomplete code.');
      }
    }
  }
  if (state != 0) {
    throw new IllegalArgumentException('URI malformed: Truncated code.');
  }
  return Strings.join(parts, '');
}

/**
 * Implementation of JavaScript's decodeURIComponent function.
 * [text] is the string to unescape.
 * Returns the unescaped string.
 */
String decodeURIComponent(text) {
  return decodeURI(text);
}
