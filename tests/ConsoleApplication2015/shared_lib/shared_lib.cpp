// shared_lib.cpp : DLL �A�v���P�[�V�����p�ɃG�N�X�|�[�g�����֐����`���܂��B
//

#include "stdafx.h"
#include "shared_lib.h"

#include "static_lib/static_lib.h"


#ifndef _UNICODE
#error NOT DEFINED _UNICODE
#endif

// ����́A�G�N�X�|�[�g���ꂽ�ϐ��̗�ł��B
SHARED_LIB_API int nshared_lib=0;

// ����́A�G�N�X�|�[�g���ꂽ�֐��̗�ł��B
SHARED_LIB_API int fnshared_lib(void)
{
  int value = static_lib_func();
  return value * 2;
}

// ����́A�G�N�X�|�[�g���ꂽ�N���X�̃R���X�g���N�^�[�ł��B
// �N���X��`�Ɋւ��Ă� shared_lib.h ���Q�Ƃ��Ă��������B
Cshared_lib::Cshared_lib()
{
    return;
}
