// Fill out your copyright notice in the Description page of Project Settings.
/* Created by christinayan01 by Takahiro Yanai. 2020.10.4
   https://christinayan01.jp/architecture/ | ARCHITECTURE GRAVURE

   手順１．■■■印から下をコピペする
   手順２．クラス名が異なるときは、「UYanaiLocalPlayer」の箇所を、あなたのC++クラス名に置換して下さい。
           クラス名とは.hファイルの↓の部分です
           class POLYGON_API UYanaiLocalPlayer : public ULocalPlayer
                             ~~~~~~~~~~~~~~~~~
*/

#include "YanaiLocalPlayer.h"

// ■■■

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
	ApplyShiftLens(SceneView);	// あおり補正処理
	return SceneView;
}

/*	あおり値をセットします
	マイナス値にすると画角は上にシフトします。
	値は -1.0～1.0 が適切。これ以外の値はセットしてもバグったりはしませんが、非現実的な描写となります
 */
void UYanaiLocalPlayer::SetShiftLens(float ShiftValue)
{
	this->ShiftLensValue = ShiftValue * -1.0f;  // 符号を反転してセットします
}

/*	プロジェクションマトリクスにあおり補正値を適用
	こういうプロジェクションマトリクスを作るのが目的で、ShiftLensValueに相当するのがM[2][1]というわけです.
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
