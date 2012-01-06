/**
 * Test Harness for Encode/Decode functions for Dart
 *
 * Copyright 2011 Google Inc.
 * Neil Fraser (fraser@google.com)
 *
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#import('EncodeDecode.dart');

void testEncodeURI() {
  Expect.equals('', encodeURI(''), 'encodeURI: Null case.');
  Expect.equals('ABCabc123',
      encodeURI('ABCabc123'),
      'encodeURI: Alphanumeric.');
  Expect.equals('-_.!~*\'()#;,/?:@&=+\$',
      encodeURI('-_.!~*\'()#;,/?:@&=+\$'),
      'encodeURI: No escape.');
  Expect.equals('%60%25%5E%7B%7D%5B%5D%5C%7C%22%3C%3E%20%09%0D%0A',
      encodeURI('`%^{}[]\\|"<> \t\r\n'),
      'encodeURI: 1 byte codes.');
  Expect.equals('%C2%80-%DA%80-%DF%BF',
      encodeURI('\u0080-\u0680-\u07ff'),
      'encodeURI: 2 byte codes.');
  Expect.equals('%E0%A0%80-%E7%A2%90-%EF%BF%BF',
      encodeURI('\u0800-\u7890-\uffff'),
      'encodeURI: 3 byte codes.');
  Expect.equals('%F3%90%80%80-%F4%8F%BF%BF',
      encodeURI('\udb00\udc00-\udbff\udfff'),
      'encodeURI: 4 byte codes.');
}

void testEncodeURIComponent() {
  Expect.equals('', encodeURIComponent(''), 'encodeURIComponent: Null case.');
  Expect.equals('ABCabc123',
      encodeURIComponent('ABCabc123'),
      'encodeURIComponent: Alphanumeric.');
  Expect.equals('-_.!~*\'()',
      encodeURIComponent('-_.!~*\'()'),
      'encodeURIComponent: No escape.');
  Expect.equals('%23%3B%2C%2F%3F%3A%40%26%3D%2B%24',
      encodeURIComponent('#;,/?:@&=+\$'),
      'encodeURIComponent: Escape beyond encodeURI.');
  Expect.equals('%60%25%5E%7B%7D%5B%5D%5C%7C%22%3C%3E%20%09%0D%0A',
      encodeURIComponent('`%^{}[]\\|"<> \t\r\n'),
      'encodeURIComponent: 1 byte codes.');
  Expect.equals('%DA%80-%E7%A2%90-%F4%8F%BF%BF',
      encodeURIComponent('\u0680-\u7890-\udbff\udfff'),
      'encodeURIComponent: Multiple byte codes.');
}

void testDecodeURI() {
  Expect.equals('', decodeURI(''), 'encodeURI: Null case.');
  Expect.equals('ABCabc123',
      decodeURI('ABCabc123'),
      'decodeURI: Alphanumeric.');
  Expect.equals('`%^{}[]\\|"<> \t\r\n',
      decodeURI('%60%25%5E%7B%7D%5B%5D%5C%7C%22%3C%3E%20%09%0D%0A'),
      'decodeURI: 1 byte codes.');
  Expect.equals('^^',
      decodeURI('%5e%5E'),
      'decodeURI: Case insensitivity.');
  Expect.equals('\u0080-\u0680-\u07ff',
      decodeURI('%C2%80-%DA%80-%DF%BF'),
      'decodeURI: 2 byte codes.');
  Expect.equals('\u0800-\u7890-\uffff',
      decodeURI('%E0%A0%80-%E7%A2%90-%EF%BF%BF'),
      'decodeURI: 3 byte codes.');
  Expect.equals('\udb00\udc00-\udbff\udfff',
      decodeURI('%F3%90%80%80-%F4%8F%BF%BF'),
      'decodeURI: 4 byte codes.');
}

void testDecodeURIComponent() {
  // No need to test, decodeURIComponent is just an alias of decodeURI.
}

// Run each test.
main() {
  testEncodeURI();
  testEncodeURIComponent();
  testDecodeURI();
  testDecodeURIComponent();

  print('All tests passed.');
}
