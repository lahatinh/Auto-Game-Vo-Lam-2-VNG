
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the ATHOOK_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// ATHOOK_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#ifdef ATHOOK_EXPORTS
#define AUTOHOOK_API __declspec(dllexport)
#else
#define AUTOHOOK_API __declspec(dllimport)
#endif


extern "C"{
	extern AUTOHOOK_API unsigned int  __stdcall GetMsg(void);
	extern AUTOHOOK_API int __stdcall InjectDll(HWND hWnd);
	extern AUTOHOOK_API int __stdcall UnmapDll(HWND hWnd);
}

#ifndef ___HOOK_VAR___
#define ___HOOK_VAR___
	HINSTANCE			hDll;	
#endif