#include "val.h"

#include <psapi.h>
#include <tchar.h>
#include <cstdio>

#include <shlwapi.h>

#using <mscorlib.dll>

using namespace System;
using namespace System::Reflection;
using namespace System::IO;
using namespace Microsoft::Win32::SafeHandles;

//Function to retrieve the usual path from the path with a dos device that GetMappedFileName returns
int GetPathByDevicePath(WCHAR* path){

	WCHAR buf[MAX_PATH+1]; //will store a string with all drive letters
	WCHAR name[MAX_PATH+1]; //will store the dos name of a drive in a string like: "A:\ B:\ C:\  " (space here means null character)
	WCHAR drive[3] = TEXT(" :");
	WCHAR* p;
	int sz;
	int h;

	//Storing drive letters in a string
	if(GetLogicalDriveStringsW(MAX_PATH,buf) == 0) return -1;

	//Looping through all drive letters and checking if the related dos device is the same device of the path we have
	//If so, we change the path string to use this drive letter instead of the device and return
	for(p=buf;*p;p++){
		drive[0] = *p;
		if(QueryDosDeviceW(drive,name,MAX_PATH) == 0) return -1;
		PathAddBackslash(name);//making sure it ends with a backslash
		sz = wcsnlen(name,MAX_PATH);

		h = wcsncmp(path,name,sz);

		if(h == 0){
			//Copying to name the portion of path that corresponds to directory path (without drive name)
			wcscpy_s(name,MAX_PATH,&path[sz]); //It becomes something like "Felipe\Fonts\wingding.ttf"
			wcscpy_s(path,MAX_PATH,p); //now path has only the drive letter (something like "C:\")
			PathAddBackslash(path); //Making sure it ends with a backslash
			sz = wcsnlen(path,MAX_PATH);
			//Copying to path the name string (that contains the directory path)
			wcscpy_s(&path[sz],MAX_PATH-sz,name);
			return 0;
		}

		while(*p) p++;
	}

	return 1;
}

//Just a function to get a Validator object all set up
Object^ GetValidator(int ntab,char** tab,int rast,Assembly^ assembly) {

	Object^ ret = nullptr;

	try{
		ret = assembly->CreateInstance("OTFontFileVal.Validator");
		Type^ type = assembly->GetType("OTFontFileVal.ValidatorParameters");
		Object^ vp = Activator::CreateInstance(type);
		int i;

		MethodInfo^ rmv = type->GetMethod("RemoveTableFromList",gcnew array<Type^>(1){String::typeid});
		for(i=0;i<ntab;i++){
			rmv->Invoke(vp,gcnew array<Object^>(1){gcnew String(tab[i])});
		}

		if(rast){
			MethodInfo^ rst = type->GetMethod("SetRasterTesting");
			rst->Invoke(vp,gcnew array<Object^>(0));
		}

		MethodInfo^ stp = type->GetMethod("SetupValidator");
		stp->Invoke(vp,gcnew array<Object^>(1){ret});
	}
	catch(Exception^ e){
		throw e;
	}

	return ret;

}

//It calls the function of the driver that gets the filestream by handle instead of name
//However, we'll need the path anyway because of the xml report file, if we don't make the report, it won't be needed
//Windows Vista (and up) has functions (GetFileInformationByHandleEx, GetFinalPathNameByHandle) that make short work of getting the name
//However, to work also with Windows XP, we use another, more complicated way
extern "C" _declspec(dllexport) int ValidateFont(HANDLE hFile,int ntab,char** tab,int rast){
	HANDLE hMap;
	int szFile;
	void* ptrMap;
	int ret;
	WCHAR fname[MAX_PATH+1];

	//If the font file is greater than 2GB it'll be a problem...but there's no font file with a size near this big
	if((szFile = GetFileSize(hFile,NULL)) == INVALID_FILE_SIZE) return -1;
	
	if((hMap = CreateFileMappingW(hFile,NULL,PAGE_READONLY,0,szFile,NULL)) == NULL) return -1;

	if((ptrMap = MapViewOfFile(hMap,FILE_MAP_READ,0,0,szFile)) == NULL) return -1;

	//very strange...once when I renamed the folder of the font file, this function retrieved the old name
	//although after some time it returned the right name. Remember that if we don't need the xml report the path will not be needed
	if(GetMappedFileName(GetCurrentProcess(),ptrMap,fname,MAX_PATH) == 0) return -1;

	UnmapViewOfFile(ptrMap);
	CloseHandle(hMap);

	if(GetPathByDevicePath(fname)) return -1;

	String^ file = gcnew String(fname);
	//the second argument MUST be false, the handle should NOT be released here
	SafeFileHandle^ sfhFile = gcnew SafeFileHandle(*(gcnew IntPtr(hFile)),false); 
	//Getting the path to this folder that will be used to load the OTFontFileVal.dll in same folder
	FileInfo^ fi = gcnew FileInfo(Assembly::GetExecutingAssembly()->Location);

	String^ path = fi->DirectoryName + (Path::DirectorySeparatorChar).ToString() + "OTFontFileVal.dll";

	//Using reflection because if calling functions normally, it looks like that .net searches
	//for the other dlls only in the folder from the original executable (signtool in this case)

	try{

		Assembly^ assembly = Assembly::LoadFrom(path);

		Type^ tdrv = assembly->GetType("OTFontFileVal.Driver");
		Object^ drv = Activator::CreateInstance(tdrv,assembly->CreateInstance("OTFontFileVal.CallbVal"));

		MethodInfo^ rval = tdrv->GetMethod("RunValidation",gcnew array<Type^>(3){assembly->GetType("OTFontFileVal.Validator"),String::typeid,SafeFileHandle::typeid});

		ret = (int) rval->Invoke(drv,gcnew array<Object^>(3){GetValidator(ntab,tab,rast,assembly),file,sfhFile});

	}
	catch(TargetInvocationException^ e){
		if(e->InnerException->GetType() == FileNotFoundException::typeid){
			FileNotFoundException^ fileex = (FileNotFoundException^) e->InnerException;
			Console::WriteLine("ERROR: Validation library incomplete. Aborting.");
			Console::WriteLine("Missing file: " + "\"" + fileex->FileName + "\"");
		}
		ret = 1;
	}
	catch(FileNotFoundException^ e){
		// Can it happen? The file is already opened by signtool, but it doesn' t hurt...
		if(file->Equals(e->FileName)){
			Console::WriteLine("ERROR: Font file " + "\"" + e->FileName + "\"" + " not found.");
		}
		else{
			Console::WriteLine("ERROR: Validation library incomplete. Aborting.");
			Console::WriteLine("Missing file: " + "\"" + e->FileName + "\"");
		}
		ret = 1;
	}

	return ret;

}
