
// MFCApplication1.h : PROJECT_NAME �A�v���P�[�V�����̃��C�� �w�b�_�[ �t�@�C���ł��B
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH �ɑ΂��Ă��̃t�@�C�����C���N���[�h����O�� 'stdafx.h' ���C���N���[�h���Ă�������"
#endif

#include "resource.h"		// ���C�� �V���{��


// CMFCApplication1App:
// ���̃N���X�̎����ɂ��ẮAMFCApplication1.cpp ���Q�Ƃ��Ă��������B
//

class CMFCApplication1App : public CWinApp
{
public:
	CMFCApplication1App();

// �I�[�o�[���C�h
public:
	virtual BOOL InitInstance();

// ����

	DECLARE_MESSAGE_MAP()
};

extern CMFCApplication1App theApp;