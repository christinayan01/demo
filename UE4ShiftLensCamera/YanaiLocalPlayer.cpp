// Fill out your copyright notice in the Description page of Project Settings.


#include "YanaiLocalPlayer.h"


#include "Engine.h"

//
void UYanaiLocalPlayer::PostInitProperties()
{
	Super::PostInitProperties();
	this->ShiftLensValue = 0.0f;
}

//
FSceneView * UYanaiLocalPlayer::CalcSceneView(
	FSceneViewFamily * ViewFamily,
	FVector &OutViewLocation,
	FRotator &OutViewRotation,
	FViewport *Viewport,
	FViewElementDrawer *ViewDrawer,
	EStereoscopicPass StereoPass)
{
	FSceneView* SceneView = Super::CalcSceneView(ViewFamily, OutViewLocation, OutViewRotation, Viewport, ViewDrawer, StereoPass);
	ApplyShiftLens(SceneView);	// ������␳����
	return SceneView;
	/*
		FSceneView* View = Super::CalcSceneView(ViewFamily, OutViewLocation, OutViewRotation, Viewport, ViewDrawer, StereoPass);
		if (View)
		{
			float Fov = View->FOV;
			Fov = PlayerController->PlayerCameraManager->GetFOVAngle(); // FOV�͂ǂ������������̂��H�@�������H

			float HalfFov = FMath::DegreesToRadians(Fov) / 2.f;
			float Width = ViewportClient->Viewport->GetSizeXY().X;
			float Height = ViewportClient->Viewport->GetSizeXY().Y;
			float ZNear = GNearClippingPlane;

			FMatrix newMatrix = FMatrix(
				FPlane(1.f / FMath::Tan(HalfFov),	0,										0,		0),
				FPlane(0,							Width / Height / FMath::Tan(HalfFov),	0,		0),
				FPlane(0,							this->ShiftLensValue,					0,		1),
				FPlane(0,							0,										ZNear,	0)
			);

			View->UpdateProjectionMatrix(newMatrix);

			//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString::Printf(TEXT("FOV=%f"), Fov));
			GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::White, FString::Printf(TEXT("Shift=%f"), this->ShiftLensValue));
		}
		return View;
	*/
}

// ������l���Z�b�g���܂�
// �}�C�i�X�l�ɂ���Ɖ�p�͏�ɃV�t�g���܂��B
// �l�� -1.0�`1.0 ���K�؁B����ȊO�̒l�̓Z�b�g���Ă��o�O������͂��܂��񂪁A�񌻎��I�ȕ`�ʂƂȂ�܂�
void UYanaiLocalPlayer::SetShiftLens(float ShiftValue)
{
	this->ShiftLensValue = ShiftValue * -1.0f;  // �����𔽓]���ăZ�b�g���܂�
}

// �v���W�F�N�V�����}�g���N�X�ɂ�����␳�l��K�p
/* ���������v���W�F�N�V�����}�g���N�X�����̂��ړI�ŁAShiftLensValue�ɑ�������̂�M[2][1]�Ƃ����킯�ł�.
	FMatrix newMatrix = FMatrix(
		FPlane(1.f / FMath::Tan(HalfFov), 0, 0, 0),
		FPlane(0, Width / Height / FMath::Tan(HalfFov), 0, 0),
		FPlane(0, this->ShiftLensValue, 0, 1),
		FPlane(0, 0, ZNear, 0)
	);
 */
void UYanaiLocalPlayer::ApplyShiftLens(FSceneView* SceneView)
{
	if (SceneView) {
		FMatrix NewMatrix = SceneView->ViewMatrices.GetProjectionMatrix();
		NewMatrix.M[2][1] = this->ShiftLensValue;
		SceneView->UpdateProjectionMatrix(NewMatrix);
	}
}
