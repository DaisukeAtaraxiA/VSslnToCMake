#include "static_lib/static_lib.h"

int static_lib_func() {
#ifdef _MYDEBUG
  return -123;
#else
  return 123;
#endif
}