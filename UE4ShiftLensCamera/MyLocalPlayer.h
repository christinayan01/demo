// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/LocalPlayer.h"
#include "MyLocalPlayer.generated.h"

/**
 * 
 */
UCLASS()
class SHIFTLENS1_API UMyLocalPlayer : public ULocalPlayer
{
	GENERATED_BODY()

		FSceneView* CalcSceneView(class FSceneViewFamily* ViewFamily,
			FVector& OutViewLocation,
			FRotator& OutViewRotation,
			FViewport* Viewport,
			class FViewElementDrawer* ViewDrawer = NULL,
			EStereoscopicPass StereoPass = eSSP_FULL) override;

protected:
	//virtual void BeginPlay() override;
	virtual void PostInitProperties() override;
};
