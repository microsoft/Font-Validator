#pragma once

#include <windows.h>

//Returns 0 if and only if the font passes all the tests of the validator (it may have warnings, but no errors)
extern "C" _declspec(dllexport) int ValidateFont(HANDLE,int,char**,int);
