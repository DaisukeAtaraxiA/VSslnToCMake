// �ȉ��� ifdef �u���b�N�� DLL ����̃G�N�X�|�[�g��e�Ղɂ���}�N�����쐬���邽�߂� 
// ��ʓI�ȕ��@�ł��B���� DLL ���̂��ׂẴt�@�C���́A�R�}���h ���C���Œ�`���ꂽ SHARED_LIB_EXPORTS
// �V���{�����g�p���ăR���p�C������܂��B���̃V���{���́A���� DLL ���g�p����v���W�F�N�g�ł͒�`�ł��܂���B
// �\�[�X�t�@�C�������̃t�@�C�����܂�ł��鑼�̃v���W�F�N�g�́A 
// SHARED_LIB_API �֐��� DLL ����C���|�[�g���ꂽ�ƌ��Ȃ��̂ɑ΂��A���� DLL �́A���̃}�N���Œ�`���ꂽ
// �V���{�����G�N�X�|�[�g���ꂽ�ƌ��Ȃ��܂��B
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

// ���̃N���X�� shared_lib.dll ����G�N�X�|�[�g����܂����B
class SHARED_LIB_API Cshared_lib {
public:
	Cshared_lib(void);
	// TODO: ���\�b�h�������ɒǉ����Ă��������B
};

extern SHARED_LIB_API int nshared_lib;

SHARED_LIB_API int fnshared_lib(void);
