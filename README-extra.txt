Extended TODO's
===============

(interesting/non-essential tasks, in no particular order)

Fix the "MONO_WINFORMS_XIM_STYLE=disabled" bug.

https://bugzilla.xamarin.com/show_bug.cgi?id=34090
"mono's xsd tool generates invalid xsd"

Fix the monodevelop PostBuildEvent bug (file it first!).

Fix the large constructor bug (file it first!).

https://bugs.winehq.org/show_bug.cgi?id=39297
"kernel32.IsValidCodePage and friends don't support code page 708."
https://bugs.winehq.org/show_bug.cgi?id=39298
"kernel32 does not support custom nls installation."
Mono does support it: mcs/class/I18N/Rare/CP708.cs

https://bugzilla.xamarin.com/show_bug.cgi?id=34088
"System.Text.Encoding.GetEncoding doesn't support code page 10001 - 10008"

https://bugzilla.xamarin.com/show_bug.cgi?id=34089
"System.Windows.Forms.MenuItem shows 'CTRL-X' for no events, MS's SWF doesn't."

https://bugzilla.xamarin.com/show_bug.cgi?id=34091
"resgen introduces dependency to versioned System.Windows.Forms"

https://bugzilla.xamarin.com/show_bug.cgi?id=35563
Bug 35563 - ASN1 class behaves differently under mono vs dotnet

https://bugzilla.xamarin.com/show_bug.cgi?id=36295
"Bug 36295 - missing attributes in System.Security.Cryptography.Pkcs.SignedCms.Decode()"

https://bugzilla.xamarin.com/show_bug.cgi?id=36297
"Bug 36297 - System.Security.Cryptography.Pkcs.SignedCms.CheckSignature() does not throw exception on empty SignedCms."

https://bugs.winehq.org/show_bug.cgi?id=39681
"Bug 39681 - Unimplemented function crypt32.dll.CryptMsgVerifyCountersignatureEncoded"

Find out why Wine-Mono complains about Illegal IL byte-codes,
and why the XML viewer never worked in wine.

IronPython scripted access to Font Validator's internals.

The AutoScaleBaseSize issue in the GUI
(http://www.mono-project.com/docs/faq/winforms/:
"My forms are sized improperly")

Extend ComputeMetrics to Type 1 fonts.
Use ftview with a broken AFM to check.

'History' effect of Freetype's load_glyph/set_pixel_size
win 8.1 symbol.ttf  (glyph# = 85)

Why FontVal cannot see C:\windows\fonts at all?

Why firefox won't display xml? (IE does)


Note:
=====
https://bugzilla.redhat.com/show_bug.cgi?id=1238845
Bug 1238845 - Needs rebuild against newer xulrunner
https://bugzilla.redhat.com/show_bug.cgi?id=1228584
Bug 1228584 - mono: pedump fails with segmentation fault
https://bugzilla.redhat.com/show_bug.cgi?id=1255204
Bug 1255204 - support xbuild targeting .net framework 2.0
https://bugzilla.xamarin.com/show_bug.cgi?id=35309
Bug 35309 - cert-sync output message glitch
https://bugzilla.xamarin.com/show_bug.cgi?id=35311
Bug 35311 - Authenticode support for win64 binaries.
https://bugzilla.redhat.com/show_bug.cgi?id=1238842
Bug 1238842 - "Warning: Mismatch between the program and library build versions detected."
https://bugzilla.redhat.com/show_bug.cgi?id=1278194
"Bug 1278194 - wget Illegal instruction (core dumped)" - openssl
Bug filed: copyright violation of a specific font
https://bugzilla.xamarin.com/show_bug.cgi?id=592
Bug 592 - [Mono-Security]: Microsoft files' digital certificates can't be traced to a trusted root
http://lists.ximian.com/pipermail/mono-bugs/2007-April/056578.html
(defunct) http://bugzilla.ximian.com/show_bug.cgi?id=81450
Two authenticode issues
