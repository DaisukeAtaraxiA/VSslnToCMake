// 以下の ifdef ブロックは DLL からのエクスポートを容易にするマクロを作成するための 
// 一般的な方法です。この DLL 内のすべてのファイルは、コマンド ラインで定義された SHARED_LIB_EXPORTS
// シンボルを使用してコンパイルされます。このシンボルは、この DLL を使用するプロジェクトでは定義できません。
// ソースファイルがこのファイルを含んでいる他のプロジェクトは、 
// SHARED_LIB_API 関数を DLL からインポートされたと見なすのに対し、この DLL は、このマクロで定義された
// シンボルをエクスポートされたと見なします。
#ifdef _WIN32
#ifdef SHARED_LIB_EXPORTS
#define SHARED_LIB_API __declspec(dllexport)
#else
#define SHARED_LIB_API __declspec(dllimport)
#endif
#endif

#ifndef SHARED_LIB_API
# define SHARED_LIB_API
#endif

// このクラスは shared_lib.dll からエクスポートされました。
class SHARED_LIB_API Cshared_lib {
public:
	Cshared_lib(void);
	// TODO: メソッドをここに追加してください。
};

extern SHARED_LIB_API int nshared_lib;

SHARED_LIB_API int fnshared_lib(void);
