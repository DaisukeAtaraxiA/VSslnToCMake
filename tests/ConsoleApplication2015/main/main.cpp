// main.cpp : コンソール アプリケーションのエントリ ポイントを定義します。
//

#include "stdafx.h"

#include <stdio.h>

#include <boost/scoped_ptr.hpp>
#include <boost/thread.hpp>

#include "shared_lib/shared_lib.h"
#include "static_lib/static_lib.h"

void func() {
  printf("thread run\n");
}

int main() {
  boost::scoped_ptr<int> i_ptr(new int[1]);
  *i_ptr = fnshared_lib();
  printf("%d\n", *i_ptr);
  printf("static_lib_func() = %d\n", static_lib_func());

  boost::thread t(func);
  t.join();

  return 0;
}
