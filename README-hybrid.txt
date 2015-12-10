The "hybrid" build is an inter-rim fork where the 2 missing
functionalities: the rasterization tests,
the XML report viewer, are provided by scavenged DLLs
from the 2003 binary. As such, it is meant to run on windows only.

To Build
========

The hybrid build consists of the following 5 code changes
(1 addition and 1 revert in Compat/Compat.cs, 1 revert in
FontVal/ResultsForm.cs,
and 2 adjustments in the Makefile)
and 3 binary changes. As can be seen the "code change"
is mainly removing/reverting work that's already done.

The public hybrid code branch consists of just the "code changes"
and it is up to the individual to acquire the DLLs from
the 2003 binary to build in this manner.

============================================================
commit ac619c90395aad24d14953bcf2da5b06bd63c6ca
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:55:27 2015 +0100

    update dependency

:100644 100644 0dc3ff9... fcbbbd4... M	Makefile

commit 3e5f55366155ae8fed2598b3fa8b7636d79dc075
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:49:21 2015 +0100

    ms scaler hooks from trunk

:100644 100644 027bde3... e753c44... M	Compat/Compat.cs

commit 2b9a39d9257434cbeaf534f9b19a3ec0ea52e6e6
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:35:48 2015 +0100

    dlls from 2003 binary

:000000 100644 0000000... 2142469... A	bin/AxInterop.SHDocVw.dll
:000000 100644 0000000... eb75127... A	bin/Interop.SHDocVw.dll
:000000 100644 0000000... c62119f... A	bin/Microsoft.mshtml.dll
:000000 100644 0000000... af9a486... A	bin/truetype.dll

commit c2930443be0c3cfa839ea0782e8a2d67b059c2c5
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:28:36 2015 +0100

    Re-enable the XML viewer.
    
    Revert "ifdef out all axWebBrowser1 stuff with __MonoCS__"
    
    This reverts commit 4526b11f8fc52fd77f7ee99924b5c5a832bb7a53.

:100644 100644 ef161fd... d8ecbd5... M	FontVal/ResultsForm.cs

commit 7b12513cd398c0b147b29e5c79e276847c004ec6
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:22:33 2015 +0100

    move 32-bit dlls to same dir

:100755 000000 78ce992... 0000000... D	bin/Win32/freetype6.dll
:100755 000000 12140fc... 0000000... D	bin/Win32/iconv.dll
:100755 000000 8691b4e... 0000000... D	bin/Win32/libbz2-1.dll
:100755 000000 78ce992... 0000000... D	bin/Win32/libfreetype-6.dll
:100755 000000 a9015eb... 0000000... D	bin/Win32/libgcc_s_sjlj-1.dll
:100755 000000 02dd00b... 0000000... D	bin/Win32/libglib-2.0-0.dll
:100755 000000 7316a1a... 0000000... D	bin/Win32/libharfbuzz-0.dll
:100755 000000 9cd3753... 0000000... D	bin/Win32/libintl-8.dll
:100755 000000 e1c18e9... 0000000... D	bin/Win32/libpng16-16.dll
:100755 000000 035aece... 0000000... D	bin/Win32/libwinpthread-1.dll
:100644 000000 b39de53... 0000000... D	bin/Win32/zlib1.dll
:000000 100755 0000000... 78ce992... A	bin/freetype6.dll
:000000 100755 0000000... 12140fc... A	bin/iconv.dll
:000000 100755 0000000... 8691b4e... A	bin/libbz2-1.dll
:000000 100755 0000000... 78ce992... A	bin/libfreetype-6.dll
:000000 100755 0000000... a9015eb... A	bin/libgcc_s_sjlj-1.dll
:000000 100755 0000000... 02dd00b... A	bin/libglib-2.0-0.dll
:000000 100755 0000000... 7316a1a... A	bin/libharfbuzz-0.dll
:000000 100755 0000000... 9cd3753... A	bin/libintl-8.dll
:000000 100755 0000000... e1c18e9... A	bin/libpng16-16.dll
:000000 100755 0000000... 035aece... A	bin/libwinpthread-1.dll
:000000 100644 0000000... b39de53... A	bin/zlib1.dll

commit efd849e12c87fd6668820e562458a6ec6c7ad8fe
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:21:54 2015 +0100

    remove 64-bit dlls

:100755 000000 1595f9d... 0000000... D	bin/Win64/freetype6.dll
:100755 000000 1c5dcca... 0000000... D	bin/Win64/iconv.dll
:100755 000000 bb0b642... 0000000... D	bin/Win64/libbz2-1.dll
:100755 000000 1595f9d... 0000000... D	bin/Win64/libfreetype-6.dll
:100755 000000 c2d0ddc... 0000000... D	bin/Win64/libgcc_s_seh-1.dll
:100755 000000 4487fd2... 0000000... D	bin/Win64/libglib-2.0-0.dll
:100755 000000 a1e58bc... 0000000... D	bin/Win64/libharfbuzz-0.dll
:100755 000000 1149748... 0000000... D	bin/Win64/libintl-8.dll
:100755 000000 dcbc691... 0000000... D	bin/Win64/libpng16-16.dll
:100755 000000 7331ae6... 0000000... D	bin/Win64/libwinpthread-1.dll
:100644 000000 19ff8bf... 0000000... D	bin/Win64/zlib1.dll

commit d20f6315c81dbe7a0f185a757064ea91cb740671
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:19:17 2015 +0100

    remove dual arch set-up

:100644 100644 990daca... 027bde3... M	Compat/Compat.cs

commit 29d6e15706d15b9c17cf6e67e6f50a85c5392846
Author: Hin-Tak Leung <htl10@users.sourceforge.net>
Date:   Sat Oct 10 17:16:10 2015 +0100

    build for x86 only

:100644 100644 81a0f1c... 0dc3ff9... M	Makefile
============================================================
