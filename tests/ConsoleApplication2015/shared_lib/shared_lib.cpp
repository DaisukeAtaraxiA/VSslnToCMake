// shared_lib.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "shared_lib.h"

#include "static_lib/static_lib.h"


#ifndef _UNICODE
#error NOT DEFINED _UNICODE
#endif

// これは、エクスポートされた変数の例です。
SHARED_LIB_API int nshared_lib=0;

// これは、エクスポートされた関数の例です。
SHARED_LIB_API int fnshared_lib(void)
{
  int value = static_lib_func();
  return value * 2;
}

// これは、エクスポートされたクラスのコンストラクターです。
// クラス定義に関しては shared_lib.h を参照してください。
Cshared_lib::Cshared_lib()
{
    return;
}
