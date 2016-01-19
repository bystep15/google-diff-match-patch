#QT += sql xml network
TEMPLATE = app
CONFIG += qt debug_and_release

mac {
  CONFIG -= app_bundle
}

# don't embed the manifest for now (doesn't work :( )
#CONFIG -= embed_manifest_exe

FORMS =

HEADERS = diff_match_patch.h

SOURCES = diff_match_patch.cpp speedtest.cpp

RESOURCES =
