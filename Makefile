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

COMPAT_LIBRARIES=Compat
LIBRARIES=OTFontFile ValCommon GMath Glyph OTFontFileVal
GENDOC_EXE=GenerateFValData
MAIN_EXE=FontVal FontValidator

default:
	mkdir -p bin
	for target in $(COMPAT_LIBRARIES) $(LIBRARIES); do \
            $(MAKE) bin/$${target}.dll ; \
        done
	for target in $(MAIN_EXE); do \
            $(MAKE) bin/$${target}.exe ; \
        done

.PHONY: clean gendoc default

clean:
	$(RM) bin/*.dll bin/*.exe GenerateFValData/bin/Debug/GenerateFValData.exe

gendoc: GenerateFValData/bin/Debug/GenerateFValData.exe

bin/Compat.dll:
	( cd Compat     && \
        mcs  -target:library -r:System.Windows.Forms -out:../$@ *.cs )

bin/OTFontFile.dll:
	( cd OTFontFile && \
        mcs -target:library -out:../$@ *.cs )

bin/ValCommon.dll:
	( cd ValCommon  && \
        mcs -lib:../bin/ -r:OTFontFile -target:library -out:../$@ *.cs )

bin/GMath.dll:
	( cd GMath      && \
	mcs -lib:../bin/ -r:OTFontFile -r:ValCommon -target:library -out:../$@ *.cs )

bin/Glyph.dll:
	( cd Glyph      && \
        mcs -lib:../bin/ -r:OTFontFile -r:ValCommon -r:GMath \
            -resource:NS_Glyph.GErrStrings.resources \
            -target:library -out:../$@ *.cs )

# running just xbuild (from monodevelop) plain also work for GenerateFValData.
# but it's implementation of PostBuildEvent is lacking.
GenerateFValData/bin/Debug/GenerateFValData.exe:
	( cd GenerateFValData && \
        mkdir -p bin/Debug && \
        mcs -r:System.Web  -target:exe \
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
        mcs -lib:../bin/ \
            -r:Compat -r:System.Windows.Forms \
            -r:OTFontFile -r:ValCommon -r:Glyph -r:GMath \
	    -resource:OTFontFileVal.ValStrings.resources \
            -target:library -out:../$@ *.cs )

bin/FontValidator.exe:
	( cd FontValidator && \
        mcs -lib:../bin/ -r:OTFontFileVal -r:OTFontFile -r:ValCommon \
        -target:exe -out:../$@ *.cs )

bin/FontVal.exe:
	( cd FontVal && \
        mcs -lib:../bin/ -lib:../00_refs/ \
            -r:OTFontFileVal -r:OTFontFile -r:ValCommon -r:System.Windows.Forms \
            -r:System.Drawing -r:System.Data -r:Compat \
            -resource:FontVal.Form1.resources \
            -resource:FontVal.FormAbout.resources \
            -resource:FontVal.FormReportOptions.resources \
            -resource:FontVal.FormTransform.resources \
            -resource:FontVal.ResultsForm.resources \
            -target:winexe -out:../$@ *.cs )
