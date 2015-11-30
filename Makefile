# Copyright (c) Hin-Tak Leung

# All rights reserved.

# MIT License

# Permission is hereby granted, free of charge, to any person obtaining a copy of
# this software and associated documentation files (the ""Software""), to deal in
# the Software without restriction, including without limitation the rights to
# use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
# of the Software, and to permit persons to whom the Software is furnished to do
# so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

# Overview:

# COMPAT_LIBRARIES: Compat - is the replacement stub dll for everything
#   that Microsoft did not release; it needs to be built first unless
#   genuine Micorsoft proprietry dlls (e.g. from the release binary) are
#   present.

# LIBRARIES: OTFontFile ValCommon GMath Glyph OTFontFileVal -
#   !!DO NOT CHANGE THE LISTED ORDER!! - the later listed one depends
#   on the earlier ones, so they needs to be build in this order.

# GENDOC_EXE: GenerateFValData - this tool generates the help files and
#   a few rarely changed source files from the complete list of tests and
#   error status'es. So it only needs to be built and run when new tests
#   or new types of errors are added.

# MAIN_EXE: FontVal(=GUI) FontValidator(=Cmd) - the executables.

# The constituents of the CHM file is dumped into NewHelp after
# GenerateFValData is run; the CHM file requires MS HTML Help Workshop
# to build.

COMPAT_LIBRARIES_1=Compat
COMPAT_LIBRARIES_2=Compat.2nd
LIBRARIES_1=OTFontFile
LIBRARIES_2=ValCommon GMath Glyph OTFontFileVal

# COMPAT_LIBRARIES_2 is not real.
COMPAT_LIBRARIES=$(COMPAT_LIBRARIES_1)
LIBRARIES=$(LIBRARIES_1) $(LIBRARIES_2)
GENDOC_EXE=GenerateFValData
MAIN_EXE=FontVal FontValidator DSIGInfo

MCS=mcs -debug- -optimize+

ifeq "$(BUILD)" ".net2"
EXTRA_DEV_OPTS=/nostdlib /platform:AnyCPU /reference:/usr/lib/mono/2.0/System.dll \
/reference:/usr/lib/mono/2.0/mscorlib.dll \
-lib:/usr/lib/mono/2.0
else
ifeq "$(USE_MONO_SECURITY)" "true"
EXTRA_DEV_OPTS=-define:HAVE_MONO_X509 -r:Mono.Security
else
EXTRA_DEV_OPTS=
endif
endif

default:
	mkdir -p bin
	for target in $(COMPAT_LIBRARIES_1) $(LIBRARIES_1) $(COMPAT_LIBRARIES_2) $(LIBRARIES_2); do \
            $(MAKE) bin/$${target}.dll ; \
        done
	for target in $(MAIN_EXE); do \
            $(MAKE) bin/$${target}.exe ; \
        done

.PHONY: clean gendoc default bin/Compat.2nd.dll

clean:
	$(RM) GenerateFValData/bin/Debug/GenerateFValData.exe
	for target in $(COMPAT_LIBRARIES) $(LIBRARIES); do \
            $(RM) bin/$${target}.dll ; \
        done
	for target in $(MAIN_EXE); do \
            $(RM) bin/$${target}.exe ; \
        done

gendoc: GenerateFValData/bin/Debug/GenerateFValData.exe

bin/Compat.dll:
	( cd Compat     && \
        $(MCS) -target:library $(EXTRA_DEV_OPTS) -lib:../bin/ -r:SharpFont -r:System.Windows.Forms -out:../$@ *.cs )

# Not a real target
bin/Compat.2nd.dll:
	( cd Compat.2nd     && \
        $(MCS) -target:library $(EXTRA_DEV_OPTS) -lib:../bin/ \
        -r:SharpFont -r:System.Windows.Forms \
        -r:System.Security \
        -r:OTFontFile \
        -out:../bin/Compat.dll *.cs \
        ../DSIGInfo/DSIGInfo.cs \
        ../mcs-class-Mono.Security/ASN1.cs \
        ../mcs-class-Mono.Security/ASN1Convert.cs \
        ../Compat/*.cs )

bin/OTFontFile.dll:
	( cd OTFontFile && \
        $(MCS) -target:library $(EXTRA_DEV_OPTS) -out:../$@ *.cs )

bin/ValCommon.dll:
	( cd ValCommon  && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) -r:OTFontFile -target:library -out:../$@ *.cs )

bin/GMath.dll:
	( cd GMath      && \
	$(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) -r:OTFontFile -r:ValCommon -target:library -out:../$@ *.cs )

bin/Glyph.dll:
	( cd Glyph      && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) -r:OTFontFile -r:ValCommon -r:GMath \
            -resource:NS_Glyph.GErrStrings.resources \
            -target:library -out:../$@ *.cs )

# running just xbuild (from monodevelop) plain also work for GenerateFValData.
# but it's implementation of PostBuildEvent is lacking.
GenerateFValData/bin/Debug/GenerateFValData.exe:
	( cd GenerateFValData && \
        mkdir -p bin/Debug && \
        $(MCS) -r:System.Web  -target:exe \
            -out:bin/Debug/GenerateFValData.exe *.cs Properties/AssemblyInfo.cs )
	@mkdir -p NewHelp
	@echo
	@echo
	@echo "The deposit location, etc mimics what msbuild/xbuild does;"
	@echo "You should go into GenerateFValData/bin/Debug/ and run"
	@echo "GenerateFValData at this point, there, which would write a lot of files"
	@echo "into NewHelp, as well as info OTFontFileVal."

bin/OTFontFileVal.dll:
	( cd OTFontFileVal && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) \
            -r:Compat -r:System.Windows.Forms \
            -r:OTFontFile -r:ValCommon -r:Glyph -r:GMath \
	    -resource:OTFontFileVal.ValStrings.resources \
            -target:library -out:../$@ *.cs )

bin/FontValidator.exe:
	( cd FontValidator && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) -r:OTFontFileVal -r:OTFontFile -r:ValCommon \
        -target:exe -out:../$@ *.cs )

bin/FontVal.exe:
	( cd FontVal && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) \
            -r:OTFontFileVal -r:OTFontFile -r:ValCommon -r:System.Windows.Forms \
            -r:System.Drawing -r:System.Data -r:Compat \
            -resource:FontVal.Form1.resources \
            -resource:FontVal.FormAbout.resources \
            -resource:FontVal.FormReportOptions.resources \
            -resource:FontVal.FormTransform.resources \
            -resource:FontVal.ResultsForm.resources \
            -target:winexe -out:../$@ *.cs )

bin/DSIGInfo.exe: DSIGInfo/DSIGInfo.cs
	( cd DSIGInfo && \
        $(MCS) -lib:../bin/ $(EXTRA_DEV_OPTS) \
        -r:System.Security \
        -target:exe -out:../$@ *.cs \
        ../mcs-class-Mono.Security/ASN1.cs \
        ../mcs-class-Mono.Security/ASN1Convert.cs \
        ../OTFontFile/* )
