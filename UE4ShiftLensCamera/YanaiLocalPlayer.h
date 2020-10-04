// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/LocalPlayer.h"
#include "YanaiLocalPlayer.generated.h"

class FSceneView;

/**
 * 
 */
UCLASS()
class POLYGON_API UYanaiLocalPlayer : public ULocalPlayer
{
	GENERATED_BODY()

public:

	/**
	 * Calculate the view settings for drawing from this view actor
	 *
	 * @param	View - output view struct
	 * @param	OutViewLocation - output actor location
	 * @param	OutViewRotation - output actor rotation
	 * @param	Viewport - current client viewport
	 * @param	ViewDrawer - optional drawing in the view
	 * @param	StereoPass - whether we are drawing the full viewport, or a stereo left / right pass
	 */
	virtual FSceneView* CalcSceneView(class FSceneViewFamily* ViewFamily,
		FVector& OutViewLocation,
		FRotator& OutViewRotation,
		FViewport* Viewport,
		class FViewElementDrawer* ViewDrawer = NULL,
		EStereoscopicPass StereoPass = eSSP_FULL) override;

	// UObject interface
	virtual void PostInitProperties() override;

	//UPROPERTY(EditAnywhere)//, Category = "YanaiLocalPlayer")
	float ShiftLensValue;

	UFUNCTION(BlueprintCallable, Category = "YanaiLocalPlayer")
		void SetShiftLens(float ShiftValue);

private:

	// プロジェクションマトリクスにあおり補正値を適用
	void ApplyShiftLens(FSceneView* View);

};
