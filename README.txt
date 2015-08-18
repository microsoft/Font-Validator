Usage:
======

"FontVal.exe" is the GUI, and "FontValidator.exe" shows usage
and example if run without arguments; both should be
self-explanatory. (prepend with "mono" if runs on non-windows).

The GUI's build-in help requires a CHM viewer, defaults to
chmsee on Linux, or via env variable MONO_HELP_VIEWER. 

The GUI on X11/mono needs the env variable
"MONO_WINFORMS_XIM_STYLE=disabled" set to work around a bug.
(https://bugzilla.xamarin.com/show_bug.cgi?id=28047
"Bug 28047 - Forms on separare threads -- Fatal errors/crashes").

Using "xsltproc" (commonly available on Linux and Mac OS X)
one can generate an HTML report from the XML one, e.g.
"xsltproc fval.xsl arial.ttf.report.xml > arial.ttf.report.htm"

Build:
======

(adapted for building with mono instead of MS C#)

Typing "make" should do. "make gendoc" and a few extra manual
steps is only needed if one is making major changes of adding
new tests or new error/warning codes. "make clean" deletes
the newly generated binaries.

Currently the CHM Help file requires MS Help Workshop to build,
so is bundled in the bin/ directory; fval.xsl is also
rarely changed, so duplicated there. The "bin" directory
after "make" is the whole usable binary package.

TODO (missing/broken):
======================

The 3 Rasterization Tests (BW, Grayscale, ClearType),
1 of the 4 LTSH tests (LTSH_yPels), 1 of the 4 VDMX tests
(VDMX_CompareToCalcData) and 1 of the 8 VDMX tests (hdmx_Widths)
requires the proprietary font scalar.

1 of 3 DSIG tests (DSIG_VerifySignature) requires
Windows Trust API.

1 of 21 OS/2 tests (OS_2_CodePageRanges) requires Windows
Code Page's.

Viewing XML reports (and print, text search/select/copy therein)
requires MSIE and Active X.

summary: 194 table tests, 5 does not work.

Annoyances:
===========

Table order is case-insensitive sorted in GUI, but case-sensitive
sorted in output. Would prefer all CAPs and sorted consistently.

GUI allows in-memory reports, so CMD does not warn nor abort when
output location is invalid, and wastes time producing no output.
