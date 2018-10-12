
// MFCApplication1.h : PROJECT_NAME アプリケーションのメイン ヘッダー ファイルです。
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH に対してこのファイルをインクルードする前に 'stdafx.h' をインクルードしてください"
#endif

#include "resource.h"		// メイン シンボル


// CMFCApplication1App:
// このクラスの実装については、MFCApplication1.cpp を参照してください。
//

class CMFCApplication1App : public CWinApp
{
public:
	CMFCApplication1App();

// オーバーライド
public:
	virtual BOOL InitInstance();

// 実装

	DECLARE_MESSAGE_MAP()
};

extern CMFCApplication1App theApp;