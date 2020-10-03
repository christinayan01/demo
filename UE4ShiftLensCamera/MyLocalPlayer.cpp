// Fill out your copyright notice in the Description page of Project Settings.
//// 手順１．■■■印から下をコピペする
//// 手順２．コメント「replace class name」に書かれたクラス名「UMyLocalPlayer」を、あなたのC++クラス名に置換して下さい。
//// 手順３．★印の箇所を、好きな値に変更します。-0.8くらいだといい感じかも。

#include "MyLocalPlayer.h"

// ■■■
// Created by christinayan01 by Takahiro Yanai. 2020.10.2
#include "Engine.h"

//
void UMyLocalPlayer::PostInitProperties()	// replace class name
{
	Super::PostInitProperties();
	_shiftLensValue = 0.f;	// ★好きな値をセットしてください。
	 _flag = false;
}

//
FSceneView * UMyLocalPlayer::CalcSceneView(	// replace class name
	FSceneViewFamily * ViewFamily, 
	FVector &OutViewLocation, 
	FRotator &OutViewRotation, 
	FViewport * Viewport, 
	FViewElementDrawer * ViewDrawer, 
	EStereoscopicPass StereoPass)
{
	FSceneView* View = Super::CalcSceneView(ViewFamily, OutViewLocation, OutViewRotation, Viewport, ViewDrawer, StereoPass);
	if (View) {
		float Fov = View->FOV;
		Fov = PlayerController->PlayerCameraManager->GetFOVAngle(); // FOVはどっちが正しいのか？　こっち？
		
		float HalfFov = FMath::DegreesToRadians(Fov) / 2.f;
		float Width = ViewportClient->Viewport->GetSizeXY().X;
		float Height = ViewportClient->Viewport->GetSizeXY().Y;
		float ZNear = GNearClippingPlane;

		FMatrix newMatrix = FMatrix(
			FPlane(1.f / FMath::Tan(HalfFov), 0, 0, 0),
			FPlane(0, Width / Height / FMath::Tan(HalfFov), 0, 0),
			FPlane(0, _shiftLensValue, 0, 1),
			FPlane(0, 0, ZNear, 0)
		);

		// プロジェクションマトリクスを更新
		View->UpdateProjectionMatrix(newMatrix);
	}

	return View;
}
