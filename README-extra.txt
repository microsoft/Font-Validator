Extended TODO's
===============

(interesting/non-essential tasks, in no particular order)

Fix the "MONO_WINFORMS_XIM_STYLE=disabled" bug.

Fix the mono xsd bug (file it first!).

Fix the monodevelop PostBuildEvent bug (file it first!).

Find out why Wine-Mono complains about Illegal IL byte-codes,
and why the XML viewer never worked in wine.

IronPython scripted access to Font Validator's internals.

Wrap the release binary's proprietary font scalar
back into the current code, for compatibility testing
on windows. (The code is about 6 years newer than the
release binary).

Spacing-layout issue in mono/X11's winform: the
"Ctrl-O" ">" in the GUI's File->Open is squashed;
the table test tab is without a scrollbar, but
is invisible unless one resize slightly.
