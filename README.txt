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

The rasterer-dependent tests (HDMX/LTSH/VDMX) requires an enhancement
which first appears in FreeType 2.6.1. Linux users can use
LD_LIBRARY_PATH env to load newer library than system's; Mac OS
users should edit "bin/SharpFont.dll.config" . The bundled
SharpFont.dll is patched with an equivalent
"0001-adding-ComputeMetrics.patch" ; The bundled win64 FreeType dll
was built with an additional win64-specific patch,
"freetype-win64.patch" .

Currently the CHM Help file requires MS Help Workshop to build,
so is bundled in the bin/ directory; fval.xsl is also
rarely changed, so duplicated there. The "bin" directory
after "make" is the whole usable binary package.

SharpFont requires xbuild from monodevelop to build.
(git://github.com/Robmaister/SharpFont.git)

TODO (missing/broken):
======================

The 3 Rasterization Tests (BW, Grayscale, ClearType)
requires a font scaler, and are not yet implemented via FreeType.

1 of 3 DSIG tests (DSIG_VerifySignature) requires
Windows Trust API.

Viewing XML reports (and print, text search/select/copy therein)
requires MSIE and Active X.

summary: 194 table tests, 1 do not work.

Many "Required field missing" in GenerateFValData/OurData.xml

Issues mentioned in "FDK/Technical Documentation/MSFontValidatorIssues.htm"

Many post-2nd (i.e. 2009) edition changes, such as CBLC/CBDT and other new tables.

Caveat:
=======

The 3 Rasterer-dependent metrics tests (LTSH/HDMX/VDMX) with
a FreeType backend are known to behave somewhat differently
compared to the MS Font Scaler backend. In particular:

HDMX: differ by up to two pixels (i.e. either side)

LTSH: FreeType backend shows a lot more non-linearity than
      an MS backend; the result with MS backend should be a sub-set
      of FreeType's, however.

VDMX: The newer code has a built-in 10% tolerance, so the newer
      FreeType backend result should be a sub-set of (the older)
      MS result. Also, note that MS 2003 binary seems to be wrong for
      non-square resolutions, so results differ by design.

On the other hand, the FreeType backend is up to 5x faster (assuming
single-thread), and support CFF rastering. It is not known
whether the MS backend is multi-threaded, but the FreeType
backend is currently single-threaded.

Incomplete CFF checks:

val_CFF.cs   I.CFF_I_NotValidated
val_head.cs: I._TEST_I_NotForCFF head_MinMaxValues
val_hhea.cs: I._TEST_I_NotForCFF hhea_MinMax
val_OS2.cs:  I._TEST_I_NotForCFF OS/2_xAvgCharWidth

Annoyances:
===========

Table order is case-insensitive sorted in GUI, but case-sensitive
sorted in output. Would prefer sorted consistently.

GUI allows in-memory reports, so CMD does not warn nor abort when
output location is invalid, and wastes time producing no output.
Only -report-dir aborts on that; no workaround to -report-in-font-dir nor
temp dir yet.
