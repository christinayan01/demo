// Fill out your copyright notice in the Description page of Project Settings.
// Created by christinayan01 by Takahiro Yanai. 2020.10.2

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

	UMyLocalPlayer() : _shiftLensValue(0.0), _flag(false) {}

	bool _flag;

	// あおり補正　シフトレンズカメラの値。範囲は -1.0～0.0　です
	float _shiftLensValue;
	

protected:
	//virtual void BeginPlay() override;
	virtual void PostInitProperties() override;
};
